namespace UiMFTemplate.Core.Security.Magic
{
	using UiMFTemplate.Infrastructure.Security;

	public class MagicRole : Role
	{
		public static MagicRole Manager = new MagicRole(nameof(Manager));
		public static MagicRole Complainer = new MagicRole(nameof(Complainer));

		private MagicRole(string name) : base(name)
		{
		}
	}
}