using Microsoft.Research.DynamicDataDisplay;
using Microsoft.Research.DynamicDataDisplay.ViewportConstraints;

namespace SynchronizedPlotters
{
    public sealed class EqualXConstraint : ViewportConstraint
    {
        private readonly Viewport2D other;

        public EqualXConstraint(Viewport2D other)
        {
            this.other = other;
        }

        public override DataRect Apply( DataRect previousDataRect, DataRect proposedDataRect, Viewport2D viewport )
        {
            DataRect dataRect = proposedDataRect.WithY(other.Visible.YMin, other.Visible.YMax);
            if (dataRect != other.Visible)
            {
                other.Visible = dataRect;
            }
            return proposedDataRect;
        }
    }
}