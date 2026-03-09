DECLARE @sql VARCHAR(8000)
DECLARE @tblName VARCHAR(100)
DECLARE @colname VARCHAR(100)
DECLARE @dbType VARCHAR(100)
DECLARE @cdcInst VARCHAR(100)
DECLARE @cdctblName VARCHAR(100)
DECLARE @cdcTemptblName VARCHAR(100)

SET @tblName = 'BlogWpis'
SET @colname = 'Tagi'
SET @dbType = 'varchar(100) NULL'--[bit] NOT NULL CONSTRAINT [DF_Slajd_CzyPokazywacTytul]  DEFAULT ((0)) -- tym z domyslna wartością 

if NOT exists (select column_name from INFORMATION_SCHEMA.columns where table_name = @tblName and column_name = @colname) 
begin
SET @cdcInst = 'dbo_' + @tblName

	---------wyłaczenia monitirowania tabeli
	--exec sys.sp_cdc_disable_table @source_schema='dbo', @source_name= @tblName, @capture_instance= @cdcInst

	------------------------------dodawanie kolumny do tabeli
	EXEC('ALTER TABLE dbo.'+ @tblName +' ADD '+ @colname + ' '+ @dbType  )  
	      
	-----------włączenie monitorowania tabeli
	----EXEC sys.sp_cdc_enable_table @source_schema = 'dbo',@source_name = @tblName,@role_name = NULL, @supports_net_changes = 1
end


SET @colname = 'KrotkiOpis'
SET @dbType = 'varchar(100) NULL'--[bit] NOT NULL CONSTRAINT [DF_Slajd_CzyPokazywacTytul]  DEFAULT ((0)) -- tym z domyslna wartością 
if NOT exists (select column_name from INFORMATION_SCHEMA.columns where table_name = @tblName and column_name = @colname) 
begin
SET @cdcInst = 'dbo_' + @tblName

	---------wyłaczenia monitirowania tabeli
	--exec sys.sp_cdc_disable_table @source_schema='dbo', @source_name= @tblName, @capture_instance= @cdcInst

	------------------------------dodawanie kolumny do tabeli
	EXEC('ALTER TABLE dbo.'+ @tblName +' ADD '+ @colname + ' '+ @dbType  )  
	      
	-----------włączenie monitorowania tabeli
	----EXEC sys.sp_cdc_enable_table @source_schema = 'dbo',@source_name = @tblName,@role_name = NULL, @supports_net_changes = 1
end

-- zmiana typu na max bo Mateuszowi ucionało wiadomości 
ALTER TABLE dbo.HistoriaWiadomosci ALTER COLUMN TrescWiadomosci nvarchar(MAX)
SET @colname = 'ZdjecieId'
SET @tblName = 'BlogKategoria'
SET @dbType = 'int NULL'--[bit] NOT NULL CONSTRAINT [DF_Slajd_CzyPokazywacTytul]  DEFAULT ((0)) -- tym z domyslna wartością 
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



SET @colname = 'ListaPowiazanychProduktow'
SET @tblName = 'BlogWpis'
SET @dbType = 'nvarchar(1000) NULL'--[bit] NOT NULL CONSTRAINT [DF_Slajd_CzyPokazywacTytul]  DEFAULT ((0)) -- tym z domyslna wartością 
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