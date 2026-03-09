DECLARE @tblName VARCHAR(100)  --zmienna do nazwy tabeli
DECLARE @colname VARCHAR(100)  --zmienna do nazwy kolumny 
DECLARE @dbType VARCHAR(100)	-- znienna do typu kolumny 
DECLARE @cdcInst VARCHAR(100)	-- nic nie wpisujemy tu

IF (EXISTS (SELECT *  FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA = 'dbo' AND  TABLE_NAME = 'Konfekcje'))
BEGIN
--exec sys.sp_cdc_disable_table @source_schema='dbo',	@source_name= 'Konfekcje', @capture_instance= 'dbo_Konfekcje'
   DROP TABLE [dbo].Konfekcje   

   CREATE TABLE [dbo].[Konfekcje](
	[Id] [bigint] NOT NULL,
	[KlientId] [bigint] NULL,
	[KategoriaKlientowId] [int] NULL,
	[ProduktId] [bigint] NULL,
	[Ilosc] [decimal](10, 2) NOT NULL,
	[Rabat] [decimal](10, 2) NULL,
	[RabatKwota] [decimal](10, 2) NULL,
	[WalutaId] [bigint] NULL,
	[CechaId] [bigint] NULL,
	[WyliczonePrzezModul] [int] NULL,
 CONSTRAINT [PK_Konfekcje] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

ALTER TABLE [dbo].[Konfekcje]  WITH CHECK ADD  CONSTRAINT [FK_Konfekcje_cechy] FOREIGN KEY([CechaId]) REFERENCES [dbo].[Cecha] ([Id]) ON DELETE CASCADE

ALTER TABLE [dbo].[Konfekcje] CHECK CONSTRAINT [FK_Konfekcje_cechy]

ALTER TABLE [dbo].[Konfekcje]  WITH NOCHECK ADD  CONSTRAINT [FK_Konfekcje_kategorie_klientow] FOREIGN KEY([KategoriaKlientowId]) REFERENCES [dbo].[KategoriaKlienta] ([Id]) ON DELETE CASCADE

ALTER TABLE [dbo].[Konfekcje] CHECK CONSTRAINT [FK_Konfekcje_kategorie_klientow]

ALTER TABLE [dbo].[Konfekcje]  WITH NOCHECK ADD  CONSTRAINT [FK_Konfekcje_produkty] FOREIGN KEY([ProduktId]) REFERENCES [dbo].[Produkt] ([Id]) ON DELETE CASCADE

ALTER TABLE [dbo].[Konfekcje] CHECK CONSTRAINT [FK_Konfekcje_produkty]

ALTER TABLE [dbo].[Konfekcje]  WITH NOCHECK ADD  CONSTRAINT [CK_Konfekcje] CHECK  (([Rabat] IS NOT NULL OR [RabatKwota] IS NOT NULL))

ALTER TABLE [dbo].[Konfekcje] CHECK CONSTRAINT [CK_Konfekcje]

ALTER TABLE [dbo].[Konfekcje]  WITH NOCHECK ADD  CONSTRAINT [CK_Konfekcje_1] CHECK  (([Rabat] IS NOT NULL AND [RabatKwota] IS NULL OR [Rabat] IS NULL AND [RabatKwota] IS NOT NULL))

ALTER TABLE [dbo].[Konfekcje] CHECK CONSTRAINT [CK_Konfekcje_1]

	--EXEC sys.sp_cdc_enable_table @source_schema = 'dbo',@source_name = 'Konfekcje',@role_name = NULL, @supports_net_changes = 1
END