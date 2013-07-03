using System;
using System.Globalization;
using System.Windows.Data;
using Microsoft.Research.DynamicDataDisplay;

namespace SynchronizedPlotters
{
    public sealed class XConverter : IValueConverter
    {
        private readonly Viewport2D viewport;

        public XConverter( Viewport2D viewport )
        {
            this.viewport = viewport;
        }

        public object Convert( object value, Type targetType, object parameter, CultureInfo culture )
        {
            DataRect source = (DataRect)value;
            return source.WithY( viewport.Visible.YMin, viewport.Visible.YMax );
        }

        public object ConvertBack( object value, Type targetType, object parameter, CultureInfo culture )
        {
            throw new NotSupportedException();
        }
    }
}