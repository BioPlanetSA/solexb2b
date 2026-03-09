

DECLARE @tblName VARCHAR(100)  --zmienna do nazwy tabeli
DECLARE @colname VARCHAR(100)  --zmienna do nazwy kolumny 
DECLARE @dbType VARCHAR(100)	-- znienna do typu kolumny 
DECLARE @cdcInst VARCHAR(100)	-- nic nie wpisujemy tu
DECLARE @newColname VARCHAR(100)	-- nic nie wpisujemy tu

-- usuwanie klucza obcego 

if exists ( SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID('NazwaKluczaObcego') AND parent_object_id = OBJECT_ID('NazwaTabeli')) ALTER TABLE NazwaTabeli DROP CONSTRAINT NazwaKluczaObcego;

-- usuwanie domyślnych wartości 
if exists ( SELECT * FROM sys.default_constraints WHERE object_id = OBJECT_ID('NazwaKluczaObcego') AND parent_object_id = OBJECT_ID('NazwaTabeli')) ALTER TABLE NazwaTabeli DROP CONSTRAINT NazwaKluczaObcego;


SET @tblName = 'NazwaTabeli'
SET @newColname = 'NowaNazwaKolumny'

if exists (select column_name from INFORMATION_SCHEMA.columns where table_name = @tblName and column_name = 'StaraNazwaKolumny') 
begin

	SET @cdcInst = 'dbo_' + @tblName

	---------wyłaczenia monitirowania tabeli
	--exec sys.sp_cdc_disable_table @source_schema='dbo', @source_name= @tblName, @capture_instance= @cdcInst

	
	----- Zmiana nazwy kolumny
	EXEC sp_rename 'NazwaTabeli.StaraNazwaKolumny', @newColname;
  
	-----------włączenie monitorowania tabeli
	--EXEC sys.sp_cdc_enable_table @source_schema = 'dbo',@source_name = @tblName,@role_name = NULL, @supports_net_changes = 1

end