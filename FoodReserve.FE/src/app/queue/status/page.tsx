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
import { useState, useEffect, useRef } from "react";
import * as signalR from "@microsoft/signalr";
import { toast } from "sonner";

export default function QueueStatusPage() {
  const searchParams = useSearchParams();
  const queueId = searchParams.get("id");
  const queueNumber = searchParams.get("number");
  const initialPosition = searchParams.get("position");
  const name = searchParams.get("name");
  const guests = searchParams.get("guests");

  const [position, setPosition] = useState<number>(
    initialPosition ? parseInt(initialPosition) : 0
  );
  const [peopleAhead, setPeopleAhead] = useState<number>(
    initialPosition ? parseInt(initialPosition) - 1 : 0
  );
  const [estimatedWaitTime, setEstimatedWaitTime] = useState<string>("30-45");
  const [status, setStatus] = useState<number>(0);
  const hubConnectionRef = useRef<signalR.HubConnection | null>(null);

  useEffect(() => {
    // Calculate estimated wait time based on position
    const minWait = Math.max(5, position * 5);
    const maxWait = Math.max(10, position * 10);
    setEstimatedWaitTime(`${minWait}-${maxWait}`);

    // Calculate people ahead
    setPeopleAhead(Math.max(0, position - 1));
  }, [position]);

  useEffect(() => {
    // Try to retrieve previously stored queue info
    const storedQueueId = sessionStorage.getItem("queueId") || queueId;
    const storedQueuePosition = sessionStorage.getItem("queuePosition");
    const storedQueueStatus = sessionStorage.getItem("queueStatus");

    if (storedQueuePosition) {
      setPosition(parseInt(storedQueuePosition));
    }

    if (storedQueueStatus) {
      setStatus(parseInt(storedQueueStatus));
    }

    // Create a new hub connection for this page
    const createHubConnection = async () => {
      const hubConnection = new signalR.HubConnectionBuilder()
        .withUrl("https://localhost:5000/queuehub")
        .withAutomaticReconnect()
        .build();

      try {
        await hubConnection.start();
        console.log("SignalR Connected in status page");

        // Join the specific queue to receive updates
        if (storedQueueId) {
          await hubConnection.invoke("JoinQueue", storedQueueId);
          console.log(`Joined queue: ${storedQueueId}`);
        }

        // Set up event handlers
        hubConnection.on("QueuePositionUpdated", (id, newPosition) => {
          console.log(
            "Queue position updated:",
            newPosition,
            id,
            storedQueueId
          );
          if (id === storedQueueId) {
            console.log("Updating position to:", newPosition);
            setPosition(newPosition);
            sessionStorage.setItem("queuePosition", newPosition.toString());

            // Alert the user about position change
            toast.info(`Your position has been updated to ${newPosition}`);
          }
        });

        hubConnection.on("QueueJoined", (id, number, newPosition) => {
          if (id === storedQueueId) {
            setPosition(newPosition);
            sessionStorage.setItem("queuePosition", newPosition.toString());
          }
        });

        hubConnection.on("QueueStatusUpdated", (id, newStatus) => {
          console.log("Status updated:", id, newStatus, storedQueueId);
          if (id === storedQueueId) {
            setStatus(newStatus);
            sessionStorage.setItem("queueStatus", newStatus.toString());

            let statusMessage = "";
            switch (newStatus) {
              case 0:
                statusMessage = "Pending";
                break;
              case 1:
                statusMessage = "Approved";
                toast.success(
                  "Your table is ready! Please proceed to the restaurant."
                );
                break;
              case 2:
                statusMessage = "Rejected";
                toast.error(
                  "Your queue request has been rejected. Please contact staff for assistance."
                );
                break;
              case 3:
                statusMessage = "Cancelled";
                toast.info("Your queue has been cancelled.");
                break;
              default:
                statusMessage = "Unknown";
            }

            toast.info(`Queue status updated: ${statusMessage}`);
          }
        });

        hubConnectionRef.current = hubConnection;
      } catch (err) {
        console.error("Error establishing SignalR connection:", err);
        toast.error("Failed to connect to queue service. Please try again.");
      }
    };

    // Always create a new connection for this page
    createHubConnection();

    // Clean up function to stop the connection when component unmounts
    return () => {
      if (hubConnectionRef.current) {
        const id = sessionStorage.getItem("queueId") || queueId;
        if (id) {
          // Attempt to leave the queue group
          hubConnectionRef.current
            .invoke("LeaveQueue", id)
            .catch((err) => console.error("Error leaving queue:", err));
        }

        hubConnectionRef.current.stop();
        console.log("SignalR Disconnected from status page");
      }
    };
  }, [queueId]);

  const handleShare = async () => {
    try {
      if (navigator.share) {
        await navigator.share({
          title: "Queue Status",
          text: `I'm in queue at position ${position}. Estimated wait time: ${estimatedWaitTime} minutes.`,
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

  const handleRefresh = async () => {
    const storedQueueId = sessionStorage.getItem("queueId") || queueId;

    if (
      hubConnectionRef.current &&
      hubConnectionRef.current.state === signalR.HubConnectionState.Connected &&
      storedQueueId
    ) {
      try {
        // Re-join the queue to get the latest position
        await hubConnectionRef.current.invoke("JoinQueue", storedQueueId);
        toast.info("Queue status refreshed");
      } catch (error) {
        console.error("Error refreshing queue status:", error);
        toast.error("Failed to refresh queue status");

        // If failed, try reconnecting
        if (hubConnectionRef.current) {
          try {
            await hubConnectionRef.current.stop();
            await hubConnectionRef.current.start();
            await hubConnectionRef.current.invoke("JoinQueue", storedQueueId);
            toast.success("Reconnected successfully");
          } catch (reconnectError) {
            console.error("Failed to reconnect:", reconnectError);
            toast.error("Connection failed. Please reload the page.");
          }
        }
      }
    } else {
      // If no connection, reload the page
      window.location.reload();
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
              {status === 1 ? (
                <div className="text-green-600 font-bold">
                  Your table is ready! Please proceed to the restaurant.
                </div>
              ) : status === 2 ? (
                <div className="text-red-600 font-bold">
                  Your queue request has been rejected. Please contact staff for
                  assistance.
                </div>
              ) : status === 3 ? (
                <div className="text-gray-600 font-bold">
                  Your queue has been cancelled.
                </div>
              ) : (
                <div className="text-primary/80">
                  {position > 1
                    ? `There are ${peopleAhead} groups ahead of you`
                    : position === 1
                    ? "You're next!"
                    : "Your table is ready!"}
                </div>
              )}
              {status === 0 && (
                <div className="text-primary mt-2 font-semibold">
                  Position: {position}
                </div>
              )}
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
                onClick={handleRefresh}
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
