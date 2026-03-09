
DECLARE @tblName VARCHAR(100)  --zmienna do nazwy tabeli
DECLARE @colname VARCHAR(100)  --zmienna do nazwy kolumny 
DECLARE @cdcInst VARCHAR(100)

SET @tblName = 'Adres'

SET @colname = 'Jednorazowy'

DECLARE @nazwaDF VARCHAR(100)
SELECT @nazwaDF = [name] FROM sys.default_constraints WHERE name like 'DF__Adres__Jednorazo%' AND parent_object_id = OBJECT_ID('Adres')
if(@nazwaDF is not null)
begin
 EXEC('ALTER TABLE Adres DROP CONSTRAINT '+@nazwaDF);
end

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
SET @dbType = 'varchar(50) NULL' --jesli ma byc wartość domyslna do dopisujemy:  DEFAULT ((  tu wartosc domyslna  ))
SET @colname = 'TypAdresu'
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
if not exists ( SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID('FK_KlientAdres_Adres') AND parent_object_id = OBJECT_ID('KlientAdres'))
begin
	DELETE FROM KlientAdres WHERE AdresId NOT IN (SELECT Id FROM Adres)
	ALTER TABLE [dbo].[KlientAdres] WITH CHECK ADD CONSTRAINT [FK_KlientAdres_Adres] FOREIGN KEY([AdresId]) REFERENCES [dbo].[Adres] ([Id]) ON DELETE CASCADE
	ALTER TABLE [dbo].[KlientAdres] CHECK CONSTRAINT [FK_KlientAdres_Adres]
end