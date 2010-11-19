//---------------------------------------------------------------------------
//
// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Limited Permissive License.
// See http://www.microsoft.com/resources/sharedsource/licensingbasics/limitedpermissivelicense.mspx
// All other rights reserved.
//
// This file is part of the 3D Tools for Windows Presentation Foundation
// project.  For more information, see:
// 
// http://CodePlex.com/Wiki/View.aspx?ProjectName=3DTools
//
// The following article discusses the mechanics behind this
// trackball implementation: http://viewport3d.com/trackball.htm
//
// Reading the article is not required to use this sample code,
// but skimming it might be useful.
//
//---------------------------------------------------------------------------

using System;
using System.Collections;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;
using System.Windows.Shapes;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Controls.Primitives; // IAddChild, ContentPropertyAttribute

namespace _3DTools
{
	public class TrackballDecorator : Viewport3DDecorator
	{
		public TrackballDecorator()
		{
			// the transform that will be applied to the viewport 3d's camera
			transform = new Transform3DGroup();
			transform.Children.Add(scale);
			transform.Children.Add(new RotateTransform3D(rotation));

			// used so that we always get events while activity occurs within
			// the viewport3D
			eventSource = new Border();
			eventSource.Background = Brushes.Transparent;

			PreViewportChildren.Add(eventSource);

			Canvas canvas = new Canvas();
			RepeatButton zoomInButton = new RepeatButton
			{
				Content = "+",
				VerticalContentAlignment = VerticalAlignment.Center,
				HorizontalContentAlignment = HorizontalAlignment.Center,
				Width = 30,
				Height = 30,
				Background = Brushes.Transparent
			};
			Canvas.SetRight(zoomInButton, 10);
			Canvas.SetTop(zoomInButton, 10);
			canvas.Children.Add(zoomInButton);
			zoomInButton.Click += ZoomInButton_Click;

			RepeatButton zoomOutButton = new RepeatButton
			{
				Content = "-",
				VerticalContentAlignment = VerticalAlignment.Center,
				HorizontalContentAlignment = HorizontalAlignment.Center,
				Width = 30,
				Height = 30,
				Background = Brushes.Transparent
			};
			Canvas.SetRight(zoomOutButton, 10);
			Canvas.SetTop(zoomOutButton, 50);
			canvas.Children.Add(zoomOutButton);
			zoomOutButton.Click += ZoomOutButton_Click;

			Button xPlus = CreateNavigationButton("+X", 10, 10);
			xPlus.Click += new RoutedEventHandler(xPlus_Click);
			canvas.Children.Add(xPlus);

			Button xMinus = CreateNavigationButton("-X", 40, 10);
			xMinus.Click += new RoutedEventHandler(xMinus_Click);
			canvas.Children.Add(xMinus);

			Button yPlus = CreateNavigationButton("+Y", 10, 40);
			yPlus.Click += new RoutedEventHandler(yPlus_Click);
			canvas.Children.Add(yPlus);

			Button yMinus = CreateNavigationButton("-Y", 40, 40);
			yMinus.Click += new RoutedEventHandler(yMinus_Click);
			canvas.Children.Add(yMinus);

			Button zPlus = CreateNavigationButton("+Z", 10, 70);
			zPlus.Click += new RoutedEventHandler(zPlus_Click);
			canvas.Children.Add(zPlus);

			Button zMinus = CreateNavigationButton("-Z", 40, 70);
			zMinus.Click += new RoutedEventHandler(zMinus_Click);
			canvas.Children.Add(zMinus);

			PreViewportChildren.Add(canvas);
		}

		void xPlus_Click(object sender, RoutedEventArgs e)
		{
			rotation.Angle = 90;
			rotation.Axis = new Vector3D(0, 1, 0);
			UpdateCamera();
		}

		void xMinus_Click(object sender, RoutedEventArgs e)
		{
			rotation.Angle = 270;
			rotation.Axis = new Vector3D(0, 1, 0);
			UpdateCamera();
		}

		void yPlus_Click(object sender, RoutedEventArgs e)
		{
			rotation.Angle = 270;
			rotation.Axis = new Vector3D(1, 0, 0);
			UpdateCamera();
		}

		void yMinus_Click(object sender, RoutedEventArgs e)
		{
			rotation.Angle = 90;
			rotation.Axis = new Vector3D(1, 0, 0);
			UpdateCamera();
		}

