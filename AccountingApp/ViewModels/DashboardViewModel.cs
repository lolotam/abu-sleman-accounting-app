using AccountingApp.Database;
using AccountingApp.Models;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using System.Windows.Media;

namespace AccountingApp.ViewModels
{
    public class DashboardViewModel : INotifyPropertyChanged
    {
        // Summary statistics
        private decimal _totalSales;
        private decimal _totalPurchases;
        private decimal _totalExpenses;
        private decimal _netProfit;
        
        private double _salesGrowth;
        private double _purchasesGrowth;
        private double _expensesGrowth;
        private double _profitGrowth;

        // Collections
        private ObservableCollection<TransactionSummary> _recentTransactions;

        // Commands
        public ICommand ViewAllTransactionsCommand { get; }
        public ICommand NewSaleCommand { get; }
        public ICommand NewPurchaseCommand { get; }
        public ICommand NewExpenseCommand { get; }
        public ICommand GenerateReportsCommand { get; }

        public DashboardViewModel()
        {
            // Initialize commands
            ViewAllTransactionsCommand = new RelayCommand(ViewAllTransactions);
            NewSaleCommand = new RelayCommand(NewSale);
            NewPurchaseCommand = new RelayCommand(NewPurchase);
            NewExpenseCommand = new RelayCommand(NewExpense);
            GenerateReportsCommand = new RelayCommand(GenerateReports);

            // Load data
            LoadDashboardData();
        }

        #region Properties
        public decimal TotalSales
        {
            get { return _totalSales; }
            set
            {
                _totalSales = value;
                OnPropertyChanged();
            }
        }

        public decimal TotalPurchases
        {
            get { return _totalPurchases; }
            set
            {
                _totalPurchases = value;
                OnPropertyChanged();
            }
        }

        public decimal TotalExpenses
        {
            get { return _totalExpenses; }
            set
            {
                _totalExpenses = value;
                OnPropertyChanged();
            }
        }

        public decimal NetProfit
        {
            get { return _netProfit; }
            set
            {
                _netProfit = value;
                OnPropertyChanged();
            }
        }

        public double SalesGrowth
        {
            get { return _salesGrowth; }
            set
            {
                _salesGrowth = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(SalesGrowthColor));
                OnPropertyChanged(nameof(SalesGrowthIcon));
            }
        }

