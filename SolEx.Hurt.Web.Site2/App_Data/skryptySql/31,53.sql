DECLARE @tblName VARCHAR(100)  --zmienna do nazwy tabeli
DECLARE @colname VARCHAR(100)  --zmienna do nazwy kolumny 
DECLARE @dbType VARCHAR(100)	-- znienna do typu kolumny 
DECLARE @cdcInst VARCHAR(100)

SET @tblName = 'Newsletter'
SET @colname = 'OdpowiedzNaAdres'
SET @dbType = 'varchar(100) NULL' --jesli ma byc wartość domyslna do dopisujemy:  DEFAULT ((  tu wartosc domyslna  ))  np. bit NOT NULL DEFAULT (1)

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



SET @tblName = 'Newsletter'
SET @colname = 'Status'
SET @dbType = 'varchar(100) NOT NULL' --jesli ma byc wartość domyslna do dopisujemy:  DEFAULT ((  tu wartosc domyslna  ))  np. bit NOT NULL DEFAULT (1)

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

------------------------------------------------------------------------------------------------------------------------------------------------
---------------------------------------------------------usuwanie
------------------------------------------------------------------------------------------

SET @tblName = 'Newsletter'
SET @colname = 'DataModyfikacji'

if exists (select column_name from INFORMATION_SCHEMA.columns where table_name = @tblName and column_name = @colname) 
begin

	SET @cdcInst = 'dbo_' + @tblName

	---------wyłaczenia monitirowania tabeli
	--exec sys.sp_cdc_disable_table @source_schema='dbo', @source_name= @tblName, @capture_instance= @cdcInst

	------------------------------Usuniecie kolumny z tabeli
	EXEC('ALTER TABLE dbo.'+ @tblName +' DROP COLUMN '+ @colname  )  
	      
	-----------włączenie monitorowania tabeli
	--EXEC sys.sp_cdc_enable_table @source_schema = 'dbo',@source_name = @tblName,@role_name = NULL, @supports_net_changes = 1

end


SET @tblName = 'Newsletter'
SET @colname = 'IloscWyslana'

if exists (select column_name from INFORMATION_SCHEMA.columns where table_name = @tblName and column_name = @colname) 
begin

	SET @cdcInst = 'dbo_' + @tblName

	---------wyłaczenia monitirowania tabeli
	--exec sys.sp_cdc_disable_table @source_schema='dbo', @source_name= @tblName, @capture_instance= @cdcInst

ALTER TABLE [dbo].[Newsletter] DROP CONSTRAINT [DF_mailing_kampanie_IloscWyslana];

	------------------------------Usuniecie kolumny z tabeli
	EXEC('ALTER TABLE dbo.'+ @tblName +' DROP COLUMN '+ @colname  )  
	      
	-----------włączenie monitorowania tabeli
	--EXEC sys.sp_cdc_enable_table @source_schema = 'dbo',@source_name = @tblName,@role_name = NULL, @supports_net_changes = 1

end



SET @tblName = 'Newsletter'
SET @colname = 'IloscWszyscy'

if exists (select column_name from INFORMATION_SCHEMA.columns where table_name = @tblName and column_name = @colname) 
begin

	SET @cdcInst = 'dbo_' + @tblName

	---------wyłaczenia monitirowania tabeli
	--exec sys.sp_cdc_disable_table @source_schema='dbo', @source_name= @tblName, @capture_instance= @cdcInst

	ALTER TABLE [dbo].[Newsletter] DROP CONSTRAINT 	[DF_mailing_kampanie_IloscWszystkich];

	------------------------------Usuniecie kolumny z tabeli
	EXEC('ALTER TABLE dbo.'+ @tblName +' DROP COLUMN '+ @colname  )  
	      
	-----------włączenie monitorowania tabeli
	--EXEC sys.sp_cdc_enable_table @source_schema = 'dbo',@source_name = @tblName,@role_name = NULL, @supports_net_changes = 1

end
