DECLARE @cdcInst VARCHAR(100)
DECLARE @tblName VARCHAR(100)
DECLARE @colname VARCHAR(100)
DECLARE @dbType VARCHAR(100)
DECLARE @newColname VARCHAR(100)

SET @tblName = 'Produkt'


SET @newColname = 'OpPaletaIloscWOpakowaniu'
if exists (select column_name from INFORMATION_SCHEMA.columns where table_name = @tblName and column_name = 'LiczbaSztukNaPalecie') 
begin

	SET @cdcInst = 'dbo_' + @tblName
	--exec sys.sp_cdc_disable_table @source_schema='dbo', @source_name= @tblName, @capture_instance= @cdcInst
	EXEC sp_rename 'Produkt.LiczbaSztukNaPalecie', @newColname;
	--EXEC sys.sp_cdc_enable_table @source_schema = 'dbo',@source_name = @tblName,@role_name = NULL, @supports_net_changes = 1
end

SET @newColname = 'OpPaletaIloscNaWarstwie'
if exists (select column_name from INFORMATION_SCHEMA.columns where table_name = @tblName and column_name = 'LiczbaSztukNaWarstwie') 
begin

	SET @cdcInst = 'dbo_' + @tblName
	--exec sys.sp_cdc_disable_table @source_schema='dbo', @source_name= @tblName, @capture_instance= @cdcInst
	EXEC sp_rename 'Produkt.LiczbaSztukNaWarstwie', @newColname;
	--EXEC sys.sp_cdc_enable_table @source_schema = 'dbo',@source_name = @tblName,@role_name = NULL, @supports_net_changes = 1
end

SET @dbType = 'decimal(10, 4) null' 
SET @colname = 'OpZbiorczeIloscWOpakowaniu'

if NOT exists (select column_name from INFORMATION_SCHEMA.columns where table_name = @tblName and column_name = @colname) 
begin
	SET @cdcInst = 'dbo_' + @tblName
	--exec sys.sp_cdc_disable_table @source_schema='dbo', @source_name= @tblName, @capture_instance= @cdcInst
	EXEC('ALTER TABLE dbo.'+ @tblName +' ADD '+ @colname + ' '+ @dbType  )  
	--EXEC sys.sp_cdc_enable_table @source_schema = 'dbo',@source_name = @tblName,@role_name = NULL, @supports_net_changes = 1
end
