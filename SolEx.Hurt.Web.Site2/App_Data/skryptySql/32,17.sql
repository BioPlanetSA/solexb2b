DECLARE @tblName VARCHAR(100)  --zmienna do nazwy tabeli
DECLARE @colname VARCHAR(100)  --zmienna do nazwy kolumny 
DECLARE @dbType VARCHAR(100)	-- znienna do typu kolumny 
DECLARE @cdcInst VARCHAR(100)


SET @tblName = 'HistoriaDokumentuListPrzewozowy'
SET @colname = 'tmpId'
SET @dbType = 'int NOT NULL'

if not exists (select column_name from INFORMATION_SCHEMA.columns where table_name = @tblName and column_name = @colname) 
begin

	SET @cdcInst = 'dbo_' + @tblName

	---------wyłaczenia monitirowania tabeli
	--exec sys.sp_cdc_disable_table @source_schema='dbo', @source_name= @tblName, @capture_instance= @cdcInst

	------------------------------Zmiana kolumny w tabeli
	EXEC('ALTER TABLE dbo.'+ @tblName +'  ADD '+ @colname + ' '+ @dbType  )  
	
	EXEC('UPDATE dbo.'+ @tblName +'  SET  '+ @colname + '= Id ')  

	EXEC('ALTER TABLE dbo.'+ @tblName +' DROP CONSTRAINT [PK_historia_dokumenty_listy_przewozowe];')
	
	EXEC('ALTER TABLE dbo.'+ @tblName +' DROP COLUMN Id' ) 
	 
	EXEC sp_RENAME 'HistoriaDokumentuListPrzewozowy.tmpId' , 'Id';

	EXEC('ALTER TABLE dbo.'+ @tblName +' ADD primary key (ID)')

	-----------włączenie monitorowania tabeli
	--EXEC sys.sp_cdc_enable_table @source_schema = 'dbo',@source_name = @tblName,@role_name = NULL, @supports_net_changes = 1

end

