--if exists (select column_name from INFORMATION_SCHEMA.columns where table_name = 'Cecha' and column_name = 'CechaNadrzednaId') ALTER TABLE Cecha  DROP COLUMN  CechaNadrzednaId;

DECLARE @tblName VARCHAR(100)  --zmienna do nazwy tabeli
DECLARE @colname VARCHAR(100)  --zmienna do nazwy kolumny 
DECLARE @Type VARCHAR(100)	-- znienna do typu kolumny 
DECLARE @cdcInst VARCHAR(100)
DECLARE @tblNamePowiazanie VARCHAR(100)  --zmienna do nazwy tabeli do powiązania

SET @tblName = 'HistoriaDokumentu'
SET @colname = 'WalutaId'
SET @Type = 'bigint NULL'
SET @tblNamePowiazanie = 'Waluta'

if exists (select column_name from INFORMATION_SCHEMA.columns where table_name = @tblName and column_name = @colname) 
begin

	SET @cdcInst = 'dbo_' + @tblName

	---------wyłaczenia monitirowania tabeli
	----exec sys.sp_cdc_disable_table @source_schema='dbo', @source_name= @tblName, @capture_instance= @cdcInst

	------------------------------Zmiana kolumny do tabeli
	EXEC('ALTER TABLE dbo.'+ @tblName +' ALTER COLUMN '+ @colname+' '+@Type   )  
	--Dodanie kluczy obcych
	EXEC('ALTER TABLE [dbo].['+@tblName+'] WITH NOCHECK ADD  CONSTRAINT [FK_'+@tblName+'_'+@tblNamePowiazanie+'_'+ @colname +'] FOREIGN KEY('+@colname+') REFERENCES [dbo].['+@tblNamePowiazanie+'] ([Id])')
	EXEC('ALTER TABLE [dbo].['+@tblName+'] CHECK CONSTRAINT [FK_'+@tblName+'_'+@tblNamePowiazanie+'_'+ @colname +']')
	      
	-----------włączenie monitorowania tabeli
	----EXEC sys.sp_cdc_enable_table @source_schema = 'dbo',@source_name = @tblName,@role_name = NULL, @supports_net_changes = 1

end

