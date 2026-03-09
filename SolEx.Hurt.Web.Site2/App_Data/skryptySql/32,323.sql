-------------------------------------------------
-- STWORZYŁ: BARTEK
-- POWÓD: bioplanet ma IloscMinimalna do 4 miejsc po przecinku
--------------------------------------------------

ALTER TABLE Produkt ALTER COLUMN IloscMinimalna decimal(12,4) NOT NULL;

