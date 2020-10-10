namespace UiMFTemplate.Web.Users.Commands
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using MediatR;
    using Microsoft.AspNetCore.DataProtection;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Identity;
    using Newtonsoft.Json;
    using UiMFTemplate.Infrastructure;
    using UiMFTemplate.Infrastructure.Forms;
    using UiMFTemplate.Infrastructure.Forms.Outputs;
    using UiMFTemplate.Infrastructure.Security;
    using UiMFTemplate.Infrastructure.User;
    using UiMFTemplate.Users.Security;
    using UiMetadataFramework.Basic.Output;
    using UiMetadataFramework.Basic.Response;
    using UiMetadataFramework.Core.Binding;
    using UiMetadataFramework.MediatR;
	using UiMFTemplate.Users;
	using UiMFTemplate.Users.Commands;

	[MyForm(Id = "start-impersonation", PostOnLoad = true, Label = "Impersonate user")]
    [Secure(typeof(UserActions), nameof(UserActions.ManageUsers))]
    public class StartImpersonation : Form<StartImpersonation.Request, ReloadResponse>
    {
        private readonly IDataProtector protector;
        private readonly SignInManager<ApplicationUser> signInManager;
        private readonly UserSession userSession;

        public StartImpersonation(
            SignInManager<ApplicationUser> signInManager,
            IDataProtectionProvider protectionProvider,
            UserSession userSession)
        {
            this.signInManager = signInManager;
            this.userSession = userSession;
            this.protector = protectionProvider.CreateProtector("app-cookie");
        }

        public static FormLink Button(int userId)
        {
            return new FormLink
            {
                Form = typeof(StartImpersonation).GetFormId(),
                Label = UiFormConstants.ImpersonationLabel,
                InputFieldValues = new Dictionary<string, object>
                {
                    { nameof(Request.Id), userId }
                }
            }.WithAction(FormLinkActions.Run).WithCustomUi(LinkStyle.Small);
        }

        protected override ReloadResponse Handle(Request message)
        {
            // If user is trying to stop the impersonation, by impersonating
            // their own account.
            if (this.userSession.ImpersonatorUserId == message.Id)
            {
                return new ReloadResponse
                {
                    Form = typeof(StopImpersonation).GetFormId()
                };
            }

            if (this.userSession.CurrentUserId != message.Id)
            {
                var user = this.signInManager.UserManager.Users.SingleOrException(t => t.Id.ToString() == message.Id);
                var roles = this.signInManager.UserManager.GetRolesAsync(user).Result;
                var session = new UserSession(message.Id, this.userSession.RealCurrentUserId)
                {
                    Roles = roles.ToList()
                };
                var json = JsonConvert.SerializeObject(session);
                var encryptedJson = this.protector.Protect(json);

                this.signInManager.Context.Response.Cookies.Delete("app-data");
                this.signInManager.Context.Response.Cookies.Delete("user-data");
                this.signInManager.Context.Response.Cookies.Append("user-data", encryptedJson, new CookieOptions
                {
                    SameSite = SameSiteMode.Lax,
                    // Refresh cookie every 60 minute to avoid stale data. 
                    Expires = DateTimeOffset.UtcNow.AddMinutes(60)
                });
            }

            return new ReloadResponse
            {
                Form = typeof(MyAccount).GetFormId()
            };
        }

        public class Request : IRequest<ReloadResponse>
        {
            [InputField(Hidden = true, Required = true)]
            public string Id { get; set; }
        }
    }
}
