CREATE TABLE [cnv].[Comment] (
    [Id]             INT              IDENTITY (1, 1) NOT NULL,
    [AuthorId]       INT              NOT NULL,
    [ConversationId] INT              NOT NULL,
    [ParentId]       INT              NULL,
    [PostedOn]       DATETIME         NOT NULL,
    [Text]           NVARCHAR (MAX)   NOT NULL,
    CONSTRAINT [PK_cnv.Comment] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_cnv.Comment_cnv.Comment_ParentId] FOREIGN KEY ([ParentId]) REFERENCES [cnv].[Comment] ([Id]),
    CONSTRAINT [FK_cnv.Comment_cnv.Conversation_ConversationId] FOREIGN KEY ([ConversationId]) REFERENCES [cnv].[Conversation] ([Id]) ON DELETE CASCADE
);


GO
CREATE NONCLUSTERED INDEX [IX_Comment_ConversationId]
    ON [cnv].[Comment]([ConversationId] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_Comment_ParentId]
    ON [cnv].[Comment]([ParentId] ASC);

