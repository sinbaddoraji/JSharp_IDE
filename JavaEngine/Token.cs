using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace JavaEngine
{
    class Token
    {
        public string Text { get; set; }
        private bool useRegex;

        public Token(string token, bool useRegex)
        {
            Text = token;
            this.useRegex = useRegex;
        }

        public bool Matches(string rawString)
        {
            if(useRegex)
            {
                return Regex.IsMatch(rawString, Text);
            }
            else
            {
                return rawString == Text;
            }
        }
    }
}
