using System.Windows;

namespace WpfApp1
{
	/// <summary>
	/// Interaction logic for FindReplaceWindow.xaml
	/// </summary>
	public partial class FindReplaceWindow : Window
	{
		public FindReplaceWindow()
		{
			InitializeComponent();
		}

		private void button_Click(object sender, RoutedEventArgs e)
		{
			if (textBox.Text == "")
			{
				MessageBox.Show("Nothing to find!", "Error", MessageBoxButton.OK, MessageBoxImage.Exclamation);
			}
			else
			{
				MainWindow.find = textBox.Text;
				MainWindow.replaceWith = textBox_Copy.Text;
				DialogResult = true;
				Close();
			}
		}

		private void button_Copy_Click(object sender, RoutedEventArgs e)
		{
			DialogResult = false;
			Close();
		}
	}
}
