--usuwam produkty które nie mają dostępnej jednostki
DELETE [HistoriaDokumentuProdukt] WHERE JednostkaMiary NOT IN (SELECT Id From Jednostka)
--usuwam klucze obce
ALTER TABLE [dbo].[ProduktJednostka] DROP CONSTRAINT [FK_ProduktyJednostki_Jednostki]
ALTER TABLE [dbo].[KoszykPozycje] DROP CONSTRAINT [FK_koszyki_pozycje_Jednostki]
--wyłączam cdc 
--EXEC sys.sp_cdc_disable_table @source_schema='dbo',	@source_name= Jednostka, @capture_instance= dbo_Jednostka
--usuwam klucz główny z jednostki
ALTER TABLE [dbo].[Jednostka] DROP CONSTRAINT PK_Jednostki
-- zmieniam typy kolumn na bigint
ALTER TABLE [dbo].[ProduktJednostka]  ALTER COLUMN  [JednostkaId] bigint not null
ALTER TABLE [dbo].[KoszykPozycje]  ALTER COLUMN  [JednostkaId] bigint not null
ALTER TABLE [dbo].[HistoriaDokumentuProdukt]  ALTER COLUMN  [JednostkaMiary] bigint not null
ALTER TABLE [dbo].[Jednostka]  ALTER COLUMN  [Id] bigint not null
--dodaje klucz główny
ALTER TABLE [dbo].[Jednostka] ADD CONSTRAINT PK_Jednostki PRIMARY KEY ([Id])
--właczam cdc
--EXEC sys.sp_cdc_enable_table @source_schema = 'dbo',@source_name = Jednostka,@role_name = NULL, @supports_net_changes = 1
--dodaje klucze obce
ALTER TABLE [dbo].[ProduktJednostka]  WITH NOCHECK ADD  CONSTRAINT [FK_ProduktyJednostki_Jednostki] FOREIGN KEY([JednostkaId])
REFERENCES [dbo].[Jednostka] ([Id])
ON DELETE CASCADE
ALTER TABLE [dbo].[ProduktJednostka] CHECK CONSTRAINT [FK_ProduktyJednostki_Jednostki]

ALTER TABLE [dbo].[KoszykPozycje]  WITH CHECK ADD  CONSTRAINT [FK_koszyki_pozycje_Jednostki] FOREIGN KEY([JednostkaId])
REFERENCES [dbo].[Jednostka] ([Id])
ON DELETE CASCADE
ALTER TABLE [dbo].[KoszykPozycje] CHECK CONSTRAINT [FK_koszyki_pozycje_Jednostki]

ALTER TABLE [dbo].[HistoriaDokumentuProdukt] WITH NOCHECK ADD  CONSTRAINT [FK_HistoriaDokumentuProdukt_Jednostka_JednostkaMiary] FOREIGN KEY(JednostkaMiary) 
REFERENCES [dbo].[Jednostka] ([Id])
ON DELETE CASCADE
ALTER TABLE [dbo].[HistoriaDokumentuProdukt] CHECK CONSTRAINT [FK_HistoriaDokumentuProdukt_Jednostka_JednostkaMiary]