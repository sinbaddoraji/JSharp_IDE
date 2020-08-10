using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace JdbWrapper
{
    static class Program
    {
        [STAThread]
        public static void Main(string[] args)
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            if(args.Length == 2)
            {
                Application.Run(new DebuggerGUI(args[0], args[1]));
            }
            else if (args.Length == 1)
            {
                Application.Run(new DebuggerGUI(args[0]));
            }
            else
            {
                Application.Run(new DebuggerGUI());
            }
        }
    }
}
