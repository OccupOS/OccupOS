CREATE TABLE [dbo].[SensorMetadata] (
    [Id]                       INT            IDENTITY (1, 1) NOT NULL,
    [ExternalId]               NVARCHAR (450) NOT NULL,
    [SensorName]               NVARCHAR (50) NULL,
    [RoomId]                   NVARCHAR (50) NOT NULL,
    [FloorNr]                  INT            NULL,
    [GeoLongitude]             DECIMAL (9, 6) NULL,
    [GeoLatidude]              DECIMAL (9, 6) NULL,
    [UpdatedAt]                DATETIME2       DEFAULT (getdate()) NOT NULL,
    [CreatedAt]                DATETIME2       DEFAULT (getdate()) NOT NULL,
    [UpdaterId]                INT            NULL,
    [CreatorId]                INT            NULL,
    [ControllerMedadataId] INT            NOT NULL,
    CONSTRAINT [PK_SensorMetadatas1] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_ControllerMedadataSensorMetadata] FOREIGN KEY ([ControllerMedadataId]) REFERENCES [dbo].[ControllerMetadata] ([Id]),
    UNIQUE NONCLUSTERED ([ExternalId] ASC)
);


GO
CREATE NONCLUSTERED INDEX [IX_FK_IntermediateHwMedadataSensorMetadata]
    ON [dbo].[SensorMetadatas1]([IntermediateHwMedadataId] ASC);

