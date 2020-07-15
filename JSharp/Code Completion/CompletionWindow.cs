﻿using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using ICSharpCode.AvalonEdit.CodeCompletion;
using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Editing;

namespace JSharp.Code_Completion
{
    public class EditorCompletionWindow : CompletionWindowBase
    {
	    public static readonly EditorCompletionList CompletionList = new EditorCompletionList();

	    private static bool _completionDataInitialized;

	    public EditorCompletionWindow(TextArea textArea) : base(textArea)
        {
            SizeToContent = SizeToContent.Height;
			MaxHeight = 300.0;
			Width = 400;
			Content = CompletionList;
			MinHeight = 15.0;
			MinWidth = 30.0;
			AttachEvents();
		}

		public static bool InitalizeCompletionData()
        {
			if (_completionDataInitialized) return true;

			string[] keywords = new[] { "abstract", "assert", "boolean", "break", "byte", "case", "catch", "char", "class", "const", 
				"continue", "default", "do", "double", "else", "enum", "extends", "final", "finally", "float", "for", "goto", "if", 
				"implements", "import", "instanceof", "int", "interface", "long", "native", "new", "package", "private", "protected", 
				"public", "return", "short", "static", "strictfp", "super", "switch", "synchronized", "this", "throw", "throws", 
				"transient", "try", "void", "volatile", "while" };

			Parallel.ForEach(keywords, key =>
			{
				lock(CompletionList) { CompletionList.Add(key, true); }
			});

			_completionDataInitialized = true;

			return false;
		}

		private void CompletionList_InsertionRequested(object sender, EventArgs e)
		{
			Close();
			CompletionList.SelectedItem?.Complete(TextArea, 
				new AnchorSegment(TextArea.Document, StartOffset - 1, EndOffset - StartOffset + 1), e);
		}

		private void AttachEvents()
		{
			CompletionList.InsertionRequested += CompletionList_InsertionRequested;
			TextArea.Caret.PositionChanged += CaretPositionChanged;
            TextArea.MouseWheel += TextArea_MouseWheel;
            TextArea.PreviewTextInput += TextArea_PreviewTextInput;
		}

		protected override void DetachEvents()
		{
			CompletionList.InsertionRequested -= CompletionList_InsertionRequested;
			TextArea.Caret.PositionChanged -= CaretPositionChanged;
			TextArea.MouseWheel -= TextArea_MouseWheel;
			TextArea.PreviewTextInput -= TextArea_PreviewTextInput;
			base.DetachEvents();
		}

		protected override void OnClosed(EventArgs e)
		{
			base.OnClosed(e);
			DetachEvents();
		}

		protected override void OnKeyDown(KeyEventArgs e)
		{
			base.OnKeyDown(e);
			if (!e.Handled)
				CompletionList.HandleKey(e);
		}

		private void TextArea_PreviewTextInput(object sender, TextCompositionEventArgs e)
		{
			e.Handled = RaiseEventPair(this, PreviewTextInputEvent, TextInputEvent, 
				new TextCompositionEventArgs(e.Device, e.TextComposition));
		}

		private void TextArea_MouseWheel(object sender, MouseWheelEventArgs e)
		{
            e.Handled = RaiseEventPair(GetScrollEventTarget(), PreviewMouseWheelEvent,
				MouseWheelEvent, new MouseWheelEventArgs(e.MouseDevice, e.Timestamp, e.Delta));
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
