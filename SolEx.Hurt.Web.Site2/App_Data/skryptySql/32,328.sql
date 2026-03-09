--dodajemy klucz unikawtowosci dla symbolu w tabeli kraje
CREATE UNIQUE NONCLUSTERED INDEX unique_symbol_kraju
ON Kraje(Symbol)
WHERE Symbol != '';
