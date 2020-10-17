CREATE TABLE [dbo].[Magic]
(
	[Id]		INT IDENTITY(1,1) NOT NULL,
	[CreatedOn] DATETIME NOT NULL, 
	[CreatedByUserId] INT NOT NULL,
	[Title]			  NVARCHAR(500) NOT NULL,
	[Details]		  ntext NOT NULL,
	[Status]		  INT NOT NULL,
	[IsDeleted]		  BIT NOT NULL DEFAULT 0,
	[SubmittedOn]	  DATETIME NULL, 
	[ClosedOn]		  DATETIME NULL, 
	CONSTRAINT [FK_Magic_CreatedByUserId] FOREIGN KEY (CreatedByUserId) REFERENCES [AspNetUsers]([Id]),
    CONSTRAINT [PK_Magic] PRIMARY KEY CLUSTERED ([Id])
)
