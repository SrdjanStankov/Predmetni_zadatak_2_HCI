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
			if (string.IsNullOrEmpty(findTb.Text) || string.IsNullOrWhiteSpace(findTb.Text))
			{
				MessageBox.Show("Nothing to find!", "Error", MessageBoxButton.OK, MessageBoxImage.Exclamation);
			}
			else
			{
				MainWindow.find = findTb.Text;
				MainWindow.replaceWith = replaceTb.Text;
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
