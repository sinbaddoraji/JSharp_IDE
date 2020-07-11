using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using ICSharpCode.AvalonEdit.CodeCompletion;
using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Editing;
using JSharp.PluginCore;

namespace JSharp.Code_Completion
{
    public class EditorCompletionWindow : CompletionWindowBase
    {
	    public static readonly EditorCompletionList CompletionList = new EditorCompletionList();

		private ToolTip _toolTip = new ToolTip();

	    private static bool _completionDataInitialized;

	    public EditorCompletionWindow(TextArea textArea) : base(textArea)
        {
            SizeToContent = SizeToContent.Height; 
			MaxHeight = 300.0;
			Width = 175.0;
			Content = CompletionList;
			MinHeight = 15.0;
			MinWidth = 30.0;
			_toolTip.PlacementTarget = this;
			_toolTip.Placement = PlacementMode.Right;
			_toolTip.Closed += ToolTip_Closed;
			AttachEvents();

			CompletionList.Background = PluginCore.PluginHolder.Instance.ParentWindow.Background;
			Background = CompletionList.Background;
		}

		public static bool InitalizeCompletionData()
        {
			if (_completionDataInitialized) return true;
			CompletionList.Background = PluginHolder.Instance.ParentWindow.Background;
			CompletionList.Background = PluginHolder.Instance.ParentWindow.Foreground;

			var data = CompletionList.CompletionData;
			data.Add(new MyCompletionData("abstract"));
			data.Add(new MyCompletionData("assert"));
			data.Add(new MyCompletionData("boolean"));
			data.Add(new MyCompletionData("break"));
			data.Add(new MyCompletionData("byte"));
			data.Add(new MyCompletionData("case"));
			data.Add(new MyCompletionData("catch"));
			data.Add(new MyCompletionData("char"));
			data.Add(new MyCompletionData("class"));
			data.Add(new MyCompletionData("const"));
			data.Add(new MyCompletionData("continue"));
			data.Add(new MyCompletionData("default"));
			data.Add(new MyCompletionData("do"));
			data.Add(new MyCompletionData("double"));
			data.Add(new MyCompletionData("else"));
			data.Add(new MyCompletionData("enum"));
			data.Add(new MyCompletionData("extends"));
			data.Add(new MyCompletionData("final"));
			data.Add(new MyCompletionData("finally"));
			data.Add(new MyCompletionData("void"));
			data.Add(new MyCompletionData("float"));
			data.Add(new MyCompletionData("for"));
			data.Add(new MyCompletionData("void"));
			data.Add(new MyCompletionData("public"));
			data.Add(new MyCompletionData("goto"));
			data.Add(new MyCompletionData("if"));
			data.Add(new MyCompletionData("implements"));
			data.Add(new MyCompletionData("import"));
			data.Add(new MyCompletionData("instanceof"));
			data.Add(new MyCompletionData("int"));
			data.Add(new MyCompletionData("interface"));
			data.Add(new MyCompletionData("long"));
			data.Add(new MyCompletionData("native"));
			data.Add(new MyCompletionData("new"));
			data.Add(new MyCompletionData("package"));
			data.Add(new MyCompletionData("private"));
			data.Add(new MyCompletionData("protected"));
			data.Add(new MyCompletionData("public"));
			data.Add(new MyCompletionData("return"));
			data.Add(new MyCompletionData("short"));
			data.Add(new MyCompletionData("static"));
			data.Add(new MyCompletionData("strictfp"));
			data.Add(new MyCompletionData("super"));
			data.Add(new MyCompletionData("switch"));
			data.Add(new MyCompletionData("synchronized"));
			data.Add(new MyCompletionData("this"));
			data.Add(new MyCompletionData("throw"));
			data.Add(new MyCompletionData("throws"));
			data.Add(new MyCompletionData("transient"));
			data.Add(new MyCompletionData("try"));
			data.Add(new MyCompletionData("void"));
			data.Add(new MyCompletionData("volatile"));
			data.Add(new MyCompletionData("while"));
			_completionDataInitialized = true;

			return false;
		}



		private void ToolTip_Closed(object sender, RoutedEventArgs e)
		{
			if (_toolTip != null) _toolTip.Content = null;
		}

		private void CompletionList_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			var selectedItem = CompletionList.SelectedItem;
			if (selectedItem == null) return;

            if (selectedItem.Description != null)
            {
                _toolTip.Content = selectedItem.Description is string text ?
                    new TextBlock { Text = text, TextWrapping = TextWrapping.Wrap } : selectedItem.Description;
                _toolTip.IsOpen = true;
            }
            else
            {
                _toolTip.IsOpen = false;
            }
        }

		private void CompletionList_InsertionRequested(object sender, EventArgs e)
		{
			Close();
			CompletionList.SelectedItem?.Complete(TextArea, new AnchorSegment(TextArea.Document, StartOffset, EndOffset - StartOffset), e);
		}

		private void AttachEvents()
		{
			CompletionList.InsertionRequested += CompletionList_InsertionRequested;
			CompletionList.SelectionChanged += CompletionList_SelectionChanged;
			TextArea.Caret.PositionChanged += CaretPositionChanged;
            TextArea.MouseWheel += TextArea_MouseWheel;
            TextArea.PreviewTextInput += TextArea_PreviewTextInput;
		}

		protected override void DetachEvents()
		{
			CompletionList.InsertionRequested -= CompletionList_InsertionRequested;
			CompletionList.SelectionChanged -= CompletionList_SelectionChanged;
			TextArea.Caret.PositionChanged -= CaretPositionChanged;
			TextArea.MouseWheel -= TextArea_MouseWheel;
			TextArea.PreviewTextInput -= TextArea_PreviewTextInput;
			base.DetachEvents();
		}

		protected override void OnClosed(EventArgs e)
		{
			base.OnClosed(e);
			if (_toolTip == null) return;
			_toolTip.IsOpen = false;
			_toolTip = null;
		}

		protected override void OnKeyDown(KeyEventArgs e)
		{
			base.OnKeyDown(e);
			if (!e.Handled)
			{
				CompletionList.HandleKey(e);
			}
		}

		private void TextArea_PreviewTextInput(object sender, TextCompositionEventArgs e)
		{
			e.Handled = RaiseEventPair(this, PreviewTextInputEvent, TextInputEvent, new TextCompositionEventArgs(e.Device, e.TextComposition));
		}

		private void TextArea_MouseWheel(object sender, MouseWheelEventArgs e)
		{
			e.Handled = RaiseEventPair(GetScrollEventTarget(), PreviewMouseWheelEvent, MouseWheelEvent, new MouseWheelEventArgs(e.MouseDevice, e.Timestamp, e.Delta));
		}

		private UIElement GetScrollEventTarget()
		{
            return CompletionList == null ? this :
				(UIElement)(CompletionList.ScrollViewer ?? CompletionList.ListBox ?? ((object)CompletionList));
        }

        private void CaretPositionChanged(object sender, EventArgs e)
		{
			var offset = TextArea.Caret.Offset;
			if (offset == StartOffset)
			{
				Close();
				CompletionList.SelectItem(string.Empty);
				return;
			}
			if (offset < StartOffset || offset > EndOffset)
			{
				Close();
				return;
			}
			var document = TextArea.Document;
			if (document != null)
			{
				CompletionList.SelectItem(document.GetText(StartOffset, offset - StartOffset));
			}
		}

    }
}
