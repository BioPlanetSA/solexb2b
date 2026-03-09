
DECLARE @tblName VARCHAR(100)  --zmienna do nazwy tabeli
DECLARE @colname VARCHAR(100)  --zmienna do nazwy kolumny 
DECLARE @dbType VARCHAR(100)	-- znienna do typu kolumny 
DECLARE @cdcInst VARCHAR(100)
DECLARE @tblName2 VARCHAR(100)  
DECLARE @colname2 VARCHAR(100)

set @colname2 = 'ProduktId'
set @tblName2 = 'ZamowienieProdukt'

SET @tblName = 'ZamowienieProdukt'
SET @colname = 'ProduktIdBazowy'
SET @dbType = 'bigint NOT NULL'


if exists (select @colname2 from INFORMATION_SCHEMA.columns where table_name = @tblName2 and column_name = @colname2) 
begin
		
	SET @cdcInst = 'dbo_' + @tblName

	---------wyłaczenia monitirowania tabeli
	--exec sys.sp_cdc_disable_table @source_schema='dbo', @source_name= @tblName, @capture_instance= @cdcInst

	------------------------------Zmiana kolumny w tabeli
	EXEC('ALTER TABLE dbo.'+ @tblName +'  ALTER COLUMN  '+ @colname + ' '+ @dbType  )  
	-----------włączenie monitorowania tabeli
	--EXEC sys.sp_cdc_enable_table @source_schema = 'dbo',@source_name = @tblName,@role_name = NULL, @supports_net_changes = 1


	ALTER TABLE [dbo].[ZamowienieProdukt] DROP CONSTRAINT [FK_zamowienia_produkty_produkty]

	ALTER TABLE [dbo].[ZamowienieProdukt]  WITH CHECK ADD  CONSTRAINT [FK_zamowienia_produkty_produkty] FOREIGN KEY(ProduktIdBazowy)  REFERENCES [dbo].[Produkt] ([Id]) ON DELETE CASCADE
	ALTER TABLE [dbo].[ZamowienieProdukt] CHECK CONSTRAINT [FK_zamowienia_produkty_produkty]

end