        public double PurchasesGrowth
        {
            get { return _purchasesGrowth; }
            set
            {
                _purchasesGrowth = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(PurchasesGrowthColor));
                OnPropertyChanged(nameof(PurchasesGrowthIcon));
            }
        }

        public double ExpensesGrowth
        {
            get { return _expensesGrowth; }
            set
            {
                _expensesGrowth = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(ExpensesGrowthColor));
                OnPropertyChanged(nameof(ExpensesGrowthIcon));
            }
        }

        public double ProfitGrowth
        {
            get { return _profitGrowth; }
            set
            {
                _profitGrowth = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(ProfitGrowthColor));
                OnPropertyChanged(nameof(ProfitGrowthIcon));
            }
        }

        public Brush SalesGrowthColor => SalesGrowth >= 0 ? Brushes.Green : Brushes.Red;
        public Brush PurchasesGrowthColor => PurchasesGrowth >= 0 ? Brushes.Green : Brushes.Red;
        public Brush ExpensesGrowthColor => ExpensesGrowth <= 0 ? Brushes.Green : Brushes.Red;
        public Brush ProfitGrowthColor => ProfitGrowth >= 0 ? Brushes.Green : Brushes.Red;

        public string SalesGrowthIcon => SalesGrowth >= 0 ? "ArrowUpBold" : "ArrowDownBold";
        public string PurchasesGrowthIcon => PurchasesGrowth >= 0 ? "ArrowUpBold" : "ArrowDownBold";
        public string ExpensesGrowthIcon => ExpensesGrowth <= 0 ? "ArrowDownBold" : "ArrowUpBold";
        public string ProfitGrowthIcon => ProfitGrowth >= 0 ? "ArrowUpBold" : "ArrowDownBold";

        public ObservableCollection<TransactionSummary> RecentTransactions
        {
            get { return _recentTransactions; }
            set
            {
                _recentTransactions = value;
                OnPropertyChanged();
            }
        }
        #endregion

        #region Methods
        private void LoadDashboardData()
        {
            try
            {
                using (var context = new AccountingDbContext())
                {
                    // Get current month and previous month dates
                    var today = DateTime.Today;
                    var currentMonthStart = new DateTime(today.Year, today.Month, 1);
                    var previousMonthStart = currentMonthStart.AddMonths(-1);
                    var previousMonthEnd = currentMonthStart.AddDays(-1);

                    // Calculate sales
                    var currentMonthSales = context.Invoices
                        .Where(i => i.Type == InvoiceType.SalesInvoice && i.InvoiceDate >= currentMonthStart && i.InvoiceDate <= today)
                        .Sum(i => (decimal?)i.Total) ?? 0;

                    var previousMonthSales = context.Invoices
                        .Where(i => i.Type == InvoiceType.SalesInvoice && i.InvoiceDate >= previousMonthStart && i.InvoiceDate <= previousMonthEnd)
                        .Sum(i => (decimal?)i.Total) ?? 0;

                    TotalSales = currentMonthSales;
                    SalesGrowth = previousMonthSales == 0 ? 100 : Math.Round((double)((currentMonthSales - previousMonthSales) / previousMonthSales * 100), 1);

                    // Calculate purchases
                    var currentMonthPurchases = context.Invoices
                        .Where(i => i.Type == InvoiceType.PurchaseInvoice && i.InvoiceDate >= currentMonthStart && i.InvoiceDate <= today)
                        .Sum(i => (decimal?)i.Total) ?? 0;

                    var previousMonthPurchases = context.Invoices
                        .Where(i => i.Type == InvoiceType.PurchaseInvoice && i.InvoiceDate >= previousMonthStart && i.InvoiceDate <= previousMonthEnd)
                        .Sum(i => (decimal?)i.Total) ?? 0;

                    TotalPurchases = currentMonthPurchases;
                    PurchasesGrowth = previousMonthPurchases == 0 ? 100 : Math.Round((double)((currentMonthPurchases - previousMonthPurchases) / previousMonthPurchases * 100), 1);

                    // Calculate expenses (simplified - in a real app, would come from expense transactions)
                    TotalExpenses = currentMonthPurchases * 0.2m; // Just for demo purposes
                    ExpensesGrowth = 5.2; // Just for demo purposes

                    // Calculate profit
                    NetProfit = TotalSales - TotalPurchases - TotalExpenses;
                    ProfitGrowth = 12.5; // Just for demo purposes

                    // Get recent transactions
                    var recentInvoices = context.Invoices
                        .Include("Contact")
                        .OrderByDescending(i => i.InvoiceDate)
                        .Take(10)
                        .ToList();

                    RecentTransactions = new ObservableCollection<TransactionSummary>(
                        recentInvoices.Select(i => new TransactionSummary
                        {
                            Date = i.InvoiceDate,
                            Type = i.Type.ToString(),
                            Number = i.InvoiceNumber,
                            ContactName = i.Contact?.Name ?? "Unknown",
                            Amount = i.Total,
                            Status = i.Status.ToString()
                        })
                    );
                }
            }
            catch (Exception ex)
            {
                // Log error
                Console.WriteLine($"Load dashboard data error: {ex.Message}");
                
                // Initialize with default values for demo
                TotalSales = 25000;
                TotalPurchases = 15000;
                TotalExpenses = 3000;
                NetProfit = 7000;
                
                SalesGrowth = 15.2;
                PurchasesGrowth = 8.7;
                ExpensesGrowth = 5.2;
                ProfitGrowth = 12.5;
                
                RecentTransactions = new ObservableCollection<TransactionSummary>();
                for (int i = 1; i <= 10; i++)
                {
                    RecentTransactions.Add(new TransactionSummary
                    {
                        Date = DateTime.Now.AddDays(-i),
                        Type = i % 2 == 0 ? "SalesInvoice" : "PurchaseInvoice",
                        Number = i % 2 == 0 ? $"SI2305{i:D4}" : $"PI2305{i:D4}",
                        ContactName = i % 2 == 0 ? $"Customer {i}" : $"Supplier {i}",
                        Amount = i * 1000,
                        Status = i % 3 == 0 ? "Paid" : (i % 3 == 1 ? "PartiallyPaid" : "Confirmed")
                    });
                }
            }
        }

        private void ViewAllTransactions(object parameter)
        {
            // Navigate to transactions view
        }

        private void NewSale(object parameter)
        {
            // Navigate to new sale view
        }

        private void NewPurchase(object parameter)
        {
            // Navigate to new purchase view
        }

        private void NewExpense(object parameter)
        {
            // Navigate to new expense view
        }

        private void GenerateReports(object parameter)
        {
            // Navigate to reports view
        }
        #endregion

        #region INotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion
    }

    public class TransactionSummary
    {
        public DateTime Date { get; set; }
        public string Type { get; set; }
        public string Number { get; set; }
        public string ContactName { get; set; }
        public decimal Amount { get; set; }
        public string Status { get; set; }
    }
}
