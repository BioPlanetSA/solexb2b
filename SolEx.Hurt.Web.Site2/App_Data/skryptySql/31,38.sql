--wersja Sylwestra po oczyszczeniu

DECLARE @tblName VARCHAR(100)  --zmienna do nazwy tabeli
DECLARE @colname VARCHAR(100)  --zmienna do nazwy kolumny 
DECLARE @dbType VARCHAR(100)	-- znienna do typu kolumny 
DECLARE @cdcInst VARCHAR(100)


SET @tblName = 'HistoriaDokumentuPlatnosciOnline'
SET @colname = 'IdDokumentu'
SET @dbType = 'bigint not null'

if exists (select column_name from INFORMATION_SCHEMA.columns where table_name = @tblName and column_name = @colname) 
begin

	SET @cdcInst = 'dbo_' + @tblName
	--------wyłaczenia monitirowania tabeli
	----exec sys.sp_cdc_disable_table @source_schema='dbo', @source_name= @tblName, @capture_instance= @cdcInst
	--------Zmiana kolumny w tabeli
	EXEC('ALTER TABLE dbo.'+ @tblName +'  ALTER COLUMN  '+ @colname + ' '+ @dbType  )  
	--------włączenie monitorowania tabeli
	----EXEC sys.sp_cdc_enable_table @source_schema = 'dbo',@source_name = @tblName,@role_name = NULL, @supports_net_changes = 1

end


IF EXISTS(SELECT * FROM sys.indexes WHERE name = 'dok_id_non_clast' AND object_id = OBJECT_ID('HistoriaDokumentuProdukt'))
    BEGIN
		DROP INDEX  dok_id_non_clast ON HistoriaDokumentuProdukt
    END


SET @tblName = 'HistoriaDokumentuProdukt'
SET @colname = 'DokumentId'
SET @dbType = 'bigint not null'

if exists (select column_name from INFORMATION_SCHEMA.columns where table_name = @tblName and column_name = @colname) 
begin

	SET @cdcInst = 'dbo_' + @tblName
	--------wyłaczenia monitirowania tabeli
	----exec sys.sp_cdc_disable_table @source_schema='dbo', @source_name= @tblName, @capture_instance= @cdcInst
	--------Zmiana kolumny w tabeli
	EXEC('ALTER TABLE dbo.'+ @tblName +'  ALTER COLUMN  '+ @colname + ' '+ @dbType  )  
	--------włączenie monitorowania tabeli
	----EXEC sys.sp_cdc_enable_table @source_schema = 'dbo',@source_name = @tblName,@role_name = NULL, @supports_net_changes = 1

end

DELETE FROM [HistoriaDokumentuProdukt]
WHERE NOT EXISTS (Select id from HistoriaDokumentu h WHERE [HistoriaDokumentuProdukt].DokumentId = h.id)



if not exists (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID('FK_HistoriaDokumentuProdukt_HistoriaDokumentu') AND parent_object_id = OBJECT_ID('HistoriaDokumentuProdukt') ) 
begin
	
	ALTER TABLE dbo.HistoriaDokumentuProdukt  ALTER COLUMN  DokumentId bigint not null
	
	ALTER TABLE [dbo].[HistoriaDokumentuProdukt]  WITH NOCHECK ADD  CONSTRAINT [FK_HistoriaDokumentuProdukt_HistoriaDokumentu] FOREIGN KEY([DokumentId])
	REFERENCES [dbo].[HistoriaDokumentu] ([Id])
	ON DELETE CASCADE
	ALTER TABLE [dbo].[HistoriaDokumentuProdukt] CHECK CONSTRAINT [FK_HistoriaDokumentuProdukt_HistoriaDokumentu]

end

	CREATE NONCLUSTERED INDEX [dok_id_non_clast] ON [dbo].[HistoriaDokumentuProdukt]
	(
		[DokumentId] ASC
	)
	INCLUDE ( 	[Id],
		[KodProduktu],
		[NazwaProduktu],
		[Ilosc],
		[JednostkaMiary],
		[CenaBrutto],
		[CenaNetto],
		[WartoscBrutto],
		[WartoscNetto],
		[Vat],
		[WartoscVat],
		[Parametry],
		[Rabat],
		[ProduktId],
		[Opis],
		[CenaNettoPoRabacie],
		[CenaBruttoPoRabacie],
		[Opis2]) WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
