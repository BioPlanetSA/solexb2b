

DECLARE @tblName VARCHAR(100)  --zmienna do nazwy tabeli
DECLARE @dbType VARCHAR(100)	-- znienna do typu kolumny 
DECLARE @cdcInst VARCHAR(100)	-- nic nie wpisujemy tu
DECLARE @colname VARCHAR(100)	-- nic nie wpisujemy tu

SET @tblName = 'BlogWpis'
SET @colname = 'Tekst1'
SET @dbType = 'nvarchar(MAX)'

if exists (select column_name from INFORMATION_SCHEMA.columns where table_name = @tblName and column_name = @colname) 
begin

	SET @cdcInst = 'dbo_' + @tblName
		---------wyłaczenia monitirowania tabeli
	--exec sys.sp_cdc_disable_table @source_schema='dbo', @source_name= @tblName, @capture_instance= @cdcInst
	EXEC('ALTER TABLE dbo.'+ @tblName +'  ALTER COLUMN  Tekst1 '+ @dbType  )  
	EXEC('ALTER TABLE dbo.'+ @tblName +'  ALTER COLUMN  Tekst2 '+ @dbType  )
	EXEC('ALTER TABLE dbo.'+ @tblName +'  ALTER COLUMN  Tekst3 '+ @dbType  )
	EXEC('ALTER TABLE dbo.'+ @tblName +'  ALTER COLUMN  Tekst4 '+ @dbType  )
	EXEC('ALTER TABLE dbo.'+ @tblName +'  ALTER COLUMN  Tekst5 '+ @dbType  )

	-----------włączenie monitorowania tabeli
	--EXEC sys.sp_cdc_enable_table @source_schema = 'dbo',@source_name = @tblName,@role_name = NULL, @supports_net_changes = 1

end