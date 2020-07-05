using ICSharpCode.AvalonEdit.CodeCompletion;
using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Editing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.SessionState;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;

namespace JSharp.Code_Completion
{
    public class EditorCompletionWindow : CompletionWindowBase
    {
        private TextArea textArea;

		public static readonly CompletionList CompletionList = new CompletionList();

		private ToolTip toolTip = new ToolTip();

		public bool CloseAutomatically
		{
			get;
			set;
		}

		protected override bool CloseOnFocusLost => CloseAutomatically;

		public bool CloseWhenCaretAtBeginning
		{
			get;
			set;
		}


		public EditorCompletionWindow(TextArea textArea) : base(textArea)
        {
            this.textArea = textArea;

			CloseAutomatically = true;
            SizeToContent = SizeToContent.Height;
			base.MaxHeight = 300.0;
			base.Width = 175.0;
			base.Content = CompletionList;
			base.MinHeight = 15.0;
			base.MinWidth = 30.0;
			toolTip.PlacementTarget = this;
			toolTip.Placement = PlacementMode.Right;
			toolTip.Closed += toolTip_Closed;
			AttachEvents();

		}
		public static bool completionDataInitialized = false;

		public static void InitalizeCompletionData()
        {
			if (completionDataInitialized) return;
			CompletionList.Background = PluginHolder.instance.ParentWindow.Background;
			CompletionList.Background = PluginHolder.instance.ParentWindow.Foreground;

			IList<ICompletionData> data = CompletionList.CompletionData;
			data.Add(new MyCompletionData("class"));
			data.Add(new MyCompletionData("void"));
			data.Add(new MyCompletionData("public"));
			data.Add(new MyCompletionData("bal.ballah("));

			completionDataInitialized = true;
		}



		private void toolTip_Closed(object sender, RoutedEventArgs e)
		{
			if (toolTip != null)
			{
				toolTip.Content = null;
			}
		}

		private void CompletionList_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			ICompletionData selectedItem = CompletionList.SelectedItem;
			if (selectedItem == null)
			{
				return;
			}
			object description = selectedItem.Description;
			if (description != null)
			{
				string text = description as string;
				if (text != null)
				{
					toolTip.Content = new TextBlock
					{
						Text = text,
						TextWrapping = TextWrapping.Wrap
					};
				}
				else
				{
					toolTip.Content = description;
				}
				toolTip.IsOpen = true;
			}
			else
			{
				toolTip.IsOpen = false;
			}
		}

		private void CompletionList_InsertionRequested(object sender, EventArgs e)
		{
			Close();
			CompletionList.SelectedItem?.Complete(base.TextArea, new AnchorSegment(base.TextArea.Document, base.StartOffset, base.EndOffset - base.StartOffset), e);
		}

		private void AttachEvents()
		{
			CompletionList.InsertionRequested += CompletionList_InsertionRequested;
			CompletionList.SelectionChanged += CompletionList_SelectionChanged;
			base.TextArea.Caret.PositionChanged += CaretPositionChanged;
            TextArea.MouseWheel += textArea_MouseWheel;
            TextArea.PreviewTextInput += textArea_PreviewTextInput;
		}

		protected override void DetachEvents()
		{
			CompletionList.InsertionRequested -= CompletionList_InsertionRequested;
			CompletionList.SelectionChanged -= CompletionList_SelectionChanged;
			base.TextArea.Caret.PositionChanged -= CaretPositionChanged;
			base.TextArea.MouseWheel -= textArea_MouseWheel;
			base.TextArea.PreviewTextInput -= textArea_PreviewTextInput;
			base.DetachEvents();
		}

		protected override void OnClosed(EventArgs e)
		{
			base.OnClosed(e);
			if (toolTip != null)
			{
				toolTip.IsOpen = false;
				toolTip = null;
			}
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
			e.Handled = CompletionWindowBase.RaiseEventPair(this, UIElement.PreviewTextInputEvent, UIElement.TextInputEvent, new TextCompositionEventArgs(e.Device, e.TextComposition));
		}

		private void textArea_MouseWheel(object sender, MouseWheelEventArgs e)
		{
			e.Handled = CompletionWindowBase.RaiseEventPair(GetScrollEventTarget(), UIElement.PreviewMouseWheelEvent, UIElement.MouseWheelEvent, new MouseWheelEventArgs(e.MouseDevice, e.Timestamp, e.Delta));
		}

		private UIElement GetScrollEventTarget()
		{
			if (CompletionList == null)
			{
				return this;
			}
			return (UIElement)(CompletionList.ScrollViewer ?? ((object)CompletionList.ListBox) ?? ((object)CompletionList));
		}

		private void CaretPositionChanged(object sender, EventArgs e)
		{
			int offset = base.TextArea.Caret.Offset;
			if (offset == base.StartOffset)
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
			if (offset < base.StartOffset || offset > base.EndOffset)
			{
				if (CloseAutomatically)
				{
					Close();
				}
				return;
			}
			TextDocument document = base.TextArea.Document;
			if (document != null)
			{
				CompletionList.SelectItem(document.GetText(base.StartOffset, offset - base.StartOffset));
			}
		}

    }
}
