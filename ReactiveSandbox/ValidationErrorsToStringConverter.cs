using System;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Markup;

namespace ReactiveSandbox;

/// <summary>
/// Segít elkerülni, hogyha nincs validálási hiba, akkor a 
/// {Binding RelativeSource={RelativeSource Self},Path=(Validation.Errors)[0].ErrorContent}
/// binding hibára fusson:
/// Cannot get ‘Item[]‘ value (type ‘ValidationError’) from ‘(Validation.Errors)’
/// 
/// innen: https://wpftutorial.net/DataValidation.html
/// </summary>
public class ValidationErrorsToStringConverter : MarkupExtension, IValueConverter
{
    public override object ProvideValue(IServiceProvider serviceProvider)
    {
        return new ValidationErrorsToStringConverter();
    }

    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is ReadOnlyObservableCollection<ValidationError> errors)
            return string.Join(Environment.NewLine, errors.Select(e => e.ErrorContent as string));

        return string.Empty;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
