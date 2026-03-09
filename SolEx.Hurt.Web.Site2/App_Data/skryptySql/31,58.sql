
if exists ( SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID('FK_ZamowieniaDokumenty-HistoriaDokumentu') AND parent_object_id = OBJECT_ID('ZamowienieDokumenty')) ALTER TABLE ZamowienieDokumenty DROP CONSTRAINT [FK_ZamowieniaDokumenty-HistoriaDokumentu];

ALTER TABLE [dbo].[ZamowienieDokumenty]  WITH CHECK ADD  CONSTRAINT [FK_ZamowieniaDokumenty_Zamowienie] FOREIGN KEY([IdZamowienia])
REFERENCES [dbo].[Zamowienie] ([Id])
ON DELETE CASCADE