DECLARE @sql VARCHAR(8000)
DECLARE @tblName VARCHAR(100)
DECLARE @colname VARCHAR(100)
DECLARE @dbType VARCHAR(100)
DECLARE @cdcInst VARCHAR(100)
DECLARE @cdctblName VARCHAR(100)
DECLARE @cdcTemptblName VARCHAR(100)

SET @tblName = 'BlogWpis'
SET @colname = 'Tekst1'
SET @dbType = 'nvarchar(4000) NULL'--[bit] NOT NULL CONSTRAINT [DF_Slajd_CzyPokazywacTytul]  DEFAULT ((0))
if NOT exists (select column_name from INFORMATION_SCHEMA.columns where table_name = 'BlogWpis' and column_name = 'Tekst1') 
begin
SET @cdcInst = 'dbo_' + @tblName

	---------wyłaczenia monitirowania tabeli
	----exec sys.sp_cdc_disable_table @source_schema='dbo', @source_name= @tblName, @capture_instance= @cdcInst

	------------------------------dodawanie kolumny do tabeli
	EXEC('ALTER TABLE dbo.'+ @tblName +' ADD '+ @colname + ' '+ @dbType  )  
	      
	-----------włączenie monitorowania tabeli
	----EXEC sys.sp_cdc_enable_table @source_schema = 'dbo',@source_name = @tblName,@role_name = NULL, @supports_net_changes = 1
end


SET @tblName = 'BlogWpis'
SET @colname = 'Tekst2'
SET @dbType = 'nvarchar(4000) NULL'--[bit] NOT NULL CONSTRAINT [DF_Slajd_CzyPokazywacTytul]  DEFAULT ((0))
if NOT exists (select column_name from INFORMATION_SCHEMA.columns where table_name = 'BlogWpis' and column_name = 'Tekst2') 
begin
SET @cdcInst = 'dbo_' + @tblName

	---------wyłaczenia monitirowania tabeli
	----exec sys.sp_cdc_disable_table @source_schema='dbo', @source_name= @tblName, @capture_instance= @cdcInst

	------------------------------dodawanie kolumny do tabeli
	EXEC('ALTER TABLE dbo.'+ @tblName +' ADD '+ @colname + ' '+ @dbType  )  
	      
	-----------włączenie monitorowania tabeli
	----EXEC sys.sp_cdc_enable_table @source_schema = 'dbo',@source_name = @tblName,@role_name = NULL, @supports_net_changes = 1
end
if NOT exists (select column_name from INFORMATION_SCHEMA.columns where table_name = 'BlogWpis' and column_name = 'Tekst3') 
begin
SET @tblName = 'BlogWpis'
SET @colname = 'Tekst3'
SET @dbType = 'nvarchar(4000) NULL'--[bit] NOT NULL CONSTRAINT [DF_Slajd_CzyPokazywacTytul]  DEFAULT ((0))
SET @cdcInst = 'dbo_' + @tblName

	---------wyłaczenia monitirowania tabeli
	----exec sys.sp_cdc_disable_table @source_schema='dbo', @source_name= @tblName, @capture_instance= @cdcInst

	------------------------------dodawanie kolumny do tabeli
	EXEC('ALTER TABLE dbo.'+ @tblName +' ADD '+ @colname + ' '+ @dbType  )  
	      
	-----------włączenie monitorowania tabeli
	----EXEC sys.sp_cdc_enable_table @source_schema = 'dbo',@source_name = @tblName,@role_name = NULL, @supports_net_changes = 1
end

if NOT exists (select column_name from INFORMATION_SCHEMA.columns where table_name = 'BlogWpis' and column_name = 'Tekst4') 
begin
SET @tblName = 'BlogWpis'
SET @colname = 'Tekst4'
SET @dbType = 'nvarchar(4000) NULL'--[bit] NOT NULL CONSTRAINT [DF_Slajd_CzyPokazywacTytul]  DEFAULT ((0))
SET @cdcInst = 'dbo_' + @tblName

	---------wyłaczenia monitirowania tabeli
	----exec sys.sp_cdc_disable_table @source_schema='dbo', @source_name= @tblName, @capture_instance= @cdcInst

	------------------------------dodawanie kolumny do tabeli
	EXEC('ALTER TABLE dbo.'+ @tblName +' ADD '+ @colname + ' '+ @dbType  )  
	      
	-----------włączenie monitorowania tabeli
	----EXEC sys.sp_cdc_enable_table @source_schema = 'dbo',@source_name = @tblName,@role_name = NULL, @supports_net_changes = 1
end

if NOT exists (select column_name from INFORMATION_SCHEMA.columns where table_name = 'BlogWpis' and column_name = 'Tekst5') 
begin
SET @tblName = 'BlogWpis'
SET @colname = 'Tekst5'
SET @dbType = 'nvarchar(4000) NULL'--[bit] NOT NULL CONSTRAINT [DF_Slajd_CzyPokazywacTytul]  DEFAULT ((0))
SET @cdcInst = 'dbo_' + @tblName

	---------wyłaczenia monitirowania tabeli
	----exec sys.sp_cdc_disable_table @source_schema='dbo', @source_name= @tblName, @capture_instance= @cdcInst

	------------------------------dodawanie kolumny do tabeli
	EXEC('ALTER TABLE dbo.'+ @tblName +' ADD '+ @colname + ' '+ @dbType  )  
	      
	-----------włączenie monitorowania tabeli
	----EXEC sys.sp_cdc_enable_table @source_schema = 'dbo',@source_name = @tblName,@role_name = NULL, @supports_net_changes = 1
