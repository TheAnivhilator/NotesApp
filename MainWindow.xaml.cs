using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using static System.Console;

namespace NotesApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Create_sheet()
        {
            TextBlock sheetName = new TextBlock
            {
                Text = "New Sheet"
            };
            sheetName.MouseRightButtonDown += RenameTab;

            Button close = new Button();
            close.Content = "x";
            close.Margin = new System.Windows.Thickness
            {
                Left = 2,
                Top = 0,
                Right = 2,
                Bottom = 2
            };
            close.BorderThickness = new System.Windows.Thickness
            {
                Left = 0,
                Top = 0,
                Right = 0,
                Bottom = 0
            };
            close.Padding = new System.Windows.Thickness
            {
                Left = 2,
                Top = 0,
                Right = 2,
                Bottom = 1
            };
            close.Background = new SolidColorBrush(Colors.Transparent);
            close.Click += CloseTab;

            WrapPanel header = new WrapPanel();
            header.Orientation = Orientation.Horizontal;

            TextBox content = new TextBox
            {
                TextWrapping = TextWrapping.Wrap,
                AcceptsReturn = true,
                AcceptsTab = true,
                VerticalScrollBarVisibility = ScrollBarVisibility.Auto
            };

            header.Children.Add(sheetName);
            header.Children.Add(close);

            TabItem newItem = new TabItem
            {
                Header = header,
                Content = content
            };

            tab.Items.Add(newItem);
            tab.SelectedItem = newItem;
        }

        private void RenameTab(object sender, RoutedEventArgs e)
        {
            TextBlock name = sender as TextBlock;
            WrapPanel wrap = name.Parent as WrapPanel;
            TabItem tabItem = wrap.Parent as TabItem;
            tab.SelectedItem = tabItem;

            ///////////////////////////////////////////////
            TextBox newName = new TextBox()
            {
                Width = double.NaN,
                Height = double.NaN
            };
            Button confirm = new Button()
            {
                Width = double.NaN,
                Height = double.NaN
            };
            confirm.Content = "Rename";
            confirm.Click += Confirm;

            StackPanel content = new StackPanel()
            {
                Width = double.NaN,
                Height = double.NaN
            };
            content.Children.Add(newName);
            content.Children.Add(confirm);

            Window renameWindow = new Window()
            {
                Width = 300,
                Height = 200
            };
            renameWindow.Content = content;
            renameWindow.Show();
        }

        private void Confirm(object sender, RoutedEventArgs e)
        {
            TabItem item = tab.SelectedItem as TabItem;
            WrapPanel header = item.Header as WrapPanel;
            TextBlock name = header.Children[0] as TextBlock;
            
            Button confirm = sender as Button;
            StackPanel content = confirm.Parent as StackPanel;
            TextBox newName = content.Children[0] as TextBox;

            name.Text = newName.Text;

            Window renameWindow = content.Parent as Window;
            renameWindow.Close();
        }

        private void New_file_Click(object sender, RoutedEventArgs e)
        {
            Create_sheet();
        }

        private void Open_file(object sender, RoutedEventArgs e)
        {
            OpenFileDialog fileDialog = new OpenFileDialog();
            fileDialog.Filter = "Text files | *.txt";
            bool? result = fileDialog.ShowDialog();

            if (result == true)
            {
                string path = fileDialog.FileName;
                using (StreamReader sr = new StreamReader(path))
                {
                    Create_sheet();

                    TabItem tabItem = tab.SelectedItem as TabItem;
                    TextBox sheet = tabItem.Content as TextBox;

                    while (!sr.EndOfStream)
                    {
                        string line = sr.ReadLine();
                        sheet.Text += line;
                    }
                }
            }
        }

        private void Save_file(object sender, RoutedEventArgs e)
        {
            var path = System.IO.Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "\\SavedNotes";
            if (Directory.Exists(path))
            {
                TabItem tabItem = tab.SelectedItem as TabItem;
                WrapPanel wrap = tabItem.Header as WrapPanel;
                TextBlock tabName = wrap.Children[0] as TextBlock;
                string name = tabName.Text;
                var filePath = path + "\\" + name + ".txt";
                if (File.Exists(filePath))
                {
                    MessageBoxResult result = MessageBox.Show("?", "There already is a file with the same name. Do you want to replace or overwrite it?", MessageBoxButton.YesNo);
                    if(result == MessageBoxResult.Yes)
                    {
                        TextBox text = tabItem.Content as TextBox;
                        using (StreamWriter sw = new StreamWriter(filePath))
                        {
                            sw.Write(text.Text);
                        }
                    }
                    else
                    {
                        MessageBox.Show("Well, then you should rename the file.");
                    }
                }
                else
                {
                    TextBox text = tabItem.Content as TextBox;
                    using (StreamWriter sw = new StreamWriter(filePath))
                    {
                        sw.Write(text.Text);
                    }
                }
            }
            else
            {
                Directory.CreateDirectory(path);
            }
        }

        private void CloseTab(object sender, RoutedEventArgs e)
        {
            Button close = sender as Button;
            WrapPanel header = close.Parent as WrapPanel;
            TabItem sheet = header.Parent as TabItem;
            tab.Items.Remove(sheet);
        }
    }
}
