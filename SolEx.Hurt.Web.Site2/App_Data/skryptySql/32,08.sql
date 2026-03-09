
DECLARE @tblName VARCHAR(100)  --zmienna do nazwy tabeli
DECLARE @colname VARCHAR(100)  --zmienna do nazwy kolumny 
DECLARE @dbType VARCHAR(100)	-- znienna do typu kolumny 
DECLARE @dbType2 VARCHAR(100)
DECLARE @cdcInst VARCHAR(100)
DECLARE @newColname VARCHAR(100)

SET @tblName = 'KoszykPozycje'
SET @colname = 'ProduktId'
SET @dbType = 'bigint NOT NULL'
SET @newColname = 'ProduktBazowyId'
if NOT exists (select column_name from INFORMATION_SCHEMA.columns where table_name = @tblName and column_name = @newColname) 
begin
	if exists ( SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID('FK_koszyki_produkty') AND parent_object_id = OBJECT_ID('KoszykPozycje')) ALTER TABLE KoszykPozycje DROP CONSTRAINT FK_koszyki_produkty;
	if exists (select column_name from INFORMATION_SCHEMA.columns where table_name = @tblName and column_name = @colname) 
	begin
		SET @cdcInst = 'dbo_' + @tblName

		---------wyłaczenia monitirowania tabeli
		--exec sys.sp_cdc_disable_table @source_schema='dbo', @source_name= @tblName, @capture_instance= @cdcInst

		------------------------------dodawanie kolumny do tabeli
		EXEC sp_rename 'KoszykPozycje.ProduktId', @newColname; 
      
		-----------włączenie monitorowania tabeli
		--EXEC sys.sp_cdc_enable_table @source_schema = 'dbo',@source_name = @tblName,@role_name = NULL, @supports_net_changes = 1
	end

	SET @colname = 'ProduktId'
	SET @dbType2 = 'bigint NULL'
	if NOT exists (select column_name from INFORMATION_SCHEMA.columns where table_name = @tblName and column_name = @colname) 
	begin
		SET @cdcInst = 'dbo_' + @tblName

		---------wyłaczenia monitirowania tabeli
		--exec sys.sp_cdc_disable_table @source_schema='dbo', @source_name= @tblName, @capture_instance= @cdcInst

		------------------------------dodawanie kolumny do tabeli
		EXEC('ALTER TABLE dbo.'+ @tblName +' ADD '+ @colname + ' '+ @dbType2  )  
		Exec ('Update KoszykPozycje set ProduktId=ProduktBazowyId')
		EXEC('ALTER TABLE dbo.'+ @tblName +'  ALTER COLUMN  '+ @colname + ' '+ @dbType  )
		-----------włączenie monitorowania tabeli
		--EXEC sys.sp_cdc_enable_table @source_schema = 'dbo',@source_name = @tblName,@role_name = NULL, @supports_net_changes = 1

	end
	ALTER TABLE [dbo].[KoszykPozycje]  WITH NOCHECK ADD  CONSTRAINT [FK_koszyki_produkty] FOREIGN KEY([ProduktBazowyId]) REFERENCES [dbo].[Produkt] ([Id]) ON DELETE CASCADE
	ALTER TABLE [dbo].[KoszykPozycje] CHECK CONSTRAINT [FK_koszyki_produkty]
end