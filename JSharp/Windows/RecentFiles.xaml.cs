using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace JSharp.Windows
{
    /// <summary>
    /// Interaction logic for RecentFiles.xaml
    /// </summary>
    public partial class RecentFiles : Window
    {
        public RecentFiles()
        {
            InitializeComponent();
            recentList.ItemsSource = Properties.Settings.Default.RecentFiles;
            recentList.MouseDoubleClick += RecentList_MouseDoubleClick;

            if(Properties.Settings.Default.DarkTheme)
            {
                this.Background = PluginHolder.instance.ParentWindow.Background;
                this.Foreground = PluginHolder.instance.ParentWindow.Background;
                recentList.Background = PluginHolder.instance.ParentWindow.Background;
                recentList.Foreground = PluginHolder.instance.ParentWindow.Foreground;
            }
        }

        private void RecentList_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            try
            {
                PluginHolder.instance.ParentWindow.OpenDocument((string)recentList.SelectedItem);
                this.Close();
            }
            catch (Exception)
            {
                // throw;
            }
            
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Properties.Settings.Default.RecentFiles.Clear();
            Properties.Settings.Default.Save();
        }
    }
}
