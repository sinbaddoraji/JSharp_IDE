using ICSharpCode.AvalonEdit.Document;
using System.Media;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;

namespace JSharp.Inbuilt_Panes
{
    /// <summary>
    /// Interaction logic for FindReplace.xaml
    /// </summary>
    public partial class FindReplace : UserControl
    {
        private Editor GetEditor() => PluginHolder.instance.ParentWindow.GetSelectedDocument();

        public FindReplace()
        {
            InitializeComponent();
            InitalizeThemeSettings();
        }

        private void InitalizeThemeSettings()
        {
            //Set Window background and foreground of the window
            Background = PluginHolder.instance.ParentWindow.Background;
            Foreground = PluginHolder.instance.ParentWindow.Foreground;
            label1.Foreground = label2.Foreground = label3.Foreground = Foreground;
            //Setup tab background and foreground
            tabMain.Background = Background;
            tabMain.Foreground = Foreground;
            //Setup foreground color of check-boxes in the window
            isCaseSensitive.Foreground = Foreground;
            searchWholeWord.Foreground = Foreground;
            isRegexSearch.Foreground = Foreground;
            useWildCards.Foreground = Foreground;
            searchUpwards.Foreground = Foreground;
        }

        private void FindNextClick(object sender, RoutedEventArgs e)
        {
            if (!FindNext(txtFind.Text))
                SystemSounds.Beep.Play();
        }

        private void FindNext2Click(object sender, RoutedEventArgs e)
        {
            if (!FindNext(txtFind2.Text))
                SystemSounds.Beep.Play();
        }

        private void ReplaceClick(object sender, RoutedEventArgs e)
        {
            Regex regex = GetRegEx(txtFind2.Text);
            string input = GetEditor().Text.Substring(GetEditor().SelectionStart, GetEditor().SelectionLength);
            Match match = regex.Match(input);
            bool replaced = false;
            if (match.Success && match.Index == 0 && match.Length == input.Length)
            {
                GetEditor().Document.Replace(GetEditor().SelectionStart, GetEditor().SelectionLength, txtReplace.Text);
                replaced = true;
            }

            if (!FindNext(txtFind2.Text) && !replaced)
                SystemSounds.Beep.Play();
        }

        private void ReplaceAllClick(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("Are you sure you want to Replace All occurrences of \"" +
            txtFind2.Text + "\" with \"" + txtReplace.Text + "\"?",
                "Replace All", MessageBoxButton.OKCancel, MessageBoxImage.Question) == MessageBoxResult.OK)
            {
                Regex regex = GetRegEx(txtFind2.Text, true);
                int offset = 0;
                GetEditor().BeginChange();
                foreach (Match match in regex.Matches(GetEditor().Text))
                {
                    GetEditor().Document.Replace(offset + match.Index, match.Length, txtReplace.Text);
                    offset += txtReplace.Text.Length - match.Length;
                }
                GetEditor().EndChange();
            }
        }

        private bool FindNext(string textToFind)
        {
            Regex regex = GetRegEx(textToFind);
            int start = (regex.Options & RegexOptions.RightToLeft) != 0 ?
            GetEditor().SelectionStart : GetEditor().SelectionStart + GetEditor().SelectionLength;
            Match match = regex.Match(GetEditor().Text, start);

            if (!match.Success)  // start again from beginning or end
            {
                if ((regex.Options & RegexOptions.RightToLeft) != 0)
                    match = regex.Match(GetEditor().Text, GetEditor().Text.Length);
                else
                    match = regex.Match(GetEditor().Text, 0);
            }

            if (match.Success)
            {
                GetEditor().Select(match.Index, match.Length);
                TextLocation loc = GetEditor().Document.GetLocation(match.Index);
                GetEditor().ScrollTo(loc.Line, loc.Column);
            }

            return match.Success;
        }

        private Regex GetRegEx(string textToFind, bool leftToRight = false)
        {
            RegexOptions options = RegexOptions.None;
            if (searchUpwards.IsChecked == true && !leftToRight)
                options |= RegexOptions.RightToLeft;
            if (isCaseSensitive.IsChecked == false)
                options |= RegexOptions.IgnoreCase;

            if (isRegexSearch.IsChecked == true)
            {
                return new Regex(textToFind, options);
            }
            else
            {
                string pattern = Regex.Escape(textToFind);
                if (useWildCards.IsChecked == true)
                    pattern = pattern.Replace("\\*", ".*").Replace("\\?", ".");
                if (searchWholeWord.IsChecked == true)
                    pattern = "\\b" + pattern + "\\b";
                return new Regex(pattern, options);
            }
        }
    }
}