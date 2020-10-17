CREATE TABLE [cnv].[ConversationDocument]
(
	[Id] INT NOT NULL PRIMARY KEY IDENTITY, 
    [CommentDataId] INT NULL, 
    [DocumentId] INT NOT NULL, 
    CONSTRAINT [FK_ConversationDocument_Comment] FOREIGN KEY (CommentDataId) REFERENCES [cnv].[Comment]([Id])
)

GO

CREATE INDEX [IX_ConversationDocument_CommentDataId] ON [cnv].[ConversationDocument] (CommentDataId)
