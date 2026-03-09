
DECLARE @tblName VARCHAR(100)  --zmienna do nazwy tabeli
DECLARE @colname1 VARCHAR(100)  --zmienna do nazwy kolumny 
DECLARE @colname2 VARCHAR(100)  --zmienna do nazwy kolumny 
DECLARE @colname3 VARCHAR(100)  --zmienna do nazwy kolumny 
DECLARE @cdcInst VARCHAR(100)
SET @tblName = 'SposobPokazywaniaStanowRegula'
-- usuwanie domyślnych wartości 
if exists ( SELECT * FROM sys.default_constraints WHERE object_id = OBJECT_ID('DF_sposoby_pokazywania_stanow_reguly_kolejnosc') AND parent_object_id = OBJECT_ID('SposobPokazywaniaStanowRegula')) ALTER TABLE SposobPokazywaniaStanowRegula DROP CONSTRAINT DF_sposoby_pokazywania_stanow_reguly_kolejnosc;

--!!!!!!!!!!!!!!Przed usunieciem kolumny sprawdź czy nie ma ona Klucza obcego nałożonego, jesli ma to pierw należy usunąć klucz obcy potem dopiero kolumne 
--usuwanie kolumny 

--if exists (select column_name from INFORMATION_SCHEMA.columns where table_name = 'NazwaTabeli' and column_name = 'NazwaKolumny') ALTER TABLE NazwaTabeli  DROP COLUMN  NazwaKolumny;




SET @colname1 = 'Provider'
SET @colname2 = 'Kolejnosc'
SET @colname3 = 'WynikHtml'
SET @cdcInst = 'dbo_' + @tblName
---------wyłaczenia monitirowania tabeli
	--exec sys.sp_cdc_disable_table @source_schema='dbo', @source_name= @tblName, @capture_instance= @cdcInst

if exists (select column_name from INFORMATION_SCHEMA.columns where table_name = @tblName and column_name = @colname1) 
begin
	------------------------------Usuniecie kolumny z tabeli
	EXEC('ALTER TABLE dbo.'+ @tblName +' DROP COLUMN '+ @colname1  ) 
end

if exists (select column_name from INFORMATION_SCHEMA.columns where table_name = @tblName and column_name = @colname2) 
begin
	------------------------------Usuniecie kolumny z tabeli
	EXEC('ALTER TABLE dbo.'+ @tblName +' DROP COLUMN '+ @colname2  ) 
end

if exists (select column_name from INFORMATION_SCHEMA.columns where table_name = @tblName and column_name = @colname3) 
begin
	------------------------------Usuniecie kolumny z tabeli
	EXEC('ALTER TABLE dbo.'+ @tblName +' DROP COLUMN '+ @colname3  ) 
end
-----------włączenie monitorowania tabeli
	--EXEC sys.sp_cdc_enable_table @source_schema = 'dbo',@source_name = @tblName,@role_name = NULL, @supports_net_changes = 1

