using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Media.Media3D;
using Microsoft.Research.DynamicDataDisplay;
using Microsoft.Research.DynamicDataDisplay.DataSources.MultiDimensional;
using Microsoft.Research.DynamicDataDisplay.Common.Auxiliary;
using Microsoft.Research.DynamicDataDisplay.DataSources;

namespace Lines3D
{
	/// <summary>
	/// Interaction logic for SectionControl.xaml
	/// </summary>
	public partial class SectionControl : UserControl
	{
		public SectionControl()
		{
			InitializeComponent();

			Loaded += new RoutedEventHandler(SectionControl_Loaded);
		}

		void SectionControl_Loaded(object sender, RoutedEventArgs e)
		{
			UpdateSlice(0.5);
		}

		private void slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
		{
			UpdateSlice(e.NewValue);
		}

		private void UpdateSlice(double value)
		{
			value /= 100;
			SliderPercentage = value;
			SliderValueChanged.Raise(this);

			if (DataContext == null)
				return;

			var dataSource3D = (IUniformDataSource3D<Vector3D>)DataContext;

			var slice = CreateSection(dataSource3D, value);
			plotter.DataContext = slice;
			isoline.DataSource = slice.GetMagnitudeDataSource();
		}

		private IDataSource2D<Vector> CreateSection(IUniformDataSource3D<Vector3D> dataSource, double ratio)
		{
			switch (Variable)
			{
				case SectionVariable.X:
					return dataSource.CreateSectionYZ(ratio);
				case SectionVariable.Y:
					return dataSource.CreateSectionXZ(ratio);
				case SectionVariable.Z:
					return dataSource.CreateSectionXY(ratio);
				default:
					throw new NotImplementedException();
			}
		}

		#region Properties

		public double SliderPercentage
		{
			get { return (double)GetValue(SliderPercentageProperty); }
			set { SetValue(SliderPercentageProperty, value); }
		}

		public static readonly DependencyProperty SliderPercentageProperty = DependencyProperty.Register(
		  "SliderPercentage",
		  typeof(double),
		  typeof(SectionControl),
		  new FrameworkPropertyMetadata(0.5));

		public SectionVariable Variable
		{
			get { return (SectionVariable)GetValue(VariableProperty); }
			set { SetValue(VariableProperty, value); }
		}

		public static readonly DependencyProperty VariableProperty = DependencyProperty.Register(
		  "Variable",
		  typeof(SectionVariable),
		  typeof(SectionControl),
		  new FrameworkPropertyMetadata(SectionVariable.X));

		public string Header
		{
			get { return (string)GetValue(HeaderProperty); }
			set { SetValue(HeaderProperty, value); }
		}

		public static readonly DependencyProperty HeaderProperty = DependencyProperty.Register(
		  "Header",
		  typeof(string),
		  typeof(SectionControl),
		  new FrameworkPropertyMetadata("X ratio:", FrameworkPropertyMetadataOptions.Inherits));
		

		#endregion Properties

		public event EventHandler SliderValueChanged;
	}
}
