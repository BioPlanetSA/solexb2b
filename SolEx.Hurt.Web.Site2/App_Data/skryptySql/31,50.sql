
if exists ( SELECT * FROM sys.default_constraints WHERE object_id = OBJECT_ID('DF_zamowienia_IdDokumentu') AND parent_object_id = OBJECT_ID('Zamowienie')) ALTER TABLE Zamowienie DROP CONSTRAINT DF_zamowienia_IdDokumentu;

DECLARE @tblName VARCHAR(100)  --zmienna do nazwy tabeli
DECLARE @colname VARCHAR(100)  --zmienna do nazwy kolumny 
DECLARE @dbType VARCHAR(100)	-- znienna do typu kolumny 
DECLARE @cdcInst VARCHAR(100)

SET @tblName = 'Zamowienie'
SET @colname = 'NumerTymczasowyZamowienia'
SET @dbType = 'varchar(200) NULL' --jesli ma byc wartość domyslna do dopisujemy:  DEFAULT ((  tu wartosc domyslna  ))  np. bit NOT NULL DEFAULT (1)

if NOT exists (select column_name from INFORMATION_SCHEMA.columns where table_name = @tblName and column_name = @colname) 
begin
	SET @cdcInst = 'dbo_' + @tblName

	---------wyłaczenia monitirowania tabeli
	--exec sys.sp_cdc_disable_table @source_schema='dbo', @source_name= @tblName, @capture_instance= @cdcInst

	------------------------------dodawanie kolumny do tabeli
	EXEC('ALTER TABLE dbo.'+ @tblName +' ADD '+ @colname + ' '+ @dbType  )  
      
	-----------włączenie monitorowania tabeli
	--EXEC sys.sp_cdc_enable_table @source_schema = 'dbo',@source_name = @tblName,@role_name = NULL, @supports_net_changes = 1

end

SET @tblName = 'Zamowienie'
SET @colname = 'IdOddzialu'
SET @dbType = 'int NULL' --jesli ma byc wartość domyslna do dopisujemy:  DEFAULT ((  tu wartosc domyslna  ))  np. bit NOT NULL DEFAULT (1)

if NOT exists (select column_name from INFORMATION_SCHEMA.columns where table_name = @tblName and column_name = @colname) 
begin
	SET @cdcInst = 'dbo_' + @tblName

	---------wyłaczenia monitirowania tabeli
	--exec sys.sp_cdc_disable_table @source_schema='dbo', @source_name= @tblName, @capture_instance= @cdcInst

	------------------------------dodawanie kolumny do tabeli
	EXEC('ALTER TABLE dbo.'+ @tblName +' ADD '+ @colname + ' '+ @dbType  )  
      
	-----------włączenie monitorowania tabeli
	--EXEC sys.sp_cdc_enable_table @source_schema = 'dbo',@source_name = @tblName,@role_name = NULL, @supports_net_changes = 1

end

IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='ZamowienieDokumenty' AND xtype='U')
begin
	CREATE TABLE [dbo].ZamowienieDokumenty(
		[Id] [bigint] NOT NULL,
		[IdZamowienia] [bigint] NOT NULL,
		[IdDokumentu] [bigint] NOT NULL,
		
	 CONSTRAINT [PK_NazwaTabeli] PRIMARY KEY CLUSTERED 
	(
		[Id] ASC
	)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
	) ON [PRIMARY]

	-- to dodajemy jak mają być klucze obce
	ALTER TABLE [dbo].[ZamowienieDokumenty]  WITH CHECK ADD  CONSTRAINT [FK_ZamowieniaDokumenty-HistoriaDokumentu] FOREIGN KEY(IdDokumentu) REFERENCES [dbo].[HistoriaDokumentu] ([Id]) ON DELETE CASCADE
	
	-- to dajemy jesli ma być zapisywana historia zmian 
	--EXEC [sys].[sp_cdc_enable_table] N'dbo','ZamowienieDokumenty', @role_name = NULL, @supports_net_changes = 1
end

SET @tblName = 'Zamowienie'
SET @colname = 'DokumentyId'

if exists (select column_name from INFORMATION_SCHEMA.columns where table_name = @tblName and column_name = @colname) 
begin

	SET @cdcInst = 'dbo_' + @tblName

	---------wyłaczenia monitirowania tabeli ------------ 
	--exec sys.sp_cdc_disable_table @source_schema='dbo', @source_name= @tblName, @capture_instance= @cdcInst

	------------------------------Usuniecie kolumny z tabeli
	EXEC('ALTER TABLE dbo.'+ @tblName +' DROP COLUMN '+ @colname  )  
	      
	-----------włączenie monitorowania tabeli
	--EXEC sys.sp_cdc_enable_table @source_schema = 'dbo',@source_name = @tblName,@role_name = NULL, @supports_net_changes = 1

end