using ICSharpCode.AvalonEdit.Highlighting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JSharp.Highlighting
{
    class JavaHighlighting : IHighlightingDefinition
    {
        string IHighlightingDefinition.Name => throw new NotImplementedException();

        HighlightingRuleSet IHighlightingDefinition.MainRuleSet => throw new NotImplementedException();

        IEnumerable<HighlightingColor> IHighlightingDefinition.NamedHighlightingColors => throw new NotImplementedException();

        IDictionary<string, string> IHighlightingDefinition.Properties => throw new NotImplementedException();

        HighlightingColor IHighlightingDefinition.GetNamedColor(string name)
        {
            throw new NotImplementedException();
        }

        HighlightingRuleSet IHighlightingDefinition.GetNamedRuleSet(string name)
        {
            throw new NotImplementedException();
        }
    }
}
