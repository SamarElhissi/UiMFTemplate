namespace UiMFTemplate.Users.Commands
{
    using System.Threading;
    using System.Threading.Tasks;
    using System.Web;
    using MediatR;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.Extensions.Options;
    using UiMFTemplate.Infrastructure;
    using UiMFTemplate.Infrastructure.Configuration;
    using UiMFTemplate.Infrastructure.Forms;
    using UiMFTemplate.Infrastructure.Forms.CustomProperties;
    using UiMFTemplate.Infrastructure.Forms.Inputs;
    using UiMFTemplate.Infrastructure.Forms.Outputs;
    using UiMFTemplate.Infrastructure.Messages;
    using UiMFTemplate.Infrastructure.Security;
    using UiMFTemplate.Users.Security;
    using UiMetadataFramework.Core;
    using UiMetadataFramework.Core.Binding;
    using UiMetadataFramework.MediatR;

    [MyForm(Id = "forgot-password", Label = "Forget password", SubmitButtonLabel = "Reset password")]
    [Secure(typeof(UserActions), nameof(UserActions.Login))]
    [CssClass(UiFormConstants.CardLayout)]
    public class ForgotPassword : AsyncForm<ForgotPassword.Request, ForgotPassword.Response>
    {
        private readonly AppConfig appConfig;
        private readonly IEmailSender emailSender;
        private readonly UserManager<ApplicationUser> userManager;

        public ForgotPassword(UserManager<ApplicationUser> userManager, IEmailSender emailSender, IOptions<AppConfig> appConfig)
        {
            this.userManager = userManager;
            this.emailSender = emailSender;
            this.appConfig = appConfig.Value;
        }

        public override async Task<Response> Handle(Request message, CancellationToken cancellationToken)
        {
            var user = await this.userManager.FindByEmailAsync(message.Email?.Value);

            if (user == null)
            {
                throw new BusinessException("The email not exists");
            }

            var token = await this.userManager.GeneratePasswordResetTokenAsync(user);

            var resetPasswordUrl = $"{this.appConfig.SiteRoot}#/form/reset-password" +
                $"?Token={HttpUtility.UrlEncode(token)}" +
                $"&Id={user.Id}";

            await this.emailSender.SendEmailAsync(
                user.Email,
                "Reset password",
                $"For reset your password, click on the following link: <a href='{resetPasswordUrl}'>{resetPasswordUrl}</a>");

            return new Response
            {
                Result = Alert.Success("Reset password link has been sent to your email, please check your inbox.")
            };
        }

        public class Response : FormResponse
        {
            public Alert Result { get; set; }
        }

        public class Request : IRequest<Response>
        {
            [InputField(Required = true)]
            public Email Email { get; set; }
        }
    }
}
