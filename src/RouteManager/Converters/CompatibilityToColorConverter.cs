using KE.MSTS.RouteManager;
using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

/// <summary>
/// Converts a <see cref="Compatibility"/> value to a <see cref="Color"/> value.
/// </summary>
[ValueConversion(typeof(Compatibility), typeof(Color))]
internal class CompatibilityToColorConverter : IValueConverter
{
    /// <summary>
    /// Converts a <see cref="Compatibility"/> value to a <see cref="Color"/> value.
    /// </summary>
    /// <param name="value">The <see cref="Compatibility"/> value to convert.</param>
    /// <param name="targetType">The target type of the conversion. This parameter is not used.</param>
    /// <param name="parameter">An optional parameter for the conversion. This parameter is not used.</param>
    /// <param name="culture">The culture to use in the converter. This parameter is not used.</param>
    /// <returns>
    /// A <see cref="Color"/> value corresponding to the <see cref="Compatibility"/> value.
    /// </returns>
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        switch ((Compatibility)value)
        {
            case Compatibility.Full:
                return Colors.Green;
            case Compatibility.Partial:
                return Colors.DodgerBlue;
            case Compatibility.None:
                return Colors.Red;
            case Compatibility.Unknown:
            default:
                return Colors.Black;
        }
    }

    /// <summary>
    /// Method not supported. Throws a <see cref="NotSupportedException"/>.
    /// </summary>
    /// <param name="value">The value that is attempted to be converted back. This parameter is not used.</param>
    /// <param name="targetType">The target type of the conversion. This parameter is not used.</param>
    /// <param name="parameter">An optional parameter for the conversion. This parameter is not used.</param>
    /// <param name="culture">The culture to use in the converter. This parameter is not used.</param>
    /// <returns>Throws a <see cref="NotSupportedException"/>.</returns>
    /// <exception cref="NotSupportedException">Thrown unconditionally.</exception>
    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotSupportedException();
    }
}
