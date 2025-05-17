using AccountingApp.Models;
using System.Data.Entity;

namespace AccountingApp.Database
{
    public class AccountingDbContext : DbContext
    {
        public AccountingDbContext() : base("name=AccountingDbConnection")
        {
            // This will create the database if it doesn't exist
            Database.SetInitializer(new AccountingDbInitializer());
        }

        // Users and Authentication
        public DbSet<User> Users { get; set; }

        // Products and Inventory
        public DbSet<Product> Products { get; set; }
        public DbSet<ProductCategory> ProductCategories { get; set; }
        public DbSet<Warehouse> Warehouses { get; set; }
        public DbSet<Inventory> Inventory { get; set; }
        public DbSet<InventoryTransaction> InventoryTransactions { get; set; }

        // Contacts (Customers & Suppliers)
        public DbSet<Contact> Contacts { get; set; }
        public DbSet<ContactTransaction> ContactTransactions { get; set; }

        // Sales and Purchases
        public DbSet<Invoice> Invoices { get; set; }
        public DbSet<InvoiceItem> InvoiceItems { get; set; }
        public DbSet<InvoicePayment> InvoicePayments { get; set; }

        // Accounting
        public DbSet<AccountGroup> AccountGroups { get; set; }
        public DbSet<Account> Accounts { get; set; }
        public DbSet<JournalEntry> JournalEntries { get; set; }
        public DbSet<JournalEntryLine> JournalEntryLines { get; set; }
        public DbSet<FiscalYear> FiscalYears { get; set; }
        public DbSet<FiscalPeriod> FiscalPeriods { get; set; }

        // Settings
        public DbSet<CompanySettings> CompanySettings { get; set; }
        public DbSet<SystemSettings> SystemSettings { get; set; }
        public DbSet<AuditLog> AuditLogs { get; set; }
        public DbSet<Notification> Notifications { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            // Configure entity relationships and constraints here
            base.OnModelCreating(modelBuilder);
        }
    }

    public class AccountingDbInitializer : CreateDatabaseIfNotExists<AccountingDbContext>
    {
        protected override void Seed(AccountingDbContext context)
        {
            // Add default admin user
            var adminUser = new User
            {
                Username = "admin",
                Password = "admin123", // This will be hashed automatically
                FullName = "System Administrator",
                Email = "admin@example.com",
                Role = UserRole.Admin,
                IsActive = true
            };

            context.Users.Add(adminUser);

            // Add default accountant user
            var accountantUser = new User
            {
                Username = "accountant",
                Password = "accountant123", // This will be hashed automatically
                FullName = "Default Accountant",
                Email = "accountant@example.com",
                Role = UserRole.Accountant,
                IsActive = true
            };

            context.Users.Add(accountantUser);

            // Add company settings
            var companySettings = new CompanySettings
            {
                CompanyName = "Abu Sleman Company",
                Address = "Main Street",
                City = "City",
                Country = "Country",
                Phone = "123456789",
                Email = "info@abusleman.com",
                Website = "www.abusleman.com",
                TaxNumber = "123456789",
                CurrencyCode = "USD",
                CurrencySymbol = "$",
                ShowCurrencySymbolBefore = true,
                DecimalPlaces = 2,
                DefaultTaxRate = 0,
                DefaultLanguage = "en-US",
                FiscalYearStart = new DateTime(DateTime.Now.Year, 1, 1)
            };

            context.CompanySettings.Add(companySettings);

            // Add default warehouses
            var mainWarehouse = new Warehouse
            {
                Name = "Main Warehouse",
                Location = "Main Location",
                IsActive = true
            };

            context.Warehouses.Add(mainWarehouse);

            // Add default product categories
            var generalCategory = new ProductCategory
            {
                Name = "General",
                Description = "General products category",
                IsActive = true
            };

            context.ProductCategories.Add(generalCategory);

            // Add default chart of accounts
            SeedChartOfAccounts(context);

            // Add fiscal year
            var currentYear = DateTime.Now.Year;
            var fiscalYear = new FiscalYear
            {
                Name = $"Fiscal Year {currentYear}",
                StartDate = new DateTime(currentYear, 1, 1),
                EndDate = new DateTime(currentYear, 12, 31),
                IsActive = true,
                IsClosed = false
            };

            context.FiscalYears.Add(fiscalYear);

            context.SaveChanges();

            base.Seed(context);
        }

