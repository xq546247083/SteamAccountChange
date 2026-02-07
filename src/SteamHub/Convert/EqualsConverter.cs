using System.Globalization;
using System.Windows.Data;

namespace SteamHub.Convert
{
    public class EqualsConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values.Length == 2)
            {
                return values[0]?.Equals(values[1]) ?? values[1] == null;
            }
            return false;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}