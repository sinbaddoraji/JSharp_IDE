﻿using System;
using System.Collections;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using AvalonDock.Layout;
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
            Initalize();

            Editor.FilterOptions = "Java Files (*.java)|*.java|Other Files (*.*)|*.*";
            openFileDialog = new OpenFileDialog { Filter = Editor.FilterOptions };

            for (int i = 1; i <= 100; i++) ZoomValue.Items.Add(i);

            if(Settings.Default.OpenedFiles == null)
            {
                Settings.Default.OpenedFiles = new System.Collections.Specialized.StringCollection();
                Settings.Default.Save();
            }
            else
            {
                foreach (string file in (IList)Settings.Default.OpenedFiles)
                {
                    try
                    {
                        OpenDocument(file);
                    }
                    catch (Exception e)
                    {
                        //MessageBox.Show(e.Message);
                    }
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
            SetWindowTheme(Settings.Default.DarkTheme);
            await AddInbuiltPanes().ConfigureAwait(false);
            await InitalizePanes().ConfigureAwait(false);
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
            var selectedDocument = GetSelectedDocument();
            if(selectedDocument != null)
                selectedDocument.FontSize = (int)ZoomValue.SelectedItem;
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            GetSelectedDocument().SaveDocument();
        }

        private void Window_Closing(object sender, CancelEventArgs e)
        {
            //Unload all registered plugins incase none managable objects were used in plugins
            PluginHolder.Instance.UnloadAllRegisteredPlugins();

            SaveAllDocuments();

            Settings.Default.OpenedFiles.Clear();
            foreach (var document in DocumentPane.Children)
            {
                var child = ((Editor)document.Content);
                
                if(child.OpenedDocument != null)
                    Settings.Default.OpenedFiles.Add(child.OpenedDocument);
            }

            Settings.Default.Save();
        }

        private void New_Click(object sender, RoutedEventArgs e)
        {
            Editor edit = new Editor();
            edit.SaveAs();

            AddDocumentPage(edit.OpenedDocumentShortName, edit);
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
            new JSharp.MainWindow.Settings().ShowDialog();
            SetWindowTheme(Settings.Default.DarkTheme);
        }

        private void Recents_Click(object sender, RoutedEventArgs e)
        {
            new RecentFiles().ShowDialog();
        }

        private void BuildClick_1(object sender, RoutedEventArgs e)
        {
            DebugCore.Compile(this.GetSelectedFile(false));
        }

        private void Run_Click(object sender, RoutedEventArgs e)
        {
            DebugCore.RunFile(this.GetSelectedFile(false));
        }

        private void BuildAndRun_Click(object sender, RoutedEventArgs e)
        {
            DebugCore.CompileProject(true);
        }

        private void RunProject_Click(object sender, RoutedEventArgs e)
        {
            DebugCore.RunProject();
            
        }
        private void BuildProject_Click(object sender, RoutedEventArgs e)
        {
            DebugCore.CompileProject(false);
        }

        private void DebugRun_Click(object sender, RoutedEventArgs e)
        {
            DebugCore.RunInDebugger(this.GetSelectedFile(false));
        }

        private void DockManager_ActiveContentChanged(object sender, EventArgs e)
        {
           //this.Title = GetSelectedFile(true);
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

        private void ClassAnalyser_click(object sender, RoutedEventArgs e)
        {
            //new ClassAnalyser().ShowDialog();
        }


        private void CreateJar_Click(object sender, RoutedEventArgs e)
        {
            DebugCore.CreatePackage();
        }
    }
}