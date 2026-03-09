--wersja Sylwestra po oczyszczeniu

DECLARE @tblName VARCHAR(100)  --zmienna do nazwy tabeli
DECLARE @colname VARCHAR(100)  --zmienna do nazwy kolumny
DECLARE @colname2 VARCHAR(100) 
DECLARE @dbType VARCHAR(100)	-- znienna do typu kolumny 
DECLARE @cdcInst VARCHAR(100)


SET @tblName = 'PunktyWpisy'
SET @colname = 'KlientId'
SET @dbType = 'bigint NOT NULL'

if exists (select column_name from INFORMATION_SCHEMA.columns where table_name = @tblName and column_name = @colname) 
begin

	SET @cdcInst = 'dbo_' + @tblName

	---------wyłaczenia monitirowania tabeli
	--exec sys.sp_cdc_disable_table @source_schema='dbo', @source_name= @tblName, @capture_instance= @cdcInst

	------------------------------Zmiana kolumny w tabeli
	EXEC('ALTER TABLE dbo.'+ @tblName +'  ALTER COLUMN  '+ @colname + ' '+ @dbType  )  
	-----------włączenie monitorowania tabeli
	--EXEC sys.sp_cdc_enable_table @source_schema = 'dbo',@source_name = @tblName,@role_name = NULL, @supports_net_changes = 1

end


IF EXISTS(SELECT * FROM sys.objects WHERE object_id = OBJECT_ID('IX_ustawienia_symbol_pracownik') AND parent_object_id = OBJECT_ID('Ustawienie'))
    BEGIN
		ALTER TABLE Ustawienie DROP CONSTRAINT IX_ustawienia_symbol_pracownik;
    END

SET @tblName = 'Ustawienie'
SET @colname = 'OddzialId'
SET @dbType = 'bigint NULL'

if exists (select column_name from INFORMATION_SCHEMA.columns where table_name = @tblName and column_name = @colname) 
begin

	SET @cdcInst = 'dbo_' + @tblName

	---------wyłaczenia monitirowania tabeli
	--exec sys.sp_cdc_disable_table @source_schema='dbo', @source_name= @tblName, @capture_instance= @cdcInst

	------------------------------Zmiana kolumny w tabeli
	EXEC('ALTER TABLE dbo.'+ @tblName +'  ALTER COLUMN  '+ @colname + ' '+ @dbType  )  

	Exec('ALTER TABLE [dbo].[Ustawienie] ADD  CONSTRAINT [IX_ustawienia_symbol_pracownik] UNIQUE NONCLUSTERED (
	[Symbol] ASC,
	[OddzialId] ASC
		)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)')
	-----------włączenie monitorowania tabeli
	--EXEC sys.sp_cdc_enable_table @source_schema = 'dbo',@source_name = @tblName,@role_name = NULL, @supports_net_changes = 1

end



SET @tblName = 'HistoriaDokumentu'
SET @colname = 'OdbiorcaId'
SET @dbType = 'bigint NULL'

if exists (select column_name from INFORMATION_SCHEMA.columns where table_name = @tblName and column_name = @colname) 
begin

	SET @cdcInst = 'dbo_' + @tblName

	---------wyłaczenia monitirowania tabeli
	--exec sys.sp_cdc_disable_table @source_schema='dbo', @source_name= @tblName, @capture_instance= @cdcInst

	------------------------------Zmiana kolumny w tabeli
	EXEC('ALTER TABLE dbo.'+ @tblName +'  ALTER COLUMN  '+ @colname + ' '+ @dbType  )  
	-----------włączenie monitorowania tabeli
	--EXEC sys.sp_cdc_enable_table @source_schema = 'dbo',@source_name = @tblName,@role_name = NULL, @supports_net_changes = 1

end

SET @tblName = 'Zamowienie'
SET @colname = 'IdOddzialu'
SET @colname2 = 'AdresId'
SET @dbType = 'bigint NULL'

