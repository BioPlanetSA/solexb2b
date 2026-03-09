DECLARE @tblName VARCHAR(100)  --zmienna do nazwy tabeli
DECLARE @colname VARCHAR(100)  --zmienna do nazwy kolumny 
DECLARE @dbType VARCHAR(100) -- znienna do typu kolumny 
DECLARE @cdcInst VARCHAR(100) -- nic nie wpisujemy tu
DECLARE @newColname VARCHAR(100) -- nic nie wpisujemy tu


SET @tblName = 'Newsletter'
SET @newColname = 'SzablonListyProduktow'

if exists (select column_name from INFORMATION_SCHEMA.columns where table_name = @tblName and column_name = 'Szablon') 
begin

 SET @cdcInst = 'dbo_' + @tblName

 ---------wyłaczenia monitirowania tabeli
 --exec sys.sp_cdc_disable_table @source_schema='dbo', @source_name= @tblName, @capture_instance= @cdcInst

 
 ----- Zmiana nazwy kolumny
 EXEC sp_rename 'Newsletter.Szablon', @newColname;
  
 -----------włączenie monitorowania tabeli
 --EXEC sys.sp_cdc_enable_table @source_schema = 'dbo',@source_name = @tblName,@role_name = NULL, @supports_net_changes = 1

end