end


/* Pol liczbowe do idZdjecia */


SET @tblName = 'BlogWpis'
SET @colname = 'ZdjecieId1'
SET @dbType = 'int NULL'--[bit] NOT NULL CONSTRAINT [DF_Slajd_CzyPokazywacTytul]  DEFAULT ((0))
if NOT exists (select column_name from INFORMATION_SCHEMA.columns where table_name = 'BlogWpis' and column_name = 'ZdjecieId1') 
begin
SET @cdcInst = 'dbo_' + @tblName

	---------wyłaczenia monitirowania tabeli
	----exec sys.sp_cdc_disable_table @source_schema='dbo', @source_name= @tblName, @capture_instance= @cdcInst

	------------------------------dodawanie kolumny do tabeli
	EXEC('ALTER TABLE dbo.'+ @tblName +' ADD '+ @colname + ' '+ @dbType  )  
	      
	-----------włączenie monitorowania tabeli
	----EXEC sys.sp_cdc_enable_table @source_schema = 'dbo',@source_name = @tblName,@role_name = NULL, @supports_net_changes = 1
end


if NOT exists (select column_name from INFORMATION_SCHEMA.columns where table_name = 'BlogWpis' and column_name = 'ZdjecieId2') 
begin
SET @tblName = 'BlogWpis'
SET @colname = 'ZdjecieId2'
SET @dbType = 'int NULL'--[bit] NOT NULL CONSTRAINT [DF_Slajd_CzyPokazywacTytul]  DEFAULT ((0))
SET @cdcInst = 'dbo_' + @tblName

	---------wyłaczenia monitirowania tabeli
	----exec sys.sp_cdc_disable_table @source_schema='dbo', @source_name= @tblName, @capture_instance= @cdcInst

	------------------------------dodawanie kolumny do tabeli
	EXEC('ALTER TABLE dbo.'+ @tblName +' ADD '+ @colname + ' '+ @dbType  )  
	      
	-----------włączenie monitorowania tabeli
	----EXEC sys.sp_cdc_enable_table @source_schema = 'dbo',@source_name = @tblName,@role_name = NULL, @supports_net_changes = 1
end


if NOT exists (select column_name from INFORMATION_SCHEMA.columns where table_name = 'BlogWpis' and column_name = 'ZdjecieId3') 
begin
SET @tblName = 'BlogWpis'
SET @colname = 'ZdjecieId3'
SET @dbType = 'int NULL'--[bit] NOT NULL CONSTRAINT [DF_Slajd_CzyPokazywacTytul]  DEFAULT ((0))
SET @cdcInst = 'dbo_' + @tblName

	---------wyłaczenia monitirowania tabeli
	----exec sys.sp_cdc_disable_table @source_schema='dbo', @source_name= @tblName, @capture_instance= @cdcInst

	------------------------------dodawanie kolumny do tabeli
	EXEC('ALTER TABLE dbo.'+ @tblName +' ADD '+ @colname + ' '+ @dbType  )  
	      
	-----------włączenie monitorowania tabeli
	----EXEC sys.sp_cdc_enable_table @source_schema = 'dbo',@source_name = @tblName,@role_name = NULL, @supports_net_changes = 1
end


if NOT exists (select column_name from INFORMATION_SCHEMA.columns where table_name = 'BlogWpis' and column_name = 'ZdjecieId4') 
begin
SET @tblName = 'BlogWpis'
SET @colname = 'ZdjecieId4'
SET @dbType = 'int NULL'--[bit] NOT NULL CONSTRAINT [DF_Slajd_CzyPokazywacTytul]  DEFAULT ((0))
SET @cdcInst = 'dbo_' + @tblName

	---------wyłaczenia monitirowania tabeli
	--exec sys.sp_cdc_disable_table @source_schema='dbo', @source_name= @tblName, @capture_instance= @cdcInst

	------------------------------dodawanie kolumny do tabeli
	EXEC('ALTER TABLE dbo.'+ @tblName +' ADD '+ @colname + ' '+ @dbType  )  
	      
	-----------włączenie monitorowania tabeli
	----EXEC sys.sp_cdc_enable_table @source_schema = 'dbo',@source_name = @tblName,@role_name = NULL, @supports_net_changes = 1
end


if NOT exists (select column_name from INFORMATION_SCHEMA.columns where table_name = 'BlogWpis' and column_name = 'ZdjecieId5') 
begin
SET @tblName = 'BlogWpis'
SET @colname = 'ZdjecieId5'
SET @dbType = 'int NULL'--[bit] NOT NULL CONSTRAINT [DF_Slajd_CzyPokazywacTytul]  DEFAULT ((0))
SET @cdcInst = 'dbo_' + @tblName

	---------wyłaczenia monitirowania tabeli
	--exec sys.sp_cdc_disable_table @source_schema='dbo', @source_name= @tblName, @capture_instance= @cdcInst

	------------------------------dodawanie kolumny do tabeli
	EXEC('ALTER TABLE dbo.'+ @tblName +' ADD '+ @colname + ' '+ @dbType  )  
	      
	-----------włączenie monitorowania tabeli
	----EXEC sys.sp_cdc_enable_table @source_schema = 'dbo',@source_name = @tblName,@role_name = NULL, @supports_net_changes = 1
end


