namespace UiMFTemplate.Conversations.Domain
{
    using System.Reflection;
    using UiMFTemplate.Infrastructure;

    public class ConversationKey
    {
        public int EntityId { get; set; }
        public string EntityType { get; set; }

        public static string Get<T>(object id)
            where T : IConversationManager
        {
            return typeof(T).GetCustomAttribute<RegisterEntryAttribute>().Key + ":" + id;
        }

        public static ConversationKey Parse(string key)
        {
            var result = key?.Split(':');

            if (result != null)
            {
                return new ConversationKey
                {
                    EntityType = result[0],
                    EntityId = int.Parse(result[1])
                };
            }

            return new ConversationKey();
        }
    }
}
