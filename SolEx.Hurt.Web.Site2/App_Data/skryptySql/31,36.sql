DECLARE @cdcInst VARCHAR(100)
DECLARE @tblName VARCHAR(100)
DECLARE @colname VARCHAR(100)
DECLARE @dbType VARCHAR(100)
DECLARE @newColname VARCHAR(100)

SET @tblName = 'Produkt'

SET @newColname = 'OpJednostkoweGlebokosc'
if exists (select column_name from INFORMATION_SCHEMA.columns where table_name = @tblName and column_name = 'GlebokoscOpakowaniaJednostkowego') 
begin

	SET @cdcInst = 'dbo_' + @tblName
	--exec sys.sp_cdc_disable_table @source_schema='dbo', @source_name= @tblName, @capture_instance= @cdcInst
	EXEC sp_rename 'Produkt.GlebokoscOpakowaniaJednostkowego', @newColname;
	--EXEC sys.sp_cdc_enable_table @source_schema = 'dbo',@source_name = @tblName,@role_name = NULL, @supports_net_changes = 1
end


SET @newColname = 'OpJednostkoweSzerokosc'
if exists (select column_name from INFORMATION_SCHEMA.columns where table_name = @tblName and column_name = 'SzerokoscOpakowaniaJednostkowego') 
begin

	SET @cdcInst = 'dbo_' + @tblName
	--exec sys.sp_cdc_disable_table @source_schema='dbo', @source_name= @tblName, @capture_instance= @cdcInst
	EXEC sp_rename 'Produkt.SzerokoscOpakowaniaJednostkowego', @newColname;
	--EXEC sys.sp_cdc_enable_table @source_schema = 'dbo',@source_name = @tblName,@role_name = NULL, @supports_net_changes = 1
end


SET @newColname = 'OpJednostkoweWysokosc'
if exists (select column_name from INFORMATION_SCHEMA.columns where table_name = @tblName and column_name = 'WysokoscOpakowaniaJednostkowego') 
begin

	SET @cdcInst = 'dbo_' + @tblName
	--exec sys.sp_cdc_disable_table @source_schema='dbo', @source_name= @tblName, @capture_instance= @cdcInst
	EXEC sp_rename 'Produkt.WysokoscOpakowaniaJednostkowego', @newColname;
	--EXEC sys.sp_cdc_enable_table @source_schema = 'dbo',@source_name = @tblName,@role_name = NULL, @supports_net_changes = 1
end


SET @newColname = 'OpZbiorczeGlebokosc'
if exists (select column_name from INFORMATION_SCHEMA.columns where table_name = @tblName and column_name = 'GlebokoscOpakowaniaZbiorczego') 
begin

	SET @cdcInst = 'dbo_' + @tblName
	--exec sys.sp_cdc_disable_table @source_schema='dbo', @source_name= @tblName, @capture_instance= @cdcInst
	EXEC sp_rename 'Produkt.GlebokoscOpakowaniaZbiorczego', @newColname;
	--EXEC sys.sp_cdc_enable_table @source_schema = 'dbo',@source_name = @tblName,@role_name = NULL, @supports_net_changes = 1
end


SET @newColname = 'OpZbiorczeSzerokosc'
if exists (select column_name from INFORMATION_SCHEMA.columns where table_name = @tblName and column_name = 'SzerokoscOpakowaniaZbiorczego') 
begin

	SET @cdcInst = 'dbo_' + @tblName
	--exec sys.sp_cdc_disable_table @source_schema='dbo', @source_name= @tblName, @capture_instance= @cdcInst
	EXEC sp_rename 'Produkt.SzerokoscOpakowaniaZbiorczego', @newColname;
	--EXEC sys.sp_cdc_enable_table @source_schema = 'dbo',@source_name = @tblName,@role_name = NULL, @supports_net_changes = 1
end


SET @newColname = 'OpZbiorczeWysokosc'
if exists (select column_name from INFORMATION_SCHEMA.columns where table_name = @tblName and column_name = 'WysokoscOpakowaniaZbiorczego') 
begin

	SET @cdcInst = 'dbo_' + @tblName
	--exec sys.sp_cdc_disable_table @source_schema='dbo', @source_name= @tblName, @capture_instance= @cdcInst
	EXEC sp_rename 'Produkt.WysokoscOpakowaniaZbiorczego', @newColname;
	--EXEC sys.sp_cdc_enable_table @source_schema = 'dbo',@source_name = @tblName,@role_name = NULL, @supports_net_changes = 1
end



SET @dbType = 'decimal(10, 4) null' 
SET @colname = 'OpJednostkoweObjetosc'

if NOT exists (select column_name from INFORMATION_SCHEMA.columns where table_name = @tblName and column_name = @colname) 
begin
	SET @cdcInst = 'dbo_' + @tblName
	--exec sys.sp_cdc_disable_table @source_schema='dbo', @source_name= @tblName, @capture_instance= @cdcInst
	EXEC('ALTER TABLE dbo.'+ @tblName +' ADD '+ @colname + ' '+ @dbType  )  
	--EXEC sys.sp_cdc_enable_table @source_schema = 'dbo',@source_name = @tblName,@role_name = NULL, @supports_net_changes = 1
end

SET @colname = 'OpJednostkoweWaga'

