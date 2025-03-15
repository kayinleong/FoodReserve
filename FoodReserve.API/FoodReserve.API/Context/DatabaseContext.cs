using FoodReserve.API.Models;
using Microsoft.EntityFrameworkCore;

namespace FoodReserve.API.Context
{
    public class DatabaseContext(DbContextOptions options) : DbContext(options)
    {
        public DbSet<User> Users { get; set; }
        public DbSet<Staff> Staff { get; set; }
        public DbSet<Customer> Customers { get; set; }
        public DbSet<Outlet> Outlets { get; set; }
        public DbSet<Reservation> Reservations { get; set; }
        public DbSet<Queue> Queues { get; set; }
    }
}
