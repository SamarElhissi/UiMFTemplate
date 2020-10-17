// ReSharper disable AutoPropertyCanBeMadeGetOnly.Local
// ReSharper disable UnusedAutoPropertyAccessor.Local

namespace UiMFTemplate.Conversations.Domain
{
    using System;
    using System.Collections.Generic;

    public class Conversation<TAuthorKey, TComment>
        where TComment : Comment<TAuthorKey>, new()
    {
        internal const string CommentsFieldName = nameof(comments);
        internal const int KeyMaxLength = 80;
        private readonly List<TComment> comments = new List<TComment>();

        public Conversation() : this(null)
        {
        }

        public Conversation(string key)
        {
            this.ChangeKey(key);
            this.CreatedOn = DateTime.UtcNow;
        }

        public IEnumerable<TComment> Comments => this.comments.AsReadOnly();
        public DateTime CreatedOn { get; private set; }
        public int Id { get; private set; }
        public string Key { get; private set; }

        public TComment AddComment(TAuthorKey authorId, string text)
        {
            var comment = new TComment();
            comment.Initialize(this.Id, authorId, text);

            this.comments.Add(comment);

            return comment;
        }

        public void ChangeKey(string key)
        {
            key.EnforceMaxLength(KeyMaxLength);
            this.Key = key;
        }
    }
}
