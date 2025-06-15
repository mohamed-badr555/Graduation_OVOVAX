IF OBJECT_ID(N'[__EFMigrationsHistory]') IS NULL
BEGIN
    CREATE TABLE [__EFMigrationsHistory] (
        [MigrationId] nvarchar(150) NOT NULL,
        [ProductVersion] nvarchar(32) NOT NULL,
        CONSTRAINT [PK___EFMigrationsHistory] PRIMARY KEY ([MigrationId])
    );
END;
GO

BEGIN TRANSACTION;
CREATE TABLE [InjectionSessions] (
    [ID] int NOT NULL IDENTITY,
    [StartTime] datetime2 NOT NULL,
    [EndTime] datetime2 NULL,
    [RangeOfInfrared] float(18) NOT NULL,
    [StepOfInjection] float(18) NOT NULL,
    [VolumeOfLiquid] float(18) NOT NULL,
    [NumberOfElements] int NOT NULL,
    [Status] int NOT NULL,
    [CreatedAt] datetime2 NOT NULL,
    [UpdatedAt] datetime2 NULL,
    CONSTRAINT [PK_InjectionSessions] PRIMARY KEY ([ID])
);

CREATE TABLE [MovementCommands] (
    [ID] int NOT NULL IDENTITY,
    [Timestamp] datetime2 NOT NULL,
    [Action] int NOT NULL,
    [Axis] int NOT NULL,
    [Direction] int NOT NULL,
    [Step] float(18) NOT NULL,
    [Speed] int NOT NULL,
    [Status] int NOT NULL,
    [ErrorMessage] nvarchar(max) NULL,
    [CreatedAt] datetime2 NOT NULL,
    [UpdatedAt] datetime2 NULL,
    CONSTRAINT [PK_MovementCommands] PRIMARY KEY ([ID])
);

CREATE TABLE [ScanResults] (
    [ID] int NOT NULL IDENTITY,
    [ScanTime] datetime2 NOT NULL,
    [DepthMeasurement] float(18) NOT NULL,
    [Status] int NOT NULL,
    [ErrorMessage] nvarchar(max) NULL,
    [CreatedAt] datetime2 NOT NULL,
    [UpdatedAt] datetime2 NULL,
    CONSTRAINT [PK_ScanResults] PRIMARY KEY ([ID])
);

CREATE TABLE [InjectionRecords] (
    [ID] int NOT NULL IDENTITY,
    [InjectionSessionId] int NOT NULL,
    [InjectionTime] datetime2 NOT NULL,
    [EggNumber] int NOT NULL,
    [VolumeInjected] float(18) NOT NULL,
    [Status] int NOT NULL,
    [CreatedAt] datetime2 NOT NULL,
    [UpdatedAt] datetime2 NULL,
    CONSTRAINT [PK_InjectionRecords] PRIMARY KEY ([ID]),
    CONSTRAINT [FK_InjectionRecords_InjectionSessions_InjectionSessionId] FOREIGN KEY ([InjectionSessionId]) REFERENCES [InjectionSessions] ([ID]) ON DELETE CASCADE
);

CREATE INDEX [IX_InjectionRecords_InjectionSessionId] ON [InjectionRecords] ([InjectionSessionId]);

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20250604005219_InitialCreate', N'9.0.5');

EXEC sp_rename N'[InjectionSessions].[RangeOfInfrared]', N'RangeOfInfraredTo', 'COLUMN';

ALTER TABLE [InjectionSessions] ADD [RangeOfInfraredFrom] float(18) NOT NULL DEFAULT 0.0E0;

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20250609013836_AddRangeTo', N'9.0.5');

COMMIT;
GO

