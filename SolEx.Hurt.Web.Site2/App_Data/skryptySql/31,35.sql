DECLARE @tblName VARCHAR(100)  --zmienna do nazwy tabeli
DECLARE @newColname VARCHAR(100)  --zmienna do nazwy kolumny 

DECLARE @cdcInst VARCHAR(100)

SET @tblName = 'Cecha'
SET @newColname = 'CssKlasy'

if exists (select column_name from INFORMATION_SCHEMA.columns where table_name = @tblName and column_name = 'Kolor') 
begin

	SET @cdcInst = 'dbo_' + @tblName

	---------wyłaczenia monitirowania tabeli
	--exec sys.sp_cdc_disable_table @source_schema='dbo', @source_name= @tblName, @capture_instance= @cdcInst

	
	----- Zmiana nazwy kolumny
	EXEC sp_rename 'Cecha.Kolor', @newColname;
  
	-----------włączenie monitorowania tabeli
	--EXEC sys.sp_cdc_enable_table @source_schema = 'dbo',@source_name = @tblName,@role_name = NULL, @supports_net_changes = 1

end

delete from Atrybut where symbol = 'automatyczne';
delete from Cecha where symbol like '%automatyczne:';
UPDATE [TrescKolumna] SET RodzajKontrolki = 'SolEx.Hurt.Web.Site2.Models.KontrolkiTresci.Koszyk.ImportPozycjiKoszyka,SolEx.Hurt.Web.Site2' 
WHERE RodzajKontrolki like 'SolEx.Hurt.Web.Site2.Models.KontrolkiTresci.ImportPozycjiKoszyka,SolEx.Hurt.Web.Site2';

UPDATE [TrescKolumna] SET RodzajKontrolki = 'SolEx.Hurt.Web.Site2.Models.KontrolkiTresci.Koszyk.KoszykPodglad,SolEx.Hurt.Web.Site2' 
  WHERE RodzajKontrolki like 'SolEx.Hurt.Web.Site2.Models.KontrolkiTresci.KoszykPodglad,SolEx.Hurt.Web.Site2';

UPDATE [TrescKolumna] SET RodzajKontrolki = 'SolEx.Hurt.Web.Site2.Models.KontrolkiTresci.Koszyk.KoszykCalosc,SolEx.Hurt.Web.Site2' 
  WHERE RodzajKontrolki like 'SolEx.Hurt.Web.Site2.Models.KontrolkiTresci.KoszykCalosc,SolEx.Hurt.Web.Site2';