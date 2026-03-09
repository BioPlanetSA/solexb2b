/* ---------------------------------------------------------------- */
-- wersja Sylwestra BEZ kliczy obcych - samo dodawanie kolumny
/* ---------------------------------------------------------------- */

DECLARE @tblName VARCHAR(100)  --zmienna do nazwy tabeli
DECLARE @tblName2 VARCHAR(100)  
DECLARE @colname VARCHAR(100)  --zmienna do nazwy kolumny 
DECLARE @dbType VARCHAR(100)	-- znienna do typu kolumny 
DECLARE @cdcInst VARCHAR(100)
DECLARE @cdcInst2 VARCHAR(100)

SET @tblName = 'SposobPokazywaniaStanow'
SET @tblName2 = 'SposobPokazywaniaStanowRegula'
SET @cdcInst = 'dbo_' + @tblName
SET @cdcInst2 = 'dbo_' + @tblName2



--exec sys.sp_cdc_disable_table @source_schema='dbo', @source_name= @tblName, @capture_instance= @cdcInst
--exec sys.sp_cdc_disable_table @source_schema='dbo',	@source_name= @tblName2, @capture_instance= @cdcInst2

if exists ( SELECT * FROM sys.default_constraints WHERE object_id = OBJECT_ID('DF_sposoby_pokazywania_stanow_Widocznosc') AND parent_object_id = OBJECT_ID('SposobPokazywaniaStanow')) ALTER TABLE SposobPokazywaniaStanow DROP CONSTRAINT DF_sposoby_pokazywania_stanow_Widocznosc;
if exists ( SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID('FK_sposoby_pokazywania_stanow_reguly_sposoby_pokazywania_stanow') AND parent_object_id = OBJECT_ID('SposobPokazywaniaStanowRegula')) ALTER TABLE SposobPokazywaniaStanowRegula DROP CONSTRAINT FK_sposoby_pokazywania_stanow_reguly_sposoby_pokazywania_stanow;



CREATE TABLE [dbo].[tmp_ms_xx_SposobPokazywaniaStanow] (
    [Id]                      BIGINT         IDENTITY (1, 1) NOT NULL,
    [Nazwa]                   VARCHAR (100)  NOT NULL,
    [DomyslnyMagazynId]       INT            NULL,
    [Dostep]                  VARCHAR (50)   CONSTRAINT [DF_sposoby_pokazywania_stanow_Widocznosc] DEFAULT ('Wszyscy') NOT NULL,
    [PozycjaLista]            VARCHAR (7000) NULL,
    [PozycjaKarta]            VARCHAR (7000) NULL,
    [KategoriaKlientaMagazyn] VARCHAR (50)   NULL,
    [DozwolonaRolaKlienta]    VARCHAR (7000) NULL,
    CONSTRAINT [tmp_ms_xx_constraint_PK_sposoby_pokazywania_stanow1] PRIMARY KEY CLUSTERED ([Id] ASC)
);

IF EXISTS (SELECT TOP 1 1 
           FROM   [dbo].[SposobPokazywaniaStanow])
    BEGIN
        SET IDENTITY_INSERT [dbo].[tmp_ms_xx_SposobPokazywaniaStanow] ON;
        INSERT INTO [dbo].[tmp_ms_xx_SposobPokazywaniaStanow] ([Id], [Nazwa], [DomyslnyMagazynId], [Dostep], [PozycjaLista], [PozycjaKarta], [KategoriaKlientaMagazyn], [DozwolonaRolaKlienta])
        SELECT   [Id],
                 [Nazwa],
                 [DomyslnyMagazynId],
                 [Dostep],
                 [PozycjaLista],
                 [PozycjaKarta],
                 [KategoriaKlientaMagazyn],
                 [DozwolonaRolaKlienta]
        FROM     [dbo].[SposobPokazywaniaStanow]
        ORDER BY [Id] ASC;
        SET IDENTITY_INSERT [dbo].[tmp_ms_xx_SposobPokazywaniaStanow] OFF;
    END

DROP TABLE [dbo].[SposobPokazywaniaStanow];

EXECUTE sp_rename N'[dbo].[tmp_ms_xx_SposobPokazywaniaStanow]', N'SposobPokazywaniaStanow';
EXECUTE sp_rename N'[dbo].[tmp_ms_xx_constraint_PK_sposoby_pokazywania_stanow1]', N'PK_sposoby_pokazywania_stanow', N'OBJECT';

ALTER TABLE [dbo].[SposobPokazywaniaStanowRegula] ALTER COLUMN [SposobId] BIGINT NOT NULL;
ALTER TABLE [dbo].[SposobPokazywaniaStanowRegula] WITH NOCHECK ADD CONSTRAINT [FK_sposoby_pokazywania_stanow_reguly_sposoby_pokazywania_stanow] FOREIGN KEY ([SposobId]) REFERENCES [dbo].[SposobPokazywaniaStanow] ([Id]) ON DELETE CASCADE ON UPDATE CASCADE;

ALTER TABLE [dbo].[SposobPokazywaniaStanowRegula] WITH CHECK CHECK CONSTRAINT [FK_sposoby_pokazywania_stanow_reguly_sposoby_pokazywania_stanow];


--EXEC sys.sp_cdc_enable_table @source_schema = 'dbo',@source_name = @tblName,@role_name = NULL, @supports_net_changes = 1
--EXEC sys.sp_cdc_enable_table @source_schema = 'dbo',@source_name = @tblName2,@role_name = NULL, @supports_net_changes = 1

