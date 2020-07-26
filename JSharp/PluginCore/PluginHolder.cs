using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows;

namespace JSharp.PluginCore
{
    public class PluginHolder
    {
        /// <summary>
        /// Plug-in holder global instance
        /// </summary>
        public static PluginHolder Instance;

        /// <summary>
        /// JSharp Window
        /// </summary>
        public Windows.MainWindow.Main ParentWindow { get; set; }

        /// <summary>
        /// A list of all valid registered plug-ins connected to JSharp
        /// </summary>
        public List<IPlugin> RegisteredPlugins { get; }

        /// <summary>
        /// A list of executables in root folder known not to be plug-in files
        /// </summary>
        private readonly string[] _exludedFiles = { 
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
            RegisteredPlugins = new List<IPlugin>();
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
        private Task<bool> LoadPlugin(string pluginPath)
        {
            if (_exludedFiles.Contains(Path.GetFileName(pluginPath)) || pluginPath.Contains("IKVM")) return Task.FromResult(true);

            var objType = Assembly.LoadFile(pluginPath).GetExportedTypes().First(x => x.Name == "Entry");

            if (objType == null) return Task.FromResult(true);

            var ipi = (IPlugin)Activator.CreateInstance(objType);
            ipi.ParentWindow = ParentWindow;
            RegisteredPlugins.Add(ipi);
            ipi.Init();

            return Task.FromResult(true);
        }

        /// <summary>
        /// Load plug-in holder instance
        /// </summary>
        public async void Load()
        {
            var pluginPath = Directory.GetParent(Environment.GetCommandLineArgs()[0]) + @"\";

            if (!Directory.Exists(pluginPath)) Directory.CreateDirectory(pluginPath);

            foreach (var plugin in Directory.EnumerateFiles(pluginPath, "*.dll"))
            {
                await LoadPlugin(plugin).ConfigureAwait(false);
            }
        }
    }
}