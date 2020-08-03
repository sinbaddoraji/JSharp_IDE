using JSharp.TextEditor;
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
using System.Windows.Shapes;

namespace JSharp.Windows
{
    /// <summary>
    /// Interaction logic for GotoDialog.xaml
    /// </summary>
    public partial class GotoDialog : Window
    {
        private TextEditor.TextEditor Editor => PluginCore.PluginHolder.Instance.ParentWindow.GetSelectedTextEditor();
        public GotoDialog()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            GotoLine();
        }

        private void GotoLine()
        {
            if (int.TryParse(txt.Text, out int output))
                Editor.ScrollToLine(output);
            Close();
        }

        private void Txt_KeyDown(object sender, KeyEventArgs e)
        {
            GotoLine();
        }
    }
}
