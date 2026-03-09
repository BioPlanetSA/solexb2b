
IF EXISTS(SELECT * FROM sys.views WHERE name = 'vKupowaneIlosci') DROP VIEW dbo.vKupowaneIlosci


EXEC('CREATE VIEW vKupowaneIlosci
AS
 SELECT        hp.ProduktId, 
SUM(CASE WHEN pj.Podstawowa = 1 THEN hp.Ilosc ELSE hp.Ilosc * pj.PrzelicznikIlosc END) AS Ilosc,
 hd.KlientId, 
 hd.DataUtworzenia AS DataZakupu,
  hd.Rodzaj AS RodzajDokumentu
FROM            dbo.HistoriaDokumentuProdukt AS hp INNER JOIN
                         dbo.HistoriaDokumentu AS hd ON hd.Id = hp.DokumentId INNER JOIN
                         dbo.ProduktJednostka AS pj ON pj.JednostkaId = hp.JednostkaMiary
WHERE        (pj.ProduktId = CASE WHEN hp.ProduktIdBazowy IS NOT NULL THEN hp.ProduktIdBazowy ELSE hp.ProduktId END)

GROUP BY hp.ProduktId,hd.KlientId,hd.DataUtworzenia, hd.Rodzaj
')
