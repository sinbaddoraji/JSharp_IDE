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
        //C++ highlighting
        public readonly IHighlightingDefinition cppLight;
        public readonly IHighlightingDefinition cppDark;
        //C# highlighting
        public readonly IHighlightingDefinition cSharpLight;
        public readonly IHighlightingDefinition cSharpDark;
        //Java highlighting
        public readonly IHighlightingDefinition javaLight;
        public readonly IHighlightingDefinition javaDark;
        //Python highlighting
        public readonly IHighlightingDefinition pythonLight;
        public readonly IHighlightingDefinition pythonDark;
        //XML highlighting
        public readonly IHighlightingDefinition xmlpLight;
        public readonly IHighlightingDefinition xmlDark;

        public InnerHighlightingManager()
        {
            
            //C# highlighting
            cppLight = HighlightingLoader.Load(new XmlTextReader(File.OpenRead("\\Resources\\CPP-DarkMode.xshd")), this);
            cppDark = HighlightingLoader.Load(new XmlTextReader(File.OpenRead("\\Resources\\CPP-LightMode.xshd")), this);
            //C# highlighting
            cSharpLight = HighlightingLoader.Load(new XmlTextReader(File.OpenRead("\\Resources\\CSharp-DarkMode.xshd")), this);
            cSharpDark = HighlightingLoader.Load(new XmlTextReader(File.OpenRead("\\Resources\\CSharp-LightMode.xshd")), this);
            //C# highlighting
            javaLight = HighlightingLoader.Load(new XmlTextReader(File.OpenRead("\\Resources\\Java-DarkMode.xshd")), this);
            javaDark = HighlightingLoader.Load(new XmlTextReader(File.OpenRead("\\Resources\\Java-LightMode.xshd")), this);
            //C# highlighting
            pythonLight = HighlightingLoader.Load(new XmlTextReader(File.OpenRead("\\Resources\\Python-DarkMode.xshd")), this);
            pythonDark = HighlightingLoader.Load(new XmlTextReader(File.OpenRead("\\Resources\\Python-LightMode.xshd")), this);
            //C# highlighting
            xmlpLight = HighlightingLoader.Load(new XmlTextReader(File.OpenRead("\\Resources\\XML-DarkMode.xshd")), this);
            xmlDark = HighlightingLoader.Load(new XmlTextReader(File.OpenRead("\\Resources\\XML-LightMode.xshd")), this); 
        }
    }
}
