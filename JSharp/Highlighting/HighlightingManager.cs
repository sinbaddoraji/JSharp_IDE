using ICSharpCode.AvalonEdit.Highlighting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using ICSharpCode.AvalonEdit.Highlighting.Xshd;

namespace JSharp.Highlighting
{
    internal class InnerHighlightingManager: HighlightingManager
    {
        //Java highlighting
        public readonly IHighlightingDefinition javaLight;
        public readonly IHighlightingDefinition javaDark;
        //Python highlighting
        public readonly IHighlightingDefinition pythonLight;
        public readonly IHighlightingDefinition pythonDark;
        //XML highlighting
        public readonly IHighlightingDefinition xmlLight;
        public readonly IHighlightingDefinition xmlDark;

        public InnerHighlightingManager()
        {
            if(!Directory.Exists("Resources"))
            {
                Directory.CreateDirectory("Resources");
                File.WriteAllBytes("Resources\\Java-LightMode.xshd", Properties.Resources.Java_LightMode);
                File.WriteAllBytes("Resources\\Java-DarkMode.xshd", Properties.Resources.Java_LightMode);
                File.WriteAllBytes("Resources\\Python-LightMode.xshd", Properties.Resources.Java_LightMode);
                File.WriteAllBytes("Resources\\Python-DarkMode.xshd", Properties.Resources.Java_LightMode);
                File.WriteAllBytes("Resources\\XML-LightMode.xshd", Properties.Resources.Java_LightMode);
                File.WriteAllBytes("Resources\\XML-DarkMode.xshd", Properties.Resources.Java_LightMode);
            }
            //Java highlighting
            javaLight = HighlightingLoader.Load(new XmlTextReader(File.OpenRead("Resources\\Java-LightMode.xshd")), this);
            javaDark = HighlightingLoader.Load(new XmlTextReader(File.OpenRead("Resources\\Java-DarkMode.xshd")), this);
            //Python highlighting
            pythonLight = HighlightingLoader.Load(new XmlTextReader(File.OpenRead("Resources\\Python-LightMode.xshd")), this);
            pythonDark = HighlightingLoader.Load(new XmlTextReader(File.OpenRead("Resources\\Python-DarkMode.xshd")), this);
            //Xml highlighting
            xmlLight = HighlightingLoader.Load(new XmlTextReader(File.OpenRead("Resources\\XML-LightMode.xshd")), this);
            xmlDark = HighlightingLoader.Load(new XmlTextReader(File.OpenRead("Resources\\XML-DarkMode.xshd")), this);
        }

        public IHighlightingDefinition GetHighlightingFromExtension(string extension)
        {
            IHighlightingDefinition d;
            if (extension == ".py" || extension == ".pyw")
            {
                return Properties.Settings.Default.DarkTheme? pythonDark : pythonLight;
            }
            else if (extension.EndsWith("ml") || extension.Contains("config"))
            {
                return Properties.Settings.Default.DarkTheme ? xmlDark : xmlLight;
            }
            else return Properties.Settings.Default.DarkTheme ? javaDark : javaLight;
        }

    }
}
