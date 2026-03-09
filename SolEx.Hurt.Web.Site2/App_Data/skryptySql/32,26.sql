IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='Sesja' AND xtype='U')
begin
CREATE TABLE [dbo].[Sesja](
	[Id] [uniqueidentifier] NOT NULL,
	[KlientId] [bigint] NOT NULL,
	[NazwaUrzadzenia] [varchar](300) NOT NULL,
	[IpKlienta] [varchar](50) NOT NULL,
	[DataUtworzenia] [datetime] NOT NULL,
	[DataZakonczenia] [datetime] NULL,
 CONSTRAINT [PK_Sesja] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]


ALTER TABLE [dbo].[Sesja]  WITH CHECK ADD  CONSTRAINT [FK_Sesja-Klient] FOREIGN KEY([KlientId])
REFERENCES [dbo].[Klient] ([Id])
ON UPDATE CASCADE
ON DELETE CASCADE


ALTER TABLE [dbo].[Sesja] CHECK CONSTRAINT [FK_Sesja-Klient]

CREATE NONCLUSTERED INDEX [IX_Sesja_idklienta] ON [dbo].[Sesja]
(
	[KlientId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)

end

