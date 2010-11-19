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
using Microsoft.Research.DynamicDataDisplay.Charts;
using System.Diagnostics;
using Microsoft.Research.DynamicDataDisplay.Charts.Maps.Network.Yandex;

namespace JamsSample
{
	/// <summary>
	/// Interaction logic for Window1.xaml
	/// </summary>
	public partial class Window1 : Window
	{
		public Window1()
		{
			InitializeComponent();
			dateTimeSelector.SelectedValueChanged += new EventHandler(dateTimeSelector_SelectedValueChanged);
		}

		void dateTimeSelector_SelectedValueChanged(object sender, EventArgs e)
		{
			jamsServer.TrafficTime = dateTimeSelector.SelectedValue;
		}

		private void backBtn_Click(object sender, RoutedEventArgs e)
		{
			jamsServer.TrafficTime = jamsServer.TrafficTime.AddSeconds(-240);
		}

		private void fwdBtn_Click(object sender, RoutedEventArgs e)
		{
			jamsServer.TrafficTime = jamsServer.TrafficTime.AddSeconds(240);
		}

		private void reloadBtn_Click(object sender, RoutedEventArgs e)
		{
			jamsServer.ForceUpdate();
		}

		private void failedBtn_Click(object sender, RoutedEventArgs e)
		{
			int times = (jamsServer.GetRoundedJamsTime(jamsServer.TrafficTime) - YandexTrafficServer.MagicSeconds) / YandexTrafficServer.UpdateDelta;
			Debug.WriteLine("Failed " + jamsServer.TrafficTime + ", " + times);
		}

		private void Window_Loaded(object sender, RoutedEventArgs e)
		{
			dateTimeSelector.Range = new Range<DateTime>(DateTime.Now.AddHours(-24), DateTime.Now);
		}
	}
}
