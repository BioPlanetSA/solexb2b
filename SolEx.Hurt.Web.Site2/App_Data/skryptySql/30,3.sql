--if NOT exists (select column_name from INFORMATION_SCHEMA.columns where table_name = 'Slajd' and column_name = 'Link') ALTER TABLE [Slajd]  ADD Link [varchar](2000) NULL;
--if NOT exists (select column_name from INFORMATION_SCHEMA.columns where table_name = 'Slajd' and column_name = 'CzyPokazywacTytul'){ ALTER TABLE [Slajd]  ADD CzyPokazywacTytul  [bit] NOT NULL CONSTRAINT [DF_Slajd_CzyPokazywacTytul]  DEFAULT ((0));
	DECLARE @sql VARCHAR(8000)
	DECLARE @tblName VARCHAR(100)
	DECLARE @colname VARCHAR(100)
	DECLARE @dbType VARCHAR(100)
	DECLARE @cdcInst VARCHAR(100)
	DECLARE @cdctblName VARCHAR(100)
	DECLARE @cdcTemptblName VARCHAR(100)

if NOT exists (select column_name from INFORMATION_SCHEMA.columns where table_name = 'Slajd' and column_name = 'Link')
begin
	SET @tblName = 'Slajd'
	SET @colname = 'Link'
	SET @dbType = 'varchar(2000) NULL'

	SET @cdcInst = 'dbo_' + @tblName

	---------wyłaczenia monitirowania tabeli
	----exec sys.sp_cdc_disable_table @source_schema='dbo', @source_name= @tblName, @capture_instance= @cdcInst

	------------------------------dodawanie kolumny do tabeli
	EXEC('ALTER TABLE dbo.'+ @tblName +' ADD '+ @colname + ' '+ @dbType  )  
	      
	-----------włączenie monitorowania tabeli
	----EXEC sys.sp_cdc_enable_table @source_schema = 'dbo',@source_name = @tblName,@role_name = NULL, @supports_net_changes = 1
end

if NOT exists (select column_name from INFORMATION_SCHEMA.columns where table_name = 'Slajd' and column_name = 'CzyPokazywacTytul')
begin
	SET @tblName = 'Slajd'
	SET @colname = 'CzyPokazywacTytul'
	SET @dbType = '[bit] NOT NULL CONSTRAINT [DF_Slajd_CzyPokazywacTytul]  DEFAULT ((0))'

	SET @cdcInst = 'dbo_' + @tblName

	---------wyłaczenia monitirowania tabeli
	----exec sys.sp_cdc_disable_table @source_schema='dbo', @source_name= @tblName, @capture_instance= @cdcInst

	------------------------------dodawanie kolumny do tabeli
	EXEC('ALTER TABLE dbo.'+ @tblName +' ADD '+ @colname + ' '+ @dbType  )  
	      
	-----------włączenie monitorowania tabeli
	----EXEC sys.sp_cdc_enable_table @source_schema = 'dbo',@source_name = @tblName,@role_name = NULL, @supports_net_changes = 1
end