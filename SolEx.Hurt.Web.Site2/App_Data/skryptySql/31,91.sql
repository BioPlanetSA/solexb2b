
IF EXISTS(select * FROM sys.views where name = 'vKategorieDrzewo')  DROP VIEW vKategorieDrzewo;


IF EXISTS(select * FROM sys.views where name = 'vProduktyKategorieParenty')  DROP VIEW vProduktyKategorieParenty;


IF EXISTS(select * FROM sys.views where name = 'vKategorieWieloWybieralne')  DROP VIEW vKategorieWieloWybieralne;
