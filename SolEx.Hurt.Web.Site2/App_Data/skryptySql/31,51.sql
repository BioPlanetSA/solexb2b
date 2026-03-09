--wersja Sylwestra po oczyszczeniu

DECLARE @tblName VARCHAR(100)  --zmienna do nazwy tabeli
DECLARE @colname VARCHAR(100)  --zmienna do nazwy kolumny 
DECLARE @dbType VARCHAR(100)	-- znienna do typu kolumny 
DECLARE @cdcInst VARCHAR(100)


SET @tblName = 'Rabat'
SET @colname = 'Id'
SET @dbType = 'bigint NOT NULL'

if exists (select column_name from INFORMATION_SCHEMA.columns where table_name = @tblName and column_name = @colname) 
begin
	SET @cdcInst = 'dbo_' + @tblName

	---------wyłaczenia monitirowania tabeli
	--exec sys.sp_cdc_disable_table @source_schema='dbo', @source_name= @tblName, @capture_instance= @cdcInst

	delete from [Rabat];

	if exists ( SELECT * FROM information_schema.table_constraints  WHERE constraint_type = 'PRIMARY KEY'   AND table_name = 'Rabat') ALTER TABLE Rabat DROP CONSTRAINT [PK_Hrabaty];

	------------------------------Zmiana kolumny w tabeli
	EXEC('ALTER TABLE dbo.'+ @tblName +'  ALTER COLUMN  '+ @colname + ' '+ @dbType  ) ; 	

	ALTER TABLE Rabat ADD CONSTRAINT [PK_Hrabaty] PRIMARY KEY  CLUSTERED (ID)
	
	-----------włączenie monitorowania tabeli
	--EXEC sys.sp_cdc_enable_table @source_schema = 'dbo',@source_name = @tblName,@role_name = NULL, @supports_net_changes = 1

end
