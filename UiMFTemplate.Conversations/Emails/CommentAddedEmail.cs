namespace UiMFTemplate.Conversations.Emails
{
	using Microsoft.Extensions.Options;
	using UiMFTemplate.Infrastructure.Configuration;
	using UiMFTemplate.Infrastructure.Emails;
	using UiMFTemplate.Infrastructure.Messages;

	public class CommentAddedEmail : EmailTemplate<CommentAddedEmail.Model>
	{
		public CommentAddedEmail(IOptions<AppConfig> appConfig, IViewRenderService viewRenderService) : base(appConfig, viewRenderService,
			nameof(CommentAddedEmail))
		{
		}

		protected override string GetSubject(Model model)
		{
			return "New comment posted";
		}

		public class Model
		{
			private readonly AppConfig appConfig;

			public Model(AppConfig appConfig, string message, string username, string email, string link)
			{
				this.appConfig = appConfig;
				this.Message = message;
				this.Username = username;
				this.Email = email;
				this.Link = link;
			}

			public string Email { get; set; }
			public string Link { get; set; }

			public string Message { get; set; }
			public string Url => $"{this.appConfig.SiteRoot}/#/form/{this.Link}";
			public string Username { get; set; }
		}
	}
}
