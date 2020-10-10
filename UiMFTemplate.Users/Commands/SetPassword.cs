namespace UiMFTemplate.Users.Commands
{
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using MediatR;
    using Microsoft.AspNetCore.Identity;
    using UiMFTemplate.Infrastructure;
    using UiMFTemplate.Infrastructure.Forms;
    using UiMFTemplate.Infrastructure.Security;
    using UiMFTemplate.Infrastructure.User;
    using UiMFTemplate.Users.Forms;
    using UiMFTemplate.Users.Security;
    using UiMetadataFramework.Basic.Input;
    using UiMetadataFramework.Basic.Output;
    using UiMetadataFramework.Basic.Response;
    using UiMetadataFramework.Core.Binding;
    using UiMetadataFramework.MediatR;

    [MyForm(Id = "set-password", Label = "Reset password", SubmitButtonLabel = "Reset")]
    [Secure(typeof(UserActions), nameof(UserActions.ManageMyAccount))]
    public class SetPassword : AsyncForm<SetPassword.Request, RedirectResponse>
    {
        private readonly UserContext userContext;
        private readonly UserManager<ApplicationUser> userManager;

        public SetPassword(UserManager<ApplicationUser> userManager, UserContext userContext)
        {
            this.userManager = userManager;
            this.userContext = userContext;
        }

        public static FormLink Button()
        {
            return new FormLink
            {
                Form = typeof(SetPassword).GetFormId(),
                Label = "Reset password"
            };
        }

        public override async Task<RedirectResponse> Handle(Request message, CancellationToken cancellationToken)
        {
            var user = this.userManager.Users.Single(t => t.UserName == this.userContext.User.UserName);

            var result = await this.userManager.AddPasswordAsync(
                user,
                message.Password.Value);

            result.EnforceSuccess("Password reset failed.");

            return MyAccount.Button().AsRedirectResponse();
        }

        public class Request : IRequest<RedirectResponse>
        {
            [InputField(Required = true)]
            [UiMFTemplatePasswordInputConfig(true)]
            public Password Password { get; set; }
        }
    }
}
