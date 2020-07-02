using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JavaEngine
{
    class Program
    {
        static void Main(string[] args)
        {
            args = new[] { "Server.java" };

            Tokenizer tokenizer = new Tokenizer(File.ReadAllText(args[0]));
            foreach (var item in tokenizer.Tokens)
            {
                Console.WriteLine(item.Text);
            }
            Console.ReadLine();
        }
    }
}
