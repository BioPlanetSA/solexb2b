
--USUWANIE ZBEDNEGO INDEKSU
IF EXISTS(SELECT * FROM sys.indexes WHERE name = 'cechy_atrybut_id' AND object_id = OBJECT_ID('Cecha'))
BEGIN
	DROP INDEX [cechy_atrybut_id] ON [dbo].[Cecha]
 END


 IF EXISTS(SELECT * FROM sys.indexes WHERE name = 'Cecha_Symbol' AND object_id = OBJECT_ID('Cecha'))
BEGIN
	ALTER TABLE [dbo].[Cecha] DROP CONSTRAINT [Cecha_Symbol]
END
 
IF EXISTS(SELECT * FROM sys.indexes WHERE name = 'IX_cechy' AND object_id = OBJECT_ID('Cecha'))
BEGIN
	ALTER TABLE [dbo].[Cecha] DROP CONSTRAINT [IX_cechy];
END



ALTER TABLE dbo.Cecha ALTER COLUMN Nazwa varchar(max);
ALTER TABLE dbo.Cecha ALTER COLUMN Symbol varchar(300);

