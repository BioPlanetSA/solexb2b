DECLARE @tblName VARCHAR(100)  --zmienna do nazwy tabeli
DECLARE @colname VARCHAR(100)  --zmienna do nazwy kolumny 
DECLARE @dbType VARCHAR(100)	-- znienna do typu kolumny 
DECLARE @cdcInst VARCHAR(100)

DECLARE @tblNamePowiazanie VARCHAR(100)  --zmienna do nazwy tabeli do powiązania

SET @tblName = 'Adres'
SET @colname = 'Jednorazowy'
SET @dbType = 'bit NOT NULL  DEFAULT ((0))'
--SET @tblNamePowiazanie = 'Powiazanie do'

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

SET @colname = 'DataDodania'
SET @dbType = 'datetime  null'--[bit] NOT NULL CONSTRAINT [DF_Slajd_CzyPokazywacTytul]  DEFAULT ((0)) -- tym z domyslna wartością 

--SET @tblNamePowiazanie = 'Powiazanie do'

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


SET @colname = 'AutorId'
SET @dbType = 'int null'--[bit] NOT NULL CONSTRAINT [DF_Slajd_CzyPokazywacTytul]  DEFAULT ((0)) -- tym z domyslna wartością 

SET @tblNamePowiazanie = 'Klient'

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

