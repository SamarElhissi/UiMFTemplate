namespace UiMFTemplate.Infrastructure.Decorators
{
	using System.Threading;
	using System.Threading.Tasks;
	using MediatR;
	using UiMFTemplate.Infrastructure.Security;

	public class SecurityDecorator<TRequest, TResponse> :
		IPipelineBehavior<TRequest, TResponse> where TRequest : IRequest<TResponse>
	{
		private readonly UserSecurityContext userSecurityContext;

		public SecurityDecorator(
			IRequestHandler<TRequest, TResponse> innerCommand,
			UserSecurityContext userSecurityContext)
		{
			this.InnerCommand = innerCommand;
			this.userSecurityContext = userSecurityContext;
		}

		public IRequestHandler<TRequest, TResponse> InnerCommand { get; }

		public async Task<TResponse> Handle(TRequest message, CancellationToken cancellationToken, RequestHandlerDelegate<TResponse> next)
		{
			var type = this.InnerCommand.GetType();

			switch (message)
			{
				case ISecureHandlerRequest r:
					this.userSecurityContext.EnforceCanAccess(type, r.ContextId);
					break;
				default:
					this.userSecurityContext.EnforceCanAccess(type);
					break;
			}

			return await this.InnerCommand.Handle(message, cancellationToken);
		}
	}
}