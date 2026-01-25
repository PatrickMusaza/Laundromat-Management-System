using System.Globalization;
using LaundromatManagementSystem.Models;

namespace LaundromatManagementSystem.Converters
{
    public class GreaterThanZeroConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is int intValue)
                return intValue > 0;

            if (value is System.Collections.ICollection collection)
                return collection.Count > 0;

            return false;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class EqualToZeroConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is int intValue)
                return intValue == 0;

            if (value is System.Collections.ICollection collection)
                return collection.Count == 0;

            return true;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class DecreaseQuantityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is CartItem item)
            {
                // Return a tuple (itemId, newQuantity)
                return (item.Id, Math.Max(1, item.Quantity - 1));
            }
            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class IncreaseQuantityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is CartItem item)
            {
                // Return a tuple (itemId, newQuantity)
                return (item.Id, item.Quantity + 1);
            }
            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}