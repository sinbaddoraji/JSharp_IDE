using System;
using System.IO;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Input;
using System.Collections.ObjectModel;

namespace JSharp
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
            FullPath = path;
            FileAttributes attr = File.GetAttributes(path);
            IsDirectory = (attr & FileAttributes.Directory) != 0;

            Name = isParent
                ? "..."
                : IsDirectory ? new DirectoryInfo(FullPath).Name
                            : new FileInfo(FullPath).Name;

            ParentDirectory = Directory.GetParent(FullPath).FullName;

            ImageSource = IsDirectory ? "/JSharp;component/Images/folder.png" : null;
        }
    }

    public partial class FileExplorer : UserControl
    {
        public ObservableCollection<FileItem> Files { get; set; }

        public string SelectedFile { get; set; }

        public FileExplorer()
        {
            InitializeComponent();
            //Initialize list view item source
            Files = new ObservableCollection<FileItem>();
            listView.ItemsSource = Files;

            //Match parent window background color 
            listView.Background = PluginHolder.instance.ParentWindow.Background;
            listView.Foreground = PluginHolder.instance.ParentWindow.Foreground;
        }

        private string GetExtension(string file) => new FileInfo(file).Extension;

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

        private void ListView_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            FileItem selectedFile = (FileItem)listView.SelectedItem;
            if(selectedFile.IsDirectory)
            {
                SetDirectory(selectedFile.FullPath);
            }
            else
            {
                PluginHolder.instance.ParentWindow.OpenDocument(selectedFile.FullPath);
            }
        }

        private void SelectedContent_IsSelectedChanged(object sender, EventArgs e)
        {
            string selectedTabPath = ((Editor)PluginHolder.instance.ParentWindow.Documents.SelectedContent.Content).OpenedDocument;
            listView.SelectedIndex = Files.IndexOf(item: Files.First(x => x.FullPath == selectedTabPath));
        }
    }
}
