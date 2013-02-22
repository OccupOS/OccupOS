CREATE TABLE [dbo].[ControllerMetadata] (
    [Id]             INT            IDENTITY (1, 1) NOT NULL,
    [ExternalId]     NVARCHAR (450) NOT NULL,
    [DepartmentName] NVARCHAR (50)  NULL,
    [BuildingName]   NVARCHAR (50)  NULL,
    [UpdatedAt]      DATETIME2      DEFAULT (getdate()) NOT NULL,
    [CreatedAt]      DATETIME2      DEFAULT (getdate()) NOT NULL,
    [UpdaterId]      INT            NULL,
    [CreatorId]      INT            NULL,
    [FloorNr]        INT            NULL,
    [RoomId]         NVARCHAR (50) NOT NULL,
    CONSTRAINT [PK_IntermediateHwMedadatas] PRIMARY KEY CLUSTERED ([Id] ASC),
    UNIQUE NONCLUSTERED ([ExternalId] ASC)
);

