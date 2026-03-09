
DECLARE @tblName VARCHAR(100)  --zmienna do nazwy tabeli
DECLARE @colname VARCHAR(100)  --zmienna do nazwy kolumny 
DECLARE @dbType VARCHAR(100)	-- znienna do typu kolumny 
DECLARE @cdcInst VARCHAR(100)
DECLARE @newColname VARCHAR(100)

SET @tblName = 'Klient'
SET @newColname = 'Koncesja'
if exists (select column_name from INFORMATION_SCHEMA.columns where table_name = @tblName and column_name = 'Kocesja') 
begin

	SET @cdcInst = 'dbo_' + @tblName

	---------wyłaczenia monitirowania tabeli
	--exec sys.sp_cdc_disable_table @source_schema='dbo', @source_name= @tblName, @capture_instance= @cdcInst
		
	EXEC sp_rename 'Klient.Kocesja', @newColname;
	
		-----------włączenie monitorowania tabeli
	--EXEC sys.sp_cdc_enable_table @source_schema = 'dbo',@source_name = @tblName,@role_name = NULL, @supports_net_changes = 1

end


SET @tblName = 'HistoriaDokumentu'
SET @newColname = 'DokumentPowiazanyId'
SET @dbType = 'int NULL'
if NOT exists (select column_name from INFORMATION_SCHEMA.columns where table_name = @tblName and column_name = @newColname) 
begin

	SET @cdcInst = 'dbo_' + @tblName

	---------wyłaczenia monitirowania tabeli
	--exec sys.sp_cdc_disable_table @source_schema='dbo', @source_name= @tblName, @capture_instance= @cdcInst
		
	EXEC('ALTER TABLE dbo.'+ @tblName +' ADD '+ @newColname + ' '+ @dbType  )
	
		-----------włączenie monitorowania tabeli
	--EXEC sys.sp_cdc_enable_table @source_schema = 'dbo',@source_name = @tblName,@role_name = NULL, @supports_net_changes = 1

end

SET @tblName = 'Produkt'
SET @newColname = 'WymaganaKoncesja'
if exists (select column_name from INFORMATION_SCHEMA.columns where table_name = @tblName and column_name = 'WymaganaKocesja') 
begin

	SET @cdcInst = 'dbo_' + @tblName

	---------wyłaczenia monitirowania tabeli
	--exec sys.sp_cdc_disable_table @source_schema='dbo', @source_name= @tblName, @capture_instance= @cdcInst
		
	EXEC sp_rename 'Produkt.WymaganaKocesja', @newColname;
	
		-----------włączenie monitorowania tabeli
	--EXEC sys.sp_cdc_enable_table @source_schema = 'dbo',@source_name = @tblName,@role_name = NULL, @supports_net_changes = 1

end

SET @tblName = 'TlumaczeniePole'
SET @colname = 'MiejsceFrazy '
SET @dbType = 'varchar(150) NOT NULL DEFAULT(''Brak'')' --jesli ma byc wartość domyslna do dopisujemy:  DEFAULT ((  tu wartosc domyslna  ))  np. bit NOT NULL DEFAULT (1)

if NOT exists (select column_name from INFORMATION_SCHEMA.columns where table_name = @tblName and column_name = @colname) 
begin
 SET @cdcInst = 'dbo_' + @tblName

 ---------wyłaczenia monitirowania tabeli
 --exec sys.sp_cdc_disable_table @source_schema='dbo', @source_name= @tblName, @capture_instance= @cdcInst

 ------------------------------dodawanie kolumny do tabeli
 EXEC('ALTER TABLE dbo.'+ @tblName +' ADD '+ @colname + ' '+ @dbType  )  
      
 -----------włączenie monitorowania tabeli
 --EXEC sys.sp_cdc_enable_table @source_schema = 'dbo',@source_name = @tblName,@role_name = NULL, @supports_net_changes = 1

end