import ReservationForm from "@/components/reservation-form";

export default function Home() {
  return (
    <main className="min-h-screen bg-background relative overflow-hidden">
      {/* Polymorphic Background Elements */}
      <div className="absolute inset-0 overflow-hidden">
        {/* Large circle gradient */}
        <div className="absolute -top-1/4 -right-1/4 w-[600px] h-[600px] rounded-full bg-gradient-to-br from-secondary/30 to-primary/30 blur-3xl" />

        {/* Middle blob */}
        <div className="absolute top-1/3 -left-1/4 w-[500px] h-[500px] rounded-full bg-gradient-to-tr from-tertiary/40 to-secondary/40 blur-3xl" />

        {/* Bottom gradient */}
        <div className="absolute -bottom-1/4 right-1/3 w-[400px] h-[400px] rounded-full bg-gradient-to-bl from-primary/20 to-tertiary/20 blur-3xl" />

        {/* Small floating elements */}
        <div className="absolute top-1/4 right-1/4 w-24 h-24 rounded-full bg-secondary/20 blur-xl animate-float-slow" />
        <div className="absolute bottom-1/4 left-1/3 w-32 h-32 rounded-full bg-primary/20 blur-xl animate-float-medium" />
        <div className="absolute top-1/2 right-1/2 w-16 h-16 rounded-full bg-tertiary/30 blur-xl animate-float-fast" />
      </div>

      {/* Content */}
      <div className="relative container mx-auto max-w-4xl px-4 py-12">
        <div className="mb-8 text-center">
          <h1 className="text-5xl font-bold text-primary mb-4 tracking-tight">
            Restaurant Reservations
          </h1>
          <p className="text-foreground/80 text-lg max-w-2xl mx-auto">
            Book your table with us for an unforgettable dining experience
          </p>
        </div>
        <ReservationForm />
      </div>
    </main>
  );
}
