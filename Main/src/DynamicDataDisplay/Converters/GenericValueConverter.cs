using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;
using System.Globalization;

namespace Microsoft.Research.DynamicDataDisplay.Converters
{
	/// <summary>
	/// Represents a typed value converter. It simplifies life of its sub-class as there no need to
	/// check what types arguments have.
	/// </summary>
	/// <typeparam name="T"></typeparam>
	public class GenericValueConverter<T> : IValueConverter
	{
		public GenericValueConverter() { }

		private Func<T, object> conversion;
		public GenericValueConverter(Func<T, object> conversion)
		{
			if (conversion == null)
				throw new ArgumentNullException("conversion");

			this.conversion = conversion;
		}

		#region IValueConverter Members

		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			if (value is T)
			{
				T genericValue = (T)value;

				object result = ConvertCore(genericValue, targetType, parameter, culture);
				return result;
			}
			return null;
		}

		public virtual object ConvertCore(T value, Type targetType, object parameter, CultureInfo culture)
		{
			if (conversion != null)
			{
				return conversion(value);
			}

			throw new NotImplementedException();
		}

		public virtual object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotSupportedException();
		}

		#endregion
	}
}
