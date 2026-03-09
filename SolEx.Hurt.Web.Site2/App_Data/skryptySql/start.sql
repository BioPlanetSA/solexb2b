-- sprawdzamy które tabele nie mają właczonego CDC i właczamy go dla nich

-- wyłaczenia mie możemy zrobić tutak bo robi to za duzo problemów dla starszych wersji skryptów
-- takie wyłaczenie tutj to chamskie rozwiązanie robiące mnóstwo problemów 
-- wyłaczenie zrobie w kodzie żeby skrypty które jerszcze z tego korzystają mogły się poprawnie wywołać 

-- BARTEK - calkowite wylaczeni CDC do czasu az zaczenie dzialac lepiej
--EXEC sys.sp_cdc_disable_db ;


DECLARE @source_schema sysname, @source_name sysname
SET @source_schema = N'dbo'
DECLARE #hinstance CURSOR LOCAL fast_forward
FOR  SELECT name  FROM [sys].[tables]  
WHERE SCHEMA_NAME(schema_id) = @source_schema  AND is_ms_shipped = 0  AND is_tracked_by_cdc=0  
AND name not in ('LogWpis', 'HistoriaWiadomosci','DzialaniaUzytkownikow','DzialaniaUzytkwonikowParametry','MaileBledneDoPonownejWysylki','Sesja', 'HistoriaDokumentu', 'HistoriaDokumentuListPrzewozowy', 'HistoriaDokumentuPlatnosciOnline', 'HistoriaDokumentuProdukt' )
OPEN #hinstance
FETCH #hinstance INTO @source_name
 
WHILE (@@fetch_status <> -1)
	BEGIN
	 EXEC sys.sp_cdc_enable_table @source_schema ,@source_name ,@role_name = NULL ,@supports_net_changes = 1
	 FETCH #hinstance INTO @source_name
	END
 
CLOSE #hinstance
DEALLOCATE #hinstance


--  wylaczanie CDC dla okreslonych tabel o ile sa wlaczone
DECLARE @cdcInst VARCHAR(100);

SET @source_schema = N'dbo'
DECLARE #hinstance CURSOR LOCAL fast_forward
FOR  SELECT name  FROM [sys].[tables]  
WHERE SCHEMA_NAME(schema_id) = @source_schema  AND is_ms_shipped = 0  AND is_tracked_by_cdc=1  
AND name in ('LogWpis', 'HistoriaWiadomosci','DzialaniaUzytkownikow','DzialaniaUzytkwonikowParametry','MaileBledneDoPonownejWysylki','Sesja', 'HistoriaDokumentu', 'HistoriaDokumentuListPrzewozowy', 'HistoriaDokumentuPlatnosciOnline', 'HistoriaDokumentuProdukt' )
OPEN #hinstance
FETCH #hinstance INTO @source_name
 
WHILE (@@fetch_status <> -1)
	BEGIN
	 SET @cdcInst = 'dbo_' + @source_name;
	 EXEC sys.sp_cdc_disable_table @source_schema ,@source_name , @capture_instance= @cdcInst
	 FETCH #hinstance INTO @source_name
	END
 
CLOSE #hinstance
DEALLOCATE #hinstance


--exec sys.sp_cdc_disable_table @source_schema='dbo',	@source_name= 'LogWpis', @capture_instance= 'dbo_LogWpis'
--exec sys.sp_cdc_disable_table @source_schema='dbo',	@source_name= 'HistoriaWiadomosci', @capture_instance= 'dbo_HistoriaWiadomosci'
--exec sys.sp_cdc_disable_table @source_schema='dbo',	@source_name= 'DzialaniaUzytkownikow', @capture_instance= 'dbo_DzialaniaUzytkownikow'

--DECLARE @cdcInst VARCHAR(100)
--SET @cdcInst = 'dbo_' + @source_name
--exec sys.sp_cdc_disable_table @source_schema='dbo',	@source_name= @source_name, @capture_instance= @cdcInst