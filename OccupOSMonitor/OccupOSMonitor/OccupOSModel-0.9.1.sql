
-- --------------------------------------------------
-- Entity Designer DDL Script for SQL Server 2005, 2008, and Azure
-- --------------------------------------------------
-- Date Created: 02/13/2013 20:41:23
-- Generated from EDMX file: C:\Users\Markus\Documents\Visual Studio 2012\Projects\C#\UCL Year 2\OccupOS\OccupOSMonitor\OccupOSMonitor\OccupOSModel.edmx
-- --------------------------------------------------

SET QUOTED_IDENTIFIER OFF;
GO
USE [OccupOS];
GO
IF SCHEMA_ID(N'dbo') IS NULL EXECUTE(N'CREATE SCHEMA [dbo]');
GO

-- --------------------------------------------------
-- Dropping existing FOREIGN KEY constraints
-- --------------------------------------------------

IF OBJECT_ID(N'[dbo].[FK_SensorDataSensorMetadata]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[SensorDatas] DROP CONSTRAINT [FK_SensorDataSensorMetadata];
GO
IF OBJECT_ID(N'[dbo].[FK_SensorDataIntermediateHwMedadata]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[SensorDatas] DROP CONSTRAINT [FK_SensorDataIntermediateHwMedadata];
GO
IF OBJECT_ID(N'[dbo].[FK_IntermediateHwMedadataSensorMetadata]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[SensorMetadatas1] DROP CONSTRAINT [FK_IntermediateHwMedadataSensorMetadata];
GO

-- --------------------------------------------------
-- Dropping existing tables
-- --------------------------------------------------

IF OBJECT_ID(N'[dbo].[SensorDatas]', 'U') IS NOT NULL
    DROP TABLE [dbo].[SensorDatas];
GO
IF OBJECT_ID(N'[dbo].[SensorMetadatas1]', 'U') IS NOT NULL
    DROP TABLE [dbo].[SensorMetadatas1];
GO
IF OBJECT_ID(N'[dbo].[IntermediateHwMedadatas]', 'U') IS NOT NULL
    DROP TABLE [dbo].[IntermediateHwMedadatas];
GO
IF OBJECT_ID(N'[dbo].[Users]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Users];
GO

-- --------------------------------------------------
-- Creating all tables
-- --------------------------------------------------

-- Creating table 'SensorDatas'
CREATE TABLE [dbo].[SensorDatas] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [SensorMetadataId] int  NOT NULL,
    [IntermediateHwMedadataId] int  NOT NULL,
    [MeasuredData] nvarchar(max)  NOT NULL,
    [UpdatedAt] datetime  NOT NULL DEFAULT getDate(),
    [CreatedAt] datetime  NOT NULL DEFAULT getDate()
);
GO

-- Creating table 'SensorMetadatas1'
CREATE TABLE [dbo].[SensorMetadatas1] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [ExternalId] nvarchar(450)  NOT NULL,
    [SensorName] nvarchar(max)  NULL,
    [RoomId] nvarchar(max)  NOT NULL,
    [FloorNr] int  NULL,
    [GeoLongitude] decimal(9,6)  NULL,
    [GeoLatidude] decimal(9,6)  NULL,
    [UpdatedAt] datetime  NOT NULL DEFAULT getDate(),
    [CreatedAt] datetime  NOT NULL DEFAULT getDate(),
    [UpdaterId] int  NULL,
    [CreatorId] int  NULL,
    [IntermediateHwMedadataId] int  NOT NULL,
	UNIQUE ([ExternalId])
);
GO

-- Creating table 'IntermediateHwMedadatas'
CREATE TABLE [dbo].[IntermediateHwMedadatas] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [ExternalId] nvarchar(450)  NOT NULL,
    [DepartmentName] nvarchar(max)  NULL,
    [BuildingName] nvarchar(max)  NULL,
    [UpdatedAt] datetime  NOT NULL DEFAULT getDate(),
    [CreatedAt] datetime  NOT NULL DEFAULT getDate(),
    [UpdaterId] int  NULL,
    [CreatorId] int  NULL,
    [FloorNr] int  NULL,
    [RoomId] nvarchar(max)  NOT NULL,
	UNIQUE ([ExternalId])
);
GO

-- Creating table 'Users'
CREATE TABLE [dbo].[Users] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [Username] nvarchar(50)  NOT NULL,
    [Email] nvarchar(100)  NOT NULL,
    [Password] nvarchar(max)  NOT NULL,
    [createdAt] datetime  NOT NULL DEFAULT getDate(),
    [updatedAt] datetime  NOT NULL DEFAULT getDate(),
    [creatorId] int  NULL,
    [updaterId] int  NULL,
    [FirstName] nvarchar(max)  NULL,
    [LastName] nvarchar(max)   NULL,
	CONSTRAINT userUnique UNIQUE ([Email],[Username])
);
GO

-- --------------------------------------------------
-- Creating all PRIMARY KEY constraints
-- --------------------------------------------------

-- Creating primary key on [Id] in table 'SensorDatas'
ALTER TABLE [dbo].[SensorDatas]
ADD CONSTRAINT [PK_SensorDatas]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'SensorMetadatas1'
ALTER TABLE [dbo].[SensorMetadatas1]
ADD CONSTRAINT [PK_SensorMetadatas1]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'IntermediateHwMedadatas'
ALTER TABLE [dbo].[IntermediateHwMedadatas]
ADD CONSTRAINT [PK_IntermediateHwMedadatas]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'Users'
ALTER TABLE [dbo].[Users]
ADD CONSTRAINT [PK_Users]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- --------------------------------------------------
-- Creating all FOREIGN KEY constraints
-- --------------------------------------------------

-- Creating foreign key on [SensorMetadataId] in table 'SensorDatas'
ALTER TABLE [dbo].[SensorDatas]
ADD CONSTRAINT [FK_SensorDataSensorMetadata]
    FOREIGN KEY ([SensorMetadataId])
    REFERENCES [dbo].[SensorMetadatas1]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_SensorDataSensorMetadata'
CREATE INDEX [IX_FK_SensorDataSensorMetadata]
ON [dbo].[SensorDatas]
    ([SensorMetadataId]);
GO

-- Creating foreign key on [IntermediateHwMedadataId] in table 'SensorDatas'
ALTER TABLE [dbo].[SensorDatas]
ADD CONSTRAINT [FK_SensorDataIntermediateHwMedadata]
    FOREIGN KEY ([IntermediateHwMedadataId])
    REFERENCES [dbo].[IntermediateHwMedadatas]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_SensorDataIntermediateHwMedadata'
CREATE INDEX [IX_FK_SensorDataIntermediateHwMedadata]
ON [dbo].[SensorDatas]
    ([IntermediateHwMedadataId]);
GO

-- Creating foreign key on [IntermediateHwMedadataId] in table 'SensorMetadatas1'
ALTER TABLE [dbo].[SensorMetadatas1]
ADD CONSTRAINT [FK_IntermediateHwMedadataSensorMetadata]
    FOREIGN KEY ([IntermediateHwMedadataId])
    REFERENCES [dbo].[IntermediateHwMedadatas]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_IntermediateHwMedadataSensorMetadata'
CREATE INDEX [IX_FK_IntermediateHwMedadataSensorMetadata]
ON [dbo].[SensorMetadatas1]
    ([IntermediateHwMedadataId]);
GO

-- --------------------------------------------------
-- Script has ended
-- --------------------------------------------------