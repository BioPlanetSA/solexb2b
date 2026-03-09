
IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='AkceptacjaKoszykow' AND xtype='U')
begin
	CREATE TABLE [dbo].AkceptacjaKoszykow(
		[Id] [bigint] NOT NULL,
		[KlientId] [bigint] NOT NULL,
		[KoszykId] [bigint] NOT NULL,
	 CONSTRAINT [PK_Id] PRIMARY KEY CLUSTERED 
	(
		[Id] ASC
	)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
	) ON [PRIMARY]

	-- to dodajemy jak mają być klucze obce
	ALTER TABLE [dbo].[AkceptacjaKoszykow]  WITH CHECK ADD  CONSTRAINT [FK_AkceptacjaKoszykow_Klient] FOREIGN KEY([KlientId]) REFERENCES [dbo].[Klient] ([Id]) ON DELETE CASCADE
	ALTER TABLE [dbo].[AkceptacjaKoszykow]  WITH CHECK ADD  CONSTRAINT [FK_AkceptacjaKoszykow_Koszyk] FOREIGN KEY([KoszykId]) REFERENCES [dbo].[Koszyk] ([Id]) ON DELETE CASCADE
	
	-- to dajemy jesli ma być zapisywana historia zmian 
	--EXEC [sys].[sp_cdc_enable_table] N'dbo','AkceptacjaKoszykow', @role_name = NULL, @supports_net_changes = 1
end