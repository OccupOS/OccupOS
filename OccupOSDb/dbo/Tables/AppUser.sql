CREATE TABLE [dbo].[AppUser] (
    [Id]        INT            IDENTITY (1, 1) NOT NULL,
    [Username]  NVARCHAR (50)  NOT NULL,
    [Email]     NVARCHAR (100) NOT NULL,
    [Password]  NVARCHAR (100) NOT NULL,
    [createdAt] DATETIME2       DEFAULT (getdate()) NOT NULL,
    [updatedAt] DATETIME2       DEFAULT (getdate()) NOT NULL,
    [creatorId] INT            NULL,
    [updaterId] INT            NULL,
    [FirstName] NVARCHAR (100) NULL,
    [LastName]  NVARCHAR (100) NULL,
    CONSTRAINT [PK_Users] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [userUnique] UNIQUE NONCLUSTERED ([Email] ASC, [Username] ASC)
);

