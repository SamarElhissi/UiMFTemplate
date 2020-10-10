namespace UiMFTemplate.Core.Commands.User
{
    using System.Collections.Generic;
    using System.Linq;
    using MediatR;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.EntityFrameworkCore;
    using UiMFTemplate.Help;
    using UiMFTemplate.Infrastructure;
    using UiMFTemplate.Infrastructure.Forms;
    using UiMFTemplate.Infrastructure.Security;
    using UiMFTemplate.Users;
    using UiMFTemplate.Users.Security;
    using UiMetadataFramework.Basic.Output;
    using UiMetadataFramework.Core;
    using UiMetadataFramework.Core.Binding;

    [MyForm(Id = "user-details", PostOnLoad = true, Label = "User details")]
    [Secure(typeof(UserActions), nameof(UserActions.ManageUsers))]
    [Documentation(DocumentationPlacement.Inline, DocumentationSourceType.String,
        "This page lists account details of the user, specifically the roles that they have inside the system. " +
        "**Each role is associated with a specific context**. `System` roles are those that are associated with the " +
        "overall system and are applicable throughout the system. **Roles define what user can and cannot " +
        "do in the system**.")]
    public class UserDetails : MyForm<UserDetails.Request, UserDetails.Response>
    {
        private readonly UserManager<ApplicationUser> userManager;

        public UserDetails(UserManager<ApplicationUser> userManager)
        {
            this.userManager = userManager;
        }

        public static FormLink Button(int id, string label = null)
        {
            return new FormLink
            {
                Form = typeof(UserDetails).GetFormId(),
                Label = label ?? id.ToString(),
                InputFieldValues = new Dictionary<string, object>
                {
                    { nameof(Request.Id), id }
                }
            };
        }

        protected override Response Handle(Request message)
        {
            var roles = new List<Role>();
            var userId = message.Id;
            var user = this.userManager.Users
                .Include(t => t.Roles)
                .ThenInclude(t => t.Role)
                .SingleOrException(s => s.Id == userId);

            roles.AddRange(user.Roles.Select(s => new Role(s.Role.Name, "System")));

            return new Response
            {
                Id = user.Id,
                Username = user.UserName,
                Email = user.Email,
                Roles = roles
            };
        }

        public class Request : IRequest<Response>
        {
            [InputField(Hidden = true, Required = false)]
            public int Id { get; set; }
        }

        public class Response : FormResponse<MyFormResponseMetadata>
        {
            [OutputField(OrderIndex = 10)]
            public string Email { get; set; }

            [OutputField(OrderIndex = 1)]
            public int Id { get; set; }

            [OutputField(OrderIndex = 20)]
            public IEnumerable<Role> Roles { get; set; }

            [OutputField(OrderIndex = 3)]
            public string Username { get; set; }
        }

        public class Role
        {
            public Role()
            {
            }

            public Role(string roleName, string context)
            {
                this.RoleName = roleName;
                this.Context = context;
            }

            [OutputField(OrderIndex = 10)]
            [Documentation(DocumentationPlacement.Hint, DocumentationSourceType.String,
                "Each role is associated with a specific context. `System` roles are those that are associated with " +
                "the overall system and are applicable throughout the system.")]
            public string Context { get; set; }

            [OutputField(OrderIndex = 1, Label = "Role")]
            public string RoleName { get; set; }
        }
    }
}
