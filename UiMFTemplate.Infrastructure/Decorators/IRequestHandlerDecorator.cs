namespace UiMFTemplate.Infrastructure.Decorators
{
	using MediatR;

	public interface IRequestHandlerDecorator<in TRequest, TResponse> : IRequestHandler<TRequest, TResponse>
		where TRequest : IRequest<TResponse>
	{
		IRequestHandler<TRequest, TResponse> InnerCommand { get; }
	}
}
