namespace UiMFTemplate.Core.Security.Magic
{
    using UiMFTemplate.Core.Domain;
    using UiMFTemplate.Infrastructure.Security;

    public class MagicAction : EntityAction<Magic, MagicRole>
    {
        public static MagicAction AddComment = new MagicAction(nameof(AddComment), MagicRole.Manager, MagicRole.Complainer);
        public static MagicAction ViewMagic = new MagicAction(nameof(ViewMagic), MagicRole.Manager, MagicRole.Complainer);

        private MagicAction(string name, params MagicRole[] roles) : base(name, roles)
        {
        }
    }
}
