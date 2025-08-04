using Microsoft.UI.Xaml.Data;

namespace AutoOS.Views.Settings.BIOS;

public partial class BooleanToVisibilityConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, string language)
    {
        bool boolValue = value is bool b && b;
        bool invert = string.Equals(parameter?.ToString(), "invert", StringComparison.OrdinalIgnoreCase);

        if (invert)
            boolValue = !boolValue;

        return boolValue ? Visibility.Visible : Visibility.Collapsed;
    }

    public object ConvertBack(object value, Type targetType, object parameter, string language)
    {
        if (value is Visibility v)
        {
            bool result = v == Visibility.Visible;
            bool invert = string.Equals(parameter?.ToString(), "invert", StringComparison.OrdinalIgnoreCase);

            return invert ? !result : result;
        }
        return false;
    }
}