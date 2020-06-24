using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using MessageBox = System.Windows.Forms.MessageBox;

namespace JSharp.MainWindow
{
    /// <summary>
    /// Interaction logic for Settings.xaml
    /// </summary>
    public partial class Settings : Window
    {
        public static string GetJdkPath()
        {
            return Properties.Settings.Default.JdkPath;
        }

        public static void SetJdkPath(string value)
        {
            Properties.Settings.Default.JdkPath = value;
        }

        private bool GetDarkMode()
        {
            return Properties.Settings.Default.DarkTheme;
        }

        private void SetDarkMode(bool value)
        {
            Properties.Settings.Default.DarkTheme = value;
        }

        public Settings()
        {
            InitializeComponent();
            darkTheme.IsChecked = GetDarkMode();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            SetJdkPath(jdkBox.Text);
            SetDarkMode(darkTheme.IsChecked == true);
            Properties.Settings.Default.Save();
            this.Close();

            MessageBox.Show("You may have to restart JSharp");
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            using (var folderDialog = new FolderBrowserDialog())
            {
                if (folderDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    SetJdkPath(folderDialog.SelectedPath);
                }
            }
        }

    }
}
