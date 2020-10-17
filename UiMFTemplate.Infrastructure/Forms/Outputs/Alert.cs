namespace UiMFTemplate.Infrastructure.Forms.Outputs
{
	using UiMetadataFramework.Core.Binding;

	[OutputFieldType("alert")]
	public class Alert
	{
		public Alert(string heading)
		{
			this.Heading = heading;
		}
		public Alert(string heading, string message, string style, string icon = null)
		{
			this.Heading = heading;
			this.Message = message;
			this.Style = style;
			this.Icon = icon;
		}

		public string Heading { get; set; }
		public string Message { get; set; }
		public string Style { get; set; }
		public string Icon { get; set; }

		public static Alert Error(string heading, string message = null, string icon = null)
		{
			return new Alert(heading, message, "danger", icon);
		}

		public static Alert Success(string heading, string message = null, string icon = null)
		{
			return new Alert(heading, message, "success", icon);
		}

		public static Alert Warning(string heading, string message = null, string icon = null)
		{
			return new Alert(heading, message, "warning", icon);
		}
	}

	public class AlertStyle
	{
		public const string Danger = "danger";
		public const string Success = "success";
		public const string Warning = "warning";
		public const string Info = "info";
	}
}
