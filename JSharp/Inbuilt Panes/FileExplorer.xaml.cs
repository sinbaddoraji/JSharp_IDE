﻿using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Windows.Input;
using System.Windows.Media;
using JSharp.PluginCore;

namespace JSharp.Inbuilt_Panes
{
    public class FileItem
    {
        public string Name { get; }
        public string ParentDirectory { get; }
        public string FullPath { get; }
        public bool IsDirectory { get; }
        public string ImageSource { get; }

        public FileItem(string path, bool isParent)
        {
            FullPath = path;
            var attr = File.GetAttributes(path);
            IsDirectory = (attr & FileAttributes.Directory) != 0;

            Name = isParent
                ? "..."
                : IsDirectory ? new DirectoryInfo(FullPath).Name
                            : new FileInfo(FullPath).Name;

            ParentDirectory = Directory.GetParent(FullPath).FullName;

            ImageSource = IsDirectory ? "/JSharp;component/Images/folder.png" : null;
        }
    }

    public partial class FileExplorer
    {
        private ObservableCollection<FileItem> Files { get; }

        public FileExplorer()
        {
            InitializeComponent();
            //Initialize list view item source
            Files = new ObservableCollection<FileItem>();
            ListView.ItemsSource = Files;

            //Match parent window background colour 
            if(Properties.Settings.Default.DarkTheme)
            {
                ListView.Background = PluginHolder.Instance.ParentWindow.Background;
                ListView.Foreground = Brushes.White;
            }
            
        }

        private static string GetExtension(string file) => new FileInfo(file).Extension;

        private void AddListItem(string path)
        {
            try
            {
                Files.Add(new FileItem(path, false));
            }
            catch
            {
                // ignored
            }
        }

        public void SetDirectory(string dir)
        {
            Files.Clear();

            Files.Add(new FileItem(Directory.GetParent(dir).FullName, true));
            foreach (var directory in Directory.GetDirectories(dir)) AddListItem(directory);

            string[] allowedExtensions = { ".java", ".cs", ".xml", ".xaml", ".html", ".js", ".css", ".py", ".php", ".json", ".txt", ".md" };
            foreach (var file in Directory.GetFiles(dir).Where(x => allowedExtensions.Contains(GetExtension(x))))
                AddListItem(file);

            ListView.Items.Refresh();
        }

        private void ListView_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            var selectedFile = (FileItem)ListView.SelectedItem;
            if(selectedFile.IsDirectory)
            {
                SetDirectory(selectedFile.FullPath);
            }
            else
            {
                PluginHolder.Instance.ParentWindow.OpenDocument(selectedFile.FullPath);
            }
        }
    }
}
