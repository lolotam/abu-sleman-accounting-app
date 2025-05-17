using System;
using System.Data.Entity;
using System.IO;
using AccountingApp.Models;

namespace AccountingApp.Database
{
    public class AccountingDbContext : DbContext
    {
        public AccountingDbContext() : base("name=AccountingDb")
        {
            // Initialize SQLite
            var sqliteAssembly = typeof(System.Data.SQLite.SQLiteConnection).Assembly;
            System.Data.Entity.DbConfiguration.SetConfiguration(new System.Data.SQLite.EF6.Configuration.SQLiteConfiguration());
        }

        // Define DbSets for all entity models
        public DbSet<User> Users { get; set; }
        // Add other models as needed:
        // public DbSet<Customer> Customers { get; set; }
        // public DbSet<Product> Products { get; set; }
        // public DbSet<Invoice> Invoices { get; set; }
        // etc.

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            // Configure entity relationships and constraints here
            
            base.OnModelCreating(modelBuilder);
        }

        // Helper method to initialize the database
        public static void Initialize()
        {
            using (var context = new AccountingDbContext())
            {
                if (!context.Database.Exists())
                {
                    // Create database
                    context.Database.Create();
                    
                    // Seed initial data
                    SeedDatabase(context);
                }
            }
        }

        private static void SeedDatabase(AccountingDbContext context)
        {
            // Add default admin user if no users exist
            if (!context.Users.Any())
            {
                var adminUser = new User
                {
                    Username = "admin",
                    // In production, this should be properly hashed
                    PasswordHash = "8c6976e5b5410415bde908bd4dee15dfb167a9c873fc4bb8a81f6f2ab448a918", // admin123 (SHA-256)
                    FirstName = "System",
                    LastName = "Administrator",
                    Email = "admin@example.com",
                    Role = "Admin",
                    IsActive = true,
                    CreatedAt = DateTime.Now
                };

                context.Users.Add(adminUser);
                context.SaveChanges();
            }
        }
    }
}