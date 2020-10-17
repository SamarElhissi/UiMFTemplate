namespace UiMFTemplate.Core.Security.Magic
{
	using UiMFTemplate.Core.DataAccess;
	using UiMFTemplate.Core.Domain;
	using UiMFTemplate.Infrastructure;
	using UiMFTemplate.Infrastructure.Security;

	[EntityRepository(EntityType = typeof(Magic))]
	public class GrantRepository : IEntityRepository
	{
		private readonly CoreDbContext context;

		public GrantRepository(CoreDbContext context)
		{
			this.context = context;
		}

		public object Find(int entityId)
		{
			return this.context.Magics
				.SingleOrException(t => t.Id == entityId);
		}
	}
}
