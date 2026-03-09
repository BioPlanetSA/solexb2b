-- zmian nazwy dla isniejących kolumn

IF EXISTS (
  SELECT * 
  FROM   sys.columns 
  WHERE  object_id = OBJECT_ID(N'[dbo].[TrescKolumna]') 
         AND name = 'DodatkoweKlasyCss'
)
BEGIN
	--exec sys.sp_cdc_disable_table @source_schema='dbo',	@source_name= 'TrescKolumna', @capture_instance='dbo_TrescKolumna'
	EXEC sp_rename 'TrescKolumna.DodatkoweKlasyCss', 'DodatkoweKlasyCssKolumny';
	--EXEC sys.sp_cdc_enable_table @source_schema = 'dbo',@source_name ='TrescKolumna',@role_name = NULL, @supports_net_changes = 1
END;


IF EXISTS (
  SELECT * 
  FROM   sys.columns 
  WHERE  object_id = OBJECT_ID(N'[dbo].[TrescKolumna]') 
         AND name = 'DodatkoweKlasyCssReczne'
)
BEGIN
	--exec sys.sp_cdc_disable_table @source_schema='dbo',	@source_name= 'TrescKolumna', @capture_instance= 'dbo_TrescKolumna'
	EXEC sp_rename 'TrescKolumna.DodatkoweKlasyCssReczne', 'DodatkoweKlasyCssReczneKolumny';
	--EXEC sys.sp_cdc_enable_table @source_schema = 'dbo',@source_name = 'TrescKolumna',@role_name = NULL, @supports_net_changes = 1
END;