		void zPlus_Click(object sender, RoutedEventArgs e)
		{
			rotation.Angle = 0;
			rotation.Axis = new Vector3D(0, 1, 0);
			UpdateCamera();
		}

		void zMinus_Click(object sender, RoutedEventArgs e)
		{
			rotation.Angle = 180;
			rotation.Axis = new Vector3D(0, 1, 0);
			UpdateCamera();
		}

		private Button CreateNavigationButton(object content, double left, double top)
		{
			Button button = new Button
			{
				Content = content,
				VerticalContentAlignment = VerticalAlignment.Center,
				HorizontalContentAlignment = HorizontalAlignment.Center,
				Width = 26,
				Height = 26,
				Background = Brushes.Transparent
			};
			Canvas.SetLeft(button, left);
			Canvas.SetTop(button, top);

			return button;
		}

		private void ZoomOutButton_Click(object sender, RoutedEventArgs e)
		{
			MouseWheelZoom(keyboardZoomDelta);
			UpdateCamera();
			e.Handled = true;
		}

		private void ZoomInButton_Click(object sender, RoutedEventArgs e)
		{
			MouseWheelZoom(-keyboardZoomDelta);
			UpdateCamera();
			e.Handled = true;
		}

		/// <summary>
		///     A transform to move the camera or scene to the trackball's
		///     current orientation and scale.
		/// </summary>
		public Transform3D Transform
		{
			get { return transform; }
		}

		#region Event Handling

		protected override void OnMouseDown(MouseButtonEventArgs e)
		{
			base.OnMouseDown(e);

			if (!subscribed)
			{
				Window window = Window.GetWindow(this);
				window.KeyDown += new KeyEventHandler(window_KeyDown);
				subscribed = true;
			}

			previousPosition2D = e.GetPosition(this);
			previousPosition3D = ProjectToTrackball(ActualWidth,
													 ActualHeight,
													 previousPosition2D);
			if (Mouse.Captured == null)
				Mouse.Capture(this, CaptureMode.Element);

			eventSource.Focus();
		}

		private bool subscribed = false;
		private const double vertRotationAngle = 6;
		private const int keyboardZoomDelta = 200;
		private void window_KeyDown(object sender, KeyEventArgs e)
		{
			bool handled = false;
			if (e.Key == Key.OemPlus)
			{
				MouseWheelZoom(-keyboardZoomDelta);
				handled = true;
			}
			else if (e.Key == Key.OemMinus)
			{
				MouseWheelZoom(keyboardZoomDelta);
				handled = true;
			}
			else if (e.Key == Key.Right)
			{
				UpdateRotationY(vertRotationAngle);
				handled = true;
			}
			else if (e.Key == Key.Left)
			{
				UpdateRotationY(-vertRotationAngle);
				handled = true;
			}
			else if (e.Key == Key.Up)
			{
				// new NotImplementedException();
				handled = true;
			}

			if (handled)
			{
				UpdateCamera();
				e.Handled = true;
			}
		}

		private void UpdateRotationXZ(double angleInDegrees) { }

		private void UpdateRotationY(double angleInDegrees)
		{
			double finalAngle = rotation.Angle + angleInDegrees;
			if (finalAngle < 0)
				finalAngle += 360;
			else if (finalAngle > 360)
				finalAngle -= 360;
			rotation.Angle = finalAngle;
		}

		private void UpdatePosition(Vector3D currentPosition3D)
		{
			Vector3D axis = Vector3D.CrossProduct(previousPosition3D, currentPosition3D);
			double angle = Vector3D.AngleBetween(previousPosition3D, currentPosition3D);

			// quaterion will throw if this happens - sometimes we can get 3D positions that
			// are very similar, so we avoid the throw by doing this check and just ignoring
			// the event 
			if (axis.Length == 0) return;

			Quaternion delta = new Quaternion(axis, -angle);

			// Get the current orientantion from the RotateTransform3D
			AxisAngleRotation3D r = rotation;
			Quaternion q = new Quaternion(rotation.Axis, rotation.Angle);

			// Compose the delta with the previous orientation
			q *= delta;

			// Write the new orientation back to the Rotation3D
			rotation.Axis = q.Axis;
			rotation.Angle = q.Angle;

			previousPosition3D = currentPosition3D;
		}

