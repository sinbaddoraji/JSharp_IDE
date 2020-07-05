using System.Windows;
using System.Windows.Controls;
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


