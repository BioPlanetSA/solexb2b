
CREATE VIEW vKategorieDrzewo
AS
WITH item AS ( 
	select id, parentID, 0 as level, CAST('' as nvarchar(500)) as path from  KategoriaProduktu where ParentId IS NULL
	UNION ALL
	select k.id, k.parentID, c.level + 1,   CAST( c.path + '/' + CAST(  k.ParentId AS nvarchar(500)) AS nvarchar(500) ) AS path  from item as c INNER JOIN KategoriaProduktu as k on k.ParentId = c.id
)
select * from item;


