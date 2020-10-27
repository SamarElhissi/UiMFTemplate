namespace UiMFTemplate.Core.Commands
{
	using System;
	using System.Collections.Generic;
	using System.Threading;
	using System.Threading.Tasks;
	using MediatR;
	using UiMetadataFramework.Basic.Output;
	using UiMetadataFramework.Core;
	using UiMetadataFramework.Core.Binding;
	using UiMFTemplate.Conversations.Forms.Outputs;
	using UiMFTemplate.Core.ConversationManagers;
	using UiMFTemplate.Core.DataAccess;
	using UiMFTemplate.Core.Extensions;
	using UiMFTemplate.Core.Security.Magic;
	using UiMFTemplate.Infrastructure;
	using UiMFTemplate.Infrastructure.Forms;
	using UiMFTemplate.Infrastructure.Forms.Outputs;
	using UiMFTemplate.Infrastructure.Security;

	[MyForm(Id = "Magic-details", PostOnLoad = true, Label = "Magic details")]
	[Secure(typeof(MagicAction), nameof(MagicAction.ViewMagic))]
	public class MagicDetails : MyAsyncForm<MagicDetails.Request, MagicDetails.Response>
	{
		private readonly CoreDbContext context;
		private readonly UserSecurityContext permissionManager;

		public MagicDetails(CoreDbContext context, UserSecurityContext permissionManager)
		{
			this.context = context;
			this.permissionManager = permissionManager;
		}

		public static FormLink Button(int id, string label = null)
		{
			return new FormLink
			{
				Form = typeof(MagicDetails).GetFormId(),
				Label = label ?? "Magic #" + id,
				InputFieldValues = new Dictionary<string, object>
				{
					{ nameof(Request.Id), id }
				}
			};
		}

		public override async Task<Response> Handle(Request message, CancellationToken cancellationToken)
		{
			var Magic = await this.context.Magics
				.SingleNotDeletedOrExceptionAsync(a => a.Id == message.Id, cancellationToken: cancellationToken);

			return new Response
			{
				Id = Magic.Id,
				Title = Magic.Title,
				CreatedOn = Magic.CreatedOn,
				SubmittedOn = Magic.SubmittedOn,
				ClosedOn = Magic.ClosedOn,
				Details = new PreformattedText(Magic.Details),
				Status = Magic.Status.AsHtmlString(),
				Actions = Magic.GetActions(this.permissionManager),
				Conversation = Conversation.Create<MagicConversationManager>(Magic.Id)
			};
		}

		public class Request : IRequest<Response>, ISecureHandlerRequest
		{
			[InputField(Hidden = true, Required = false)]
			public int Id { get; set; }

			[NotField]
			public int ContextId => this.Id;
		}

		public class Response : FormResponse<MyFormResponseMetadata>
		{
			[OutputField(OrderIndex = -1)]
			public ActionList Actions { get; set; }

			[OutputField(OrderIndex = 7, Label = "Date closed")]
			public DateTime? ClosedOn { get; set; }

			[OutputField(OrderIndex = 100, Label = null)]
			public InlineForm Conversation { get; set; }

			[OutputField(OrderIndex = 3, Label = "Date created")]
			public DateTime CreatedOn { get; set; }

			[OutputField(OrderIndex = 9)]
			public PreformattedText Details { get; set; }

			[OutputField(OrderIndex = 1)]
			public int Id { get; set; }

			[OutputField(OrderIndex = 4)]
			public HtmlString Status { get; set; }

			[OutputField(OrderIndex = 5, Label = "Date submitted")]
			public DateTime? SubmittedOn { get; set; }

			[OutputField(OrderIndex = 2)]
			public string Title { get; set; }
		}
	}
}