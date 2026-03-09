--wersja Sylwestra po oczyszczeniu
if exists ( SELECT * FROM sys.default_constraints WHERE object_id = OBJECT_ID('DF__historia___jedno__0AFD888E') AND parent_object_id = OBJECT_ID('HistoriaDokumentuProdukt')) ALTER TABLE HistoriaDokumentuProdukt DROP CONSTRAINT DF__historia___jedno__0AFD888E;
 IF EXISTS (SELECT *  FROM sys.indexes  WHERE name='dok_id_non_clast' 
    AND object_id = OBJECT_ID('[dbo].[HistoriaDokumentuProdukt]'))
  begin
    DROP INDEX [dok_id_non_clast] ON [dbo].[HistoriaDokumentuProdukt];
  end
DECLARE @tblName VARCHAR(100)  --zmienna do nazwy tabeli
DECLARE @colname VARCHAR(100)  --zmienna do nazwy kolumny 
DECLARE @dbType VARCHAR(100)	-- znienna do typu kolumny 
DECLARE @cdcInst VARCHAR(100)
DECLARE @Deleted_Rows INT;
SET @Deleted_Rows = 1;

SET @tblName = 'HistoriaDokumentuProdukt'
SET @colname = 'JednostkaMiary'
SET @dbType = 'bigint NULL'

if exists (select column_name from INFORMATION_SCHEMA.columns where table_name = @tblName and column_name = @colname) 
begin

--exec sys.sp_cdc_disable_table @source_schema='dbo',	@source_name= 'HistoriaDokumentu', @capture_instance= 'dbo_HistoriaDokumentu'

EXEC('DELETE FROM dbo.HistoriaDokumentu')

--EXEC sys.sp_cdc_enable_table @source_schema = 'dbo',@source_name = 'HistoriaDokumentu',@role_name = NULL, @supports_net_changes = 1		
			

	SET @cdcInst = 'dbo_' + @tblName

	---------wyłaczenia monitirowania tabeli
	--exec sys.sp_cdc_disable_table @source_schema='dbo', @source_name= @tblName, @capture_instance= @cdcInst
	

	------------------------------Zmiana kolumny w tabeli
	EXEC('ALTER TABLE dbo.'+ @tblName +'  ALTER COLUMN  '+ @colname + ' '+ @dbType  )  
	-----------włączenie monitorowania tabeli
	--EXEC sys.sp_cdc_enable_table @source_schema = 'dbo',@source_name = @tblName,@role_name = NULL, @supports_net_changes = 1

end