using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows;
using JSharp.PluginCore;

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
            List<PluginItem> pluginItems = new List<PluginItem>();
            listView.ItemsSource = pluginItems;

            pluginItems.AddRange(from item in Entry.Parent.RegisteredPlugins
                                 let newPluginItem = new PluginItem
                                 {
                                     Name = item.Name,
                                     Description = item.Description,
                                     Author = item.Author,
                                     Version = item.Version
                                 }
                                 select newPluginItem);
        }
    }
}
