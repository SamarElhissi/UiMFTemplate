CREATE TABLE [dbo].[Article]
(
	[Id] INT IDENTITY(1,1) NOT NULL,
	[CreatedOn] DATETIME NOT NULL, 
	[CreatedByUserId] INT NOT NULL,
	[UpdatedOn] DATETIME NULL, 
	[UpdatedByUserId] INT NULL,

	[Title] NVARCHAR(500) NOT NULL,
	[Details] ntext NOT NULL,
	[Phase] INT NOT NULL,
	[Status] INT NOT NULL,
	[IsDeleted] BIT NOT NULL DEFAULT 0,
	[PublishedOn] DATETIME NULL, 
	[Type] INT NOT NULL,

	CONSTRAINT [FK_Article_CreatedByUserId] FOREIGN KEY (CreatedByUserId) REFERENCES [AspNetUsers]([Id]),
	CONSTRAINT [FK_Article_UpdatedByUserId] FOREIGN KEY (UpdatedByUserId) REFERENCES [AspNetUsers]([Id]),
    CONSTRAINT [PK_Article] PRIMARY KEY CLUSTERED ([Id])
)
