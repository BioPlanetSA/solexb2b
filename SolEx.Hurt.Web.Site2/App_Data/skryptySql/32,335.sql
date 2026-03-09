---------------
-- Stworzył Paweł
-- zwiększenie zakresu kolumn
---------------
DECLARE @tblName VARCHAR(100)
DECLARE @colname VARCHAR(100) 
DECLARE @dbType VARCHAR(100) 
DECLARE @cdcInst VARCHAR(100) 

SET @tblName = 'Cecha'
SET @colname = 'Symbol'
SET @dbType = 'varchar(2000)'

if exists (select column_name from INFORMATION_SCHEMA.columns where table_name = @tblName and column_name = @colname) 
begin

	SET @cdcInst = 'dbo_' + @tblName
	------------------------Zmiana kolumny w tabeli
	EXEC('ALTER TABLE dbo.'+ @tblName +'  ALTER COLUMN  '+ @colname + ' '+ @dbType  )  

end
