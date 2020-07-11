using ICSharpCode.AvalonEdit.CodeCompletion;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JSharp.Code_Completion
{
    public class EditorCompletionList: CompletionList
    {
        List<string> AutoCompleteStrings = new List<string>();

        public void Add(string data)
        {
            if(!AutoCompleteStrings.Contains(data))
            {
                this.CompletionData.Add(new MyCompletionData(data));
                AutoCompleteStrings.Add(data);
            }
        }

        public bool Contains(string data)
        {
            return AutoCompleteStrings.Contains(data);
        }
    }
}
