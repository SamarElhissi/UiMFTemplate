namespace UiMFTemplate.Infrastructure.Forms.Inputs
{
	using UiMetadataFramework.Core.Binding;

	public class HtmlEditor
	{
		public HtmlEditor(string value)
		{
			this.Value = value;
		}

		public HtmlEditor()
		{
		}

		public string Value { get; set; }
	}

	public class HtmlEditorInputFieldBinding : InputFieldBinding
	{
		public HtmlEditorInputFieldBinding() : base(typeof(HtmlEditor), "html-editor")
		{
		}
	}
}