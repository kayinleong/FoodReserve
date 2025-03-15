"use client";

import { zodResolver } from "@hookform/resolvers/zod";
import { useForm } from "react-hook-form";
import { useRouter, useSearchParams } from "next/navigation";
import * as z from "zod";
import { Button } from "@/components/ui/button";
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
import { toast } from "sonner";
import { useState, useEffect, useRef } from "react";
import * as signalR from "@microsoft/signalr";

// Updated form schema to match QueueRequest (removed specialRequests)
const formSchema = z.object({
  name: z.string().min(2, {
    message: "Name must be at least 2 characters.",
  }),
  phoneNumber: z.string().min(8, {
    message: "Please enter a valid contact number.",
  }),
  numberOfGuest: z.string().min(1, {
    message: "Please select number of guests.",
  }),
});

export default function Queue() {
  const router = useRouter();
  const searchParams = useSearchParams();
  const outletId = searchParams.get("outlet");

  // Check if outletId exists, if not redirect or show error
  useEffect(() => {
    if (!outletId) {
      toast.error("Outlet ID is required");
      // Optionally redirect to a different page
      // router.push('/');
    }
  }, [outletId, router]);

  const form = useForm<z.infer<typeof formSchema>>({
    resolver: zodResolver(formSchema),
    defaultValues: {
      name: "",
      phoneNumber: "",
      numberOfGuest: "",
    },
  });

  const [queueId, setQueueId] = useState<string | null>(null);
  const [queueNumber, setQueueNumber] = useState<number | null>(null);
  const [queuePosition, setQueuePosition] = useState<number | null>(null);
  const hubConnectionRef = useRef<signalR.HubConnection | null>(null);

  useEffect(() => {
    // Create SignalR connection
    const createHubConnection = async () => {
      const hubConnection = new signalR.HubConnectionBuilder()
        .withUrl("https://localhost:5000/queuehub")
        .withAutomaticReconnect()
        .build();

      try {
        await hubConnection.start();
        console.log("SignalR Connected in queue page");

        // Set up event handlers
        hubConnection.on(
          "QueueCreated",
          (id, name, numberOfGuest, number, position) => {
            setQueueId(id);
            setQueueNumber(number);
            setQueuePosition(position);

            // Store queue data in sessionStorage but not connection status
            sessionStorage.setItem("queueId", id);
            sessionStorage.setItem("queueNumber", number.toString());
            sessionStorage.setItem("queuePosition", position.toString());
            sessionStorage.setItem("queueStatus", "0"); // Initial status is Pending

            // Redirect to status page with queue details
            router.push(
              `/queue/status?id=${id}&number=${number}&position=${position}&name=${name}&outlet=${outletId}&guests=${numberOfGuest}`
            );
          }
        );

        hubConnection.on("QueuePositionUpdated", (id, position) => {
          setQueuePosition(position);
          sessionStorage.setItem("queuePosition", position.toString());
        });

        hubConnection.on("QueueStatusUpdated", (id, status) => {
          toast.info(
            `Queue status updated: ${
              status === 1
                ? "Approved"
                : status === 2
                ? "Rejected"
                : status === 3
                ? "Cancelled"
                : "Pending"
            }`
          );
        });

        hubConnection.on("QueueApproved", () => {
          toast.success(
            "Your table is ready! Please proceed to the restaurant."
          );
        });

        hubConnectionRef.current = hubConnection;
      } catch (err) {
        console.error("Error establishing SignalR connection:", err);
        toast.error("Failed to connect to queue service. Please try again.");
      }
    };

    // Always create a new connection in this page
    createHubConnection();

    // Clean up function to stop the connection when component unmounts
    return () => {
      if (hubConnectionRef.current) {
        hubConnectionRef.current.stop();
        console.log("SignalR Disconnected from queue page");
      }
    };
  }, [router, outletId]);

  async function onSubmit(values: z.infer<typeof formSchema>) {
    try {
      if (
        !hubConnectionRef.current ||
        hubConnectionRef.current.state !== signalR.HubConnectionState.Connected
      ) {
        toast.error(
          "Not connected to queue service. Please refresh and try again."
        );
        return;
      }

      if (!outletId) {
        toast.error("Outlet ID is missing. Please try again.");
        return;
      }

      // Store form values in sessionStorage for thankyou page to access
      sessionStorage.setItem("queueUserName", values.name);
      sessionStorage.setItem("queueOutletId", outletId || "");
      sessionStorage.setItem("queueGuests", values.numberOfGuest);

      // Create queue request object
      const queueRequest = {
        outletId: outletId,
        name: values.name,
        phoneNumber: values.phoneNumber,
        numberOfGuest: values.numberOfGuest,
        queueNumber: 0,
        status: 0,
        date: new Date().toISOString(),
      };

      // Call the CreateQueue method on the hub
      await hubConnectionRef.current.invoke("CreateQueue", queueRequest);

      toast.success("Queue request submitted successfully!");

      // Note: The actual redirect happens in the QueueCreated event handler
    } catch (error) {
      console.error("Error creating queue:", error);
      toast.error("Something went wrong. Please try again.");
    }
  }

  return (
    <main className="min-h-screen bg-background relative overflow-hidden">
      {/* Polymorphic Background Elements */}
      <div className="absolute inset-0 overflow-hidden">
        <div className="absolute -top-1/4 -right-1/4 w-[600px] h-[600px] rounded-full bg-gradient-to-br from-secondary/30 to-primary/30 blur-3xl" />
        <div className="absolute top-1/3 -left-1/4 w-[500px] h-[500px] rounded-full bg-gradient-to-tr from-tertiary/40 to-secondary/40 blur-3xl" />
        <div className="absolute -bottom-1/4 right-1/3 w-[400px] h-[400px] rounded-full bg-gradient-to-bl from-primary/20 to-tertiary/20 blur-3xl" />
      </div>

      {/* Content */}
      <div className="relative container mx-auto max-w-lg px-4 py-12">
        <Card className="bg-tertiary/90 backdrop-blur-sm border-tertiary/20 shadow-xl">
          <CardHeader>
            <CardTitle className="text-primary">Join the Queue</CardTitle>
            <CardDescription className="text-primary/80">
              Please fill in your details to join the waiting list
            </CardDescription>
          </CardHeader>
          <CardContent>
            <Form {...form}>
              <form
                onSubmit={form.handleSubmit(onSubmit)}
                className="space-y-4"
              >
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
                      <FormMessage />
                    </FormItem>
                  )}
                />

                <FormField
                  control={form.control}
                  name="phoneNumber"
                  render={({ field }) => (
                    <FormItem>
                      <FormLabel className="text-primary">
                        Contact Number
                      </FormLabel>
                      <FormControl>
                        <Input
                          placeholder="Enter your contact number"
                          type="tel"
                          {...field}
                          className="bg-background text-primary"
                        />
                      </FormControl>
                      <FormMessage />
                    </FormItem>
                  )}
                />

                <FormField
                  control={form.control}
                  name="numberOfGuest"
                  render={({ field }) => (
                    <FormItem>
                      <FormLabel className="text-primary">
                        Number of Guests
                      </FormLabel>
                      <FormControl>
                        <Input
                          placeholder="Enter number of guests"
                          type="number"
                          min="1"
                          max="8"
                          {...field}
                          className="bg-background text-primary"
                        />
                      </FormControl>
                      <FormMessage />
                    </FormItem>
                  )}
                />

                <Button
                  type="submit"
                  className="w-full bg-primary text-white hover:bg-primary/90"
                  disabled={!outletId}
                >
                  Join Queue
                </Button>
              </form>
            </Form>
          </CardContent>
        </Card>
      </div>
    </main>
  );
}
