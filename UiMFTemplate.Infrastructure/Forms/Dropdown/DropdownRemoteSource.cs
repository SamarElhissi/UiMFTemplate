namespace UiMFTemplate.Infrastructure.Forms.Dropdown
{
	using MediatR;
	using UiMetadataFramework.Basic.Input.Dropdown;
	using UiMetadataFramework.MediatR;

	public abstract class IDropdownInlineSource<TRequest> :
		Form<TRequest, DropdownResponse>, IDropdownRemoteSource
		where TRequest : IRequest<DropdownResponse>
	{
	}
}