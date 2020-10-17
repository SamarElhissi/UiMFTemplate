CREATE TABLE [cnv].[Conversation] (
    [Id]         INT          IDENTITY (1, 1) NOT NULL,
    [Key]        VARCHAR (80) NOT NULL,
    [CreatedOn]  DATETIME     NOT NULL,
    CONSTRAINT [PK_cnv.Conversation] PRIMARY KEY CLUSTERED ([Id] ASC)
);


GO
CREATE UNIQUE NONCLUSTERED INDEX [IX_Conversation_ApplicationId_Key]
    ON [cnv].[Conversation]([Key] ASC);

