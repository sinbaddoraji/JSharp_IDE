using System.Windows;
using System.Windows.Forms;
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
            jdkBox.Text = Properties.Settings.Default.JdkPath;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            SetDarkMode(darkTheme.IsChecked == true);
            Properties.Settings.Default.Save();
            Close();

            MessageBox.Show("You may have to restart JSharp");
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            using (var folderDialog = new FolderBrowserDialog())
            {
                if (folderDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    SetJdkPath(folderDialog.SelectedPath);
                    jdkBox.Text = folderDialog.SelectedPath;
                }
            }
        }

    }
}
