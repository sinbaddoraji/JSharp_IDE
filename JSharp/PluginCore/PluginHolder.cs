using JSharp.PluginCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows;

namespace JSharp
{
    public class PluginHolder
    {
        /// <summary>
        /// Plug-in holder global instance
        /// </summary>
        public static PluginHolder instance;

        /// <summary>
        /// JSharp Window
        /// </summary>
        public Main ParentWindow { get; set; }

        /// <summary>
        /// A list of all valid registered plug-ins connected to JSharp
        /// </summary>
        public List<Plugin> RegisteredPlugins { get; set; }

        /// <summary>
        /// A list of executables in root folder known not to be plug-in files
        /// </summary>
        private readonly string[] exludedFiles = new[]
        { 
            "AvalonDock.Themes.VS2013.dll", 
            "AvalonDock.dll", 
            "ICSharpCode.AvalonEdit.dll", 
            "JSharp.dll",
            "ControlzEx.dll",
            "MahApps.Metro.dll", 
            "Microsoft.Xaml.Behaviors.dll"
        };

        /// <summary>
        /// Class that handles all plug-in related tasks
        /// </summary>
        public PluginHolder()
        {
            RegisteredPlugins = new List<Plugin>();
        }

        /// <summary>
        /// Unload all registered plug-ins and detach them from JSharp
        /// </summary>
        public void UnloadAllRegisteredPlugins()
        {
            Parallel.ForEach(RegisteredPlugins, plugin => { plugin.Unload(); });
        }

        /// <summary>
        /// Load JSharp plug-in from path
        /// </summary>
        private void LoadPlugin(string pluginPath)
        {
            try
            {
                if (exludedFiles.Contains(Path.GetFileName(pluginPath))) return;

                Assembly asm = Assembly.LoadFile(pluginPath);

                if (asm == null) return;

                Type objType = asm.GetExportedTypes().First(x => x.Name == "Entry");

                if (objType != null)
                {
                    Plugin ipi = (Plugin)Activator.CreateInstance(objType);
                    if (ipi != null)
                    {
                        ipi.ParentWindow = ParentWindow;
                        RegisteredPlugins.Add(ipi);
                        ipi.Init();
                    }
                }
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message);
            }
        }

        /// <summary>
        /// Load plug-in holder instance
        /// </summary>
        public void Load()
        {
            string pluginPath = Directory.GetParent(Environment.GetCommandLineArgs()[0]) + @"\";

            if (!Directory.Exists(pluginPath)) Directory.CreateDirectory(pluginPath);

            foreach (string plugin in Directory.EnumerateFiles(pluginPath, "*.dll"))
            {
                LoadPlugin(plugin);
            }
        }
    }
}