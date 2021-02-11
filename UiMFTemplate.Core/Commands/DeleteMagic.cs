namespace UiMFTemplate.Core.Commands
{
	using System.Collections.Generic;
	using System.Threading;
	using System.Threading.Tasks;
	using MediatR;
	using UiMetadataFramework.Basic.Output;
	using UiMetadataFramework.Core;
	using UiMetadataFramework.Core.Binding;
	using UiMFTemplate.Core.DataAccess;
	using UiMFTemplate.Core.Extensions;
	using UiMFTemplate.Core.Security;
	using UiMFTemplate.Infrastructure;
	using UiMFTemplate.Infrastructure.Forms;
	using UiMFTemplate.Infrastructure.Forms.ClientFunctions;
	using UiMFTemplate.Infrastructure.Security;

	[MyForm(Id = "delete-Magic", PostOnLoad = true)]
	[Secure(typeof(CoreActions), nameof(CoreActions.CreateMagic))]
	public class DeleteMagic : MyAsyncForm<DeleteMagic.Request, DeleteMagic.Response>
	{
		private readonly CoreDbContext context;

		public DeleteMagic(CoreDbContext context)
		{
			this.context = context;
		}

		public static FormLink Button(int userId, string label = null)
		{
			return new FormLink
			{
				Form = typeof(DeleteMagic).GetFormId(),
				Label = label ?? "Delete",
				InputFieldValues = new Dictionary<string, object>
				{
					{ nameof(Request.Id), userId }
				}
			}.WithAction(FormLinkActions.Run);
		}

		public override async Task<Response> Handle(Request message, CancellationToken cancellationToken)
		{
			var Magic = await this.context.Magics
				.SingleNotDeletedOrExceptionAsync(t => t.Id == message.Id, cancellationToken: cancellationToken);

			Magic.Delete();
			await this.context.SaveChangesAsync(cancellationToken);

			return new Response()
				.WithGrowlMessage($"Magic #{Magic.Id} was deleted.", GrowlNotificationStyle.Success);
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
