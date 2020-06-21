using JSharp.PluginCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace JSharp
{
    public class PluginHolder
    {
        public static PluginHolder instance;
        public Main ParentWindow { get; set; } = null;

        public List<Plugin> RegisteredPlugins { get; set; }

        private readonly string[] exludedFiles = new[]
        { "AvalonDock.Themes.VS2013.dll", "AvalonDock.dll", "ICSharpCode.AvalonEdit.dll", "JSharp.dll", "HL.dll" };

        public PluginHolder()
        {
            RegisteredPlugins = new List<Plugin>();
        }

        public void UnloadAllRegisteredPlugins()
        {
            Parallel.ForEach(RegisteredPlugins, plugin => { plugin.Unload(); });
        }

        private void LoadPlugin(string pluginPath)
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