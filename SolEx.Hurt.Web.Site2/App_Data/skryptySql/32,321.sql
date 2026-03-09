-------------------------------------------------
-- STWORZYŁ: BARTEK
-- POWÓD: bioplanet ma ilosci w opakowaniu w formie 3 miejsc po przecinku. Usuwam kolumne StanMaksymalny - nigdy nie wykorzystywana
--------------------------------------------------


IF OBJECT_ID('dbo.DF_produkty_stan', 'D') IS NOT NULL
BEGIN
    ALTER TABLE Produkt DROP CONSTRAINT DF_produkty_stan;
	ALTER TABLE Produkt DROP COLUMN StanMaksymalny;
END


ALTER TABLE Produkt ALTER COLUMN IloscWOpakowaniu decimal(12,4) NOT NULL;
ALTER TABLE Produkt ALTER COLUMN StanMin decimal(12,4) NOT NULL;
