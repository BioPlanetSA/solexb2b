delete FROM HistoriaDokumentu where KlientId in (select Id from Klient where Aktywny=0);

IF NOT EXISTS ( SELECT  * FROM    sys.objects WHERE   type = 'TR' AND name = 'usuwanie_dokumentow_zdezaktywowani_klienci' ) 
BEGIN
    EXEC ('CREATE TRIGGER usuwanie_dokumentow_zdezaktywowani_klienci 
	   ON  Klient 
	   AFTER UPDATE
	AS 
	BEGIN
		SET NOCOUNT ON;
		delete from HistoriaDokumentu where KlientId in(
			select KlientId from inserted where Aktywny=0
		)
	END');
END;

