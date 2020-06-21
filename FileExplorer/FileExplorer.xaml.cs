using JSharp.PluginCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using FileExplorer;
using FileExplorer.Properties;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using JSharp;

namespace FileExplorer
{
    public class FileItem
    {
        public string Name { get; set; }
        public string ParentDirectory { get; set; }
        public string FullPath { get; set; }
        public bool IsDirectory { get; set; }
        public string ImageSource { get; set; }

        public FileItem(string path, bool isParent)
        {
            FileAttributes attr = File.GetAttributes(path);
            IsDirectory = attr.HasFlag(FileAttributes.Directory);
            FullPath = path;

            if (isParent) Name = "...";

            else Name = IsDirectory ? new DirectoryInfo(FullPath).Name
                            : new FileInfo(FullPath).Name;

            ParentDirectory = Directory.GetParent(FullPath).FullName;

            ImageSource = IsDirectory ? @"/FileExplorer;component/folder.png" : null;
        }
    }
    public partial class FileExplorer : UserControl
    {
        public ObservableCollection<FileItem> Files { get; set; }

        public string SelectedFile { get; set; }

        public FileExplorer()
        {
            InitializeComponent();
            Files = new ObservableCollection<FileItem>();
            listView.ItemsSource = Files;
        }

        private string GetExtension(string file)
        {
            return new FileInfo(file).Extension;
        }

        private void AddListItem(string path)
        {
            try
            {
                Files.Add(new FileItem(path, false));
            }
            catch 
            {
            }
        }

        public void SetDirectory(string dir)
        {
            Files.Clear();

            Files.Add(new FileItem(Directory.GetParent(dir).FullName, true));
            foreach (var directory in Directory.GetDirectories(dir)) AddListItem(directory);

            string[] allowedExtensions = new[] { ".java", ".cs", ".xml", ".xaml", ".html", ".js", ".css", ".py", ".php", ".json", ".txt", ".md" };
            foreach (var file in Directory.GetFiles(dir).Where(x => allowedExtensions.Contains(GetExtension(x))))
                AddListItem(file);

            listView.Items.Refresh();
        }

        private void listView_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            FileItem selectedFile = (FileItem)listView.SelectedItem;
            if(selectedFile.IsDirectory)
            {
                SetDirectory(selectedFile.FullPath);
            }
            else
            {
                Entry.Window.OpenDocument(selectedFile.FullPath);
            }
        }

        private void SelectedContent_IsSelectedChanged(object sender, EventArgs e)
        {
            string selectedTabPath = ((Editor)Entry.Window.Documents.SelectedContent.Content).OpenedDocument;
            listView.SelectedIndex = Files.IndexOf(Files.Where(x => x.FullPath == selectedTabPath).First());
        }

        private void listView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            
        }
    }
}
