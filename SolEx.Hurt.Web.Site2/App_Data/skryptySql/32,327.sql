ALTER  view [dbo].[vKategorieWieloWybieralne]
AS
WITH item AS ( 
select v.produktID, v.kid as kid, parentID  from vProduktyKategorieParenty as v 
-- poniższy warunek pwoduje szukanie od lisci w gore - jest bardziej optymalny, ale niektorzy klienci maja rozne produkty w roznych lisciach i dlatego go komentuje. Teraz bedzie wszystko z wszystkim porownane
--where v.kid in ( select id from KategoriaProduktu where id not in (select distinct parentID from KategoriaProduktu where parentID is not null) and parentID is not null )
--zaczynamy od tylko dzieci
UNION ALL
	select i.produktID, i.parentID, v.parentID  from vProduktyKategorieParenty as v INNER JOIN item i on v.kid = i.parentID
)

-- nie zwracamy tych powiazan ktore juz sa w tabeli ProduktKategoria 
select  distinct produktID as ProduktID, kid as  KategoriaID  from item i WHERE produktID is not null AND not exists (select * from ProduktKategoria pk where pk.ProduktId = i.produktID AND pk.KategoriaId = i.kid)
