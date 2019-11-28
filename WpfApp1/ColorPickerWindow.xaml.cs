using System.Windows;
using System.Windows.Media;

namespace WpfApp1
{
    /// <summary>
    /// Interaction logic for ColorPickerWindow.xaml
    /// </summary>
    public partial class ColorPickerWindow : Window
    {
        private Color color;

        public ColorPickerWindow()
        {
            InitializeComponent();
            sliderR.Value = 0;
            sliderG.Value = 0;
            sliderB.Value = 0;
            SetColor();
            rectangle.Fill = new SolidColorBrush(color);
            labelHexVal.Content = "#FF000000";
        }

        private void SetColor()
        {
            color = Color.FromArgb(255, (byte)sliderR.Value, (byte)sliderG.Value, (byte)sliderB.Value);
        }

        private void slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            SetColor();
            rectangle.Fill = new SolidColorBrush(color);
            labelHexVal.Content = "#" + color.A.ToString("X2") + color.R.ToString("X2") + color.G.ToString("X2") + color.B.ToString("X2");
            labelSliderRVal.Content = (int)sliderR.Value;
            labelSliderGVal.Content = (int)sliderG.Value;
            labelSliderBVal.Content = (int)sliderB.Value;
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
