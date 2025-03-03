"use client";

import { useSearchParams } from "next/navigation";
import {
  Card,
  CardContent,
  CardDescription,
  CardHeader,
  CardTitle,
} from "@/components/ui/card";
import { Button } from "@/components/ui/button";
import { Users, Clock, Share2 } from "lucide-react";

export default function QueueStatusPage() {
  const searchParams = useSearchParams();
  const queueNumber = searchParams.get("number");
  const name = searchParams.get("name");
  const guests = searchParams.get("guests");

  const estimatedWaitTime = "30-45";
  const peopleAhead = Math.floor(Math.random() * 10) + 1;

  const handleShare = async () => {
    try {
      if (navigator.share) {
        await navigator.share({
          title: "Queue Status",
          text: `I'm in queue at position ${queueNumber}. Estimated wait time: ${estimatedWaitTime} minutes.`,
          url: window.location.href,
        });
      } else {
        // Fallback to copying to clipboard
        await navigator.clipboard.writeText(window.location.href);
        alert("Queue status link copied to clipboard!");
      }
    } catch (error) {
      console.error("Error sharing:", error);
    }
  };

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
          <CardHeader className="text-center">
            <CardTitle className="text-primary text-3xl">
              Queue Status
            </CardTitle>
            <CardDescription className="text-primary/80">
              Welcome, {name}! Here&apos;s your queue information
            </CardDescription>
          </CardHeader>
          <CardContent className="space-y-6">
            <div className="text-center">
              <div className="text-6xl font-bold text-primary mb-2">
                #{queueNumber}
              </div>
              <div className="text-primary/80">Your Queue Number</div>
            </div>

            <div className="grid grid-cols-2 gap-4">
              <div className="bg-background/50 rounded-lg p-4 text-center">
                <Users className="h-6 w-6 mx-auto mb-2 text-primary" />
                <div className="text-sm text-primary/80">Group Size</div>
                <div className="text-lg font-semibold text-primary">
                  {guests}{" "}
                  {Number.parseInt(guests || "1") === 1 ? "Person" : "People"}
                </div>
              </div>
              <div className="bg-background/50 rounded-lg p-4 text-center">
                <Clock className="h-6 w-6 mx-auto mb-2 text-primary" />
                <div className="text-sm text-primary/80">Estimated Wait</div>
                <div className="text-lg font-semibold text-primary">
                  {estimatedWaitTime} mins
                </div>
              </div>
            </div>

            <div className="bg-primary/10 rounded-lg p-4 text-center">
              <div className="text-primary font-semibold">Current Status</div>
              <div className="text-primary/80">
                There are {peopleAhead} groups ahead of you
              </div>
            </div>

            <div className="flex flex-col gap-2">
              <Button
                variant="outline"
                className="w-full bg-background text-primary hover:bg-primary hover:text-primary-foreground"
                onClick={handleShare}
              >
                <Share2 className="mr-2 h-4 w-4" />
                Share Queue Status
              </Button>
              <Button
                variant="outline"
                className="w-full bg-background text-primary hover:bg-primary hover:text-primary-foreground"
                onClick={() => window.location.reload()}
              >
                Refresh Status
              </Button>
            </div>
          </CardContent>
        </Card>
      </div>
    </main>
  );
}
