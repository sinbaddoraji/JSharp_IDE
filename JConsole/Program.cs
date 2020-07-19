using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JConsole
{
    static class Program
    {

        static void Main(string[] args)
        {
            //PauseAfter Program Args
            if(args.Length >= 3)
            {
                bool pauseAfter = bool.Parse(args[0]);
                string program = args[1];

                string[] arguments = new string[args.Length - 2];
                Array.Copy(args, 2, arguments, 0, args.Length - 2);


            }
        }
    }
}
