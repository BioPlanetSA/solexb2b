-------------------------------------------------
-- STWORZYŁ: BARTEK
-- POWÓD: klucze dla tabelki cen !!
--------------------------------------------------


IF OBJECT_ID('dbo.FK_CenaPoziomu_PoziomCenowy', 'F') IS  NULL
BEGIN
   ALTER TABLE [dbo].[CenaPoziomu]  WITH CHECK ADD  CONSTRAINT [FK_CenaPoziomu_PoziomCenowy] FOREIGN KEY([PoziomId])
	REFERENCES [dbo].[PoziomCenowy] ([Id])
	ON DELETE CASCADE;

	ALTER TABLE [dbo].[CenaPoziomu] CHECK CONSTRAINT [FK_CenaPoziomu_PoziomCenowy];
END


IF OBJECT_ID('dbo.FK_CenaPoziomu_Produkt', 'F') IS  NULL
BEGIN
ALTER TABLE [dbo].[CenaPoziomu]  WITH CHECK ADD  CONSTRAINT [FK_CenaPoziomu_Produkt] FOREIGN KEY([ProduktId])
REFERENCES [dbo].[Produkt] ([Id])
ON DELETE CASCADE;


ALTER TABLE [dbo].[CenaPoziomu] CHECK CONSTRAINT [FK_CenaPoziomu_Produkt];
END


IF OBJECT_ID('dbo.FK_CenaPoziomu_Waluta', 'F') IS  NULL
BEGIN
ALTER TABLE [dbo].[CenaPoziomu]  WITH CHECK ADD  CONSTRAINT [FK_CenaPoziomu_Waluta] FOREIGN KEY([WalutaId])
REFERENCES [dbo].[Waluta] ([Id])
ON DELETE CASCADE

ALTER TABLE [dbo].[CenaPoziomu] CHECK CONSTRAINT [FK_CenaPoziomu_Waluta]
END

