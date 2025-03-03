"use client";

import { zodResolver } from "@hookform/resolvers/zod";
import { useForm } from "react-hook-form";
import { useRouter } from "next/navigation";
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
  FormDescription,
  FormField,
  FormItem,
  FormLabel,
  FormMessage,
} from "@/components/ui/form";
import { Input } from "@/components/ui/input";
import { Textarea } from "@/components/ui/textarea";
import { toast } from "sonner";

const formSchema = z.object({
  name: z.string().min(2, {
    message: "Name must be at least 2 characters.",
  }),
  contact: z.string().min(8, {
    message: "Please enter a valid contact number.",
  }),
  guests: z.string().min(1, {
    message: "Please select number of guests.",
  }),
  specialRequests: z.string().optional(),
});

export default function Queue() {
  const router = useRouter();
  const form = useForm<z.infer<typeof formSchema>>({
    resolver: zodResolver(formSchema),
    defaultValues: {
      name: "",
      contact: "",
      guests: "",
      specialRequests: "",
    },
  });

  async function onSubmit(values: z.infer<typeof formSchema>) {
    try {
      // Here you would typically make an API call to save the queue registration
      // For demo, we'll generate a random queue number
      const queueNumber = Math.floor(Math.random() * 100) + 1;

      // Redirect to status page with queue details
      router.push(
        `/queue/status?number=${queueNumber}&name=${values.name}&guests=${values.guests}`
      );
    } catch {
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
                  name="contact"
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
                  name="guests"
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

                <FormField
                  control={form.control}
                  name="specialRequests"
                  render={({ field }) => (
                    <FormItem>
                      <FormLabel className="text-primary">
                        Special Requests
                      </FormLabel>
                      <FormControl>
                        <Textarea
                          placeholder="Any special requests or preferences?"
                          {...field}
                          className="bg-background text-primary resize-none"
                        />
                      </FormControl>
                      <FormDescription className="text-primary/70">
                        Optional: Let us know if you have any special
                        requirements
                      </FormDescription>
                      <FormMessage />
                    </FormItem>
                  )}
                />

                <Button
                  type="submit"
                  className="w-full bg-primary text-white hover:bg-primary/90"
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
