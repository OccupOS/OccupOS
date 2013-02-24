CREATE TABLE [dbo].[SensorMetadata] (
    [Id]                       INT            IDENTITY (1, 1) NOT NULL,
    [ExternalId]               NVARCHAR (450) NOT NULL,
    [SensorName]               NVARCHAR (50)  NULL,
    [RoomId]                   NVARCHAR (50)  NOT NULL,
    [FloorNr]                  INT            NULL,
    [GeoLongitude]             DECIMAL (9, 6) NULL,
    [GeoLatidude]              DECIMAL (9, 6) NULL,
    [UpdatedAt]                DATETIME2      DEFAULT (getdate()) NOT NULL,
    [CreatedAt]                DATETIME2      DEFAULT (getdate()) NOT NULL,
    [UpdaterId]                INT            NULL,
    [CreatorId]                INT            NULL,
    [HwControllerMetadataId]   INT            NOT NULL,
    CONSTRAINT [PK_SensorMetadata] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_HwControllerMedadataSensorMetadata] FOREIGN KEY ([HwControllerMetadataId]) REFERENCES [dbo].[HwControllerMetadata] ([Id]),
    UNIQUE NONCLUSTERED ([ExternalId] ASC)
);


GO
CREATE NONCLUSTERED INDEX [IX_FK_IntermediateHwMedadataSensorMetadata]
    ON [dbo].[SensorMetadata]([HwControllerMetadataId] ASC);

