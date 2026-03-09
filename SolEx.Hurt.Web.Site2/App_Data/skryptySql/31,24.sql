if exists ( SELECT * FROM sys.default_constraints WHERE object_id = OBJECT_ID('DF_produkty_dostepny_dla_wszystkich') AND parent_object_id = OBJECT_ID('Produkt')) ALTER TABLE Produkt DROP CONSTRAINT DF_produkty_dostepny_dla_wszystkich;


DECLARE @tblName VARCHAR(100)  --zmienna do nazwy tabeli
DECLARE @colname VARCHAR(100)  --zmienna do nazwy kolumny 
DECLARE @cdcInst VARCHAR(100)

SET @tblName = 'Produkt'
SET @colname = 'DostepnyDlaWszystkich'

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