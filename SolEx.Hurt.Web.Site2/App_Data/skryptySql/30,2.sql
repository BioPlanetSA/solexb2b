--IF NOT EXISTS (SELECT * FROM sys.objects WHERE type = 'TR' AND name = 'usuwanie_zadan_podrzednych')
--exec('CREATE TRIGGER [usuwanie_zadan_podrzednych] ON  [Zadanie] INSTEAD OF DELETE AS BEGIN DELETE from Zadanie where ZadanieNadrzedne in (select id from deleted); DELETE Zadanie FROM DELETED D INNER JOIN Zadanie T ON T.id = D.id END')

--DECLARE @source_schema sysname, @source_name sysname
--SET @source_schema = N'dbo'
--DECLARE #hinstance CURSOR LOCAL fast_forward
--FOR  SELECT name  FROM [sys].[tables]  WHERE SCHEMA_NAME(schema_id) = @source_schema  AND is_ms_shipped = 0  AND name not in ('LogWpis', 'HistoriaWiadomosci','DzialaniaUzytkownikow','DzialaniaUzytkwonikowParametry','MaileBledneDoPonownejWysylki','Sesja', 'HistoriaDokumentu', 'HistoriaDokumentuListPrzewozowy', 'HistoriaDokumentuPlatnosciOnline', 'HistoriaDokumentuProdukt' )  
--OPEN #hinstance
--FETCH #hinstance INTO @source_name
 
--WHILE (@@fetch_status <> -1)
--	BEGIN
--	 EXEC [sys].[sp_cdc_enable_table]
--	  @source_schema
--	  ,@source_name
--	  ,@role_name = NULL
--	  ,@supports_net_changes = 1
   
--	 FETCH #hinstance INTO @source_name
--	END
 
--CLOSE #hinstance
--DEALLOCATE #hinstance

--exec sys.sp_cdc_disable_table @source_schema='dbo',	@source_name= 'LogWpis', @capture_instance= 'dbo_LogWpis'
--exec sys.sp_cdc_disable_table @source_schema='dbo',	@source_name= 'HistoriaWiadomosci', @capture_instance= 'dbo_HistoriaWiadomosci'