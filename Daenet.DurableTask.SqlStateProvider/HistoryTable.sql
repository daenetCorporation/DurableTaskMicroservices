CREATE TABLE [dbo].[HistoryTable]
(
	[Id] INT NOT NULL PRIMARY KEY IDENTITY, 
    [InstanceId] NVARCHAR(50) NOT NULL, 
    [ExeutionId] NVARCHAR(50) NOT NULL, 
    [SequenceNumber] INT NOT NULL, 
    [HistoryEvent] NVARCHAR(MAX) NOT NULL, 
    [EventTimestamp] DATETIME NOT NULL,
	[TimeStamp] DATETIMEOFFSET NOT NULL
)
