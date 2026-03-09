BEGIN
    EXEC ('CREATE TRIGGER czyszczenie_pol_po_usunieciu_szablonu_akceptacji 
	   ON  SzablonAkceptacji 
	  INSTEAD OF DELETE
	AS 
	BEGIN
		SET NOCOUNT ON;
		Update SubkontoGrupa Set SzablonAkceptacjiId=NULL, SzablonAkceptacjiPrzekrocznyLimitId=NULL
		 where SzablonAkceptacjiId in (select ID from deleted) or SzablonAkceptacjiPrzekrocznyLimitId in (select ID from deleted)

		 Update Klient Set SzablonAkceptacjiId=NULL, SzablonAkceptacjiPrzekrocznyLimitId=NULL
		 where SzablonAkceptacjiId in (select ID from deleted) or SzablonAkceptacjiPrzekrocznyLimitId in (select ID from deleted)

		 DELETE sa FROM dbo.SzablonAkceptacji AS sa INNER JOIN deleted AS d ON sa.Id = d.Id;		
	END');
END;
