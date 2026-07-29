using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Smart_Employee_Management_System.Models;

namespace Smart_Employee_Management_System.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Category> Categories { get; set; }

        public DbSet<Worker> Workers { get; set; }

        public DbSet<Customer> Customers { get; set; }

        public DbSet<Booking> Bookings { get; set; }

        public DbSet<Review> Reviews { get; set; }

        public DbSet<Payment> Payments { get; set; }
    }
}