using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using JSharp.PluginCore;


namespace PluginList
    {
        public class Entry : Plugin
        {
            ViewPluginsWindow viewPluginsWindow;
            MenuItem pluginsMenuItem = new MenuItem();
            MenuItem[] plugins = new MenuItem[1];

            public override MenuItem[] GetMenuItems() => plugins;

            public override void Init()
            {
                Name = "Plugins";
                Description = "Built-In plugin for viewing JSharp plugins";
                Version = "1.0.0.0";

                HasWindow = true;
                AddToMenu = true;

                plugins[0] = pluginsMenuItem;
                pluginsMenuItem.Header = "Show Plugins";
                pluginsMenuItem.Click += PluginsMenuItem_Click;
            }

            private void PluginsMenuItem_Click(object sender, RoutedEventArgs e)
            {
                new ViewPluginsWindow().ShowDialog();
            }

            public override void Unload()
            {
            }

            public override object[] GetToolbarItems()
            {
                return null;
            }

        public override UserControl[] GetPaneControls()
        {
            return new UserControl[] { };
        }
    }
    }


