using System;
using System.Windows;

namespace AccountingApp
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            
            // Initialize database if needed
            // InitializeDatabase();
            
            // Set application culture for localization
            // SetCulture("ar-SA"); // For Arabic
        }
        
        private void InitializeDatabase()
        {
            try
            {
                // Database initialization code will go here
                // using (var context = new AccountingDbContext())
                // {
                //     context.Database.Initialize(force: false);
                // }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error initializing database: {ex.Message}", "Database Error", 
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        
        private void SetCulture(string cultureName)
        {
            try
            {
                // Set application culture
                // var culture = new System.Globalization.CultureInfo(cultureName);
                // System.Threading.Thread.CurrentThread.CurrentCulture = culture;
                // System.Threading.Thread.CurrentThread.CurrentUICulture = culture;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error setting culture: {ex.Message}", "Culture Error", 
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