        private void SeedChartOfAccounts(AccountingDbContext context)
        {
            // Create account groups
            var assetGroup = new AccountGroup { Code = "1", Name = "Assets", Type = AccountType.Asset, IsActive = true };
            var liabilityGroup = new AccountGroup { Code = "2", Name = "Liabilities", Type = AccountType.Liability, IsActive = true };
            var equityGroup = new AccountGroup { Code = "3", Name = "Equity", Type = AccountType.Equity, IsActive = true };
            var revenueGroup = new AccountGroup { Code = "4", Name = "Revenue", Type = AccountType.Revenue, IsActive = true };
            var expenseGroup = new AccountGroup { Code = "5", Name = "Expenses", Type = AccountType.Expense, IsActive = true };

            context.AccountGroups.Add(assetGroup);
            context.AccountGroups.Add(liabilityGroup);
            context.AccountGroups.Add(equityGroup);
            context.AccountGroups.Add(revenueGroup);
            context.AccountGroups.Add(expenseGroup);

            context.SaveChanges();

            // Create basic accounts
            var accounts = new List<Account>
            {
                new Account { Code = "1010", Name = "Cash", Type = AccountType.Asset, GroupId = assetGroup.Id, IsCashOrBank = true, IsActive = true },
                new Account { Code = "1020", Name = "Bank Account", Type = AccountType.Asset, GroupId = assetGroup.Id, IsCashOrBank = true, IsActive = true },
                new Account { Code = "1200", Name = "Accounts Receivable", Type = AccountType.Asset, GroupId = assetGroup.Id, IsActive = true },
                new Account { Code = "1300", Name = "Inventory", Type = AccountType.Asset, GroupId = assetGroup.Id, IsActive = true },

                new Account { Code = "2100", Name = "Accounts Payable", Type = AccountType.Liability, GroupId = liabilityGroup.Id, IsActive = true },
                new Account { Code = "2200", Name = "Accrued Expenses", Type = AccountType.Liability, GroupId = liabilityGroup.Id, IsActive = true },

                new Account { Code = "3100", Name = "Capital", Type = AccountType.Equity, GroupId = equityGroup.Id, IsActive = true },
                new Account { Code = "3200", Name = "Retained Earnings", Type = AccountType.Equity, GroupId = equityGroup.Id, IsActive = true },

                new Account { Code = "4100", Name = "Sales Revenue", Type = AccountType.Revenue, GroupId = revenueGroup.Id, IsActive = true },
                new Account { Code = "4200", Name = "Other Revenue", Type = AccountType.Revenue, GroupId = revenueGroup.Id, IsActive = true },

                new Account { Code = "5100", Name = "Cost of Goods Sold", Type = AccountType.Expense, GroupId = expenseGroup.Id, IsActive = true },
                new Account { Code = "5200", Name = "Salaries Expense", Type = AccountType.Expense, GroupId = expenseGroup.Id, IsActive = true },
                new Account { Code = "5300", Name = "Rent Expense", Type = AccountType.Expense, GroupId = expenseGroup.Id, IsActive = true },
                new Account { Code = "5400", Name = "Utilities Expense", Type = AccountType.Expense, GroupId = expenseGroup.Id, IsActive = true },
                new Account { Code = "5500", Name = "Office Supplies", Type = AccountType.Expense, GroupId = expenseGroup.Id, IsActive = true }
            };

            foreach (var account in accounts)
            {
                context.Accounts.Add(account);
            }
        }
    }
}
