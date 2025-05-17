using AccountingApp.Database;
using AccountingApp.Models;
using AccountingApp.Views;
using System;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;

namespace AccountingApp.ViewModels
{
    public class LoginViewModel : INotifyPropertyChanged
    {
        private string _username;
        private string _errorMessage;
        private bool _isLoading;

        public string Username
        {
            get { return _username; }
            set
            {
                _username = value;
                OnPropertyChanged();
            }
        }

        public string ErrorMessage
        {
            get { return _errorMessage; }
            set
            {
                _errorMessage = value;
                OnPropertyChanged();
            }
        }

        public bool IsLoading
        {
            get { return _isLoading; }
            set
            {
                _isLoading = value;
                OnPropertyChanged();
            }
        }

        public ICommand LoginCommand { get; }

        public LoginViewModel()
        {
            LoginCommand = new RelayCommand(Login, CanLogin);
        }

        private bool CanLogin(object parameter)
        {
            var passwordBox = parameter as System.Windows.Controls.PasswordBox;
            return !string.IsNullOrEmpty(Username) && 
                   passwordBox != null && 
                   !string.IsNullOrEmpty(passwordBox.Password) && 
                   !IsLoading;
        }

        private void Login(object parameter)
        {
            var passwordBox = parameter as System.Windows.Controls.PasswordBox;
            if (passwordBox == null) return;

            string password = passwordBox.Password;
            
            IsLoading = true;
            ErrorMessage = string.Empty;

            try
            {
                using (var context = new AccountingDbContext())
                {
                    var user = context.Users
                        .FirstOrDefault(u => u.Username == Username && u.IsActive);

                    if (user != null && user.VerifyPassword(password))
                    {
                        // Update last login time
                        user.LastLogin = DateTime.Now;
                        context.SaveChanges();

                        // Open main window
                        var mainWindow = new MainWindow(user);
                        mainWindow.Show();

                        // Close login window
                        Application.Current.Windows.OfType<LoginWindow>().FirstOrDefault()?.Close();
                    }
                    else
                    {
                        ErrorMessage = "Invalid username or password";
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Login error: {ex.Message}";
            }
            finally
            {
                IsLoading = false;
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    public class RelayCommand : ICommand
    {
        private readonly Action<object> _execute;
        private readonly Predicate<object> _canExecute;

        public RelayCommand(Action<object> execute, Predicate<object> canExecute = null)
        {
            _execute = execute ?? throw new ArgumentNullException(nameof(execute));
            _canExecute = canExecute;
        }

        public bool CanExecute(object parameter)
        {
            return _canExecute == null || _canExecute(parameter);
        }

        public void Execute(object parameter)
        {
            _execute(parameter);
        }

        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }
    }
}
