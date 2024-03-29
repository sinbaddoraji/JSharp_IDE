﻿using System;
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
using JdbWrapper;
using System.Windows.Input;

namespace JSharp.Windows.MainWindow
{
    /// <inheritdoc>
    ///     <cref>MainWIndow</cref>
    /// </inheritdoc>
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        /*
         * This handles the events of the main window (Strictly)
         */

        /// <inheritdoc />
        /// <summary>
        /// Main window for JSharp
        /// </summary>
        public MainWindow()
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
                        if(File.Exists(file))
                        {
                            OpenDocument(file);
                        }
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

        public AboutDialog AboutDialog
        {
            get => default;
            set
            {
            }
        }

        public RecentFiles RecentFiles
        {
            get => default;
            set
            {
            }
        }

        public Settings Settings
        {
            get => default;
            set
            {
            }
        }

        public GotoDialog GotoDialog
        {
            get => default;
            set
            {
            }
        }

        public PluginHolder PluginHolder
        {
            get => default;
            set
            {
            }
        }

        private async void Initalize()
        {
            await LoadPlugins().ConfigureAwait(false);
            UseDarkTheme(Properties.Settings.Default.DarkTheme);
            await InitalizePanes().ConfigureAwait(false);
            await AddInbuiltPanes().ConfigureAwait(false);
        }

        private void Open_Click(object sender, ExecutedRoutedEventArgs e)
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

        private void Save_Click(object sender, ExecutedRoutedEventArgs e)
        {
            GetSelectedTextEditor().SaveDocument();
        }

        private void Window_Closing(object sender, CancelEventArgs e)
        {
            try
            {
                PerformClosingCommands();
            }
            finally
            {
                System.Windows.Application.Current.Shutdown();
            }
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

        private void SaveAll_Click(object sender, ExecutedRoutedEventArgs e)
        {
            SaveAllDocuments();
        }

        private void Exit_Click(object sender, RoutedEventArgs e)
        {
            Environment.Exit(0);
        }

        private void Settings_Click(object sender, RoutedEventArgs e)
        {
            new Settings().ShowDialog();
            UseDarkTheme(Properties.Settings.Default.DarkTheme);
        }

        private void Recents_Click(object sender, RoutedEventArgs e)
        {
            new RecentFiles().ShowDialog();
        }

        private void BuildClick_1(object sender, ExecutedRoutedEventArgs e)
        {
            SaveAllDocuments();
            DebugCore.Compile(GetSelectedFile(false));
        }

        private void Run_Click(object sender, ExecutedRoutedEventArgs e)
        {
            DebugCore.Run(GetSelectedFile(false));
        }

        private void BuildAndRun_Click(object sender, ExecutedRoutedEventArgs e)
        {
            SaveAllDocuments();
            DebugCore.CompileProject(true);
        }

        private void BuildProject_Click(object sender, ExecutedRoutedEventArgs e)
        {
            SaveAllDocuments();
            DebugCore.CompileProject(false);
        }

        private void DebugRun_Click(object sender, RoutedEventArgs e)
        {
            SaveAllDocuments();
            DebugCore.RunInDebugger();
        }

        private void Open_Project_Folder_Click(object sender, RoutedEventArgs e)
        {
            using(FolderBrowserDialog f = new FolderBrowserDialog())
            {
                if(f.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    SetProjectFolder(f.SelectedPath);
                }
            }
        }

        private void Goto_Click(object sender, RoutedEventArgs e)
        {
            new GotoDialog().ShowDialog();
        }

        private void CreateJar_Click(object sender, RoutedEventArgs e)
        {
            SaveAllDocuments();
            DebugCore.CreatePackage();
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