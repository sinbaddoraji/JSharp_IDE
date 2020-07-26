using java.lang;
using java.net;
using System.Windows;

namespace JSharp.Windows
{
    /// <summary>
    /// Interaction logic for ClassAnalyse.xaml
    /// </summary>
    public partial class ClassAnalyser : Window
    {
        private readonly System.Windows.Forms.OpenFileDialog o = new System.Windows.Forms.OpenFileDialog();

        public ClassAnalyser()
        {
            //InitializeComponent();
        }

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            if (o.ShowDialog() == System.Windows.Forms.DialogResult.OK && o.FileName.EndsWith(".class"))
            {
                try
                {
                    Analyse(o.FileName);
                }
                catch (System.Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
                
            }
        }
        private static Class getClassFromFile(string fullClassName)
        {
            URLClassLoader loader = new URLClassLoader(new URL[] {
                new URL("file://" + System.IO.Directory.GetParent(fullClassName))
            }); ;
            return loader.loadClass(fullClassName);
        }

        private void Analyse(string classFile)
        {
            Class c = getClassFromFile(classFile);
            declearedFieldsBox.Items.Clear();
            declearedFieldsBox.Items.Clear();

            foreach (var item in c.getDeclaredFields())
            {
                declearedFieldsBox.Items.Add(item.getName());
            }

            foreach (var item in c.getDeclaredMethods())
            {
                string method = $"{item.getReturnType().getName()} {item.getName()}";
                declearedMethodsBox.Items.Add(method);
            }
        }


    }
}
