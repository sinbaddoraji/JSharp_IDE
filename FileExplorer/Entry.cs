using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
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
using JSharp;
using JSharp.PluginCore;

namespace FileExplorer
{
    public class Entry : Plugin
    {
        public override MenuItem[] GetMenuItems() => null;

        FileExplorer fileExplorer = null;
        UserControl[] paneControls;
        public override UserControl[] GetPaneControls()
        {
            if (fileExplorer == null)
            {
                fileExplorer = new FileExplorer
                {
                    Name = "File_Explorer"
                };
                fileExplorer.SetDirectory(Directory.GetCurrentDirectory());
                paneControls = new[] { fileExplorer };
            }
            return paneControls;
        }

        public override object[] GetToolbarItems() => null;

        public static Main Window { get; set; }
        public override void Init()
        {
            Name = "File Explorer";
            Description = "Built-In plugin for JSharp for exploring files";
            Version = "1.0.0.0";
            Window = ParentWindow;
        }

        public override void Unload()
        {
        }
    }
}

