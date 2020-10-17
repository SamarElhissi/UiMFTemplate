namespace UiMFTemplate.Conversations.Forms.Outputs
{
    using System;
    using System.Collections.Generic;
    using UiMetadataFramework.Basic.Output;
    using UiMetadataFramework.Core.Binding;
    using UiMFTemplate.Conversations.Domain;

    [OutputFieldType("conversation")]
    public class Conversation
    {
        public Conversation(string key)
        {
            this.Key = key;
        }

        public Conversation()
        {
        }

        public string ArchivedByUser { get; set; }
        public bool CanAddComments { get; set; }
        public bool CanDelete { get; set; }
        public IEnumerable<Comment> Comments { get; set; }
        public DateTime CreatedOn { get; set; }
        public int Id { get; set; }
        public string Key { get; set; }
        public List<Participant> Participants { get; set; }
        public string UserName { get; set; }

        public static InlineForm Create<T>(object id) where T : IConversationManager
        {
            var key = ConversationKey.Get<T>(id);

            return new InlineForm
            {
                Form = typeof(Commands.GetConversation).GetFormId(),
                InputFieldValues = new Dictionary<string, object>
                {
                    { nameof(Commands.GetConversation.Request.Key), key }
                }
            };
        }
    }

    public class Comment
    {
        public string Author { get; set; }
        public bool CanDelete { get; set; }
        public ICollection<Comment> Children { get; set; }
        public int ContextId { get; set; }
        public string ContextType { get; set; }
        public int Id { get; set; }
        public int? ParentId { get; set; }
        public DateTime PostedOn { get; set; }
        public string Text { get; set; }
    }
}
