DECLARE @tblName VARCHAR(100)  --zmienna do nazwy tabeli
DECLARE @colname VARCHAR(100)  --zmienna do nazwy kolumny 
DECLARE @dbType VARCHAR(100)	-- znienna do typu kolumny 
DECLARE @cdcInst VARCHAR(100)


SET @tblName = 'Klient'
SET @colname = 'JezykId'
SET @dbType = 'int NOT NULL'

if exists (select column_name from INFORMATION_SCHEMA.columns where table_name = @tblName and column_name = @colname) 
begin
	SET @cdcInst = 'dbo_' + @tblName
	---------wyłaczenia monitirowania tabeli
	--exec sys.sp_cdc_disable_table @source_schema='dbo', @source_name= @tblName, @capture_instance= @cdcInst
	------------------------------Zmiana kolumny w tabeli
	EXEC('ALTER TABLE dbo.'+ @tblName +'  ALTER COLUMN  '+ @colname + ' '+ @dbType  )  
	-----------włączenie monitorowania tabeli
	--EXEC sys.sp_cdc_enable_table @source_schema = 'dbo',@source_name = @tblName,@role_name = NULL, @supports_net_changes = 1
end

SET @tblName = 'KlientAdres'
DELETE FROM KlientAdres;
SET @colname = 'Id'
SET @dbType = 'bigint NOT NULL'
	---------wyłaczenia monitirowania tabeli
	SET @cdcInst = 'dbo_' + @tblName
	--exec sys.sp_cdc_disable_table @source_schema='dbo', @source_name= @tblName, @capture_instance= @cdcInst

if exists ( SELECT * FROM sys.objects  WHERE object_id = OBJECT_ID('PK_KlienAdres') AND parent_object_id = OBJECT_ID(N'dbo.KlientAdres'))
 ALTER TABLE KlientAdres DROP CONSTRAINT PK_KlienAdres;


if exists (select column_name from INFORMATION_SCHEMA.columns where table_name = @tblName and column_name = @colname) 
begin
	------------------------------Zmiana kolumny w tabeli
	EXEC('ALTER TABLE dbo.'+ @tblName +'  ALTER COLUMN  '+ @colname + ' '+ @dbType  )  	
	EXEC('ALTER TABLE dbo.'+ @tblName +' ADD CONSTRAINT PK_KlienAdres PRIMARY KEY CLUSTERED (Id)');
	-----------włączenie monitorowania tabeli
end
--EXEC sys.sp_cdc_enable_table @source_schema = 'dbo',@source_name = @tblName,@role_name = NULL, @supports_net_changes = 1


SET @tblName = 'KlientKategoriaKlienta'
DELETE FROM KlientKategoriaKlienta;
SET @colname = 'Id'
SET @dbType = 'bigint NOT NULL'

SET @cdcInst = 'dbo_' + @tblName
--exec sys.sp_cdc_disable_table @source_schema='dbo', @source_name= @tblName, @capture_instance= @cdcInst

if exists ( SELECT * FROM sys.objects  WHERE object_id = OBJECT_ID('PK_klienci_kategorie_2') AND parent_object_id = OBJECT_ID(N'dbo.KlientKategoriaKlienta'))
 ALTER TABLE KlientKategoriaKlienta DROP CONSTRAINT PK_klienci_kategorie_2;

if exists (select column_name from INFORMATION_SCHEMA.columns where table_name = @tblName and column_name = @colname) 
begin
	EXEC('ALTER TABLE dbo.'+ @tblName +'  ALTER COLUMN  '+ @colname + ' '+ @dbType  )
	EXEC('ALTER TABLE dbo.'+ @tblName +' ADD CONSTRAINT PK_klienci_kategorie_2 PRIMARY KEY CLUSTERED (Id)');  	
end
-----------włączenie monitorowania tabeli
--EXEC sys.sp_cdc_enable_table @source_schema = 'dbo',@source_name = @tblName,@role_name = NULL, @supports_net_changes = 1


