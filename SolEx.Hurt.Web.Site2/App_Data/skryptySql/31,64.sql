
update TrescWiersz  set ObrazekTla = null where ObrazekTla not in (select id from Plik);

if exists ( SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID('FK_TrescWiersz_ObrazekTlo') AND parent_object_id = OBJECT_ID('TrescWiersz')) 
	ALTER TABLE [TrescWiersz] DROP CONSTRAINT [FK_TrescWiersz_ObrazekTlo];





ALTER TABLE [dbo].[TrescWiersz]  WITH CHECK ADD  CONSTRAINT [FK_TrescWiersz_ObrazekTlo] FOREIGN KEY([ObrazekTla])
REFERENCES [dbo].[Plik] ([Id])
ON DELETE SET NULL;

ALTER TABLE [dbo].[TrescWiersz] CHECK CONSTRAINT [FK_TrescWiersz_ObrazekTlo];


