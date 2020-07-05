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
	    public static readonly CompletionList CompletionList = new CompletionList();

		private ToolTip _toolTip = new ToolTip();

	    private bool CloseAutomatically
		{
			get;
	    }

		protected override bool CloseOnFocusLost => CloseAutomatically;

	    private static bool CloseWhenCaretAtBeginning => false;

	    private static bool _completionDataInitialized;

	    public EditorCompletionWindow(TextArea textArea) : base(textArea)
        {
	        CloseAutomatically = true;
            SizeToContent = SizeToContent.Height;
			MaxHeight = 300.0;
			Width = 175.0;
			Content = CompletionList;
			MinHeight = 15.0;
			MinWidth = 30.0;
			_toolTip.PlacementTarget = this;
			_toolTip.Placement = PlacementMode.Right;
			_toolTip.Closed += toolTip_Closed;
			AttachEvents();

		}

		public static void InitalizeCompletionData()
        {
			if (_completionDataInitialized) return;
			CompletionList.Background = PluginHolder.Instance.ParentWindow.Background;
			CompletionList.Background = PluginHolder.Instance.ParentWindow.Foreground;

			var data = CompletionList.CompletionData;
			data.Add(new MyCompletionData("class"));
			data.Add(new MyCompletionData("void"));
			data.Add(new MyCompletionData("public"));
			data.Add(new MyCompletionData("bal.ballah("));

			_completionDataInitialized = true;
		}



		private void toolTip_Closed(object sender, RoutedEventArgs e)
		{
			if (_toolTip != null)
			{
				_toolTip.Content = null;
			}
		}

		private void CompletionList_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			var selectedItem = CompletionList.SelectedItem;
			if (selectedItem == null) return;
			
			var description = selectedItem.Description;
			if (description != null)
			{
				if (description is string text)
				{
					_toolTip.Content = new TextBlock
					{
						Text = text,
						TextWrapping = TextWrapping.Wrap
					};
				}
				else
				{
					_toolTip.Content = description;
				}
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
            TextArea.MouseWheel += textArea_MouseWheel;
            TextArea.PreviewTextInput += textArea_PreviewTextInput;
		}

		protected override void DetachEvents()
		{
			CompletionList.InsertionRequested -= CompletionList_InsertionRequested;
			CompletionList.SelectionChanged -= CompletionList_SelectionChanged;
			TextArea.Caret.PositionChanged -= CaretPositionChanged;
			TextArea.MouseWheel -= textArea_MouseWheel;
			TextArea.PreviewTextInput -= textArea_PreviewTextInput;
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

		private void textArea_PreviewTextInput(object sender, TextCompositionEventArgs e)
		{
			e.Handled = RaiseEventPair(this, PreviewTextInputEvent, TextInputEvent, new TextCompositionEventArgs(e.Device, e.TextComposition));
		}

		private void textArea_MouseWheel(object sender, MouseWheelEventArgs e)
		{
			e.Handled = RaiseEventPair(GetScrollEventTarget(), PreviewMouseWheelEvent, MouseWheelEvent, new MouseWheelEventArgs(e.MouseDevice, e.Timestamp, e.Delta));
		}

		private UIElement GetScrollEventTarget()
		{
			if (CompletionList == null)
			{
				return this;
			}
			return (UIElement)(CompletionList.ScrollViewer ?? CompletionList.ListBox ?? ((object)CompletionList));
		}

		private void CaretPositionChanged(object sender, EventArgs e)
		{
			var offset = TextArea.Caret.Offset;
			if (offset == StartOffset)
			{
				if (CloseAutomatically && CloseWhenCaretAtBeginning)
				{
					Close();
				}
				else
				{
					CompletionList.SelectItem(string.Empty);
				}
				return;
			}
			if (offset < StartOffset || offset > EndOffset)
			{
				if (CloseAutomatically)
				{
					Close();
				}
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
