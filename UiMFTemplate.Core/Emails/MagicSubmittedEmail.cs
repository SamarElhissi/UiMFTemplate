namespace UiMFTemplate.Core.Emails
{
    using Microsoft.Extensions.Options;
    using UiMFTemplate.Infrastructure.Configuration;
    using UiMFTemplate.Infrastructure.Emails;
    using UiMFTemplate.Infrastructure.Messages;

    public class MagicSubmittedEmail : EmailTemplate<MagicSubmittedEmail.Model>
    {
        public MagicSubmittedEmail(IOptions<AppConfig> appConfig, IViewRenderService viewRenderService) : base(appConfig, viewRenderService,
            nameof(MagicSubmittedEmail))
        {
        }

        protected override string GetSubject(Model model)
        {
            return "New Magic submitted";
        }

        public class Model
        {

            public Model(string message, string username, string email, string link)
            {
                this.Message = message;
                this.Username = username;
                this.Email = email;
                this.Link = link;
            }

            public string Email { get; set; }
            public string Link { get; set; }

            public string Message { get; set; }
            public string Url => $"{this.Link}";
            public string Username { get; set; }
        }
    }
}
