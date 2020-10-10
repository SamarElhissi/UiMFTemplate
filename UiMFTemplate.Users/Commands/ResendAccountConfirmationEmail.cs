namespace UiMFTemplate.Users.Commands
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using MediatR;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.Extensions.Options;
    using UiMFTemplate.Infrastructure;
    using UiMFTemplate.Infrastructure.Configuration;
    using UiMFTemplate.Infrastructure.Forms;
    using UiMFTemplate.Infrastructure.Forms.ClientFunctions;
    using UiMFTemplate.Infrastructure.Messages;
    using UiMFTemplate.Infrastructure.Security;
    using UiMFTemplate.Users.Security;
    using UiMetadataFramework.Basic.Output;
    using UiMetadataFramework.Core;
    using UiMetadataFramework.Core.Binding;

    [MyForm(Id = "resend-account-confirmation-email", PostOnLoad = true)]
    [Secure(typeof(UserActions), nameof(UserActions.ManageUsers))]
    public class ResendAccountConfirmationEmail : MyAsyncForm<ResendAccountConfirmationEmail.Request, ResendAccountConfirmationEmail.Response>
    {
        private readonly AppConfig appConfig;
        private readonly IEmailSender emailSender;
        private readonly UserManager<ApplicationUser> userManager;

        public ResendAccountConfirmationEmail(UserManager<ApplicationUser> userManager, IOptions<AppConfig> appConfig, IEmailSender emailSender)
        {
            this.userManager = userManager;
            this.appConfig = appConfig.Value;
            this.emailSender = emailSender;
        }

        public static FormLink Button(int userId, string label = null)
        {
            return new FormLink
            {
                Form = typeof(ResendAccountConfirmationEmail).GetFormId(),
                Label = label ?? "Resend activation email",
                InputFieldValues = new Dictionary<string, object>
                {
                    { nameof(Request.Id), userId }
                }
            }.WithAction(FormLinkActions.Run);
        }

        public override async Task<Response> Handle(Request message, CancellationToken cancellationToken)
        {
            var user = this.userManager.Users.SingleOrDefault(t => t.Id == message.Id);

            if (user == null)
            {
                return new Response();
            }

            var hasPassword = await this.userManager.HasPasswordAsync(user);
            if (user.EmailConfirmed || hasPassword)
            {
                throw new BusinessException("The user has already activated their email.");
            }

            await user.SendConfirmationEmail(this.appConfig, this.emailSender, this.userManager);

            return new Response()
                .WithGrowlMessage($"Activation email for user {user.UserName} was sent.", GrowlNotificationStyle.Success);
        }

        public class Response : FormResponse<MyFormResponseMetadata>
        {
        }

        public class Request : IRequest<Response>
        {
            [InputField(Hidden = true)]
            public int Id { get; set; }
        }
    }
}
