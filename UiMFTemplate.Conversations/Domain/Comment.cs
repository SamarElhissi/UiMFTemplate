// ReSharper disable AutoPropertyCanBeMadeGetOnly.Local
// ReSharper disable UnusedAutoPropertyAccessor.Local

namespace UiMFTemplate.Conversations.Domain
{
    using System;
    using System.Collections.Generic;

    public class Comment<TAuthorKey>
    {
        internal const string ChildrenFieldName = nameof(children);
        private readonly List<Comment<TAuthorKey>> children = new List<Comment<TAuthorKey>>();

        public Comment()
        {
            // This constructor is used by EF only.
        }

        protected internal Comment(int conversationId, TAuthorKey authorId, string text)
        {
            this.Initialize(conversationId, authorId, text);
        }

        public TAuthorKey AuthorId { get; private set; }
        public IEnumerable<Comment<TAuthorKey>> Children => this.children.AsReadOnly();
        public int ConversationId { get; private set; }
        public int Id { get; private set; }
        public int? ParentId { get; private set; }

        public DateTime PostedOn { get; private set; }
        public string Text { get; private set; }

        public Comment<TAuthorKey> Reply(TAuthorKey authorId, string text)
        {
            var reply = new Comment<TAuthorKey>
            {
                ConversationId = this.ConversationId,
                ParentId = this.Id,
                AuthorId = authorId,
                PostedOn = DateTime.UtcNow,
                Text = text
            };

            this.children.Add(reply);

            return reply;
        }

        internal void Initialize(
            int conversationId,
            TAuthorKey authorId,
            string text)
        {
            this.ConversationId = conversationId;
            this.ParentId = null;
            this.AuthorId = authorId;
            this.PostedOn = DateTime.UtcNow;
            this.Text = text;
        }
    }
}
