using System;
using System.Collections;
using System.Collections.Specialized;
using System.ComponentModel;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Xml;
using AvalonDock.Layout;
using JSharp.PluginCore;

namespace JSharp.Windows.MainWindow
{
    /// <inheritdoc>
    ///     <cref>MainWIndow</cref>
    /// </inheritdoc>
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class Main
    {
        /*
         * This handles the events of the main window (Strictly)
         */

        /// <inheritdoc />
        /// <summary>
        /// Main window for JSharp
        /// </summary>
        public Main()
        {
            InitializeComponent();
            Initalize();

            openFileDialog = new OpenFileDialog { Filter = TextEditor.TextEditor.FilterOptions };

            for (int i = 1; i <= 100; i++) ZoomValue.Items.Add(i);

            if(Properties.Settings.Default.OpenedFiles == null)
            {
                Properties.Settings.Default.OpenedFiles = new StringCollection();
                Properties.Settings.Default.Save();
            }
            else
            {
                foreach (string file in (IList)Properties.Settings.Default.OpenedFiles)
                {
                    try
                    {
                        OpenDocument(file);
                    }
                    catch {}
                }
            }
            

            string[] args = Environment.GetCommandLineArgs();
            if (args.Length > 1)
            {
                for (int i = 1; i < args.Length; i++)
                {
                    OpenDocument(args[i]);
                }
            }
        }

        private async void Initalize()
        {
            await LoadPlugins().ConfigureAwait(false);
            UseDarkTheme(Properties.Settings.Default.DarkTheme);
            await InitalizePanes().ConfigureAwait(false);
            await AddInbuiltPanes().ConfigureAwait(false);
           

        }

        private void Open_Click(object sender, RoutedEventArgs e)
        {
            if (openFileDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                OpenDocuments(openFileDialog.FileNames);
        }

        private void SaveAs_Click(object sender, RoutedEventArgs e)
        {
            GetSelectedTextEditor().SaveAs();
        }

        private void ZoomValue_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var selectedDocument = GetSelectedTextEditor();
            if(selectedDocument != null)
                selectedDocument.FontSize = (int)ZoomValue.SelectedItem;
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            GetSelectedTextEditor().SaveDocument();
        }

        private void Window_Closing(object sender, CancelEventArgs e)
        {
            //Unload all registered plugins incase none managable objects were used in plugins
            PluginHolder.Instance.UnloadAllRegisteredPlugins();

            SaveAllDocuments();

            Properties.Settings.Default.OpenedFiles.Clear();
            foreach (var document in DocumentPane.Children)
            {
                var child = ((TextEditor.TextEditor)document.Content);
                
                if(child.OpenedDocument != null)
                    Properties.Settings.Default.OpenedFiles.Add(child.OpenedDocument);
            }

            UpdatePanes();
            //Write layout
            XmlWriter xmlWriter = XmlWriter.Create("layout.xml");
            xmlWriter.WriteStartDocument();
            xmlWriter.WriteStartElement("panes");

            foreach (var pane in dictionaryOfPanes)
            {
                xmlWriter.WriteStartElement("pane");
                xmlWriter.WriteAttributeString("title", pane.Key);
                xmlWriter.WriteAttributeString("paneLocation", pane.Value.paneLocation.ToString());
                xmlWriter.WriteAttributeString("isCollapsed", pane.Value.isCollapsed.ToString());
                xmlWriter.WriteEndElement();
            }

            xmlWriter.WriteEndElement();
            xmlWriter.WriteEndDocument();
            xmlWriter.Close();

            Properties.Settings.Default.Save();
        }

        private void New_Click(object sender, RoutedEventArgs e)
        {
            TextEditor.TextEditor edit = new TextEditor.TextEditor();
            edit.SaveAs();

            if(!edit.IsUnoccupied())
            {
                AddDocumentPage(edit.OpenedDocumentShortName, edit);
            }
            
        }

        private void SaveAll_Click(object sender, RoutedEventArgs e)
        {
            SaveAllDocuments();
        }

        private void Exit_Click(object sender, RoutedEventArgs e)
        {
            Environment.Exit(0);
        }

        private void Settings_Click(object sender, RoutedEventArgs e)
        {
            //new Settings().ShowDialog();
            UseDarkTheme(Properties.Settings.Default.DarkTheme);
        }

        private void Recents_Click(object sender, RoutedEventArgs e)
        {
            new RecentFiles().ShowDialog();
        }

        private void BuildClick_1(object sender, RoutedEventArgs e)
        {
            DebugCore.Compile(GetSelectedFile(false));
        }

        private void Run_Click(object sender, RoutedEventArgs e)
        {
            DebugCore.Run(GetSelectedFile(false));
        }

        private void BuildAndRun_Click(object sender, RoutedEventArgs e)
        {
            DebugCore.CompileProject(true);
        }

        private void BuildProject_Click(object sender, RoutedEventArgs e)
        {
            DebugCore.CompileProject(false);
        }

        private void DebugRun_Click(object sender, RoutedEventArgs e)
        {
            DebugCore.RunInDebugger(GetSelectedFile(false));
        }

        private void Open_Project_Folder_Click(object sender, RoutedEventArgs e)
        {
            using(FolderBrowserDialog f = new FolderBrowserDialog())
            {
                if(f.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    ProjectFolder = f.SelectedPath;
                    fileExplorer.SetDirectory(ProjectFolder);
                }
            }
        }

        private void Goto_Click(object sender, RoutedEventArgs e)
        {
            new GotoDialog().ShowDialog();
        }

        private void CreateJar_Click(object sender, RoutedEventArgs e)
        {
            DebugCore.CreatePackage();
        }

        private void NewDocument_IsSelectedChanged(object sender, EventArgs e)
        {
            var doc = (LayoutDocument)sender;
            if (!doc.IsSelected) return;
            var data = ((TextEditor.TextEditor)doc.Content).OpenedDocumentShortName;
            Title = data != null ? $"JSharp ({data})" : "JSharp";

            try
            {
                var dir = Directory.GetParent(GetSelectedFile(false)).FullName;
                fileExplorer.SetDirectory(dir);
                ProjectFolder = dir;
            }
            catch 
            {
            }
        }

        private void About_Click(object sender, RoutedEventArgs e)
        {
            new AboutDialog().ShowDialog();
        }

        private void ClearOutput_Click(object sender, RoutedEventArgs e)
        {
            DebugCore.OutputTextbox.Clear();
        }
    }
}