using AccountingApp.Models;
using AccountingApp.ViewModels;
using System.Linq;
using System.Windows;
using System.Windows.Data;

namespace AccountingApp.Views
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow(User user)
        {
            InitializeComponent();
            DataContext = new MainViewModel(user);
        }
    }
    
    public class InitialsConverter : IValueConverter
    {
        public object Convert(object value, System.Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value is string fullName && !string.IsNullOrEmpty(fullName))
            {
                var nameParts = fullName.Split(' ');
                if (nameParts.Length > 1)
                {
                    return $"{nameParts[0][0]}{nameParts[1][0]}".ToUpper();
                }
                else if (nameParts.Length == 1 && nameParts[0].Length > 0)
                {
                    return nameParts[0][0].ToString().ToUpper();
                }
            }
            return "U";
        }

        public object ConvertBack(object value, System.Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return null;
        }
    }
}
