using System;
using System.Windows;
using AccountingApp.ViewModels;

namespace AccountingApp.Views
{
    /// <summary>
    /// Interaction logic for LoginWindow.xaml
    /// </summary>
    public partial class LoginWindow : Window
    {
        private LoginViewModel _viewModel;

        public LoginWindow()
        {
            InitializeComponent();
            
            _viewModel = new LoginViewModel();
            _viewModel.LoginSuccessful += ViewModel_LoginSuccessful;
            
            DataContext = _viewModel;
            
            // Set focus to username field
            Loaded += (s, e) => txtUsername.Focus();
            
            // Allow pressing Enter in password field to submit
            txtPassword.KeyDown += (s, e) => {
                if (e.Key == System.Windows.Input.Key.Enter && _viewModel.IsNotLoggingIn)
                {
                    _viewModel.LoginCommand.Execute(txtPassword);
                }
            };
        }

        private void ViewModel_LoginSuccessful(object sender, EventArgs e)
        {
            // Open the main window
            var mainWindow = new MainWindow();
            mainWindow.Show();
            
            // Close the login window
            this.Close();
        }
    }
}