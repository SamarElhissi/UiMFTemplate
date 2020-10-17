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
	using UiMFTemplate.Core.Domain.Enum;
	using UiMFTemplate.Core.Extensions;
	using UiMFTemplate.Core.Menus;
	using UiMFTemplate.Core.Security;
	using UiMFTemplate.Infrastructure;
	using UiMFTemplate.Infrastructure.EntityFramework;
	using UiMFTemplate.Infrastructure.Forms;
	using UiMFTemplate.Infrastructure.Forms.CustomProperties;
	using UiMFTemplate.Infrastructure.Forms.Outputs;
	using UiMFTemplate.Infrastructure.Security;

	[MyForm(Id = "Magics", PostOnLoad = true, Label = "Magics", Menu = CoreMenus.Settings, MenuOrderIndex = 1,
		SubmitButtonLabel = UiFormConstants.SearchLabel)]
	[Secure(typeof(CoreActions), nameof(CoreActions.ViewAllMagic))]
	[CssClass(UiFormConstants.InputsVerticalMultipleColumn)]
	public class Magics : Form<Magics.Request, Magics.Response>
	{
		private readonly CoreDbContext context;
		private readonly UserSecurityContext permissionManager;

		public Magics(
			CoreDbContext context,
			UserSecurityContext permissionManager)
		{
			this.context = context;
			this.permissionManager = permissionManager;
		}

		protected override Response Handle(Request message)
		{
			var query = this.context.Magics
				.Include(a => a.CreatedByUser)
				.NotDeleted()
				.Where(a => a.Status != MagicStatus.Draft)
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
				Magics = result
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
				this.Complainer = t.CreatedByUser.Name;
				this.CreatedOn = t.CreatedOn;
			}

			[OutputField(OrderIndex = 20, Label = "")]
			[HiddenInExcel]
			public ActionList Actions { get; set; }

			[OutputField(OrderIndex = 6)]
			public HtmlString Details { get; set; }

			[OutputField(OrderIndex = 1)]
			public FormLink Id { get; set; }

			[OutputField(OrderIndex = 8)]
			public Alert Status { get; set; }

			[OutputField(OrderIndex = 2)]
			public string Title { get; set; }

			[OutputField(OrderIndex = 4, Label = "Complainer")]
			public string Complainer { get; set; }

			[OutputField(OrderIndex = 5, Label = "Date created")]
			public DateTime CreatedOn { get; set; }
		}
	}
}
