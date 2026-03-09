
DECLARE @tblName VARCHAR(100)  --zmienna do nazwy tabeli
DECLARE @colname VARCHAR(100)  --zmienna do nazwy kolumny 
DECLARE @cdcInst VARCHAR(100)

SET @tblName = 'HistoriaDokumentu'
SET @colname = 'Hash'

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




DECLARE @newColname VARCHAR(100)  --zmienna do nazwy kolumny 

SET @tblName = 'Zamowienie'
SET @newColname = 'NumerWlasnyZamowieniaKlienta'

if exists (select column_name from INFORMATION_SCHEMA.columns where table_name = @tblName and column_name = 'NumerZamowieniaKlienta') 
begin

	SET @cdcInst = 'dbo_' + @tblName

	---------wyłaczenia monitirowania tabeli
	--exec sys.sp_cdc_disable_table @source_schema='dbo', @source_name= @tblName, @capture_instance= @cdcInst

	
	----- Zmiana nazwy kolumny
	EXEC sp_rename 'Zamowienie.NumerZamowieniaKlienta', @newColname;
  
	-----------włączenie monitorowania tabeli
	--EXEC sys.sp_cdc_enable_table @source_schema = 'dbo',@source_name = @tblName,@role_name = NULL, @supports_net_changes = 1

end