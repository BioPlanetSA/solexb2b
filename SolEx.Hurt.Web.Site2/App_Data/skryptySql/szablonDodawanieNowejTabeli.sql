
IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='NazwaTabeli' AND xtype='U')
begin
	CREATE TABLE [dbo].NazwaTabeli(
		definicja kolumn
	 CONSTRAINT [PK_NazwaTabeli] PRIMARY KEY CLUSTERED 
	(
		[Id] ASC
	)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
	) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

	-- to dodajemy jak mają być klucze obce
	ALTER TABLE [dbo].[NazwaTabeli]  WITH CHECK ADD  CONSTRAINT [FK_NazwaKluczaObcego-Dowolna] FOREIGN KEY([BlogGrupaId]) REFERENCES [dbo].[BlogGrupa] ([Id]) ON DELETE CASCADE
	
	-- to dajemy jesli ma być zapisywana historia zmian 
	EXEC [sys].[sp_cdc_enable_table] N'dbo','NazwaTabeli', @role_name = NULL, @supports_net_changes = 1
end