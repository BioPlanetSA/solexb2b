CREATE VIEW vRodzinyCechyUnikalne
AS
select p.rodzina, p.id as pid, c.Id as cid, c.AtrybutId 
--,(select count(*) from produkt pp LEFT JOIN ProduktCecha pc on pp.Id = pc.ProduktId where pp.Rodzina = p.Rodzina AND pc.CechaId = c.id) as iloscWystapien 
  from produkt p LEFT JOIN ProduktCecha pc on p.Id = pc.ProduktId LEFT JOIN Cecha c on c.Id = pc.CechaId LEFT JOIN Atrybut a on a.Id = c.AtrybutId
  where p.Rodzina IS NOT NULL AND p.Widoczny = 1 AND a.Widoczny = 1 AND c.Widoczna = 1  
AND (select count(*) from produkt pp LEFT JOIN ProduktCecha pc on pp.Id = pc.ProduktId where pp.Rodzina = p.Rodzina AND pc.CechaId = c.id) = 1 -- wystepuje tylko 1 raz unikalna
