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
using Microsoft.Research.DynamicDataDisplay;
using Microsoft.Research.DynamicDataDisplay.ViewportConstraints;
using System.Diagnostics;

namespace VisibleCoercionBug
{
	/// <summary>
	/// Interaction logic for Window1.xaml
	/// </summary>
	public partial class Window1 : Window
	{
		public Window1()
		{
			InitializeComponent();
			plotter.Viewport.PropertyChanged += new EventHandler<ExtendedPropertyChangedEventArgs>(Viewport_PropertyChanged);
			plotter.Constraints.Add(new PhysicalProportionsConstraint());
		}

		private void Viewport_PropertyChanged(object sender, ExtendedPropertyChangedEventArgs e)
		{
			if (e.PropertyName == "Visible")
			{
				Dispatcher.Invoke(new Action<DataRect>(ChangeVisible), e.NewValue);
			}
		}

		private void ChangeVisible(DataRect newVisible)
		{
			// Debug.WriteLine("ChangeVisible: old = " + plotter.Visible);
			// Debug.WriteLine("ChangeVisible: new = " + newVisible);
			if (!Object.Equals(plotter.Visible, newVisible))
				plotter.Visible = newVisible;
			if (!plotter.Visible.Equals(newVisible))
				Debug.WriteLine("ChangedVisible: Visible property is different!");
		}
	}
}
