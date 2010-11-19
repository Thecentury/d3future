using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Windows;
using System.Windows.Shapes;
using System.Windows.Media;
using Microsoft.Research.DynamicDataDisplay.Maps.Charts.VectorFields.Convolution;
using System.Windows.Threading;
using Microsoft.Research.DynamicDataDisplay.Charts;
using Microsoft.Research.DynamicDataDisplay.Common.Auxiliary;
using System.Windows.Media.Effects;

namespace Microsoft.Research.DynamicDataDisplay.Maps.Charts.VectorFields
{
	public abstract class ParticlesEmitterBase : VectorFieldChartBase
	{
		protected int width;
		protected int height;
		protected readonly Stopwatch stopwatch = new Stopwatch();
		protected TimeSpan prevUpdateTime = new TimeSpan();
		protected TimeSpan prevEmitTime = new TimeSpan();
		protected TimeSpan emitDelta = TimeSpan.FromSeconds(20);
		protected double particleVelocity = 0.05;
		protected readonly List<FrameworkElement> particles = new List<FrameworkElement>();
		protected UniformField2DWrapper fieldWrapper;
		protected readonly DispatcherTimer timer = new DispatcherTimer(DispatcherPriority.Send) { Interval = TimeSpan.FromMilliseconds(40) };
		protected int maxParticlesCount = 100;
		protected DataRect bounds;

		protected ParticlesEmitterBase()
		{
			timer.Tick += OnTimerTick;
			prevEmitTime = -emitDelta;
			panel.UseContentBounds = false;
		}

		protected virtual void OnTimerTick(object sender, EventArgs e)
		{
			UpdateParticles(width, height);
		}

		private Func<FrameworkElement> createParticle = () => new Ellipse
		{
			Width = 8,
			Height = 8,
			Fill = Brushes.White.MakeTransparent(0.4),
			Stroke = Brushes.Blue,
			StrokeThickness = 1
		};

		public Func<FrameworkElement> CreateParticle
		{
			get { return createParticle; }
			set { createParticle = value; }
		}

		private PointSetPattern pattern = new RandomPattern();
		public PointSetPattern Pattern
		{
			get { return pattern; }
			set { pattern = value; }
		}

		protected Point PointToViewport(Point p)
		{
			Point result = new Point();
			result.X = p.X * bounds.Width + bounds.XMin;
			result.Y = p.Y * bounds.Height + bounds.YMin;

			return result;
		}

		protected Point PointFromViewport(Point p)
		{
			Point result = new Point();

			result.X = (p.X - bounds.XMin) / bounds.Width;
			result.Y = (p.Y - bounds.YMin) / bounds.Height;

			return result;
		}

		protected override void RebuildUI()
		{
			if (Plotter == null)
				return;
			if (DataSource == null)
				return;

			width = DataSource.Width;
			height = DataSource.Height;
			fieldWrapper = new UniformField2DWrapper(DataSource.Data);

			bounds = DataSource.Grid.GetGridBounds();
			particleVelocity = (width + height) * 0.0001;
			Viewport2D.SetContentBounds(this, bounds);

			UpdateParticles(width, height);
		}

		protected void UpdateParticles(int width, int height)
		{
			if (fieldWrapper == null)
				return;

			TimeSpan updateTime = stopwatch.Elapsed;
			var dt = updateTime.TotalMilliseconds - prevUpdateTime.TotalMilliseconds;
			prevUpdateTime = updateTime;
			var particlesArray = particles.ToArray();
			foreach (var particle in particlesArray)
			{
				Point position = new Point(ViewportPanel.GetX(particle), ViewportPanel.GetY(particle));
				position = PointFromViewport(position);
				var vector = fieldWrapper.GetVector(position.X, position.Y).ChangeLength(1.0 / width, 1.0 / height);

				var shift = particleVelocity * vector * dt;
				position += shift;

				var viewportPosition = PointToViewport(position);

				if (viewportPosition.X < bounds.XMin || viewportPosition.X > bounds.XMax || viewportPosition.Y < bounds.YMin || viewportPosition.Y > bounds.YMax)
				{
					particles.Remove(particle);
					panel.Children.Remove(particle);
					continue;
				}

				ViewportPanel.SetX(particle, viewportPosition.X);
				ViewportPanel.SetY(particle, viewportPosition.Y);
			}
		}

		public override void OnPlotterAttached(Plotter plotter)
		{
			base.OnPlotterAttached(plotter);
			stopwatch.Start();
			timer.Start();
		}

		public override void OnPlotterDetaching(Plotter plotter)
		{
			stopwatch.Reset();
			particles.Clear();
			timer.Stop();
			base.OnPlotterDetaching(plotter);
		}

	}
}
