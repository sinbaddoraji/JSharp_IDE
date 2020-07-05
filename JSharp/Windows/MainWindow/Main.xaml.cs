using System;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using JSharp.PluginCore;
using JSharp.Properties;
using JSharp.TextEditor;
using MahApps.Metro.Controls;

namespace JSharp.Windows.MainWindow
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class Main : MetroWindow
    {
        /*
         * This handles the events of the main window (Strictly)
         */

        public Main()
        {
            InitializeComponent();
            LoadPlugins();
            SetWindowTheme(Settings.Default.DarkTheme);
            AddInbuiltPanes();
            InitalizePanes();
            
            Editor.FilterOptions = "Java Files (*.java)|*.java|Other Files (*.*)|*.*";
            openFileDialog = new OpenFileDialog
            {
                Filter = Editor.FilterOptions
            };

            for (int i = 1; i <= 100; i++) ZoomValue.Items.Add(i);

            var previouslyOpenedDocuments = GetOpenedFiles(true);
            OpenDocuments(previouslyOpenedDocuments);

            string[] args = Environment.GetCommandLineArgs();
            if (args.Length > 1)
            {
                for (int i = 1; i < args.Length; i++)
                {
                    OpenDocument(args[i]);
                }
            }
        }

       

        private void SelectedContent_Closed(object sender, EventArgs e)
        {
            //OpenedFiles.Remove(GetSelectedDocument().OpenedDocument);
        }

        private void Open_Click(object sender, RoutedEventArgs e)
        {
            if (openFileDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                OpenDocuments(openFileDialog.FileNames);
        }

        private void SaveAs_Click(object sender, RoutedEventArgs e)
        {
            GetSelectedDocument().SaveAs();
        }

        private void ZoomValue_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            GetSelectedDocument().FontSize = (int)ZoomValue.SelectedItem;
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            GetSelectedDocument().SaveDocument();
        }

        private void Window_Closing(object sender, CancelEventArgs e)
        {
            //Unload all registered plugins incase none managable objects were used in plugins
            PluginHolder.Instance.UnloadAllRegisteredPlugins();

            GetOpenedFiles(false);
        }

        private void New_Click(object sender, RoutedEventArgs e)
        {
            AddDocumentPage("untitled");
        }

        private void SaveAll_Click(object sender, RoutedEventArgs e)
        {
            Parallel.ForEach(DocumentPane.Children, document
                =>
            { ((Editor)document.Content).SaveDocument(); });
        }

        private void Exit_Click(object sender, RoutedEventArgs e)
        {
            Environment.Exit(0);
        }

        private void Settings_Click(object sender, RoutedEventArgs e)
        {
            new JSharp.MainWindow.Settings().ShowDialog();
            SetWindowTheme(Settings.Default.DarkTheme);
        }

        private void Recents_Click(object sender, RoutedEventArgs e)
        {
            (new RecentFiles()).ShowDialog();
        }
    }
}