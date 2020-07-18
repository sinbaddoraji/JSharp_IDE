using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Simple.Wpf.Terminal;

namespace JSharp.Inbuilt_Panes
{
    /// <summary>
    /// Interaction logic for JSharpTerminal.xaml
    /// </summary>
    public partial class JSharpTerminal : UserControl
    {
        public Terminal Terminal = new Terminal();

        public JSharpTerminal()
        {
            InitializeComponent();

            Terminal.HorizontalAlignment = HorizontalAlignment.Stretch;
            Terminal.VerticalAlignment = VerticalAlignment.Stretch;
            ContentHolder.Children.Add(Terminal);
        }
    }
}
