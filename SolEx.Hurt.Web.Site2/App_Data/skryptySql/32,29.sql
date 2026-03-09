IF EXISTS ( SELECT  * FROM sys.objects WHERE   type = 'TR' AND name = 'usuwanie_dokumentow_zdezaktywowani_klienci' ) 

DROP TRIGGER usuwanie_dokumentow_zdezaktywowani_klienci;
BEGIN
    EXEC ('CREATE TRIGGER usuwanie_dokumentow_zdezaktywowani_klienci 
	   ON  Klient 
	   AFTER UPDATE
	AS 
	BEGIN
		DECLARE @Deleted_Rows INT;
		SET @Deleted_Rows = 1;
		WHILE (@Deleted_Rows > 0)
		BEGIN
			DELETE TOP (100) from HistoriaDokumentu where KlientId in (select Id from inserted where Aktywny=0)	
			 SET @Deleted_Rows = @@ROWCOUNT;
		END
			
	END');
END;
