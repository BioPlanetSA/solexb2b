/* ---------------------------------------------------------------- */
--wersja Sylwestra Z KLUCZAMI obcymi
/* ---------------------------------------------------------------- */

DECLARE @tblName VARCHAR(100)  --zmienna do nazwy tabeli
DECLARE @colname VARCHAR(100)  --zmienna do nazwy kolumny 
DECLARE @dbType VARCHAR(100)	-- znienna do typu kolumny 
DECLARE @cdcInst VARCHAR(100)	-- nic nie wpisujemy tu

DECLARE @tblNamePowiazanie VARCHAR(100)  --zmienna do nazwy tabeli do powiązania

SET @tblName = 'NazwaTabeli'
SET @colname = 'NazwaKolumny'
SET @dbType = 'varchar(50) NULL' --PAMIĘTAJ DODAĆ INFORMACJHE CZY MA BYĆ NULL LUB NOT NULL --jesli ma byc wartość domyslna do dopisujemy:  DEFAULT ((  tu wartosc domyslna  ))

SET @tblNamePowiazanie = 'Powiazanie do'

if NOT exists (select column_name from INFORMATION_SCHEMA.columns where table_name = @tblName and column_name = @colname) 
begin

	SET @cdcInst = 'dbo_' + @tblName

	---------wyłaczenia monitirowania tabeli
	--exec sys.sp_cdc_disable_table @source_schema='dbo', @source_name= @tblName, @capture_instance= @cdcInst

	------------------------------dodawanie kolumny do tabeli
	EXEC('ALTER TABLE dbo.'+ @tblName +' ADD '+ @colname + ' '+ @dbType  )  

	--Dodanie kluczy obcych
	EXEC('ALTER TABLE [dbo].['+@tblName+'] WITH NOCHECK ADD  CONSTRAINT [FK_'+@tblName+'_'+@tblNamePowiazanie+'_'+ @colname +'] FOREIGN KEY('+@colname+') REFERENCES [dbo].['+@tblNamePowiazanie+'] ([Id]) ON DELETE CASCADE')
	EXEC('ALTER TABLE [dbo].['+@tblName+'] CHECK CONSTRAINT [FK_'+@tblName+'_'+@tblNamePowiazanie+'_'+ @colname +']')
	      
	-----------włączenie monitorowania tabeli
	--EXEC sys.sp_cdc_enable_table @source_schema = 'dbo',@source_name = @tblName,@role_name = NULL, @supports_net_changes = 1

end


/* ---------------------------------------------------------------- */
-- wersja Sylwestra BEZ kliczy obcych - samo dodawanie kolumny
/* ---------------------------------------------------------------- */

DECLARE @tblName VARCHAR(100)  --zmienna do nazwy tabeli
DECLARE @colname VARCHAR(100)  --zmienna do nazwy kolumny 
DECLARE @dbType VARCHAR(100)	-- znienna do typu kolumny 
DECLARE @cdcInst VARCHAR(100)

SET @tblName = 'NazwaTabeli'
SET @colname = 'NazwaKolumny'
SET @dbType = 'varchar(50) NULL' --PAMIĘTAJ DODAĆ INFORMACJHE CZY MA BYĆ NULL LUB NOT NULL --jesli ma byc wartość domyslna do dopisujemy:  DEFAULT ((  tu wartosc domyslna  ))  np. bit NOT NULL DEFAULT (1)

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


/* ---------------------------------------------------------------- */
-- Wersja Bartka
-- to jest złeeeeeeeeeeeeeeeeeeeeeeeee nie wolna tak robić bo wtedy tracimy możliwość zapisywanie historii zmian
/* ---------------------------------------------------------------- */


IF NOT EXISTS (
  SELECT * 
  FROM   sys.columns 
  WHERE  object_id = OBJECT_ID(N'[dbo].[Atrybut]') 
         AND name = 'PokazujNazweAtrybutuJakoNaglowekFiltra'
)
BEGIN
 ALTER TABLE Atrybut ADD PokazujNazweAtrybutuJakoNaglowekFiltra BIT NOT NULL DEFAULT 1
END;
