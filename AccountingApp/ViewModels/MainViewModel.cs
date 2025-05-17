using AccountingApp.Models;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;

namespace AccountingApp.ViewModels
{
    public class MainViewModel : INotifyPropertyChanged
    {
        private User _currentUser;
        private string _pageTitle;
        private object _currentView;

        public User CurrentUser
        {
            get { return _currentUser; }
            set
            {
                _currentUser = value;
                OnPropertyChanged();
            }
        }

        public string PageTitle
        {
            get { return _pageTitle; }
            set
            {
                _pageTitle = value;
                OnPropertyChanged();
            }
        }

        public object CurrentView
        {
            get { return _currentView; }
            set
            {
                _currentView = value;
                OnPropertyChanged();
            }
        }

        public ICommand DashboardCommand { get; }
        public ICommand ProductsCommand { get; }
        public ICommand CustomersCommand { get; }
        public ICommand SuppliersCommand { get; }
        public ICommand SalesCommand { get; }
        public ICommand PurchasesCommand { get; }
        public ICommand AccountingCommand { get; }
        public ICommand ReportsCommand { get; }
        public ICommand SettingsCommand { get; }
        public ICommand LogoutCommand { get; }

        public MainViewModel(User user)
        {
            CurrentUser = user;
            PageTitle = "Dashboard";

            // Initialize commands
            DashboardCommand = new RelayCommand(ShowDashboard);
            ProductsCommand = new RelayCommand(ShowProducts);
            CustomersCommand = new RelayCommand(ShowCustomers);
            SuppliersCommand = new RelayCommand(ShowSuppliers);
            SalesCommand = new RelayCommand(ShowSales);
            PurchasesCommand = new RelayCommand(ShowPurchases);
            AccountingCommand = new RelayCommand(ShowAccounting);
            ReportsCommand = new RelayCommand(ShowReports);
            SettingsCommand = new RelayCommand(ShowSettings);
            LogoutCommand = new RelayCommand(Logout);

            // Set initial view
            ShowDashboard(null);
        }

        private void ShowDashboard(object obj)
        {
            PageTitle = "Dashboard";
            CurrentView = new DashboardView();
        }

        private void ShowProducts(object obj)
        {
            PageTitle = "Products Management";
            // CurrentView = new ProductsView();
        }

        private void ShowCustomers(object obj)
        {
            PageTitle = "Customers Management";
            // CurrentView = new CustomersView();
        }

        private void ShowSuppliers(object obj)
        {
            PageTitle = "Suppliers Management";
            // CurrentView = new SuppliersView();
        }

        private void ShowSales(object obj)
        {
            PageTitle = "Sales Management";
            // CurrentView = new SalesView();
        }

        private void ShowPurchases(object obj)
        {
            PageTitle = "Purchases Management";
            // CurrentView = new PurchasesView();
        }

        private void ShowAccounting(object obj)
        {
            PageTitle = "Accounting";
            // CurrentView = new AccountingView();
        }

        private void ShowReports(object obj)
        {
            PageTitle = "Reports";
            // CurrentView = new ReportsView();
        }

        private void ShowSettings(object obj)
        {
            PageTitle = "Settings";
            // CurrentView = new SettingsView();
        }

        private void Logout(object obj)
        {
            var result = MessageBox.Show("Are you sure you want to logout?", "Logout Confirmation",
                MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                // Show login window
                var loginWindow = new LoginWindow();
                loginWindow.Show();

                // Close main window
                Application.Current.Windows.OfType<Window>()
                    .FirstOrDefault(w => w is MainWindow)?.Close();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
