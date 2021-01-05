USE master
GO

CREATE DATABASE Something
GO

SELECT * 
FROM master.dbo.sysdatabases;
GO

USE Something
GO

CREATE TABLE Wicked (
Id INT NOT NULL
);

BACKUP DATABASE Something
TO DISK = 'c:\Something.bak';
GO

USE master
GO

DROP DATABASE Something
GO

RESTORE DATABASE Something
FROM DISK = 'c:\Something.bak';
GO