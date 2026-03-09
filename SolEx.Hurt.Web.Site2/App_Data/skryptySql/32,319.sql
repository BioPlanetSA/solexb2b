-------------------------------------------------
-- STWORZYŁ: BARTEK
-- POWÓD: aktualizacja kolumny wartoscBat - kolumna nowo dodana w poprzednim skrypcie 32.318 i jest domyslnie 0. Przeliczam kolumne wg. brutto - netto zeby nie bylo 0.
--------------------------------------------------

 UPDATE HistoriaDokumentu SET WartoscVat = WartoscBrutto - WartoscNetto;




