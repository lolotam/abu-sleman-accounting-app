using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Security;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using AccountingApp.Models;
using AccountingApp.Services;

namespace AccountingApp.ViewModels
{
    public class LoginViewModel : INotifyPropertyChanged
    {
        private readonly AuthService _authService;
        private string _username;
        private string _errorMessage;
        private bool _isLoggingIn;

        public string Username
        {
            get => _username;
            set
            {
                _username = value;
                OnPropertyChanged();
            }
        }

        public string ErrorMessage
        {
            get => _errorMessage;
            set
            {
                _errorMessage = value;
                OnPropertyChanged();
            }
        }

        public bool IsLoggingIn
        {
            get => _isLoggingIn;
            set
            {
                _isLoggingIn = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(IsNotLoggingIn));
            }
        }

        public bool IsNotLoggingIn => !IsLoggingIn;

        public ICommand LoginCommand { get; }

        // Event that fires when login is successful
        public event EventHandler LoginSuccessful;

        public LoginViewModel()
        {
            _authService = new AuthService();
            LoginCommand = new RelayCommand<PasswordBox>(ExecuteLogin, CanExecuteLogin);
        }

        private bool CanExecuteLogin(PasswordBox parameter)
        {
            return !string.IsNullOrWhiteSpace(Username) && parameter != null && parameter.SecurePassword.Length > 0;
        }

        private async void ExecuteLogin(PasswordBox parameter)
        {
            try
            {
                IsLoggingIn = true;
                ErrorMessage = string.Empty;

                var result = await Task.Run(() => _authService.Login(Username, parameter.Password));
                
                if (result != null)
                {
                    // Login successful
                    LoginSuccessful?.Invoke(this, EventArgs.Empty);
                }
                else
                {
                    // Login failed
                    ErrorMessage = "Invalid username or password.";
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = $"An error occurred: {ex.Message}";
            }
            finally
            {
                IsLoggingIn = false;
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    // Simple relay command implementation
    public class RelayCommand<T> : ICommand
    {
        private readonly Action<T> _execute;
        private readonly Predicate<T> _canExecute;

        public RelayCommand(Action<T> execute, Predicate<T> canExecute = null)
        {
            _execute = execute ?? throw new ArgumentNullException(nameof(execute));
            _canExecute = canExecute;
        }

        public bool CanExecute(object parameter)
        {
            return _canExecute == null || _canExecute((T)parameter);
        }

        public void Execute(object parameter)
        {
            _execute((T)parameter);
        }

        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }
    }
}