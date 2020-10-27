namespace UiMFTemplate.Core.Emails
{
	using Microsoft.Extensions.Options;
	using UiMFTemplate.Infrastructure.Configuration;
	using UiMFTemplate.Infrastructure.Emails;
	using UiMFTemplate.Infrastructure.Messages;

	public class MagicClosedEmail : EmailTemplate<MagicClosedEmail.Model>
	{
		public MagicClosedEmail(IOptions<AppConfig> appConfig, IViewRenderService viewRenderService) : base(appConfig, viewRenderService,
			nameof(MagicSubmittedEmail))
		{
		}

		protected override string GetSubject(Model model)
		{
			return $"Magic #{model.Id} closed";
		}

		public class Model
		{
			public Model(int id, string message, string username, string email, string link)
			{
				this.Message = message;
				this.Username = username;
				this.Email = email;
				this.Link = link;
				this.Id = id;
			}

			public string Email { get; set; }
			public int Id { get; set; }
			public string Link { get; set; }

			public string Message { get; set; }
			public string Url => $"{this.Link}";
			public string Username { get; set; }
		}
	}
}