SET @tblName = 'ProduktJednostka'
DELETE FROM ProduktJednostka;
SET @colname = 'Id'
SET @dbType = 'bigint NOT NULL'

SET @cdcInst = 'dbo_' + @tblName
--exec sys.sp_cdc_disable_table @source_schema='dbo', @source_name= @tblName, @capture_instance= @cdcInst

if exists ( SELECT * FROM sys.objects  WHERE object_id = OBJECT_ID('PK_ProduktyJednostki') AND parent_object_id = OBJECT_ID(N'dbo.'+@tblName))
 EXEC('ALTER TABLE '+@tblName+' DROP CONSTRAINT PK_ProduktyJednostki');

if exists (select column_name from INFORMATION_SCHEMA.columns where table_name = @tblName and column_name = @colname) 
begin
	------------------------------Zmiana kolumny w tabeli
	EXEC('ALTER TABLE dbo.'+ @tblName +'  ALTER COLUMN  '+ @colname + ' '+ @dbType  )  	
	EXEC('ALTER TABLE dbo.'+ @tblName +' ADD CONSTRAINT PK_ProduktyJednostki PRIMARY KEY CLUSTERED (Id)');
end
-----------włączenie monitorowania tabeli
--EXEC sys.sp_cdc_enable_table @source_schema = 'dbo',@source_name = @tblName,@role_name = NULL, @supports_net_changes = 1


SET @tblName = 'ProduktKategoria'
DELETE FROM ProduktKategoria;
SET @colname = 'Id'
SET @dbType = 'bigint NOT NULL'

SET @cdcInst = 'dbo_' + @tblName
--exec sys.sp_cdc_disable_table @source_schema='dbo', @source_name= @tblName, @capture_instance= @cdcInst

if exists ( SELECT * FROM sys.objects  WHERE object_id = OBJECT_ID('PK_produkty_kategorie') AND parent_object_id = OBJECT_ID(N'dbo.'+@tblName))
 EXEC('ALTER TABLE '+@tblName+' DROP CONSTRAINT PK_produkty_kategorie');
if exists ( SELECT * FROM sys.objects  WHERE object_id = OBJECT_ID('PK_'+ @tblName) AND parent_object_id = OBJECT_ID(N'dbo.'+@tblName))
 EXEC('ALTER TABLE '+@tblName+' DROP CONSTRAINT PK_'+ @tblName);
if exists (select column_name from INFORMATION_SCHEMA.columns where table_name = @tblName and column_name = @colname) 
begin
	------------------------------Zmiana kolumny w tabeli
	EXEC('ALTER TABLE dbo.'+ @tblName +'  ALTER COLUMN  '+ @colname + ' '+ @dbType  )
	EXEC('ALTER TABLE dbo.'+ @tblName +' ADD CONSTRAINT PK_'+ @tblName +' PRIMARY KEY CLUSTERED (Id)');  
end
	-----------włączenie monitorowania tabeli
	--EXEC sys.sp_cdc_enable_table @source_schema = 'dbo',@source_name = @tblName,@role_name = NULL, @supports_net_changes = 1

SET @tblName = 'ProduktPlik'
DELETE FROM ProduktPlik;
SET @colname = 'Id'
SET @dbType = 'bigint NOT NULL'

SET @cdcInst = 'dbo_' + @tblName
--exec sys.sp_cdc_disable_table @source_schema='dbo', @source_name= @tblName, @capture_instance= @cdcInst

if exists ( SELECT * FROM sys.objects  WHERE object_id = OBJECT_ID('PK_produkty_obrazki') AND parent_object_id = OBJECT_ID(N'dbo.'+@tblName))
 EXEC('ALTER TABLE '+@tblName+' DROP CONSTRAINT PK_produkty_obrazki');
 
if exists ( SELECT * FROM sys.objects  WHERE object_id = OBJECT_ID('PK_'+ @tblName) AND parent_object_id = OBJECT_ID(N'dbo.'+@tblName))
 EXEC('ALTER TABLE '+@tblName+' DROP CONSTRAINT PK_'+ @tblName);

