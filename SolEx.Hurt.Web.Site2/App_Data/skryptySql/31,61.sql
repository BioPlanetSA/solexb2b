
DECLARE @tblName VARCHAR(100)  --zmienna do nazwy tabeli
DECLARE @colname VARCHAR(100)  --zmienna do nazwy kolumny 
DECLARE @cdcInst VARCHAR(100)

SET @tblName = 'ZamowienieProdukt'
SET @colname = 'Jednostka'

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





DECLARE @dbType VARCHAR(100)	-- znienna do typu kolumny 

DECLARE @tblNamePowiazanie VARCHAR(100)  --zmienna do nazwy tabeli do powiązania

SET @tblName = 'ZamowienieProdukt'
SET @colname = 'JednostkaMiary'
SET @dbType = 'bigint NOT NULL' --jesli ma byc wartość domyslna do dopisujemy:  DEFAULT ((  tu wartosc domyslna  ))

if NOT exists (select column_name from INFORMATION_SCHEMA.columns where table_name = @tblName and column_name = @colname) 
begin

	delete from ZamowienieProdukt;

	SET @cdcInst = 'dbo_' + @tblName

	---------wyłaczenia monitirowania tabeli
	--exec sys.sp_cdc_disable_table @source_schema='dbo', @source_name= @tblName, @capture_instance= @cdcInst

	------------------------------dodawanie kolumny do tabeli
	EXEC('ALTER TABLE dbo.'+ @tblName +' ADD '+ @colname + ' '+ @dbType  )  
      
	-----------włączenie monitorowania tabeli
	--EXEC sys.sp_cdc_enable_table @source_schema = 'dbo',@source_name = @tblName,@role_name = NULL, @supports_net_changes = 1

end
