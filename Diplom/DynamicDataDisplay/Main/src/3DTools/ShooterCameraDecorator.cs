using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Media3D;
using System.Windows;
using System.Diagnostics;
using Microsoft.Research.DynamicDataDisplay.Common.Auxiliary;

namespace _3DTools
{
	public sealed class ShooterCameraDecorator : Viewport3DDecorator
	{
		private readonly Border eventSource = new Border { Background = Brushes.Orange };

		private Vector3D cameraUp = new Vector3D(0, 1, 0);
		private Vector3D cameraDirection = new Vector3D(0, 0, -1);
		private Point3D cameraPosition = new Point3D(0, 0, 16);

		private Vector3D prevCameraDirection = new Vector3D(0, 0, -1);

		public ShooterCameraDecorator()
		{
			PreViewportChildren.Add(eventSource);
		}

		protected override void OnMouseDown(MouseButtonEventArgs e)
		{
			base.OnMouseDown(e);

			//prevPosition2D = e.GetPosition(this);
			//prevPosition3D = ProjectToTrackball(prevPosition2D);

			if (Mouse.Captured == null)
				Mouse.Capture(this, CaptureMode.Element);
		}

		protected override void OnMouseUp(MouseButtonEventArgs e)
		{
			base.OnMouseUp(e);

			if (IsMouseCaptured)
				Mouse.Capture(this, CaptureMode.None);
		}

		protected override void OnMouseMove(MouseEventArgs e)
		{
			base.OnMouseMove(e);

			var currentPosition = ProjectToTrackball(e.GetPosition(this));
			cameraDirection += (currentPosition - prevCameraDirection) / 20;
			prevCameraDirection = currentPosition;

			Window window = Window.GetWindow(this);
			//var shift = (currentPosition - prevPosition3D) / 4;
			//translate.OffsetX += shift.X;
			//translate.OffsetY += shift.Y;
			//translate.OffsetZ += shift.Z;

			Debug.WriteLine(currentPosition);

			e.Handled = true;
			UpdateCamera();
		}

		protected override void OnKeyDown(KeyEventArgs e)
		{
			base.OnKeyDown(e);
		}

		private void UpdateCamera()
		{
			Viewport3D viewport3D = this.Viewport3D;
			if (viewport3D != null && viewport3D.Camera != null)
			{
				if (viewport3D.Camera.IsFrozen)
					viewport3D.Camera = viewport3D.Camera.Clone();

				PerspectiveCamera camera = (PerspectiveCamera)viewport3D.Camera;
				camera.LookDirection = cameraDirection;
				camera.Position = cameraPosition;
				camera.UpDirection = cameraUp;
			}
		}

		private Vector3D ProjectToTrackball(Point point)
		{
			double width = ActualWidth;
			double height = ActualHeight;

			double x = point.X / (width / 2);    // Scale so bounds map to [0,0] - [2,2]
			double y = point.Y / (height / 2);

			x = x - 1;                           // Translate 0,0 to the center
			y = 1 - y;                           // Flip so +Y is up instead of down

			double z2 = 1 - x * x - y * y;       // z^2 = 1 - x^2 - y^2
			double z = z2 > 0 ? Math.Sqrt(z2) : 0;

			return new Vector3D(x, y, z);
		}
	}
}
