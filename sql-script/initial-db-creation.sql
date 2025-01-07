-- first create db then comment it again
-- CREATE DATABASE [EMSDb];


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
CREATE TABLE [Departments] (
    [Id] int NOT NULL IDENTITY,
    [Name] nvarchar(max) NOT NULL,
    [ManagerId] int NULL,
    [Budget] decimal(18,2) NOT NULL,
    CONSTRAINT [PK_Departments] PRIMARY KEY ([Id])
);

CREATE TABLE [Employees] (
    [Id] int NOT NULL IDENTITY,
    [Name] nvarchar(max) NOT NULL,
    [Email] nvarchar(max) NOT NULL,
    [Phone] nvarchar(max) NULL,
    [Position] nvarchar(max) NOT NULL,
    [JoiningDate] datetime2 NOT NULL,
    [DepartmentId] int NOT NULL,
    [IsActive] bit NOT NULL,
    [CreatedAt] datetime2 NOT NULL,
    [UpdatedAt] datetime2 NULL,
    [IsDeleted] bit NOT NULL,
    [DeletedAt] datetime2 NULL,
    CONSTRAINT [PK_Employees] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_Employees_Departments_DepartmentId] FOREIGN KEY ([DepartmentId]) REFERENCES [Departments] ([Id])
);

CREATE TABLE [PerformanceReviews] (
    [Id] int NOT NULL IDENTITY,
    [EmployeeId] int NOT NULL,
    [ReviewDate] datetime2 NOT NULL,
    [ReviewScore] int NOT NULL,
    [ReviewNotes] nvarchar(max) NOT NULL,
    [CreatedAt] datetime2 NOT NULL,
    [UpdatedAt] datetime2 NULL,
    [IsDeleted] bit NOT NULL,
    [DeletedAt] datetime2 NULL,
    CONSTRAINT [PK_PerformanceReviews] PRIMARY KEY ([Id]),
    CONSTRAINT [CK_ReviewScore] CHECK ([ReviewScore] >= 1 AND [ReviewScore] <= 10),
    CONSTRAINT [FK_PerformanceReviews_Employees_EmployeeId] FOREIGN KEY ([EmployeeId]) REFERENCES [Employees] ([Id]) ON DELETE CASCADE
);

CREATE UNIQUE INDEX [IX_Departments_ManagerId] ON [Departments] ([ManagerId]) WHERE [ManagerId] IS NOT NULL;

CREATE INDEX [IX_Employees_DepartmentId] ON [Employees] ([DepartmentId]);

CREATE INDEX [IX_PerformanceReviews_EmployeeId] ON [PerformanceReviews] ([EmployeeId]);

ALTER TABLE [Departments] ADD CONSTRAINT [FK_Departments_Employees_ManagerId] FOREIGN KEY ([ManagerId]) REFERENCES [Employees] ([Id]) ON DELETE SET NULL;

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20250107084614_AddedModelsConstrataintsRelations', N'9.0.0');

COMMIT;
GO
