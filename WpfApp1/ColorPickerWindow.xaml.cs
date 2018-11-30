using System;
using System.Windows;
using System.Windows.Media;

namespace WpfApp1
{
	/// <summary>
	/// Interaction logic for ColorPickerWindow.xaml
	/// </summary>
	public partial class ColorPickerWindow : Window
	{
		Color color;

		public ColorPickerWindow()
		{
			InitializeComponent();
			slider.Value = 0;
			slider2.Value = 0;
			slider3.Value = 0;
			SetColor();
			rectangle.Fill = new SolidColorBrush(color);
			label1.Content = "#FF000000";
		}

		private void SetColor()
		{
			color = Color.FromArgb(255, (byte) slider.Value, (byte) slider2.Value, (byte) slider3.Value);
		}

		private void slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
		{
			SetColor();
			rectangle.Fill = new SolidColorBrush(color);
			label1.Content = "#" + color.A.ToString("X2") + color.R.ToString("X2") + color.G.ToString("X2") + color.B.ToString("X2");
			labelSlider.Content = (int) slider.Value;
			labelSlider2.Content = (int) slider2.Value;
			labelSlider3.Content = (int) slider3.Value;
		}

		private void button_Click(object sender, RoutedEventArgs e)
		{
			SetColor();
			MainWindow.color = color;
			DialogResult = true;
			Close();
		}

		private void button_Copy_Click(object sender, RoutedEventArgs e)
		{
			DialogResult = false;
			Close();
		}
	}
}
