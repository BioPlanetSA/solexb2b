EXEC sp_rename 'Koszyk.MagazynPodstawowy', 'MagazynDlaMm';
EXEC sp_rename 'Zamowienie.MagazynPodstawowy', 'MagazynDlaMm';
EXEC('ALTER TABLE dbo.Koszyk ADD MagazynRealizujacy varchar(50)');
EXEC sp_rename 'Magazyn.MagazynPodstawowy', 'MagazynRealizujacy';