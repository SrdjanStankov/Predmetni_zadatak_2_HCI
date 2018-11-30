using System;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using Microsoft.Win32;

namespace WpfApp1
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		public static BindingList<double> FontSizes { get; set; }
		public static RoutedCommand commands { get; set; } = new RoutedCommand();
		public static string find;
		public static string replaceWith;
		public static Color color;

		private bool firstTime = false;
		private bool wasChanged = false;
		private bool firstSave = true;
		private bool wasSaved = false;
		private OpenFileDialog openDialog;
		private SaveFileDialog saveDialog;

		public MainWindow()
		{
			InitializeComponent();
			cmbFontFamily.ItemsSource = Fonts.SystemFontFamilies.OrderBy(f => f.Source);
			DataContext = this;
			FontSizes = new BindingList<double>();
			PopulateFontSizes();
			commands.InputGestures.Add(new KeyGesture(Key.F5));
			openDialog = new OpenFileDialog()
			{
				Filter = "Files|*.rtf"
			};
			saveDialog = new SaveFileDialog()
			{
				Filter = "Files|*.rtf"
			};
			lbWordCount.Content = "Broj reci: 0";
			rtbEditor.AcceptsReturn = true;
			rtbEditor.AcceptsTab = true;
			rtbEditor.IsUndoEnabled = true;
		}

		private void PopulateFontSizes()
		{
			for (int i = 8 ; i <= 28 ; i++)
			{
				if (i > 12)
				{
					i++;
				}
				FontSizes.Add(i);
			}
			FontSizes.Add(36);
			FontSizes.Add(48);
			FontSizes.Add(72);
		}

		private void cmbFontFamily_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			if (cmbFontFamily.SelectedItem != null)
			{
				rtbEditor.Selection.ApplyPropertyValue(Inline.FontFamilyProperty, cmbFontFamily.SelectedItem);
			}
		}

		private void rtbEditor_SelectionChanged(object sender, RoutedEventArgs e)
		{
			object temp;

			temp = rtbEditor.Selection.GetPropertyValue(Inline.FontWeightProperty);
			btnBold.IsChecked = (temp != DependencyProperty.UnsetValue) && (temp.Equals(FontWeights.Bold));

			temp = rtbEditor.Selection.GetPropertyValue(Inline.FontStyleProperty);
			btnItalic.IsChecked = (temp != DependencyProperty.UnsetValue) && (temp.Equals(FontStyles.Italic));

			temp = rtbEditor.Selection.GetPropertyValue(Inline.TextDecorationsProperty);
			btnUnderline.IsChecked = (temp != DependencyProperty.UnsetValue) && (temp.Equals(TextDecorations.Underline));

			if (!rtbEditor.Selection.IsEmpty || !firstTime)
			{
				temp = rtbEditor.Selection.GetPropertyValue(Inline.FontFamilyProperty);
				cmbFontFamily.SelectedItem = temp;

				temp = rtbEditor.Selection.GetPropertyValue(Inline.FontSizeProperty);
				cmbFontSize.SelectedItem = temp;
			}
			firstTime = true;
		}

		private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
		{
			DragMove();
		}

		private void cmbFontSize_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			if (cmbFontSize.SelectedItem != null)
			{
				rtbEditor.Selection.ApplyPropertyValue(Inline.FontSizeProperty, cmbFontSize.SelectedItem);
			}
		}

		private void Color_Click(object sender, RoutedEventArgs e)
		{
			ColorPickerWindow window = new ColorPickerWindow
			{
				Left = PointToScreen(Mouse.GetPosition(this)).X,
				Top = PointToScreen(Mouse.GetPosition(this)).Y
			};
			window.ShowDialog();

			if (window.DialogResult.Value)
			{
				if (!rtbEditor.Selection.IsEmpty)
				{
					rtbEditor.Selection.ApplyPropertyValue(Inline.ForegroundProperty, new SolidColorBrush(color));
					wasChanged = true;
				}
			}
		}

		private void SaveDocument_Click(object sender, RoutedEventArgs e)
		{
			SelectAll();
			if (firstSave)
			{
				if (rtbEditor.Selection.Text == Environment.NewLine)
				{
					MessageBox.Show("Nema sta da se sacuva.", "Prazno polje", MessageBoxButton.OK, MessageBoxImage.Asterisk);
					return;
				}
				if (saveDialog.ShowDialog() == true)
				{
					FileStream fs = new FileStream(saveDialog.FileName, FileMode.OpenOrCreate);
					rtbEditor.Selection.Save(fs, DataFormats.Rtf);
					fs.Dispose();
					wasChanged = false;
					firstSave = false;
					wasSaved = true;
				}
			}
			else
			{
				if (!wasChanged)
				{
					return;
				}
				FileStream fs = new FileStream(saveDialog.FileName, FileMode.Open);
				rtbEditor.Selection.Save(fs, DataFormats.Rtf);
				fs.Dispose();
				wasChanged = false;
				firstSave = false;
				wasSaved = true;
			}
			Deselect();
		}



		private void OpenDocument_Click(object sender, RoutedEventArgs e)
		{
			if (!wasSaved && wasChanged)
			{
				MessageBoxResult mbres = MessageBox.Show("Sve sto nije sacuvano bice izgubljeno." + Environment.NewLine + "Da li ste sigurni da zelite da otvorite fajl?", "Nije sacuvano", MessageBoxButton.YesNoCancel, MessageBoxImage.Exclamation);
				if (mbres != MessageBoxResult.Yes)
				{
					return;
				}
			}
			if (openDialog.ShowDialog() == true)
			{
				SelectAll();
				rtbEditor.Selection.Text = "";
				FileStream fs = new FileStream(openDialog.FileName, FileMode.Open);
				rtbEditor.Selection.Load(fs, DataFormats.Rtf);
				fs.Dispose();
				saveDialog.FileName = openDialog.FileName;
				wasChanged = false;
				firstSave = false;
				wasSaved = true;
				Deselect();
			}
		}

		private void NewDocument_Click(object sender, RoutedEventArgs e)
		{
			if (!wasSaved && wasChanged)
			{
				MessageBoxResult mbres = MessageBox.Show("Sve sto nije sacuvano bice izgubljeno" + Environment.NewLine + "Da li ste sigurni da zelite da otvorite novi fajl?", "Nije sacuvano", MessageBoxButton.YesNoCancel, MessageBoxImage.Exclamation);
				if (mbres != MessageBoxResult.Yes)
				{
					return;
				}
			}
			SelectAll();
			rtbEditor.Selection.Text = "";
			firstSave = true;
			wasChanged = false;
			wasSaved = false;
			Deselect();
		}

		private void FindAndReplace_Click(object sender, RoutedEventArgs e)
		{
			FindReplaceWindow w = new FindReplaceWindow
			{
				Left = PointToScreen(Mouse.GetPosition(this)).X,
				Top = PointToScreen(Mouse.GetPosition(this)).Y
			};
			w.ShowDialog();

			if (w.DialogResult.Value)
			{
				StreamReader reader;
				string s;
				rtbEditor.SelectAll();

				using (MemoryStream memoryStream = new MemoryStream())
				{
					rtbEditor.Selection.Save(memoryStream, DataFormats.Rtf);
					reader = new StreamReader(memoryStream);
					reader.BaseStream.Seek(0, SeekOrigin.Begin);
					s = reader.ReadToEnd();
				}
				s = System.Text.RegularExpressions.Regex.Replace(s, find, replaceWith);
				using (MemoryStream memoryStream = new MemoryStream(Encoding.ASCII.GetBytes(s)))
				{
					rtbEditor.Selection.Load(memoryStream, DataFormats.Rtf);
				}
				//MessageBox.Show(s);
				wasChanged = true;
				wasSaved = false;
			}
		}

		private void InsertDateTime_Click(object sender, RoutedEventArgs e)
		{
			rtbEditor.Selection.Text = DateTime.Now.ToShortDateString();
			wasChanged = true;
		}

		private void Close_Click(object sender, RoutedEventArgs e)
		{
			if (wasChanged && !wasSaved)
			{
				MessageBoxResult mbres = MessageBox.Show("Sve sto nije sacuvano bice izgubljeno" + Environment.NewLine + "Da li ste sigurni da zelite da izadjete?", "Nije sacuvano", MessageBoxButton.YesNoCancel, MessageBoxImage.Exclamation);
				if (mbres != MessageBoxResult.Yes)
				{
					return;
				}
			}
			Close();
		}

		private void rtbEditor_TextChanged(object sender, TextChangedEventArgs e)
		{
			wasChanged = true;

			TextRange textRange = new TextRange(rtbEditor.Document.ContentStart, rtbEditor.Document.ContentEnd);

			string[] temp = textRange.Text.Split(' ', '\n', '\t');
			int count = -1;

			foreach (string item in temp)
			{
				if (item != "")
				{
					count++;
				}
			}

			lbWordCount.Content = "Broj reci: " + count;
		}
		private void SelectAll()
		{
			bool tmp = wasChanged;
			rtbEditor.SelectAll();
			wasChanged = tmp;
		}

		private void Deselect()
		{
			rtbEditor.Selection.Select(rtbEditor.Selection.End, rtbEditor.Selection.End);
			wasChanged = false;
		}
	}
}
