using System.Media;
using System.Text.RegularExpressions;
using System.Windows;
using JSharp.PluginCore;

namespace JSharp.Inbuilt_Panes
{
    /// <inheritdoc cref="FindReplace" />
    /// <summary>
    /// Interaction logic for FindReplace.xaml
    /// </summary>
    public partial class FindReplace
    {
        private static TextEditor.TextEditor GetEditor() => PluginHolder.Instance.ParentWindow.GetSelectedTextEditor();

        public FindReplace()
        {
            InitializeComponent();
            InitalizeThemeSettings();
        }

        private void InitalizeThemeSettings()
        {
            //Set Window background and foreground of the window
            Background = PluginHolder.Instance.ParentWindow.Background;
            Foreground = PluginHolder.Instance.ParentWindow.Foreground;
            Label1.Foreground = Label2.Foreground = Label3.Foreground = Foreground;
            //Setup tab background and foreground
            TabMain.Background = Background;
            TabMain.Foreground = Foreground;
            //Setup foreground color of check-boxes in the window
            IsCaseSensitive.Foreground = Foreground;
            SearchWholeWord.Foreground = Foreground;
            IsRegexSearch.Foreground = Foreground;
            UseWildCards.Foreground = Foreground;
            SearchUpwards.Foreground = Foreground;
        }

        private void FindNextClick(object sender, RoutedEventArgs e)
        {
            if (!FindNext(TxtFind.Text))
                SystemSounds.Beep.Play();
        }

        private void FindNext2Click(object sender, RoutedEventArgs e)
        {
            if (!FindNext(TxtFind2.Text))
                SystemSounds.Beep.Play();
        }

        private void ReplaceClick(object sender, RoutedEventArgs e)
        {
            var regex = GetRegEx(TxtFind2.Text);
            var input = GetEditor().Text.Substring(GetEditor().SelectionStart, GetEditor().SelectionLength);
            var match = regex.Match(input);
            var replaced = false;
            if (match.Success && match.Index == 0 && match.Length == input.Length)
            {
                GetEditor().Document.Replace(GetEditor().SelectionStart, GetEditor().SelectionLength, TxtReplace.Text);
                replaced = true;
            }

            if (!FindNext(TxtFind2.Text) && !replaced)
                SystemSounds.Beep.Play();
        }

        private void ReplaceAllClick(object sender, RoutedEventArgs e)
        {
            //Confirm if user wants to procede
            if (MessageBox.Show("Are you sure you want to Replace All occurrences of \"" +
                                TxtFind2.Text + "\" with \"" + TxtReplace.Text + "\"?",
                    "Replace All", MessageBoxButton.OKCancel, MessageBoxImage.Question) != MessageBoxResult.OK) return;
            
            //procede
            var regex = GetRegEx(TxtFind2.Text, true);
            var offset = 0;
            GetEditor().BeginChange();
            foreach (Match match in regex.Matches(GetEditor().Text))
            {
                GetEditor().Document.Replace(offset + match.Index, match.Length, TxtReplace.Text);
                offset += TxtReplace.Text.Length - match.Length;
            }
            GetEditor().EndChange();
        }

        private bool FindNext(string textToFind)
        {
            var regex = GetRegEx(textToFind);
            var start = (regex.Options & RegexOptions.RightToLeft) != 0 ?
            GetEditor().SelectionStart : GetEditor().SelectionStart + GetEditor().SelectionLength;
            var match = regex.Match(GetEditor().Text, start);

            if (!match.Success)  // start again from beginning or end
            {
                match = regex.Match(GetEditor().Text, (regex.Options & RegexOptions.RightToLeft) != 0 ? GetEditor().Text.Length : 0);
            }

            if (!match.Success) return match.Success;
            
            GetEditor().Select(match.Index, match.Length);
            var loc = GetEditor().Document.GetLocation(match.Index);
            GetEditor().ScrollTo(loc.Line, loc.Column);

            return match.Success;
        }

        private Regex GetRegEx(string textToFind, bool leftToRight = false)
        {
            var options = RegexOptions.None;
            if (SearchUpwards.IsChecked == true && !leftToRight)
                options |= RegexOptions.RightToLeft;
            if (IsCaseSensitive.IsChecked == false)
                options |= RegexOptions.IgnoreCase;

            if (IsRegexSearch.IsChecked == true)
            {
                return new Regex(textToFind, options);
            }

            var pattern = Regex.Escape(textToFind);
            if (UseWildCards.IsChecked == true)
                pattern = pattern.Replace("\\*", ".*").Replace("\\?", ".");
            if (SearchWholeWord.IsChecked == true)
                pattern = "\\b" + pattern + "\\b";
            return new Regex(pattern, options);
        }
    }
}