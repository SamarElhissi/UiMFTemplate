namespace UiMFTemplate.Core.Commands
{
	using System;
	using System.Threading;
	using System.Threading.Tasks;
	using MediatR;
	using UiMetadataFramework.Basic.Input;
	using UiMetadataFramework.Basic.Output;
	using UiMetadataFramework.Core.Binding;
	using UiMFTemplate.Core.DataAccess;
	using UiMFTemplate.Core.Domain;
	using UiMFTemplate.Core.Security;
	using UiMFTemplate.Infrastructure.Forms;
	using UiMFTemplate.Infrastructure.Security;
	using UiMFTemplate.Infrastructure.User;

	[MyForm(Id = "create-Magic", Label = "Create Magic")]
	[Secure(typeof(CoreActions), nameof(CoreActions.CreateMagic))]
	public class CreateMagic : MyAsyncForm<CreateMagic.Request, CreateMagic.Response>
	{
		private readonly CoreDbContext context;
		private readonly UserContext userContext;

		public CreateMagic(CoreDbContext context, UserContext userContext)
		{
			this.context = context;
			this.userContext = userContext;
		}

		public static FormLink Button()
		{
			return new FormLink
			{
				Form = typeof(CreateMagic).GetFormId(),
				Label = "Create new Magic"
			};
		}

		public override async Task<Response> Handle(Request message, CancellationToken cancellationToken)
		{
			var userId = Convert.ToInt32(this.userContext.User.UserId);
			var magic = new Magic(message.Title, message.Description?.Value, userId);

			await this.context.Magics.AddAsync(magic, cancellationToken);
			await this.context.SaveChangesAsync(cancellationToken);

			return new Response();
		}

		public class Request : IRequest<Response>
		{
			[InputField(OrderIndex = 2, Required = true)]
			public TextareaValue Description { get; set; }

			[InputField(OrderIndex = 0, Required = true)]
			public string Title { get; set; }
		}

		public class Response : MyFormResponse
		{
		}
	}
}
