using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;
using System.Globalization;
using System.Diagnostics.Contracts;

namespace Microsoft.Research.DynamicDataDisplay.Common.Auxiliary
{
	public class GenericLambdaConverter<TIn, TOut> : IValueConverter
	{
		private readonly Func<TIn, TOut> lambda;

		/// <summary>
		/// Initializes a new instance of the <see cref="GenericLambdaConverter&lt;TIn, TOut&gt;"/> class.
		/// </summary>
		/// <param name="lambda">The lambda.</param>
		public GenericLambdaConverter(Func<TIn, TOut> lambda)
		{
			Contract.Assert(lambda != null);

			this.lambda = lambda;
		}

		#region IValueConverter Members

		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			TIn arg = (TIn)value;
			TOut result = lambda(arg);
			return result;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}

		#endregion
	}
}
