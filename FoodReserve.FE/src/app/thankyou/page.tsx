"use client";

import { useSearchParams } from "next/navigation";
import { useEffect, useState } from "react";
import {
  Card,
  CardContent,
  CardDescription,
  CardHeader,
  CardTitle,
} from "@/components/ui/card";
import { CheckCircle, Calendar, MapPin, Users, Clock } from "lucide-react";
import { getOutletById } from "@/lib/actions/outlet.action";
import { Outlet } from "@/lib/domains/outlet.domain";

export default function ThankYouPage() {
  const searchParams = useSearchParams();
  const [outlet, setOutlet] = useState<Outlet | null>(null);
  const [loading, setLoading] = useState(true);

  const name = searchParams.get("name") || "Guest";
  const outletId = searchParams.get("outlet") || "";
  const guests = searchParams.get("guests") || "0";
  const date = searchParams.get("date") || "";
  const time = searchParams.get("time") || "";

  useEffect(() => {
    async function loadOutlet() {
      if (outletId) {
        try {
          const outletData = await getOutletById(outletId);
          setOutlet(outletData);
        } catch (error) {
          console.error("Failed to fetch outlet:", error);
        }
      }
      setLoading(false);
    }

    loadOutlet();
  }, [outletId]); // Add outletId as a dependency

  // Format date for display
  const formatDate = (dateString: string) => {
    if (!dateString) return "";
    try {
      const date = new Date(dateString);
      return date.toLocaleDateString("en-US", {
        weekday: "long",
        year: "numeric",
        month: "long",
        day: "numeric",
      });
    } catch {
      return dateString;
    }
  };

  // Default outlet info if not found
  const outletInfo = outlet || {
    name: "Our Restaurant",
    address: "",
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
          <CardHeader className="text-center pb-2">
            <div className="mx-auto mb-4 flex h-16 w-16 items-center justify-center rounded-full bg-primary/20">
              <CheckCircle className="h-10 w-10 text-primary" />
            </div>
            <CardTitle className="text-primary text-2xl">
              Reservation Confirmed!
            </CardTitle>
            <CardDescription className="text-primary/80">
              Thank you, {name}! Your table has been reserved.
            </CardDescription>
          </CardHeader>

          <CardContent className="space-y-6">
            <div className="space-y-3">
              <div className="flex items-start">
                <MapPin className="h-5 w-5 text-primary mr-3 mt-0.5" />
                <div>
                  <div className="font-medium text-primary">
                    {outletInfo.name}
                  </div>
                </div>
              </div>

              <div className="flex items-center">
                <Calendar className="h-5 w-5 text-primary mr-3" />
                <div className="text-primary">{formatDate(date)}</div>
              </div>

              <div className="flex items-center">
                <Clock className="h-5 w-5 text-primary mr-3" />
                <div className="text-primary">{time}</div>
              </div>

              <div className="flex items-center">
                <Users className="h-5 w-5 text-primary mr-3" />
                <div className="text-primary">
                  {guests} {Number.parseInt(guests) === 1 ? "Guest" : "Guests"}
                </div>
              </div>
            </div>

            <div className="rounded-lg bg-primary/10 p-4 text-center">
              <p className="text-primary text-sm">
                A confirmation has been sent to your WhatsApp. Please arrive 10
                minutes before your reservation time.
              </p>
            </div>
          </CardContent>
        </Card>
      </div>
    </main>
  );
}
