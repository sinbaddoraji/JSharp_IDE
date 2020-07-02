using System;
using System.Collections.Generic;
using System.Diagnostics.Eventing.Reader;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace JavaEngine
{
    class Tokenizer
    {
        private string code;
        public int offset = 0;

        string largerView;
        string immediateView;

        Dictionary<string, Token> knownTokens = new Dictionary<string, Token>();
        public List<Token> Tokens = new List<Token>();

        public Tokenizer(string code)
        {
            this.code = code;

            foreach (var t in JavaTokens.Tokens)
            {
                knownTokens.Add(t.Text, t);
            }

            knownTokens.Add("literalDoubleQuote", new Token("\\\"", false));
            code.Replace("\\\"", "literalDoubleQuote");

            int count = 0;
            
            foreach (Match match in Regex.Matches(code, "\\\".*\""))
            {
                code.Replace(match.Value, $"raw-string{count}");
                knownTokens.Add($"raw-string{count}", new Token(match.Value, false));
                count++;
            }

            //Remove Comments
            //foreach (Match match in Regex.Matches(code, @"\/\/.*")) { code.Replace(match.Value, ""); }

            Token token;
            while ((token = NextCode()) != null)
            {
                Tokens.Add(token);
            }
        }

        bool KeysMatch(string rawText)
        {
            try
            {
                Token token = knownTokens.First(x => x.Value.Matches(rawText)).Value;
                return token != null;
            }
            catch (Exception)
            {
                return false;
            }
            
        }

        public Token NextCode()
        {
            if (offset >= code.Length) return null;

            if (immediateView == null) immediateView = "";

            char[] ignoredCharacters = { '\r', '\n', ' '};
            while(!KeysMatch(immediateView))
            {
                if(ignoredCharacters.Contains(code[offset]))
                {
                    if (code[offset] == ' ') break;

                    offset++;
                    if (offset >= code.Length) return new Token(immediateView, false);
                }
                else
                {
                    immediateView += code[offset].ToString();
                    offset++;

                    if (offset >= code.Length) return new Token(immediateView, false);
                }
                
            }

            string output = immediateView;
            immediateView = "";
            return new Token(output, false);
        }
    }
}
