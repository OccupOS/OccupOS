CREATE TABLE [dbo].[SensorData] (
    [Id]                       INT            IDENTITY (1, 1) NOT NULL,
    [SensorMetadataId]         INT            NOT NULL,
    [IntermediateHwMedadataId] INT            NOT NULL,
    [MeasuredData]             NVARCHAR (MAX) NOT NULL,
    [MeasuredAt]               DATETIME2      NOT NULL,
    [SendAt]                   DATETIME2      NULL,
    [PolledAt]                 DATETIME2      NULL,
    [UpdatedAt]                DATETIME2      DEFAULT (getdate()) NOT NULL,
    [CreatedAt]                DATETIME2      DEFAULT (getdate()) NOT NULL,
    CONSTRAINT [PK_SensorDatas] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_SensorDataHwControllerMedadata] FOREIGN KEY ([IntermediateHwMedadataId]) REFERENCES [dbo].[HwControllerMetadata] ([Id]),
    CONSTRAINT [FK_SensorDataSensorMetadata] FOREIGN KEY ([SensorMetadataId]) REFERENCES [dbo].[SensorMetadata] ([Id])
);


GO
CREATE NONCLUSTERED INDEX [IX_FK_SensorDataSensorMetadata]
    ON [dbo].[SensorDatas]([SensorMetadataId] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_FK_SensorDataIntermediateHwMedadata]
    ON [dbo].[SensorDatas]([IntermediateHwMedadataId] ASC);