if exists (select column_name from INFORMATION_SCHEMA.columns where table_name = @tblName and column_name = @colname) 
begin

	SET @cdcInst = 'dbo_' + @tblName

	---------wyłaczenia monitirowania tabeli
	--exec sys.sp_cdc_disable_table @source_schema='dbo', @source_name= @tblName, @capture_instance= @cdcInst

	------------------------------Zmiana kolumny w tabeli
	EXEC('ALTER TABLE dbo.'+ @tblName +'  ALTER COLUMN  '+ @colname + ' '+ @dbType  )  

	EXEC('ALTER TABLE dbo.'+ @tblName +'  ALTER COLUMN  '+ @colname2 + ' '+ @dbType  )  

	-----------włączenie monitorowania tabeli
	--EXEC sys.sp_cdc_enable_table @source_schema = 'dbo',@source_name = @tblName,@role_name = NULL, @supports_net_changes = 1

end

IF (EXISTS (SELECT *  FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA = 'dbo' AND  TABLE_NAME = 'SklepKategoriaSklepu'))
BEGIN
   DROP TABLE [dbo].SklepKategoriaSklepu;

   CREATE TABLE [dbo].[SklepKategoriaSklepu](
	[Id] [bigint] NOT NULL,
	[SklepId] [bigint] NOT NULL,
	[KategoriaSklepuId] [bigint] NOT NULL,
	 CONSTRAINT [PK_sklepy_kategorie_polaczenia] PRIMARY KEY CLUSTERED 
	(
		[Id] ASC
	)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, FILLFACTOR = 80) ON [PRIMARY]
	) ON [PRIMARY]

	
	ALTER TABLE [dbo].[SklepKategoriaSklepu]  WITH CHECK ADD  CONSTRAINT [FK_sklepy_kategorie_polaczenia_sklepy_kategorie] FOREIGN KEY([KategoriaSklepuId])
	REFERENCES [dbo].[KategoriaSklepu] ([Id])
	ON DELETE CASCADE

	ALTER TABLE [dbo].[SklepKategoriaSklepu] CHECK CONSTRAINT [FK_sklepy_kategorie_polaczenia_sklepy_kategorie]

END





IF (EXISTS (SELECT *  FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA = 'dbo' AND  TABLE_NAME = 'Sklep'))
BEGIN

   DROP TABLE [dbo].Sklep;

   CREATE TABLE [dbo].[Sklep](
	[Id] [bigint] NOT NULL,
	[Nazwa] [varchar](500) NULL,
	[Aktywny] [bit] NOT NULL CONSTRAINT [DF_sklepy_aktywny]  DEFAULT ((1)),
	[DataUtworzenia] [datetime] NOT NULL,
	[Www] [varchar](800) NULL,
	[ObrazekId] [int] NULL,
	[Opis] [varchar](1000) NULL,
	[AutorId] [bigint] NULL,
	[AutomatyczneKoordynaty] [bit] NOT NULL CONSTRAINT [DF_sklepy_AutomatyczneKoordynaty]  DEFAULT ((1)),
	[KoordynatyZERP] [bit] NOT NULL CONSTRAINT [DF_sklepy_KoordynatyZERP]  DEFAULT ((0)),
	[Siedziba] [bit] NOT NULL CONSTRAINT [DF_sklepy_Siedziba]  DEFAULT ((0)),
	[AdresId] [bigint] NULL,
 CONSTRAINT [PK_sklepy] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]


ALTER TABLE [dbo].[SklepKategoriaSklepu]  WITH CHECK ADD  CONSTRAINT [FK_sklepy_kategorie_polaczenia_sklepy] FOREIGN KEY([SklepId])
	REFERENCES [dbo].[Sklep] ([Id])
	ON DELETE CASCADE

		ALTER TABLE [dbo].[SklepKategoriaSklepu] CHECK CONSTRAINT [FK_sklepy_kategorie_polaczenia_sklepy]

END



SET @tblName = 'Adres'
SET @colname = 'AutorId'
SET @dbType = 'bigint NULL'

