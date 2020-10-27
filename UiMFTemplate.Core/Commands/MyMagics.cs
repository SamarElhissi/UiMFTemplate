namespace UiMFTemplate.Core.Commands
{
	using System;
	using System.Linq;
	using MediatR;
	using Microsoft.EntityFrameworkCore;
	using UiMetadataFramework.Basic.Input;
	using UiMetadataFramework.Basic.Output;
	using UiMetadataFramework.Core;
	using UiMetadataFramework.Core.Binding;
	using UiMetadataFramework.MediatR;
	using UiMFTemplate.Core.DataAccess;
	using UiMFTemplate.Core.Domain;
	using UiMFTemplate.Core.Extensions;
	using UiMFTemplate.Core.Menus;
	using UiMFTemplate.Core.Security;
	using UiMFTemplate.Help;
	using UiMFTemplate.Infrastructure;
	using UiMFTemplate.Infrastructure.EntityFramework;
	using UiMFTemplate.Infrastructure.Forms;
	using UiMFTemplate.Infrastructure.Forms.CustomProperties;
	using UiMFTemplate.Infrastructure.Forms.Outputs;
	using UiMFTemplate.Infrastructure.Security;
	using UiMFTemplate.Infrastructure.User;

	[MyForm(Id = "my-Magics", PostOnLoad = true, Label = "My Magics", Menu = CoreMenus.Settings, MenuOrderIndex = 1,
		SubmitButtonLabel = UiFormConstants.SearchLabel)]
	[Secure(typeof(CoreActions), nameof(CoreActions.CreateMagic))]
	[Documentation(DocumentationPlacement.Inline, DocumentationSourceType.String,
		"This page shows list of all users registered in the system. As `SysAdmin` you are able to " +
		"create and deactivate user accounts. Deactivated accounts won't be able to login and use the " +
		"system (if user is already logged in, they will be logged out automatically).")]
	[CssClass(UiFormConstants.InputsVerticalMultipleColumn)]
	public class MyMagics : Form<MyMagics.Request, MyMagics.Response>
	{
		private readonly CoreDbContext context;
		private readonly UserSecurityContext permissionManager;
		private readonly UserContext userContext;

		public MyMagics(
			CoreDbContext context,
			UserSecurityContext permissionManager,
			UserContext userContext)
		{
			this.context = context;
			this.permissionManager = permissionManager;
			this.userContext = userContext;
		}

		protected override Response Handle(Request message)
		{
			var query = this.context.Magics
				.Where(a => a.CreatedByUserId.ToString() == this.userContext.User.UserId)
				.NotDeleted()
				.AsNoTracking();

			if (message.Id != null)
			{
				query = query.Where(u => u.Id.Equals(message.Id));
			}

			if (!string.IsNullOrEmpty(message.Title))
			{
				query = query.Where(u => u.Title.Contains(message.Title));
			}

			var result = query
				.OrderBy(t => t.Id)
				.Paginate(t => new Item(t, t.GetActions(this.permissionManager)), message.Paginator);

			return new Response
			{
				Magics = result,
				Actions = this.permissionManager.CanAccess<CreateMagic>()
					? new ActionList(CreateMagic.Button())
					: null
			};
		}

		public class Request : IRequest<Response>
		{
			[InputField(OrderIndex = 0)]
			[PositiveIntInput]
			public int? Id { get; set; }

			public Paginator Paginator { get; set; }

			[InputField(OrderIndex = 5)]
			public string Title { get; set; }
		}

		public class Response : FormResponse
		{
			[OutputField(OrderIndex = -10)]
			public ActionList Actions { get; set; }

			[PaginatedData(nameof(Request.Paginator), Label = "")]
			public PaginatedData<Item> Magics { get; set; }
		}

		public class Item
		{
			public Item(Magic t, ActionList actions)
			{
				this.Id = MagicDetails.Button(t.Id);
				this.Title = t.Title;
				this.Details = t.Details.AsHtmlString();
				this.Status = t.GetStatus();
				this.Actions = actions;
				this.CreatedOn = t.CreatedOn;
			}

			[OutputField(OrderIndex = 20, Label = "")]
			[HiddenInExcel]
			public ActionList Actions { get; set; }

			[OutputField(OrderIndex = 3, Label = "Date created")]
			public DateTime CreatedOn { get; set; }

			[OutputField(OrderIndex = 5)]
			public HtmlString Details { get; set; }

			[OutputField(OrderIndex = 1)]
			public FormLink Id { get; set; }

			[OutputField(OrderIndex = 8)]
			public Alert Status { get; set; }

			[OutputField(OrderIndex = 2)]
			public string Title { get; set; }
		}
	}
}