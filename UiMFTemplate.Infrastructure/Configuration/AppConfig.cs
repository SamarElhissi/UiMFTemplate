namespace UiMFTemplate.Infrastructure.Configuration
{
	public class AppConfig
	{
		public string EmailDeliveryMethod { get; set; }
		public string Environment { get; set; }
		public string MailJetApiKey { get; set; }
		public string MailJetApiSecret { get; set; }
		public string NoReplyEmail { get; set; }
		public string SendGridApiKey { get; set; }
		public string SiteRoot { get; set; }
		public string Version { get; set; }
	}
}
