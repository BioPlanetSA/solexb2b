
-- usuwanie klucza obcego 

if exists ( SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID('NazwaKluczaObcego') AND parent_object_id = OBJECT_ID('NazwaTabeli')) ALTER TABLE NazwaTabeli DROP CONSTRAINT NazwaKluczaObcego;

-- usuwanie domyślnych wartości 
if exists ( SELECT * FROM sys.default_constraints WHERE object_id = OBJECT_ID('NazwaKluczaObcego') AND parent_object_id = OBJECT_ID('NazwaTabeli')) ALTER TABLE NazwaTabeli DROP CONSTRAINT NazwaKluczaObcego;

--!!!!!!!!!!!!!!Przed usunieciem kolumny sprawdź czy nie ma ona Klucza obcego nałożonego, jesli ma to pierw należy usunąć klucz obcy potem dopiero kolumne 
--usuwanie kolumny 

--if exists (select column_name from INFORMATION_SCHEMA.columns where table_name = 'NazwaTabeli' and column_name = 'NazwaKolumny') ALTER TABLE NazwaTabeli  DROP COLUMN  NazwaKolumny;


DECLARE @tblName VARCHAR(100)  --zmienna do nazwy tabeli
DECLARE @colname VARCHAR(100)  --zmienna do nazwy kolumny 
DECLARE @cdcInst VARCHAR(100)

SET @tblName = 'NazwaTabeli'
SET @colname = 'NazwaKolumny'

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