if NOT EXISTS (SELECT * FROM sysobjects WHERE name='Indiwidualizacja' AND xtype='U')
Begin

CREATE TABLE [dbo].[Indiwidualizacja](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[ListaCechWmaganych] [nvarchar](200) NULL,
	[RodzajKontrolki] [nvarchar](100) NOT NULL,
	[ListAtrybutow] [varchar](200) NULL,
	[Nazwa] [nvarchar](500) NOT NULL,
	[Opis] [nvarchar](2000) NULL,
	[SposobIndywidualizacji][varchar](100) NOT NULL,
	[Wymagane][bit] Default(1),
 CONSTRAINT [PK_Indiwidualizacja] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY])


end


IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='CenyIndywidualizacja' AND xtype='U')
Begin

CREATE TABLE [dbo].[CenyIndywidualizacja](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[WalutaId] [bigint] NOT NULL,
	[Cena] [decimal](15, 2) NULL,
	[IdIndywidualizacji] [bigint] NULL,
	[NarzutTyp] [nvarchar](100) NOT NULL,
 CONSTRAINT [PK_CenyIndywidualizacja] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY])


end
ALTER TABLE [dbo].CenyIndywidualizacja  WITH NOCHECK ADD  CONSTRAINT FK_Waluta_CenyIndywidualizacja FOREIGN KEY([WalutaId]) REFERENCES [dbo].[Waluta] ([Id]) ON DELETE cascade

