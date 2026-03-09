

create view vProduktyKategorieParenty
as
select pk.produktID, v.id as kid, v.path, v.level, v.parentID from ProduktKategoria pk RIGHT JOIN vKategorieDrzewo v on pk.KategoriaId = v.id  



