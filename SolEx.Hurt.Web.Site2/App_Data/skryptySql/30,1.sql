IF NOT EXISTS (SELECT * FROM sys.objects WHERE type = 'TR' AND name = 'usuwanie_zadan_podrzednych')
exec('CREATE TRIGGER [usuwanie_zadan_podrzednych] 
ON  [Zadanie] 
INSTEAD OF DELETE 
AS BEGIN 
	DELETE from Zadanie where ZadanieNadrzedne in (select id from deleted); 
	DELETE Zadanie FROM DELETED D 
	INNER JOIN Zadanie T ON T.id = D.id 
END')
