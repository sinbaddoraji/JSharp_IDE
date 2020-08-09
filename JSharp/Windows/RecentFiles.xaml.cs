using System;
using System.Windows;
using System.Windows.Input;
using JSharp.PluginCore;
using JSharp.Properties;

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
                Background = PluginHolder.Instance.ParentWindow.Background;
                Foreground = PluginHolder.Instance.ParentWindow.Background;
                recentList.Background = PluginHolder.Instance.ParentWindow.Background;
                recentList.Foreground = PluginHolder.Instance.ParentWindow.Foreground;
            }
        }

        private void RecentList_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            try
            {
                PluginHolder.Instance.ParentWindow.OpenDocument((string)recentList.SelectedItem);
                Close();
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
