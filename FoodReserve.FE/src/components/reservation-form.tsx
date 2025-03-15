"use client";

import { CalendarIcon } from "lucide-react";
import { format } from "date-fns";
import { cn } from "@/lib/utils";
import { useEffect, useState } from "react";
import { zodResolver } from "@hookform/resolvers/zod";
import * as z from "zod";
import { useRouter } from "next/navigation";

import { Button } from "@/components/ui/button";
import { Calendar } from "@/components/ui/calendar";
import {
  Card,
  CardContent,
  CardDescription,
  CardHeader,
  CardTitle,
} from "@/components/ui/card";
import {
  Form,
  FormControl,
  FormField,
  FormItem,
  FormLabel,
  FormMessage,
} from "@/components/ui/form";
import { Input } from "@/components/ui/input";
import {
  Popover,
  PopoverContent,
  PopoverTrigger,
} from "@/components/ui/popover";
import {
  Select,
  SelectContent,
  SelectItem,
  SelectTrigger,
  SelectValue,
} from "@/components/ui/select";
import { useForm } from "react-hook-form";
import { toast } from "sonner";
import { getAllOutlet } from "@/lib/actions/outlet.action";
import { Outlet } from "@/lib/domains/outlet.domain";
import { createReservation } from "@/lib/actions/reservation.action";
import { Reservation } from "@/lib/domains/reservation.domain";

// Define the validation schema with Zod
const reservationSchema = z.object({
  name: z.string().min(2, "Name must be at least 2 characters"),
  outlet: z.string().min(1, "Please select an outlet"),
  phone: z
    .string()
    .min(10, "Phone number must be at least 10 digits")
    .regex(/^\+?[0-9\s\-()]+$/, "Invalid phone number format"),
  guests: z.string().min(1, "Please select number of guests"),
  date: z.date({
    required_error: "Please select a date for your reservation",
  }),
  time: z.string().min(1, "Please select a time slot"),
});

type ReservationFormValues = z.infer<typeof reservationSchema>;

