namespace UiMFTemplate.Infrastructure.Forms
{
	using System.Collections.Generic;
	using System.Linq;
	using System.Reflection;
	using System.Threading;
	using System.Threading.Tasks;
	using MediatR;
	using UiMetadataFramework.Core;
	using UiMetadataFramework.MediatR;
	using UiMFTemplate.Infrastructure.Forms.Menu;
	using UiMFTemplate.Infrastructure.Security;
	using UiMFTemplate.Infrastructure.User;

	/// <summary>
	/// Gets all forms available to the current user.
	/// </summary>
	public class MyMenu : IRequestHandler<MyMenu.Request, MyMenu.Response>
	{
		private readonly FormRegister formRegister;
		private readonly MenuRegister menuRegister;
		private readonly SystemPermissionManager permissionManager;
		private readonly UserContext userContext;

		public MyMenu(UserContext userContext,
			FormRegister formRegister,
			MenuRegister menuRegister,
			SystemPermissionManager permissionManager)
		{
			this.userContext = userContext;
			this.formRegister = formRegister;
			this.menuRegister = menuRegister;
			this.permissionManager = permissionManager;
		}

		public Task<Response> Handle(Request message, CancellationToken cancellationToken)
		{
			// Get forms with [SecureForm] attribute, which use non-context-based permissions.
			var allForms = this.formRegister.RegisteredForms
				.Select(t => new
				{
					Attribute = t.FormType.GetCustomAttribute<SecureAttribute>(),
					FormInfo = t
				})
				.ToList();

			var list = new List<FormMetadata>();

			foreach (var form in allForms)
			{
				if (form.Attribute != null)
				{
					if (form.Attribute.ContextType == null)
					{
						var canDo = this.permissionManager.CanDo(form.Attribute.Permission, this.userContext);

						if (canDo)
						{
							// Form with context-less security.
							list.Add(form.FormInfo.Metadata);
						}
					}
					else if (message.IncludeWithContextBasedSecurity)
					{
						// Form with context-based security.
						list.Add(form.FormInfo.Metadata);
					}
				}
				else
				{
					// Form without any security , i.e. - available to all users.
					list.Add(form.FormInfo.Metadata);
				}
			}

			return Task.FromResult(new Response
			{
				Menu = this.menuRegister.BuildMenu(list)
			});
		}

		public class Request : IRequest<Response>
		{
			/// <summary>
			/// Indicates whether to retrieve forms which use context-based security.
			/// </summary>
			public bool IncludeWithContextBasedSecurity { get; set; }
		}

		public class Response
		{
			public IMenuNode Menu { get; set; }
		}
	}
}