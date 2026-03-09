
DECLARE @tblName VARCHAR(100)  --zmienna do nazwy tabeli
DECLARE @colname VARCHAR(100)  --zmienna do nazwy kolumny 
DECLARE @cdcInst VARCHAR(100)

SET @tblName = 'DzialaniaUzytkownikow'
SET @colname = 'Parametry'

if exists (select column_name from INFORMATION_SCHEMA.columns where table_name = @tblName and column_name = @colname) 
begin

	SET @cdcInst = 'dbo_' + @tblName
	------------------------------Usuniecie kolumny z tabeli
	EXEC('ALTER TABLE dbo.'+ @tblName +' DROP COLUMN '+ @colname  )  
	      
end

IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='DzialaniaUzytkwonikowParametry' AND xtype='U')
begin
	CREATE TABLE [dbo].DzialaniaUzytkwonikowParametry(
		[Id] [varchar](200) NOT NULL,
		[NazwaParametru] [varchar](100) NOT NULL,
		[IdDzialania] int NOT NULL,
		[Wartosc] [varchar](200) NOT NULL,
	 CONSTRAINT [PK_DzialaniaUzytkwonikowParametry] PRIMARY KEY CLUSTERED 
	(
		[Id] ASC
	)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
	) ON [PRIMARY]

	-- to dodajemy jak mają być klucze obce
	ALTER TABLE [dbo].[DzialaniaUzytkwonikowParametry]  WITH CHECK ADD  CONSTRAINT [FK_DzialaniaUzytkwonikowParametry-DzialaniaUzytkwonikow] FOREIGN KEY([IdDzialania]) REFERENCES [dbo].[DzialaniaUzytkownikow] ([Id]) ON DELETE CASCADE
end