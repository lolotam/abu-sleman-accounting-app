using System;
using System.Windows;
using System.Windows.Controls;
using AccountingApp.Services;

namespace AccountingApp.Views
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            
            // Set user info
            if (AuthService.CurrentUser != null)
            {
                TxtUserName.Text = AuthService.CurrentUser.FullName;
                TxtUserRole.Text = AuthService.CurrentUser.Role;
            }
            
            // Navigate to dashboard by default
            NavigateToDashboard();
        }

        private void MenuButton_Click(object sender, RoutedEventArgs e)
        {
            Button clickedButton = sender as Button;
            
            // Update page title
            TxtPageTitle.Text = clickedButton.Name.Replace("Btn", "");
            
            // Handle navigation based on button clicked
            switch (clickedButton.Name)
            {
                case "BtnDashboard":
                    NavigateToDashboard();
                    break;
                    
                case "BtnProducts":
                case "BtnCustomers":
                case "BtnSuppliers":
                case "BtnSales":
                case "BtnPurchases":
                case "BtnAccounting":
                case "BtnReports":
                case "BtnSettings":
                    // For now, show "Coming Soon" for all other pages
                    ShowComingSoonPage(clickedButton.Name.Replace("Btn", ""));
                    break;
            }
        }

        private void BtnLogout_Click(object sender, RoutedEventArgs e)
        {
            var result = MessageBox.Show("Are you sure you want to logout?", 
                "Logout Confirmation", MessageBoxButton.YesNo, MessageBoxImage.Question);
                
            if (result == MessageBoxResult.Yes)
            {
                // Logout user
                new AuthService().Logout();
                
                // Navigate back to login
                var loginWindow = new LoginWindow();
                loginWindow.Show();
                this.Close();
            }
        }
        
        private void NavigateToDashboard()
        {
            // In a real application, this would navigate to a real dashboard page
            // For this demo, let's just show a placeholder
            TextBlock content = new TextBlock
            {
                Text = "Welcome to the Dashboard!",
                FontSize = 24,
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center
            };
            
            MainFrame.Content = content;
        }
        
        private void ShowComingSoonPage(string pageName)
        {
            TextBlock content = new TextBlock
            {
                Text = $"The {pageName} module is coming soon!",
                FontSize = 24,
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center
            };
            
            MainFrame.Content = content;
        }
    }
}