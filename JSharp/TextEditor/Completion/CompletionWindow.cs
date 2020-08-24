using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using ICSharpCode.AvalonEdit.CodeCompletion;
using ICSharpCode.AvalonEdit.Document;

namespace JSharp.TextEditor.Completion
{
	/// <inheritdoc />
	/// <summary>
	/// Completion window for text editor
	/// </summary>
	public class EditorCompletionWindow : CompletionWindowBase
    {
	    public static readonly EditorCompletionList _editorCompletionList = new EditorCompletionList();

	    private static bool _completionDataInitialized;

	    readonly TextEditor _editor;
	    public EditorCompletionWindow(TextEditor editor) : base(editor.TextArea)
        {
			_editor = editor;
            SizeToContent = SizeToContent.Height;
			MaxHeight = 150.0;
			Width = 400;
			Content = _editorCompletionList;
			MinHeight = 15.0;
			MinWidth = 30.0;
			AttachEvents();
		}

        public EditorCompletionList EditorCompletionList
        {
            get => default;
            set
            {
            }
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
				lock(_editorCompletionList) { _editorCompletionList.Add(key, true); }
			});

			_completionDataInitialized = true;

			return false;
		}

		private void CompletionList_InsertionRequested(object sender, EventArgs e)
		{
			Close();
			_editorCompletionList.SelectedItem?.Complete(TextArea, 
				new AnchorSegment(TextArea.Document, _editor.ClosestWordOffset, EndOffset - _editor.ClosestWordOffset), e);
		}

		private void AttachEvents()
		{
			_editorCompletionList.InsertionRequested += CompletionList_InsertionRequested;
			TextArea.Caret.PositionChanged += CaretPositionChanged;
            TextArea.MouseWheel += TextArea_MouseWheel;
            TextArea.PreviewTextInput += TextArea_PreviewTextInput;
		}

		protected override void DetachEvents()
		{
			_editorCompletionList.InsertionRequested -= CompletionList_InsertionRequested;
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
				_editorCompletionList.HandleKey(e);
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
            return _editorCompletionList == null ? this :
				(UIElement)(_editorCompletionList.ScrollViewer ?? _editorCompletionList.ListBox ?? ((object)_editorCompletionList));
        }

        private void CaretPositionChanged(object sender, EventArgs e)
		{
			var offset = TextArea.Caret.Offset;
			if (offset == StartOffset)
			{
				Close();
				_editorCompletionList.SelectItem(string.Empty);
				return;
			}
			if (offset < StartOffset || offset > EndOffset)
			{
				Close();
				return;
			}
			var document = TextArea.Document;
			if (document != null)
				_editorCompletionList.SelectItem(document.GetText(StartOffset, offset - StartOffset));
		}

    }
}
