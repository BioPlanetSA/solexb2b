-------------------------------------------------
-- STWORZYŁ: BARTEK
-- POWÓD: dodanie kolumny wartosc vat do obiektu dokumentu, po to aby bylo to policzone po stronie klienta i zwalidowane czy jest poprawnie policzone.
-- Obliczenie wartości Vat nastepuje poprzez przeliczenie vatów wszystkich pozycji - tak jak ksiegowo powinno sie robic, a nie proste brutto - netto
-- tu instrukcja: http://itpomocni.pl/przeliczenia-na-fakturach-vat/
-- tabelka nie bedzie CDC wiec skrypt bez CDC
--------------------------------------------------

IF NOT EXISTS (
  SELECT * 
  FROM   sys.columns 
  WHERE  object_id = OBJECT_ID(N'[dbo].[HistoriaDokumentu]')  AND name = 'WartoscVat'
)
BEGIN
 ALTER TABLE HistoriaDokumentu ADD WartoscVat decimal(15,2) NOT NULL DEFAULT 0;
END;



