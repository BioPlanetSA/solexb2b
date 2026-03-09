DECLARE @tblName VARCHAR(100)  --zmienna do nazwy tabeli
DECLARE @colname VARCHAR(100)  --zmienna do nazwy kolumny 
DECLARE @dbType VARCHAR(100)	-- znienna do typu kolumny 
DECLARE @cdcInst VARCHAR(100)


SET @tblName = 'Tlumaczenie'
EXEC('DELETE FROM '+ @tblName);
SET @colname = 'Id'
SET @dbType = 'bigint NOT NULL'

	---------wyłaczenia monitirowania tabeli
	SET @cdcInst = 'dbo_' + @tblName
	--exec sys.sp_cdc_disable_table @source_schema='dbo', @source_name= @tblName, @capture_instance= @cdcInst

if exists ( SELECT * FROM sys.objects  WHERE object_id = OBJECT_ID('PK_slowniki') AND parent_object_id = OBJECT_ID(N'dbo.Tlumaczenie'))
 ALTER TABLE Tlumaczenie DROP CONSTRAINT PK_slowniki;

if exists (select column_name from INFORMATION_SCHEMA.columns where table_name = @tblName and column_name = @colname) 
begin

	------------------------------Zmiana kolumny w tabeli
	EXEC('ALTER TABLE dbo.'+ @tblName +'  ALTER COLUMN  '+ @colname + ' '+ @dbType  )  
	EXEC('ALTER TABLE dbo.'+ @tblName +' ADD CONSTRAINT PK_Tlumaczenie PRIMARY KEY CLUSTERED (Id)');
	EXEC('CREATE NONCLUSTERED INDEX IX_Tlumaczenia ON  dbo.'+ @tblName +'([JezykId] ASC,[Typ] ASC)');

	-----------włączenie monitorowania tabeli
	--EXEC sys.sp_cdc_enable_table @source_schema = 'dbo',@source_name = @tblName,@role_name = NULL, @supports_net_changes = 1
end

SET @colname = 'ObiektId'
SET @dbType = 'bigint NULL'

	SET @cdcInst = 'dbo_' + @tblName
	---------wyłaczenia monitirowania tabeli
	--exec sys.sp_cdc_disable_table @source_schema='dbo', @source_name= @tblName, @capture_instance= @cdcInst

if exists ( SELECT * FROM sys.objects  WHERE object_id = OBJECT_ID('DF_slowniki_obiekt_id') AND parent_object_id = OBJECT_ID(N'dbo.Tlumaczenie'))
 ALTER TABLE Tlumaczenie DROP CONSTRAINT DF_slowniki_obiekt_id;

if exists (select column_name from INFORMATION_SCHEMA.columns where table_name = @tblName and column_name = @colname) 
begin

	------------------------------Zmiana kolumny w tabeli
	EXEC('ALTER TABLE dbo.'+ @tblName +'  ALTER COLUMN  '+ @colname + ' '+ @dbType  )  
	-----------włączenie monitorowania tabeli
	--EXEC sys.sp_cdc_enable_table @source_schema = 'dbo',@source_name = @tblName,@role_name = NULL, @supports_net_changes = 1
end


SET @tblName = 'TlumaczeniePole'
EXEC('DELETE FROM '+ @tblName);
SET @colname = 'Id'
SET @dbType = 'bigint NOT NULL'


	SET @cdcInst = 'dbo_' + @tblName

	---------wyłaczenia monitirowania tabeli
	--exec sys.sp_cdc_disable_table @source_schema='dbo', @source_name= @tblName, @capture_instance= @cdcInst
if exists ( SELECT * FROM sys.objects  WHERE object_id = OBJECT_ID('PK_TlumaczeniePole') AND parent_object_id = OBJECT_ID(N'dbo.TlumaczeniePole')) ALTER TABLE TlumaczeniePole DROP CONSTRAINT PK_TlumaczeniePole;
if exists (select column_name from INFORMATION_SCHEMA.columns where table_name = @tblName and column_name = @colname) 
begin



	------------------------------Zmiana kolumny w tabeli
	EXEC('ALTER TABLE dbo.'+ @tblName +'  ALTER COLUMN  '+ @colname + ' '+ @dbType  )  
	
	EXEC('ALTER TABLE dbo.'+ @tblName +' ADD CONSTRAINT PK_'+@tblName+' PRIMARY  KEY CLUSTERED (Id)');
	-----------włączenie monitorowania tabeli
	--EXEC sys.sp_cdc_enable_table @source_schema = 'dbo',@source_name = @tblName,@role_name = NULL, @supports_net_changes = 1

end