export default function ReservationForm() {
  const router = useRouter();
  const [outlets, setOutlets] = useState<Outlet[]>([]);
  const [timeSlots, setTimeSlots] = useState<string[]>([]);
  const [isLoading, setIsLoading] = useState<boolean>(true);

  // Update form to use Zod resolver
  const form = useForm<ReservationFormValues>({
    resolver: zodResolver(reservationSchema),
    defaultValues: {
      name: "",
      outlet: "",
      phone: "",
      guests: "",
      date: undefined,
      time: "",
    },
  });

  // Fetch outlets when component mounts
  useEffect(() => {
    async function fetchOutlets() {
      try {
        const outletData = await getAllOutlet(1, 100); // fetch with a large page size to get all outlets
        setOutlets(outletData.response);
        setIsLoading(false);
      } catch (error) {
        console.error("Failed to fetch outlets:", error);
        toast.error("Could not load restaurant outlets");
        setIsLoading(false);
      }
    }

    fetchOutlets();
  }, []);

  // Generate time slots based on operation hours when outlet changes
  const handleOutletChange = (outletId: string) => {
    form.setValue("outlet", outletId);
    form.setValue("time", ""); // Reset time when outlet changes

    const selectedOutlet = outlets.find((outlet) => outlet.id === outletId);
    if (!selectedOutlet) return;

    // Use operating_hours instead of operations_hours
    const timeSlots = generateTimeSlotsFromHours(
      selectedOutlet.operating_hours
    );
    setTimeSlots(timeSlots);
  };

  // Function to generate time slots from operations_hours string
  const generateTimeSlotsFromHours = (operationHours: string): string[] => {
    try {
      // Add null check to prevent error with undefined
      if (!operationHours) {
        console.error("Operation hours is undefined or null");
        return [];
      }

      const slots: string[] = [];
      // Parse JSON string to array
      const hoursArray = JSON.parse(operationHours);

      if (!Array.isArray(hoursArray) || hoursArray.length < 2) {
        console.error("Invalid operation hours format:", operationHours);
        return [];
      }

      // Assume first element is opening time and second is closing time
      const openingTime = hoursArray[0];
      const closingTime = hoursArray[1];

      // Parse hours and minutes
      const [openHour, openMinute] = openingTime
        .split(":")
        .map((num) => parseInt(num));
      const [closeHour, closeMinute] = closingTime
        .split(":")
        .map((num) => parseInt(num));

      // Generate hourly slots
      let currentHour = openHour;
      let currentMinute = openMinute || 0;

      while (
        currentHour < closeHour ||
        (currentHour === closeHour && currentMinute < closeMinute)
      ) {
        // Format time slot
        const ampm = currentHour >= 12 ? "PM" : "AM";
        const hour =
          currentHour > 12
            ? currentHour - 12
            : currentHour === 0
            ? 12
            : currentHour;

        const timeSlot = `${hour}:${currentMinute
          .toString()
          .padStart(2, "0")} ${ampm}`;
        slots.push(timeSlot);

        // Increment by 1 hour
        currentHour++;
      }

      return slots;
    } catch (error) {
      console.error(
        "Error parsing operation hours:",
        error,
        "Value:",
        operationHours
      );
      return [];
    }
  };

  async function onSubmit(data: ReservationFormValues) {
    try {
      // Find the selected outlet to get its ID
      const selectedOutlet = outlets.find(
        (outlet) => outlet.id === data.outlet
      );

      if (!selectedOutlet) {
        toast.error("Selected outlet not found. Please try again.");
        return;
      }

      // Create a date object and set hours and minutes based on selected time
      const reservationDate = new Date(data.date);
      const timeMatch = data.time.match(/(\d+):(\d+)\s(AM|PM)/i);

      if (!timeMatch) {
        toast.error("Invalid time format. Please try again.");
        return;
      }

      let hours = parseInt(timeMatch[1]);
      const minutes = parseInt(timeMatch[2]);
      const ampm = timeMatch[3].toUpperCase();

      // Convert to 24-hour format
      if (ampm === "PM" && hours !== 12) {
        hours += 12;
      } else if (ampm === "AM" && hours === 12) {
        hours = 0;
      }

      reservationDate.setHours(hours, minutes, 0, 0);

      // Format the date directly as an ISO string to ensure proper serialization
      const isoDate = reservationDate.toISOString();

      const reservation: Reservation = {
        user_id: null,
        outlet_id: selectedOutlet.id,
        name: data.name,
        phone_number: data.phone,
        number_of_guest: parseInt(data.guests),
        status: 0,
        date: isoDate, // Use the ISO string directly
      };

      await createReservation(reservation);
      toast.success(
        "Reservation submitted successfully! Check your WhatsApp for confirmation."
      );

      // Redirect to thank you page with parameters
      const formattedDate = format(data.date, "yyyy-MM-dd");
      router.push(
        `/thankyou?name=${encodeURIComponent(
          data.name
        )}&outlet=${encodeURIComponent(
          data.outlet
        )}&guests=${encodeURIComponent(data.guests)}&date=${encodeURIComponent(
          formattedDate
        )}&time=${encodeURIComponent(data.time)}`
      );
    } catch (error) {
      console.error("Reservation error:", error);
      toast.error("Something went wrong. Please try again.");
    }
  }

  return (
    <Card className="bg-tertiary/90 backdrop-blur-sm border-tertiary/20 shadow-xl">
      <CardHeader>
        <CardTitle className="text-primary">Make a Reservation</CardTitle>
        <CardDescription className="text-primary/80">
          Fill in your details to book a table
        </CardDescription>
      </CardHeader>
      <CardContent>
        {isLoading ? (
          <div className="text-center text-primary py-4">
            Loading outlets...
          </div>
        ) : (
          <Form {...form}>
            <form onSubmit={form.handleSubmit(onSubmit)} className="space-y-4">
              <FormField
                control={form.control}
                name="name"
                render={({ field }) => (
                  <FormItem>
                    <FormLabel className="text-primary">Name</FormLabel>
                    <FormControl>
                      <Input
                        placeholder="Enter your name"
                        {...field}
                        className="bg-background text-primary"
                      />
                    </FormControl>
                    <FormMessage className="text-red-500" />
                  </FormItem>
                )}
              />

              <FormField
                control={form.control}
                name="outlet"
                render={({ field }) => (
                  <FormItem>
                    <FormLabel className="text-primary">
                      Select Outlet
                    </FormLabel>
                    <Select
                      onValueChange={handleOutletChange}
                      defaultValue={field.value}
                    >
                      <FormControl>
                        <SelectTrigger className="bg-background text-primary">
                          <SelectValue placeholder="Choose a restaurant outlet" />
                        </SelectTrigger>
                      </FormControl>
                      <SelectContent>
                        {outlets.map((outlet) => (
                          <SelectItem key={outlet.id} value={outlet.id}>
                            <div className="flex flex-col">
                              <span>{outlet.name}</span>
                              <span className="text-xs text-muted-foreground">
                                {outlet.location}
                              </span>
                            </div>
                          </SelectItem>
                        ))}
                      </SelectContent>
                    </Select>
                    <FormMessage className="text-red-500" />
                  </FormItem>
                )}
              />

              <FormField
                control={form.control}
                name="phone"
                render={({ field }) => (
                  <FormItem>
                    <FormLabel className="text-primary">Phone Number</FormLabel>
                    <FormControl>
                      <Input
                        placeholder="Enter your phone number"
                        type="tel"
                        {...field}
                        className="bg-background text-primary"
                      />
                    </FormControl>
                    <FormMessage className="text-red-500" />
                  </FormItem>
                )}
              />

              <FormField
                control={form.control}
                name="guests"
                render={({ field }) => (
                  <FormItem>
                    <FormLabel className="text-primary">
                      Number of Guests
                    </FormLabel>
                    <Select
                      onValueChange={field.onChange}
                      defaultValue={field.value}
                    >
                      <FormControl>
                        <SelectTrigger className="bg-background text-primary">
                          <SelectValue placeholder="Select number of guests" />
                        </SelectTrigger>
                      </FormControl>
                      <SelectContent>
                        {[1, 2, 3, 4, 5, 6, 7, 8].map((num) => (
                          <SelectItem key={num} value={num.toString()}>
                            {num} {num === 1 ? "Guest" : "Guests"}
                          </SelectItem>
                        ))}
                      </SelectContent>
                    </Select>
                    <FormMessage className="text-red-500" />
                  </FormItem>
                )}
              />

              <FormField
                control={form.control}
                name="date"
                render={({ field }) => (
                  <FormItem className="flex flex-col">
                    <FormLabel className="text-primary">Date</FormLabel>
                    <Popover>
                      <PopoverTrigger asChild>
                        <FormControl>
                          <Button
                            variant={"outline"}
                            className={cn(
                              "w-full pl-3 text-left font-normal bg-background text-primary",
                              !field.value && "text-muted-foreground"
                            )}
                          >
                            {field.value ? (
                              format(field.value, "PPP")
                            ) : (
                              <span>Pick a date</span>
                            )}
                            <CalendarIcon className="ml-auto h-4 w-4 opacity-50" />
                          </Button>
                        </FormControl>
                      </PopoverTrigger>
                      <PopoverContent className="w-auto p-0" align="start">
                        <Calendar
                          mode="single"
                          selected={field.value}
                          onSelect={field.onChange}
                          disabled={(date) =>
                            date < new Date() || date > new Date(2025, 12)
                          }
                          initialFocus
                        />
                      </PopoverContent>
                    </Popover>
                    <FormMessage className="text-red-500" />
                  </FormItem>
                )}
              />

              <FormField
                control={form.control}
                name="time"
                render={({ field }) => (
                  <FormItem>
                    <FormLabel className="text-primary">Time</FormLabel>
                    <Select
                      onValueChange={field.onChange}
                      defaultValue={field.value}
                      disabled={timeSlots.length === 0}
                    >
                      <FormControl>
                        <SelectTrigger className="bg-background text-primary">
                          <SelectValue
                            placeholder={
                              timeSlots.length === 0
                                ? "Select an outlet first"
                                : "Select time slot"
                            }
                          />
                        </SelectTrigger>
                      </FormControl>
                      <SelectContent>
                        {timeSlots.map((time) => (
                          <SelectItem key={time} value={time}>
                            {time}
                          </SelectItem>
                        ))}
                      </SelectContent>
                    </Select>
                    <FormMessage className="text-red-500" />
                  </FormItem>
                )}
              />

              <Button
                type="submit"
                className="w-full bg-primary text-white hover:bg-primary/90"
              >
                Book Table
              </Button>
            </form>
          </Form>
        )}
      </CardContent>
    </Card>
  );
}
