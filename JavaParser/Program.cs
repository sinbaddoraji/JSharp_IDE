using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Antlr4.Runtime.Tree;
using JavaParser;
using Antlr4.Runtime;

namespace JavaParser
{
    class Program
    {
        public static Parser core;
        static void Main(string[] args)
        {
            //JavaParser javaParser = new JavaParser();
            //JavaLexer javaLexer = new JavaLexer();
            args = new[] { "Server.java" };
            if (args == null || args.Length < 1) return;

            FileStream fileStream = new FileStream(args[0], (FileMode)FileAccess.ReadWrite);
            ICharStream inputStream = new AntlrInputStream(fileStream);

            core = new Parser(inputStream);


        }
    }
}
