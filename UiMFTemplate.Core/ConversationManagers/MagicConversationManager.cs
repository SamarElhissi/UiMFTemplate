namespace UiMFTemplate.Core.ConversationManagers
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Threading.Tasks;
	using Microsoft.AspNetCore.Identity;
	using Microsoft.EntityFrameworkCore;
	using UiMetadataFramework.Basic.Output;
	using UiMFTemplate.Conversations;
	using UiMFTemplate.Conversations.Domain;
	using UiMFTemplate.Core.Commands;
	using UiMFTemplate.Core.DataAccess;
	using UiMFTemplate.Core.Extensions;
	using UiMFTemplate.Core.Security;
	using UiMFTemplate.Core.Security.Magic;
	using UiMFTemplate.Infrastructure;
	using UiMFTemplate.Infrastructure.User;
	using UiMFTemplate.Users;

	[RegisterEntry("UiMFTemplate.Core.Domain.Magic")]
	public class MagicConversationManager : IConversationManager
	{
		private readonly CoreDbContext context;
		private readonly MagicPermissionManager permissionManager;
		private readonly UserContext userContext;
		private readonly UserManager<ApplicationUser> userManager;

		public MagicConversationManager(CoreDbContext context,
			UserContext userContext,
			UserManager<ApplicationUser> userManager,
			MagicPermissionManager permissionManager)
		{
			this.context = context;
			this.userContext = userContext;
			this.userManager = userManager;
			this.permissionManager = permissionManager;
		}

		public bool CanAddNewComments(object entityId)
		{
			var id = Convert.ToInt32(entityId);
			var Magic = this.context.Magics
				.SingleNotDeletedOrException(t => t.Id == id);

			return this.permissionManager.CanDo(MagicAction.AddComment, this.userContext, Magic);
		}

		public bool CanDeleteConversation(object entityId)
		{
			return false;
		}

		public bool CanViewConversation(object entityId)
		{
			return true;
		}

		public async Task<ConversationParticipants> GetParticipants(object entityId)
		{
			var Magic = this.context.Magics
				.Include(a => a.CreatedByUser)
				.SingleNotDeletedOrException(t => t.Id == (int)entityId);

			var managers = await this.userManager
				.GetUsersInRoleAsync(CoreRoles.Supervisor.Name);

			return new ConversationParticipants(new List<Participant>
			{
				new Participant("Complainer", new List<UserParticipant>
				{
					new UserParticipant(Magic.CreatedByUserId, Magic.CreatedByUser.Name, Magic.CreatedByUser.Email)
				}),
				new Participant("Managers", managers.Select(a => new UserParticipant(a.Id, a.UserName, a.Email)))
			});
		}

		public FormLink Link(object entityId)
		{
			var id = Convert.ToInt32(entityId.ToString());
			return MagicDetails.Button(id);
		}

		public void PostAddComment(object entityId)
		{
		}

		public void PostDeleteComment(object entityId)
		{
		}
	}
}