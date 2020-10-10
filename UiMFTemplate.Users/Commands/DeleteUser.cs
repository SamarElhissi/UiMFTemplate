namespace UiMFTemplate.Users.Commands
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using MediatR;
    using Microsoft.AspNetCore.Identity;
    using UiMFTemplate.Infrastructure;
    using UiMFTemplate.Infrastructure.Forms;
    using UiMFTemplate.Infrastructure.Security;
    using UiMFTemplate.Users.Security;
    using UiMetadataFramework.Basic.Output;
    using UiMetadataFramework.Core;
    using UiMetadataFramework.Core.Binding;

    [MyForm(Id = "delete-user", PostOnLoad = true)]
    [Secure(typeof(UserActions), nameof(UserActions.ManageUsers))]
    public class DeleteUser : MyAsyncForm<DeleteUser.Request, DeleteUser.Response>
    {
        private readonly UserManager<ApplicationUser> userManager;

        public DeleteUser(UserManager<ApplicationUser> userManager)
        {
            this.userManager = userManager;
        }

        public static FormLink Button(int userId, string label = null)
        {
            return new FormLink
            {
                Form = typeof(DeleteUser).GetFormId(),
                Label = label ?? "Delete",
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

            if (user.HasLoggedIn)
            {
                throw new BusinessException("Cannot delete user who has logged in the past.");
            }

            var result = await this.userManager.DeleteAsync(user);
            result.EnforceSuccess("Cannot delete user.");

            return new Response();
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
