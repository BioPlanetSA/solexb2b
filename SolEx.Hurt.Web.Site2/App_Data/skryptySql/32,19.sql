--exec sys.sp_cdc_disable_table @source_schema='dbo',	@source_name= 'Zamowienie', @capture_instance= 'dbo_Zamowienie';


ALTER TABLE [dbo].[ZamowienieDokumenty] DROP CONSTRAINT [FK_ZamowieniaDokumenty_Zamowienie];
ALTER TABLE [dbo].[ZamowienieProdukt] DROP CONSTRAINT [FK_Hzamowienia_produkty_Hzamowienia];


drop table [Zamowienie];


CREATE TABLE [dbo].[Zamowienie](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[KlientId] [bigint] NOT NULL,
	[DataUtworzenia] [datetime] NULL,
	[StatusId] [int] NOT NULL,
	[Uwagi] [nvarchar](3000) NULL,
	[PoziomCenyId] [int] NULL,
	[WartoscNetto] [decimal](10, 4) NOT NULL,
	[WartoscBrutto] [decimal](10, 4) NOT NULL,
	[MagazynRealizujacy] [varchar](50) NULL,
	[AdresId] [bigint] NULL,
	[WalutaId] [bigint] NULL,
	[TerminDostawy] [datetime] NULL,
	[MagazynPodstawowy] [varchar](50) NULL,
	[BladKomunikat] [varchar](4000) NULL,
	[KategoriaZamowienia] [varchar](200) NULL,
	[PracownikSkladajacyId] [bigint] NULL,
	[NazwaPlatnosci] [varchar](200) NULL,
	[NumerWlasnyZamowieniaKlienta] [varchar](200) NULL,
	[DodatkowePola] [varchar](max) NULL,
	[DefinicjaDokumentuERP] [varchar](20) NULL,
	[Adres] [nvarchar](max) NULL,
	[Pliki] [varchar](4000) NULL,
	[NumerTymczasowyZamowienia] [varchar](200) NULL,
	[IdOddzialu] [bigint] NULL,
 CONSTRAINT [PK_Hzamowienia] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]



ALTER TABLE [dbo].[Zamowienie] ADD  CONSTRAINT [DF_zamowienia_WartoscNetto]  DEFAULT ((0)) FOR [WartoscNetto]

ALTER TABLE [dbo].[Zamowienie] ADD  CONSTRAINT [DF_zamowienia_WartoscNetto1]  DEFAULT ((0)) FOR [WartoscBrutto]

ALTER TABLE [dbo].[Zamowienie]  WITH CHECK ADD  CONSTRAINT [FK_zamowienia_zamowienia_statusy] FOREIGN KEY([StatusId]) REFERENCES [dbo].[StatusZamowienia] ([Id])

ALTER TABLE [dbo].[Zamowienie] CHECK CONSTRAINT [FK_zamowienia_zamowienia_statusy]


ALTER TABLE [dbo].[ZamowienieProdukt]  WITH CHECK ADD  CONSTRAINT [FK_Hzamowienia_produkty_Hzamowienia] FOREIGN KEY([DokumentID])
REFERENCES [dbo].[Zamowienie] ([Id])
ON DELETE CASCADE


ALTER TABLE [dbo].[ZamowienieProdukt] CHECK CONSTRAINT [FK_Hzamowienia_produkty_Hzamowienia]





ALTER TABLE [dbo].[ZamowienieDokumenty]  WITH CHECK ADD  CONSTRAINT [FK_ZamowieniaDokumenty_Zamowienie] FOREIGN KEY([IdZamowienia])
REFERENCES [dbo].[Zamowienie] ([Id])
ON DELETE CASCADE


ALTER TABLE [dbo].[ZamowienieDokumenty] CHECK CONSTRAINT [FK_ZamowieniaDokumenty_Zamowienie]








--exec sys.sp_cdc_enable_table @source_schema='dbo',	@source_name= 'Zamowienie', @capture_instance= 'dbo_Zamowienie', @role_name = NULL, @supports_net_changes = 1;
