CREATE TABLE [dbo].[SensorData] (
    [Id]                       INT            IDENTITY (1, 1) NOT NULL,
    [SensorMetadataId]         INT            NOT NULL,
    [IntermediateHwMedadataId] INT            NOT NULL,
    [MeasuredData]             NVARCHAR (MAX) NOT NULL,
    [MeasuredAt]               DATETIME2       NOT NULL,
    [UpdatedAt]                DATETIME2       DEFAULT (getdate()) NOT NULL,
    [CreatedAt]                DATETIME2       DEFAULT (getdate()) NOT NULL,
    CONSTRAINT [PK_SensorDatas] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_SensorDataIntermediateHwMedadata] FOREIGN KEY ([IntermediateHwMedadataId]) REFERENCES [dbo].[IntermediateHwMedadatas] ([Id]),
    CONSTRAINT [FK_SensorDataSensorMetadata] FOREIGN KEY ([SensorMetadataId]) REFERENCES [dbo].[SensorMetadatas1] ([Id])
);


GO
CREATE NONCLUSTERED INDEX [IX_FK_SensorDataSensorMetadata]
    ON [dbo].[SensorDatas]([SensorMetadataId] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_FK_SensorDataIntermediateHwMedadata]
    ON [dbo].[SensorDatas]([IntermediateHwMedadataId] ASC);

