using System;
using System.Windows;
using System.Globalization;
using System.Threading;
using System.IO;

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

            // Configure Arabic culture support
            ConfigureCulture();

            // Global exception handling
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
            Current.DispatcherUnhandledException += Current_DispatcherUnhandledException;

            // Initialize database if needed
            InitializeDatabase();
        }

        private void ConfigureCulture()
        {
            // Get user preferred culture from settings or use system default
            try
            {
                // For Arabic (ar-SA)
                // CultureInfo ci = new CultureInfo("ar-SA");
                // Thread.CurrentThread.CurrentCulture = ci;
                // Thread.CurrentThread.CurrentUICulture = ci;
                
                // For English (en-US)
                CultureInfo ci = new CultureInfo("en-US");
                Thread.CurrentThread.CurrentCulture = ci;
                Thread.CurrentThread.CurrentUICulture = ci;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error setting culture: {ex.Message}", "Culture Error", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void InitializeDatabase()
        {
            try
            {
                // Create the data directory if it doesn't exist
                string dataDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Data");
                Directory.CreateDirectory(dataDir);
                
                // Initialize database connection or create database
                // Database.AccountingDbContext.Initialize();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error initializing database: {ex.Message}", "Database Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            Exception ex = e.ExceptionObject as Exception;
            LogError(ex);
            MessageBox.Show($"An unexpected error occurred: {ex?.Message}\n\nThe application will now close.", 
                "Fatal Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }

        private void Current_DispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        {
            LogError(e.Exception);
            MessageBox.Show($"An error occurred: {e.Exception.Message}", 
                "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            e.Handled = true;
        }

        private void LogError(Exception ex)
        {
            try
            {
                string logDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Logs");
                Directory.CreateDirectory(logDir);

                string logFile = Path.Combine(logDir, $"error_{DateTime.Now:yyyyMMdd}.log");
                using (StreamWriter writer = File.AppendText(logFile))
                {
                    writer.WriteLine($"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] Error: {ex?.Message}");
                    writer.WriteLine($"Stack Trace: {ex?.StackTrace}");
                    writer.WriteLine(new string('-', 80));
                }
            }
            catch
            {
                // Can't log the error - nothing more we can do
            }
        }
    }
}