if exists (select column_name from INFORMATION_SCHEMA.columns where table_name = @tblName and column_name = @colname) 
begin

	SET @cdcInst = 'dbo_' + @tblName

	---------wyłaczenia monitirowania tabeli
	--exec sys.sp_cdc_disable_table @source_schema='dbo', @source_name= @tblName, @capture_instance= @cdcInst

	------------------------------Zmiana kolumny w tabeli
	EXEC('ALTER TABLE dbo.'+ @tblName +'  ALTER COLUMN  '+ @colname + ' '+ @dbType  )  
	-----------włączenie monitorowania tabeli
	--EXEC sys.sp_cdc_enable_table @source_schema = 'dbo',@source_name = @tblName,@role_name = NULL, @supports_net_changes = 1

end


SET @tblName = 'MiejsceKosztow'
SET @colname = 'KlientId'
SET @dbType = 'bigint NOT NULL'

if exists (select column_name from INFORMATION_SCHEMA.columns where table_name = @tblName and column_name = @colname) 
begin

	SET @cdcInst = 'dbo_' + @tblName

	---------wyłaczenia monitirowania tabeli
	--exec sys.sp_cdc_disable_table @source_schema='dbo', @source_name= @tblName, @capture_instance= @cdcInst

	------------------------------Zmiana kolumny w tabeli
	EXEC('ALTER TABLE dbo.'+ @tblName +'  ALTER COLUMN  '+ @colname + ' '+ @dbType  )  
	-----------włączenie monitorowania tabeli
	--EXEC sys.sp_cdc_enable_table @source_schema = 'dbo',@source_name = @tblName,@role_name = NULL, @supports_net_changes = 1

end

SET @tblName = 'SubkontoGrupa'
SET @colname = 'KlientId'
SET @dbType = 'bigint NULL'

if exists (select column_name from INFORMATION_SCHEMA.columns where table_name = @tblName and column_name = @colname) 
begin

	SET @cdcInst = 'dbo_' + @tblName

	---------wyłaczenia monitirowania tabeli
	--exec sys.sp_cdc_disable_table @source_schema='dbo', @source_name= @tblName, @capture_instance= @cdcInst

	------------------------------Zmiana kolumny w tabeli
	EXEC('ALTER TABLE dbo.'+ @tblName +'  ALTER COLUMN  '+ @colname + ' '+ @dbType  )  
	-----------włączenie monitorowania tabeli
	--EXEC sys.sp_cdc_enable_table @source_schema = 'dbo',@source_name = @tblName,@role_name = NULL, @supports_net_changes = 1

end
SET @tblName = 'SzablonAkceptacji'
SET @colname = 'Tworca'
SET @dbType = 'bigint NOT NULL'

if exists (select column_name from INFORMATION_SCHEMA.columns where table_name = @tblName and column_name = @colname) 
begin

	SET @cdcInst = 'dbo_' + @tblName

	---------wyłaczenia monitirowania tabeli
	--exec sys.sp_cdc_disable_table @source_schema='dbo', @source_name= @tblName, @capture_instance= @cdcInst

	------------------------------Zmiana kolumny w tabeli
	EXEC('ALTER TABLE dbo.'+ @tblName +'  ALTER COLUMN  '+ @colname + ' '+ @dbType  )  
	-----------włączenie monitorowania tabeli
	--EXEC sys.sp_cdc_enable_table @source_schema = 'dbo',@source_name = @tblName,@role_name = NULL, @supports_net_changes = 1

end
SET @tblName = 'SzablonLimitow'
SET @colname = 'Tworca'
SET @dbType = 'bigint NOT NULL'

if exists (select column_name from INFORMATION_SCHEMA.columns where table_name = @tblName and column_name = @colname) 
begin

	SET @cdcInst = 'dbo_' + @tblName

	---------wyłaczenia monitirowania tabeli
	--exec sys.sp_cdc_disable_table @source_schema='dbo', @source_name= @tblName, @capture_instance= @cdcInst

	------------------------------Zmiana kolumny w tabeli
	EXEC('ALTER TABLE dbo.'+ @tblName +'  ALTER COLUMN  '+ @colname + ' '+ @dbType  )  
	-----------włączenie monitorowania tabeli
	--EXEC sys.sp_cdc_enable_table @source_schema = 'dbo',@source_name = @tblName,@role_name = NULL, @supports_net_changes = 1

end
