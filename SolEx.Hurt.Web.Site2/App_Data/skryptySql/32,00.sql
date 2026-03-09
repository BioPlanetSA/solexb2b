
IF EXISTS (SELECT * FROM sysobjects WHERE name='Indiwidualizacja' AND xtype='U')
Begin
drop table Indiwidualizacja
end

IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='Indywidualizacja' AND xtype='U')
begin
CREATE TABLE [dbo].[Indywidualizacja](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[ListaCechWmaganych] [nvarchar](200) NULL,
	[RodzajKontrolki] [nvarchar](100) NOT NULL,
	[Atrybut] [int] NULL,
	[Nazwa] [nvarchar](500) NOT NULL,
	[Opis] [nvarchar](2000) NULL,
	[SposobIndywidualizacji][varchar](100) NOT NULL,
	[Wymagane][bit] Default(1),
 CONSTRAINT [PK_Indywidualizacja] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY])

end




IF EXISTS (SELECT * FROM sysobjects WHERE name='CenyIndywidualizacja' AND xtype='U')
Begin
	drop table CenyIndywidualizacja
end

IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='IndywidualizacjaCena' AND xtype='U')
Begin
	CREATE TABLE [dbo].[IndywidualizacjaCena](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[WalutaId] [bigint] NOT NULL,
	[Cena] [decimal](15, 2) NULL,
	[IdIndywidualizacji] [bigint] NULL,
	[NarzutTyp] [nvarchar](100) NOT NULL,
 CONSTRAINT [PK_IndywidualizacjaCena] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY])


ALTER TABLE [dbo].IndywidualizacjaCena  WITH NOCHECK ADD  CONSTRAINT FK_Waluta_IndywidualizacjaCena FOREIGN KEY([WalutaId]) REFERENCES [dbo].[Waluta] ([Id]) ON DELETE cascade

end


