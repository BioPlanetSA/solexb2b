-------------------------------------------------
-- STWORZYŁ: BARTEK
-- POWÓD: dodanie kolumny do atrybutow zeby mozna bylo wybierac jawnie jakie atrybuty maja byc w plikach integracyjnych - wczesniej byly wszystkie i byl maksymalny balagan
--------------------------------------------------

IF NOT EXISTS (
  SELECT * 
  FROM   sys.columns 
  WHERE  object_id = OBJECT_ID(N'[dbo].[Atrybut]')  AND name = 'PokazujWPlikachIntegracji'
)
BEGIN
 ALTER TABLE Atrybut ADD PokazujWPlikachIntegracji bit NOT NULL DEFAULT 0;
END;

