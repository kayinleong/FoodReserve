using FoodReserve.API.Models;
using Microsoft.EntityFrameworkCore;

namespace FoodReserve.API.Context
{
    public class DatabaseContext(DbContextOptions options) : DbContext(options)
    {
        public DbSet<User> Users { get; set; }
    }
}