		protected override void OnMouseUp(MouseButtonEventArgs e)
		{
			base.OnMouseUp(e);

			if (IsMouseCaptured)
			{
				Mouse.Capture(this, CaptureMode.None);
			}
		}

		protected override void OnMouseMove(MouseEventArgs e)
		{
			base.OnMouseMove(e);

			if (IsMouseCaptured)
			{
				Point currentPosition = e.GetPosition(this);

				// avoid any zero axis conditions
				if (currentPosition == previousPosition2D) return;

				// Prefer tracking to zooming if both buttons are pressed.
				if (e.LeftButton == MouseButtonState.Pressed)
				{
					Track(currentPosition);
				}
				else if (e.RightButton == MouseButtonState.Pressed)
				{
					Zoom(currentPosition);
				}

				previousPosition2D = currentPosition;

				Viewport3D viewport3D = this.Viewport3D;
				UpdateCamera();
			}
		}

		protected override void OnMouseWheel(MouseWheelEventArgs e)
		{
			base.OnMouseWheel(e);

			Point currentPosition = e.GetPosition(this);

			MouseWheelZoom(-e.Delta);

			previousPosition2D = currentPosition;
			UpdateCamera();
		}

		private void UpdateCamera()
		{
			Viewport3D viewport3D = this.Viewport3D;
			if (viewport3D != null && viewport3D.Camera != null)
			{
				if (viewport3D.Camera.IsFrozen)
					viewport3D.Camera = viewport3D.Camera.Clone();

				if (viewport3D.Camera.Transform != transform)
					viewport3D.Camera.Transform = transform;
			}
		}

		#endregion Event Handling

		private void Track(Point currentPosition)
		{
			Vector3D currentPosition3D = ProjectToTrackball(
				ActualWidth, ActualHeight, currentPosition);

			Vector3D axis = Vector3D.CrossProduct(previousPosition3D, currentPosition3D);
			double angle = Vector3D.AngleBetween(previousPosition3D, currentPosition3D);

			// quaterion will throw if this happens - sometimes we can get 3D positions that
			// are very similar, so we avoid the throw by doing this check and just ignoring
			// the event 
			if (axis.Length == 0) return;

			Quaternion delta = new Quaternion(axis, -angle);

			// Get the current orientantion from the RotateTransform3D
			AxisAngleRotation3D r = rotation;
			Quaternion q = new Quaternion(rotation.Axis, rotation.Angle);

			// Compose the delta with the previous orientation
			q *= delta;

			// Write the new orientation back to the Rotation3D
			rotation.Axis = q.Axis;
			rotation.Angle = q.Angle;

			previousPosition3D = currentPosition3D;
		}

		private Vector3D ProjectToTrackball(double width, double height, Point point)
		{
			double x = point.X / (width / 2);    // Scale so bounds map to [0,0] - [2,2]
			double y = point.Y / (height / 2);

			x = x - 1;                           // Translate 0,0 to the center
			y = 1 - y;                           // Flip so +Y is up instead of down

			double z2 = 1 - x * x - y * y;       // z^2 = 1 - x^2 - y^2
			double z = z2 > 0 ? Math.Sqrt(z2) : 0;

			return new Vector3D(x, y, z);
		}

		private void MouseWheelZoom(int delta)
		{
			double scaleFactor = Math.Exp(delta / (double)Mouse.MouseWheelDeltaForOneLine / 10);

			scale.ScaleX *= scaleFactor;
			scale.ScaleY *= scaleFactor;
			scale.ScaleZ *= scaleFactor;
		}

		private void Zoom(Point currentPosition)
		{
			double yDelta = currentPosition.Y - previousPosition2D.Y;

			double scaleFactor = Math.Exp(yDelta / 100);    // e^(yDelta/100) is fairly arbitrary.

			scale.ScaleX *= scaleFactor;
			scale.ScaleY *= scaleFactor;
			scale.ScaleZ *= scaleFactor;
		}

		//--------------------------------------------------------------------
		//
		// Private data
		//
		//--------------------------------------------------------------------

		private Point previousPosition2D;
		private Vector3D previousPosition3D = new Vector3D(0, 0, 1);

		private Transform3DGroup transform;
		private ScaleTransform3D scale = new ScaleTransform3D();
		private AxisAngleRotation3D rotation = new AxisAngleRotation3D();

		private Border eventSource;
	}
}
