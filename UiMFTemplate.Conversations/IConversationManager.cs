namespace UiMFTemplate.Conversations
{
	using System.Threading.Tasks;
	using UiMetadataFramework.Basic.Output;
	using UiMFTemplate.Conversations.Domain;

	public interface IConversationManager
	{
		bool CanAddNewComments(object entityId);
		bool CanDeleteConversation(object entityId);
		bool CanViewConversation(object entityId);
		Task<ConversationParticipants> GetParticipants(object entityId);

		/// <summary>
		/// Gets link to the page where conversation can be seen.
		/// </summary>
		/// <returns><see cref="FormLink"/> instance or null.</returns>
		FormLink Link(object entityId);

		void PostAddComment(object entityId);
		void PostDeleteComment(object entityId);
	}
}