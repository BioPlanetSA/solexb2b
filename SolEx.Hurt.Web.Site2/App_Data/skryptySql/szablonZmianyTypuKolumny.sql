--wersja Sylwestra po oczyszczeniu

DECLARE @tblName VARCHAR(100)  --zmienna do nazwy tabeli
DECLARE @colname VARCHAR(100)  --zmienna do nazwy kolumny 
DECLARE @dbType VARCHAR(100)	-- znienna do typu kolumny 
DECLARE @cdcInst VARCHAR(100)


SET @tblName = 'NazwaTabeli'
SET @colname = 'NazwaKolumny'
SET @dbType = 'varchar(50) NULL' --PAMIĘTAJ DODAĆ INFORMACJHE CZY MA BYĆ NULL LUB NOT NULL

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

--zmiana typu klucz głównego !!!!!!!!!!!!!!!!!!!!!!!!!!!

--usun WSZYTKIE klucze obce poniżej jest jak usuną jeden klucz 
ALTER TABLE [dbo].[ProduktJednostka] DROP CONSTRAINT [FK_ProduktyJednostki_Jednostki]
--wyłączam cdc 
EXEC sys.sp_cdc_disable_table @source_schema='dbo',	@source_name= Jednostka, @capture_instance= dbo_Jednostka
--usuwam klucz główny z jednostki
ALTER TABLE [dbo].[Jednostka] DROP CONSTRAINT PK_Jednostki

-- zmieniam typy kolumn na bigint
ALTER TABLE [dbo].[Jednostka]  ALTER COLUMN  [Id] bigint not null --WAŻNE ŻEBY DODAć NOT NULL
-- zmieniamy typy w innych powiązanych kolumnach 
ALTER TABLE [dbo].[ProduktJednostka]  ALTER COLUMN  [JednostkaId] bigint not null
--dodaje klucz główny
ALTER TABLE [dbo].[Jednostka] ADD CONSTRAINT PK_Jednostki PRIMARY KEY ([Id])
--właczam cdc
EXEC sys.sp_cdc_enable_table @source_schema = 'dbo',@source_name = Jednostka,@role_name = NULL, @supports_net_changes = 1
--dodaje klucze obce
ALTER TABLE [dbo].[ProduktJednostka]  WITH NOCHECK ADD  CONSTRAINT [FK_ProduktyJednostki_Jednostki] FOREIGN KEY([JednostkaId]) REFERENCES [dbo].[Jednostka] ([Id])
ON DELETE CASCADE
ALTER TABLE [dbo].[ProduktJednostka] CHECK CONSTRAINT [FK_ProduktyJednostki_Jednostki]


--jesli chcemy zmieniac klucz glowny to najlepiej zrobic drop tabeli i reCreate

IF (EXISTS (SELECT *  FROM INFORMATION_SCHEMA.TABLES 
                 WHERE TABLE_SCHEMA = 'dbo' 
                 AND  TABLE_NAME = 'WidocznosciTypow'))
BEGIN
   DROP TABLE [dbo].[WidocznosciTypow];

   CREATE TABLE [dbo].[WidocznosciTypow](
	[Id] [bigint] NOT NULL,
	[ObiektId] [bigint] NULL,
	[Typ] [varchar](200) NULL,
	[KategoriaKlientaIdWszystkie] [varchar](400) NOT NULL,
	[Kierunek] [varchar](50) NOT NULL,
	[KategoriaKlientaIdKtorakolwiek] [varchar](400) NOT NULL,
	[Nazwa] [varchar](500) NULL,
 CONSTRAINT [PK_WidocznosciTypow] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY];

END






