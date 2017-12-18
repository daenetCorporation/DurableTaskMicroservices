CREATE TABLE [dbo].[StateTable]
(
	[Id] INT NOT NULL PRIMARY KEY IDENTITY,
	[InstanceId] NVARCHAR(50) NOT NULL,
	[ExecutionId] NVARCHAR(50) NOT NULL,
	[SequenceNumber] BIGINT NOT NULL,
	[JumpStartTime] DATETIME NOT NULL,
	[CompletedTime] DATETIME NOT NULL,
	[CompressedSize] BIGINT NOT NULL, 
	[CreatedTime] DATETIME NOT NULL, 
	[Input] NVARCHAR(MAX) NOT NULL, 
	[LastUpdatedTime] DATETIME NOT NULL, 
	[Name] NVARCHAR(100) NOT NULL, 
	[OrchestrationInstance] NVARCHAR(MAX) NOT NULL, 
	[OrchestrationStatus] NVARCHAR(MAX) NOT NULL, 
	[Output] NVARCHAR(MAX) NULL, 
	[ParentInstance] NVARCHAR(MAX) NOT NULL, 
	[Size] BIGINT NOT NULL, 
	[Status] NVARCHAR(50) NOT NULL, 
	[Tags] NVARCHAR(MAX) NULL, 
	[Version] NVARCHAR(50) NOT NULL
)