if exists (select column_name from INFORMATION_SCHEMA.columns where table_name = @tblName and column_name = @colname) 
begin
	------------------------------Zmiana kolumny w tabeli
	EXEC('ALTER TABLE dbo.'+ @tblName +'  ALTER COLUMN  '+ @colname + ' '+ @dbType  )  	
	EXEC('ALTER TABLE dbo.'+ @tblName +' ADD CONSTRAINT PK_'+ @tblName +' PRIMARY KEY CLUSTERED (Id)');  
end
	-----------włączenie monitorowania tabeli
	--EXEC sys.sp_cdc_enable_table @source_schema = 'dbo',@source_name = @tblName,@role_name = NULL, @supports_net_changes = 1

SET @tblName = 'ProduktStan'
DELETE FROM ProduktStan;
SET @colname = 'Id'
SET @dbType = 'bigint NOT NULL'


SET @cdcInst = 'dbo_' + @tblName
--exec sys.sp_cdc_disable_table @source_schema='dbo', @source_name= @tblName, @capture_instance= @cdcInst

if exists ( SELECT * FROM sys.objects  WHERE object_id = OBJECT_ID('PK_ProduktStan') AND parent_object_id = OBJECT_ID(N'dbo.'+@tblName))
 EXEC('ALTER TABLE '+@tblName+' DROP CONSTRAINT PK_ProduktStan');
 
if exists ( SELECT * FROM sys.objects  WHERE object_id = OBJECT_ID('PK_'+ @tblName) AND parent_object_id = OBJECT_ID(N'dbo.'+@tblName))
 EXEC('ALTER TABLE '+@tblName+' DROP CONSTRAINT PK_'+ @tblName);

if exists (select column_name from INFORMATION_SCHEMA.columns where table_name = @tblName and column_name = @colname) 
begin
	------------------------------Zmiana kolumny w tabeli
	EXEC('ALTER TABLE dbo.'+ @tblName +'  ALTER COLUMN  '+ @colname + ' '+ @dbType  )  
	EXEC('ALTER TABLE dbo.'+ @tblName +' ADD CONSTRAINT PK_'+ @tblName +' PRIMARY KEY CLUSTERED (Id)');  
	-----------włączenie monitorowania tabeli
end
--EXEC sys.sp_cdc_enable_table @source_schema = 'dbo',@source_name = @tblName,@role_name = NULL, @supports_net_changes = 1

SET @tblName = 'ProduktyZamienniki'
DELETE FROM ProduktyZamienniki;
SET @colname = 'Id'
SET @dbType = 'bigint NOT NULL'


SET @cdcInst = 'dbo_' + @tblName
--exec sys.sp_cdc_disable_table @source_schema='dbo', @source_name= @tblName, @capture_instance= @cdcInst

if exists ( SELECT * FROM sys.objects  WHERE object_id = OBJECT_ID('PK_ProduktyZamienniki') AND parent_object_id = OBJECT_ID(N'dbo.'+@tblName))
 EXEC('ALTER TABLE '+@tblName+' DROP CONSTRAINT PK_ProduktyZamienniki');
 
if exists ( SELECT * FROM sys.objects  WHERE object_id = OBJECT_ID('PK_'+ @tblName) AND parent_object_id = OBJECT_ID(N'dbo.'+@tblName))
 EXEC('ALTER TABLE '+@tblName+' DROP CONSTRAINT PK_'+ @tblName);

if exists (select column_name from INFORMATION_SCHEMA.columns where table_name = @tblName and column_name = @colname) 
begin
	------------------------------Zmiana kolumny w tabeli
	EXEC('ALTER TABLE dbo.'+ @tblName +'  ALTER COLUMN  '+ @colname + ' '+ @dbType  )  
	EXEC('ALTER TABLE dbo.'+ @tblName +' ADD CONSTRAINT PK_'+ @tblName +' PRIMARY KEY CLUSTERED (Id)');  
end
	-----------włączenie monitorowania tabeli
	--EXEC sys.sp_cdc_enable_table @source_schema = 'dbo',@source_name = @tblName,@role_name = NULL, @supports_net_changes = 1
