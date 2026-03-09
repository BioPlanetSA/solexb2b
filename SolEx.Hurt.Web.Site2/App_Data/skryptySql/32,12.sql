
ALTER view vKategorieWieloWybieralne
AS
WITH item AS ( 

select v.produktID, v.kid as kid, parentID  from vProduktyKategorieParenty as v where v.kid in ( select id from KategoriaProduktu where id not in (select distinct parentID from KategoriaProduktu where parentID is not null) and parentID is not null )

UNION ALL

select i.produktID, i.parentID, v.parentID  from vProduktyKategorieParenty as v INNER JOIN item i on v.kid = i.parentID

)

select   produktID as ProduktID, kid as  KategoriaID  from item i where not exists (select * from ProduktKategoria pk where pk.ProduktId = i.produktID AND pk.KategoriaId = i.kid) AND produktID is not null

