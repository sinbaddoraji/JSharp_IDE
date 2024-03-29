﻿using System.IO;
using System.Xml;
using ICSharpCode.AvalonEdit.Highlighting;
using ICSharpCode.AvalonEdit.Highlighting.Xshd;
using JSharp.Properties;

namespace JSharp.TextEditor.Highlighting
{
    internal class InnerHighlightingManager : HighlightingManager
    {
        //Java highlighting
        private readonly IHighlightingDefinition _javaLight;
        private readonly IHighlightingDefinition _javaDark;
        //Python highlighting
        private readonly IHighlightingDefinition _pythonLight;
        private readonly IHighlightingDefinition _pythonDark;
        //XML highlighting
        private readonly IHighlightingDefinition _xmlLight;
        private readonly IHighlightingDefinition _xmlDark;

        public InnerHighlightingManager()
        {
            if (!Directory.Exists("Resources"))
            {
                Directory.CreateDirectory("Resources");
                File.WriteAllText("Resources\\Java-LightMode.xshd", Resources.Java_LightMode);
                File.WriteAllText("Resources\\Java-DarkMode.xshd", Resources.Java_LightMode);
                File.WriteAllText("Resources\\Python-LightMode.xshd", Resources.Java_LightMode);
                File.WriteAllText("Resources\\Python-DarkMode.xshd", Resources.Java_LightMode);
                File.WriteAllText("Resources\\XML-LightMode.xshd", Resources.Java_LightMode);
                File.WriteAllText("Resources\\XML-DarkMode.xshd", Resources.Java_LightMode);
            }
            //Java highlighting
            _javaLight = HighlightingLoader.Load(new XmlTextReader(File.OpenRead("Resources\\Java-LightMode.xshd")), this);
            _javaDark = HighlightingLoader.Load(new XmlTextReader(File.OpenRead("Resources\\Java-DarkMode.xshd")), this);
            //Python highlighting
            _pythonLight = HighlightingLoader.Load(new XmlTextReader(File.OpenRead("Resources\\Python-LightMode.xshd")), this);
            _pythonDark = HighlightingLoader.Load(new XmlTextReader(File.OpenRead("Resources\\Python-DarkMode.xshd")), this);
            //XML highlighting
            _xmlLight = HighlightingLoader.Load(new XmlTextReader(File.OpenRead("Resources\\XML-LightMode.xshd")), this);
            _xmlDark = HighlightingLoader.Load(new XmlTextReader(File.OpenRead("Resources\\XML-DarkMode.xshd")), this);
        }

        public IHighlightingDefinition GetHighlightingFromExtension(string extension)
        {
            if (extension == ".py" || extension == ".pyw")
            {
                return Settings.Default.DarkTheme ? _pythonDark : _pythonLight;
            }

            if (extension.EndsWith("ml") || extension.Contains("config"))
            {
                return Settings.Default.DarkTheme ? _xmlDark : _xmlLight;
            }

            return Settings.Default.DarkTheme ? _javaDark : _javaLight;
        }

    }
}
