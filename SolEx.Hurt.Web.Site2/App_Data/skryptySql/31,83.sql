/* ---------------------------------------------------------------- */
--wersja Sylwestra Z KLUCZAMI obcymi
/* ---------------------------------------------------------------- */

DECLARE @tblName VARCHAR(100)  --zmienna do nazwy tabeli
DECLARE @colname VARCHAR(100)  --zmienna do nazwy kolumny 
DECLARE @dbType VARCHAR(100)	-- znienna do typu kolumny 
DECLARE @cdcInst VARCHAR(100)	-- nic nie wpisujemy tu

DECLARE @tblNamePowiazanie VARCHAR(100)  --zmienna do nazwy tabeli do powiązania

SET @tblName = 'MaileBledneDoPonownejWysylki'
SET @colname = 'DoKogoPierwotnieJesliPrzechwycony'
SET @dbType =  'varchar(500) NULL'  --jesli ma byc wartość domyslna do dopisujemy:  DEFAULT ((  tu wartosc domyslna  ))

if NOT exists (select column_name from INFORMATION_SCHEMA.columns where table_name = @tblName and column_name = @colname) 
begin
	------------------------------dodawanie kolumny do tabeli
	EXEC('ALTER TABLE dbo.'+ @tblName +' ADD '+ @colname + ' '+ @dbType  )  
end



SET @tblName = 'HistoriaWiadomosci'
SET @colname = 'DoKogoPierwotnieJesliPrzechwycony'
SET @dbType =  'varchar(500) NULL'  --jesli ma byc wartość domyslna do dopisujemy:  DEFAULT ((  tu wartosc domyslna  ))

if NOT exists (select column_name from INFORMATION_SCHEMA.columns where table_name = @tblName and column_name = @colname) 
begin
	EXEC('ALTER TABLE dbo.'+ @tblName +' ADD '+ @colname + ' '+ @dbType  )  
end
