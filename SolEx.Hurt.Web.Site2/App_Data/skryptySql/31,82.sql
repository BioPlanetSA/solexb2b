IF (EXISTS (SELECT *  FROM INFORMATION_SCHEMA.TABLES 
                 WHERE TABLE_SCHEMA = 'dbo' 
                 AND  TABLE_NAME = 'ProfilKlienta'))
BEGIN
   DROP TABLE [dbo].ProfilKlienta;
END



CREATE TABLE [dbo].[ProfilKlienta](
	[Id] [bigint] NOT NULL,
	[KlientId] [bigint] NULL,
	[Dodatkowe] [varchar](500) NULL,
	[TypUstawienia] [varchar](100) NULL,
	[Wartosc] [nvarchar](500) NOT NULL,
	[Dopisek] [varchar](100) NULL,
 CONSTRAINT [PK_ProfilKlienta] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY];




ALTER TABLE [dbo].[ProfilKlienta]  WITH CHECK ADD  CONSTRAINT [FK_ProfilKlienta_Klient] FOREIGN KEY([KlientId])
REFERENCES [dbo].[Klient] ([Id])
ON DELETE CASCADE;


ALTER TABLE [dbo].[ProfilKlienta] CHECK CONSTRAINT [FK_ProfilKlienta_Klient];