if NOT exists (select column_name from INFORMATION_SCHEMA.columns where table_name = @tblName and column_name = @colname) 
begin
	SET @cdcInst = 'dbo_' + @tblName
	--exec sys.sp_cdc_disable_table @source_schema='dbo', @source_name= @tblName, @capture_instance= @cdcInst
	EXEC('ALTER TABLE dbo.'+ @tblName +' ADD '+ @colname + ' '+ @dbType  )  
	--EXEC sys.sp_cdc_enable_table @source_schema = 'dbo',@source_name = @tblName,@role_name = NULL, @supports_net_changes = 1
end

SET @colname = 'OpZbiorczeObjetosc'

if NOT exists (select column_name from INFORMATION_SCHEMA.columns where table_name = @tblName and column_name = @colname) 
begin
	SET @cdcInst = 'dbo_' + @tblName
	--exec sys.sp_cdc_disable_table @source_schema='dbo', @source_name= @tblName, @capture_instance= @cdcInst
	EXEC('ALTER TABLE dbo.'+ @tblName +' ADD '+ @colname + ' '+ @dbType  )  
	--EXEC sys.sp_cdc_enable_table @source_schema = 'dbo',@source_name = @tblName,@role_name = NULL, @supports_net_changes = 1
end

SET @colname = 'OpZbiorczeWaga'

if NOT exists (select column_name from INFORMATION_SCHEMA.columns where table_name = @tblName and column_name = @colname) 
begin
	SET @cdcInst = 'dbo_' + @tblName
	--exec sys.sp_cdc_disable_table @source_schema='dbo', @source_name= @tblName, @capture_instance= @cdcInst
	EXEC('ALTER TABLE dbo.'+ @tblName +' ADD '+ @colname + ' '+ @dbType  )  
	--EXEC sys.sp_cdc_enable_table @source_schema = 'dbo',@source_name = @tblName,@role_name = NULL, @supports_net_changes = 1
end


SET @colname = 'OpPaletaGlebokosc'

if NOT exists (select column_name from INFORMATION_SCHEMA.columns where table_name = @tblName and column_name = @colname) 
begin
	SET @cdcInst = 'dbo_' + @tblName
	--exec sys.sp_cdc_disable_table @source_schema='dbo', @source_name= @tblName, @capture_instance= @cdcInst
	EXEC('ALTER TABLE dbo.'+ @tblName +' ADD '+ @colname + ' '+ @dbType  )  
	--EXEC sys.sp_cdc_enable_table @source_schema = 'dbo',@source_name = @tblName,@role_name = NULL, @supports_net_changes = 1
end
SET @colname = 'OpPaletaSzerokosc'

if NOT exists (select column_name from INFORMATION_SCHEMA.columns where table_name = @tblName and column_name = @colname) 
begin
	SET @cdcInst = 'dbo_' + @tblName
	--exec sys.sp_cdc_disable_table @source_schema='dbo', @source_name= @tblName, @capture_instance= @cdcInst
	EXEC('ALTER TABLE dbo.'+ @tblName +' ADD '+ @colname + ' '+ @dbType  )  
	--EXEC sys.sp_cdc_enable_table @source_schema = 'dbo',@source_name = @tblName,@role_name = NULL, @supports_net_changes = 1
end
SET @colname = 'OpPaletaWysokosc'

if NOT exists (select column_name from INFORMATION_SCHEMA.columns where table_name = @tblName and column_name = @colname) 
begin
	SET @cdcInst = 'dbo_' + @tblName
	--exec sys.sp_cdc_disable_table @source_schema='dbo', @source_name= @tblName, @capture_instance= @cdcInst
	EXEC('ALTER TABLE dbo.'+ @tblName +' ADD '+ @colname + ' '+ @dbType  )  
	--EXEC sys.sp_cdc_enable_table @source_schema = 'dbo',@source_name = @tblName,@role_name = NULL, @supports_net_changes = 1
end
SET @colname = 'OpPaletaObjetosc'

if NOT exists (select column_name from INFORMATION_SCHEMA.columns where table_name = @tblName and column_name = @colname) 
begin
	SET @cdcInst = 'dbo_' + @tblName
	--exec sys.sp_cdc_disable_table @source_schema='dbo', @source_name= @tblName, @capture_instance= @cdcInst
	EXEC('ALTER TABLE dbo.'+ @tblName +' ADD '+ @colname + ' '+ @dbType  )  
	--EXEC sys.sp_cdc_enable_table @source_schema = 'dbo',@source_name = @tblName,@role_name = NULL, @supports_net_changes = 1
end
SET @colname = 'OpPaletaWaga'

if NOT exists (select column_name from INFORMATION_SCHEMA.columns where table_name = @tblName and column_name = @colname) 
begin
	SET @cdcInst = 'dbo_' + @tblName
	--exec sys.sp_cdc_disable_table @source_schema='dbo', @source_name= @tblName, @capture_instance= @cdcInst
	EXEC('ALTER TABLE dbo.'+ @tblName +' ADD '+ @colname + ' '+ @dbType  )  
	--EXEC sys.sp_cdc_enable_table @source_schema = 'dbo',@source_name = @tblName,@role_name = NULL, @supports_net_changes = 1
end
