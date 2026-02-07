using System.Globalization;
using System.Windows.Data;

namespace SteamHub.Convert
{
    public class InverseEnumToBooleanConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null || parameter == null)
            {
                return false;
            }

            string checkValue = value.ToString();
            string targetValue = parameter.ToString();
            return !checkValue.Equals(targetValue, StringComparison.InvariantCultureIgnoreCase);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return System.Windows.Data.Binding.DoNothing;
        }
    }
}