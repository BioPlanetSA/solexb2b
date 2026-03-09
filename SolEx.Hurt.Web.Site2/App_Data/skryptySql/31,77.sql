DECLARE @tblName VARCHAR(100)  --zmienna do nazwy tabeli
DECLARE @colname VARCHAR(100)  --zmienna do nazwy kolumny 
DECLARE @dbType VARCHAR(100)	-- znienna do typu kolumny 
DECLARE @cdcInst VARCHAR(100)


SET @tblName = 'FlatCeny'
EXEC('DELETE FROM '+ @tblName);
SET @colname = 'Id'
SET @dbType = 'bigint NOT NULL'

	---------wyłaczenia monitirowania tabeli
	SET @cdcInst = 'dbo_' + @tblName
	--exec sys.sp_cdc_disable_table @source_schema='dbo', @source_name= @tblName, @capture_instance= @cdcInst

if exists ( SELECT * FROM sys.objects  WHERE object_id = OBJECT_ID('PK_flat_ceny') AND parent_object_id = OBJECT_ID(N'dbo.FlatCeny'))
 ALTER TABLE FlatCeny DROP CONSTRAINT PK_flat_ceny;

if exists (select column_name from INFORMATION_SCHEMA.columns where table_name = @tblName and column_name = @colname) 
begin

	------------------------------Zmiana kolumny w tabeli
	EXEC('ALTER TABLE dbo.'+ @tblName +'  ALTER COLUMN  '+ @colname + ' '+ @dbType  )  
	EXEC('ALTER TABLE dbo.'+ @tblName +' ADD CONSTRAINT PK_flat_ceny PRIMARY KEY CLUSTERED (Id)');

	-----------włączenie monitorowania tabeli
	--EXEC sys.sp_cdc_enable_table @source_schema = 'dbo',@source_name = @tblName,@role_name = NULL, @supports_net_changes = 1
end