using JSharp.PluginCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace PluginList
{
    public class PluginItem
    {
        public string Name { get; set; }

        public string Description { get; set; }

        public string Author { get; set; }

        public string Version { get; set; }
    }

    public partial class ViewPluginsWindow : Window
    {
        public ViewPluginsWindow()
        {
            InitializeComponent();

            string pluginPath = Directory.GetCurrentDirectory() + @"\"; ;

            List<PluginItem> pluginItems = new List<PluginItem>();
            listView.ItemsSource = pluginItems;

            if (!Directory.Exists(pluginPath)) Directory.CreateDirectory(pluginPath);

            String[] pluginFiles = Directory.GetFiles(pluginPath, "*.dll");
            string[] exludedFiles = new[] { "AvalonDock.Themes.VS2013.dll", "AvalonDock.dll", "ICSharpCode.AvalonEdit.dll", "JSharp.dll" };

            for (int i = 0; i < pluginFiles.Length; i++)
            {
                string pluginFileName = new FileInfo(pluginFiles[i]).Name;
                if (exludedFiles.Contains(pluginFileName))
                    continue;

                string plugin = pluginFiles[i];
                Assembly asm = Assembly.LoadFile(plugin);
                if (asm != null)
                {
                    Type objType = null;
                    Type[] types = asm.GetExportedTypes();

                    for (int j = 0; j < asm.GetExportedTypes().Length; j++)
                    {
                        if (types[j].Name == "Entry") objType = types[j];
                    }

                    if (objType != null)
                    {
                        var ipi = (Plugin)Activator.CreateInstance(objType);
                        ipi.Init();

                        PluginItem newPluginItem = new PluginItem
                        {
                            Name = ipi.Name,
                            Description = ipi.Description,
                            Author = ipi.Author,
                            Version = ipi.Version
                        };

                        pluginItems.Add(newPluginItem);
                    }
                }
            }
        }
    }
}
