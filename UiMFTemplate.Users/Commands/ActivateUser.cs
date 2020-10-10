namespace UiMFTemplate.Users.Commands
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using MediatR;
    using Microsoft.AspNetCore.Identity;
    using UiMFTemplate.Infrastructure;
    using UiMFTemplate.Infrastructure.Forms;
    using UiMFTemplate.Infrastructure.Forms.ClientFunctions;
    using UiMFTemplate.Infrastructure.Security;
    using UiMFTemplate.Users;
    using UiMFTemplate.Users.Security;
    using UiMetadataFramework.Basic.Output;
    using UiMetadataFramework.Core;
    using UiMetadataFramework.Core.Binding;

    [MyForm(Id = "activate-user", PostOnLoad = true)]
    [Secure(typeof(UserActions), nameof(UserActions.ManageUsers))]
    public class ActivateUser : MyForm<ActivateUser.Request, ActivateUser.Response>
    {
        private readonly ApplicationDbContext applicationDbContext;
        private readonly UserManager<ApplicationUser> userManager;

        public ActivateUser(UserManager<ApplicationUser> userManager, ApplicationDbContext applicationDbContext)
        {
            this.userManager = userManager;
            this.applicationDbContext = applicationDbContext;
        }

        public static FormLink Button(int userId, string label = null)
        {
            return new FormLink
            {
                Form = typeof(ActivateUser).GetFormId(),
                Label = label ?? "Activate",
                InputFieldValues = new Dictionary<string, object>
                {
                    { nameof(Request.Id), userId }
                }
            }.WithAction(FormLinkActions.Run);
        }

        protected override Response Handle(Request message)
        {
            var user = this.userManager.Users.SingleOrDefault(t => t.Id == message.Id);

            if (user == null)
            {
                return new Response();
            }

            if (user.Active)
            {
                throw new BusinessException("The user is already activated");
            }

            user.DateActivated = DateTime.Now;
            this.applicationDbContext.SaveChanges();

            return new Response()
                .WithGrowlMessage($"User {user.UserName} was activated.", GrowlNotificationStyle.Success);
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
