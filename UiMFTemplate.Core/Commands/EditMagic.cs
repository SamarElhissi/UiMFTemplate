namespace UiMFTemplate.Core.Commands
{
	using System.Collections.Generic;
	using System.Threading;
	using System.Threading.Tasks;
	using UiMetadataFramework.Basic.EventHandlers;
	using UiMetadataFramework.Basic.Input;
	using UiMetadataFramework.Basic.Output;
	using UiMetadataFramework.Core.Binding;
	using UiMFTemplate.Core.DataAccess;
	using UiMFTemplate.Core.Extensions;
	using UiMFTemplate.Core.Security;
	using UiMFTemplate.Infrastructure.Forms;
	using UiMFTemplate.Infrastructure.Forms.Record;
	using UiMFTemplate.Infrastructure.Security;

	[MyForm(Id = "edit-Magic", PostOnLoad = true, Label = "Edit Magic", SubmitButtonLabel = UiFormConstants.EditSubmitLabel,
		PostOnLoadValidation = false)]
	[Secure(typeof(CoreActions), nameof(CoreActions.CreateMagic))]
	public class EditMagic : MyAsyncForm<EditMagic.Request, EditMagic.Response>
	{
		private readonly CoreDbContext context;

		public EditMagic(CoreDbContext context)
		{
			this.context = context;
		}

		public static FormLink Button(int userId, string label = null)
		{
			return new FormLink
			{
				Form = typeof(EditMagic).GetFormId(),
				Label = label ?? "Edit",
				InputFieldValues = new Dictionary<string, object>
				{
					{ nameof(Request.Id), userId }
				}
			};
		}

		public override async Task<Response> Handle(Request message, CancellationToken cancellationToken)
		{
			var Magic = await this.context.Magics
				.SingleNotDeletedOrExceptionAsync(t => t.Id == message.Id, cancellationToken: cancellationToken);

			if (message.Operation?.Value == RecordRequestOperation.Post)
			{
				Magic.Edit(message.Title, message.Details?.Value);
				await this.context.SaveChangesAsync(cancellationToken);
			}

			return new Response
			{
				Title = Magic.Title,
				Details = Magic.Details,
				Metadata = new MyFormResponseMetadata
				{
					Title = $"Edit Magic #{Magic.Id}"
				}
			};
		}

		public class Request : RecordRequest<Response>
		{
			[InputField(OrderIndex = 5, Required = true)]
			[BindToOutput(nameof(Response.Details))]
			public TextareaValue Details { get; set; }

			[InputField(Hidden = true, Required = true)]
			public int Id { get; set; }

			[InputField(OrderIndex = 1, Required = true)]
			[BindToOutput(nameof(Response.Title))]
			public string Title { get; set; }
		}

		public class Response : RecordResponse
		{
			[NotField]
			public string Details { get; set; }

			[NotField]
			public string Title { get; set; }
		}
	}
}
