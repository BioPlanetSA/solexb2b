
CREATE TABLE [dbo].[Adres](
	[Id] [bigint] NOT NULL,
	[Nazwa] [nvarchar](250) NULL,
	[UlicaNr] [nvarchar](100) NULL,
	[Miasto] [nvarchar](100) NULL,
	[KodPocztowy] [nvarchar](100) NULL,
	[Telefon] [varchar](100) NULL CONSTRAINT [DF_Adresy_Telefon]  DEFAULT (''),
	[KrajId] [int] NULL,
	[RegionId] [int] NULL,
	[Email] [varchar](100) NULL,
	[Lat] [decimal](15, 10) NULL,
	[Lon] [decimal](15, 10) NULL,
 CONSTRAINT [PK_adresy] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]


CREATE TABLE [dbo].[Atrybut](
	[Id] [int] NOT NULL,
	[Nazwa] [nvarchar](300) NOT NULL,
	[Widoczny] [bit] NOT NULL CONSTRAINT [DF_Hatrybuty_czy_widoczny]  DEFAULT ((0)),
	[Grupowalny] [bit] NOT NULL CONSTRAINT [DF_atrybuty_grupowalny]  DEFAULT ((0)),
	[Kolejnosc] [int] NOT NULL CONSTRAINT [DF__atrybuty__kolejn__2645B050]  DEFAULT ((0)),
	[ProviderWyswietlania] [varchar](200) NOT NULL CONSTRAINT [DF_atrybuty_provider_wyswietlania]  DEFAULT ('Filtr'),
	[Symbol] [nvarchar](100) NULL,
	[ZawszeWszystkieCechy] [bit] NOT NULL CONSTRAINT [DF__atrybuty__zawsze__0F431ABE]  DEFAULT ((0)),
	[NazwaOpisowa] [nvarchar](4000) NULL,
	[PokazujWWyszukiwaniu] [bit] NOT NULL CONSTRAINT [DF__atrybuty__pokazu__23D42350]  DEFAULT ((0)),
	[UkryjJednaWartosc] [bit] NOT NULL CONSTRAINT [DF__atrybuty__ukryj___4440F2E2]  DEFAULT ((0)),
	[PokazujNaLiscieProduktow] [bit] NOT NULL CONSTRAINT [DF__atrybuty__pokazu__43F6DA1F]  DEFAULT ((1)),
	[CechyPokazujKatalog] [bit] NOT NULL CONSTRAINT [DF_atrybuty_cechy_pokazuj_katalog]  DEFAULT ((0)),
	[PokazujOpisMetki] [bit] NOT NULL CONSTRAINT [DF_atrybuty_PokazujOpisMetki]  DEFAULT ((0)),
	[PobierajCechy] [bit] NOT NULL CONSTRAINT [DF_atrybuty_PobierajCechy]  DEFAULT ((1)),
	[Szerokosc] [nvarchar](16) NULL,
	[UniwersalnaMetkaOpis] [nvarchar](2000) NULL,
	[MetkaPozycjaLista] [varchar](150) NULL CONSTRAINT [DF_Atrybut_MetkaPozycjaLista]  DEFAULT ('Brak'),
	[MetkaPozycjaRodziny] [varchar](150) NULL CONSTRAINT [DF_Atrybut_MetkaPozycjaRodzina]  DEFAULT ('Brak'),
	[MetkaPozycjaSzczegoly] [varchar](150) NULL CONSTRAINT [DF_Atrybut_MetkaPozycjaSzczely]  DEFAULT ('Brak'),
	[MetkaPozycjaSzczegolyWarianty] [varchar](150) NULL CONSTRAINT [DF_Atrybut_MetkaPozycjaSzczelyWarianty]  DEFAULT ('Brak'),
	[MetkaPozycjaKoszykProdukty] [varchar](150) NULL CONSTRAINT [DF_Atrybut_MetkaPozycjaKoszykProdukty]  DEFAULT ('Brak'),
	[MetkaPozycjaKoszykAutomatyczne] [varchar](150) NULL CONSTRAINT [DF_Atrybut_MetkaPozycjaKoszykAutomatyczne]  DEFAULT ('Brak'),
	[MetkaPozycjaKoszykGratisy] [varchar](150) NULL CONSTRAINT [DF_Atrybut_MetkaPozycjaKoszykGratisy]  DEFAULT ('Brak'),
	[MetkaPozycjaKoszykGratisyPopUp] [varchar](150) NULL CONSTRAINT [DF_Atrybut_MetkaPozycjaKoszykGratisyPopUp]  DEFAULT ('Brak'),
 CONSTRAINT [PK_Hatrybuty] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]


CREATE TABLE [dbo].[BlogKategoria](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[Nazwa] [nvarchar](200) NOT NULL,
	[Aktywna] [bit] NOT NULL,
 CONSTRAINT [PK_BlogKategoria] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]


CREATE TABLE [dbo].[BlogWpis](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[Tytul] [nvarchar](200) NOT NULL,
	[DataDodania] [datetime] NOT NULL,
	[AutorId] [bigint] NOT NULL,
	[PoziomWidocznosci] [varchar](200) NULL,
	[Aktywny] [bit] NOT NULL CONSTRAINT [DF_BlogWpis_Aktywny]  DEFAULT ((1)),
	[Tresc] [nvarchar](max) NOT NULL,
	[ZdjecieId] [bigint] NULL,
 CONSTRAINT [PK_BlogWpis] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]


CREATE TABLE [dbo].[BlogWpisBlogKategoria](
	[Id] [varchar](200) NOT NULL,
	[BlogWpisId] [bigint] NOT NULL,
	[BlogKategoriaId] [bigint] NOT NULL,
 CONSTRAINT [PK_BlogWpisBlogKategoria] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]


CREATE TABLE [dbo].[Cecha](
	[Id] [bigint] NOT NULL,
	[Nazwa] [nvarchar](400) NOT NULL,
	[Widoczna] [bit] NOT NULL CONSTRAINT [DF_Hcechy_produktow_widoczna]  DEFAULT ((0)),
	[Symbol] [nvarchar](400) NOT NULL,
	[ObrazekId] [int] NULL,
	[Kolejnosc] [int] NULL CONSTRAINT [DF_cechy_ikona_kolejnosc]  DEFAULT ((0)),
	[AtrybutId] [int] NULL,
	[Opis] [nvarchar](2000) NULL,
	[CechaNadrzednaId] [int] NULL,
	[MetkaOpis] [nvarchar](2000) NULL,
	[MetkaPozycjaSzczegoly] [varchar](150) NOT NULL CONSTRAINT [DF_cechy_MetkaPozycjaSzczegoly]  DEFAULT ('Brak'),
	[MetkaKatalog] [nvarchar](2000) NULL,
	[MetkaPozycjaLista] [varchar](150) NOT NULL CONSTRAINT [DF_cechy_MetkaPozycjaLista]  DEFAULT ('Brak'),
	[MetkaPozycjaRodziny] [varchar](150) NOT NULL CONSTRAINT [DF_cechy_MetkaPozycjaRodziny]  DEFAULT ('Brak'),
	[MetkaPozycjaSzczegolyWarianty] [varchar](150) NOT NULL CONSTRAINT [DF_cechy_MetkaPozycjaSzczegolyWarianty]  DEFAULT ('Brak'),
	[OpisNaProdukcie] [nvarchar](2000) NULL,
	[MetkaPozycjaKoszykProdukty] [varchar](150) NOT NULL CONSTRAINT [DF_cechy_MetkaPozycjaKoszykProdukty]  DEFAULT ('Brak'),
	[MetkaPozycjaKoszykAutomatyczne] [varchar](150) NOT NULL CONSTRAINT [DF_cechy_MetkaPozycjaKoszykAutomatyczne]  DEFAULT ('Brak'),
	[MetkaPozycjaKoszykGratisy] [varchar](150) NOT NULL CONSTRAINT [DF_cechy_MetkaPozycjaKoszykGratisy]  DEFAULT ('Brak'),
	[MetkaPozycjaKoszykGratisyPopUp] [varchar](150) NOT NULL CONSTRAINT [DF_cechy_MetkaPozycjaKoszykGratisyPopUp]  DEFAULT ('Brak'),
 CONSTRAINT [cechy_produktow_pk] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY],
 CONSTRAINT [IX_cechy] UNIQUE NONCLUSTERED 
(
	[Symbol] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]


CREATE TABLE [dbo].[CenaPoziomu](
	[Id] [varchar](200) NOT NULL,
	[ProduktId] [bigint] NOT NULL,
	[PoziomId] [int] NOT NULL,
	[Netto] [decimal](10, 2) NOT NULL CONSTRAINT [DF_ceny_poziomy_netto]  DEFAULT ((0)),
	[WalutaId] [bigint] NULL,
 CONSTRAINT [PK_ceny_poziomy] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY],
 CONSTRAINT [IX_ceny_poziomy] UNIQUE NONCLUSTERED 
(
	[ProduktId] ASC,
	[PoziomId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]


CREATE TABLE [dbo].[DzialaniaUzytkownikow](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Parametry] [varchar](max) NOT NULL,
	[Zdarzenie] [varchar](50) NOT NULL,
	[IpKlienta] [varchar](50) NOT NULL,
	[EmailKlienta] [varchar](50) NOT NULL,
	[Data] [datetime] NOT NULL,
	[ZdarzenieGlowne] [varchar](50) NOT NULL,
 CONSTRAINT [PK_DzialaniaUzytkownikow] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]


CREATE TABLE [dbo].[FlatCeny](
	[Id] [varchar](500) NOT NULL,
	[KlientId] [bigint] NOT NULL,
	[ProduktId] [bigint] NOT NULL,
	[WalutaId] [bigint] NOT NULL,
	[CenaNetto] [decimal](10, 2) NOT NULL,
	[CenaHurtowaNetto] [decimal](16, 2) NOT NULL,
	[CenaNettoDokladna] [decimal](18, 5) NOT NULL,
	[TypRabatu] [int] NOT NULL,
	[Rabat] [decimal](5, 2) NOT NULL,
 CONSTRAINT [PK_flat_ceny] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, FILLFACTOR = 80) ON [PRIMARY]
) ON [PRIMARY]


CREATE TABLE [dbo].[Grupa](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Nazwa] [nvarchar](50) NOT NULL,
	[Widoczna] [bit] NOT NULL CONSTRAINT [DF_grupy_widoczna]  DEFAULT ((0)),
	[Producencka] [bit] NOT NULL CONSTRAINT [DF_grupy_producencka]  DEFAULT ((0)),
	[ListaZObrazkami] [bit] NOT NULL CONSTRAINT [DF_grupy_lista_z_obrazkami]  DEFAULT ((0)),
	[Kolejnosc] [int] NOT NULL CONSTRAINT [DF_grupy_kolejnosc_na_stronie]  DEFAULT ((1)),
	[GrupaKomplementarnaId] [int] NULL,
	[ObrazekId] [int] NULL,
	[Dostep] [varchar](50) NOT NULL CONSTRAINT [DF_grupy_dostep_id]  DEFAULT ((2)),
	[Parametry] [varchar](5000) NULL,
	[GrupujWyszukiwanie] [bit] NOT NULL CONSTRAINT [DF_grupy_GrupujWyszukiwanie]  DEFAULT ((0)),
	[OpisZbiorczy] [varchar](5000) NULL,
 CONSTRAINT [PK_grupy] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]


CREATE TABLE [dbo].[HistoriaDokumentu](
	[Id] [bigint] NOT NULL,
	[KlientId] [bigint] NOT NULL,
	[Rodzaj] [varchar](50) NOT NULL,
	[PartnerId] [int] NULL,
	[OdbiorcaId] [int] NULL,
	[StatusId] [int] NULL,
	[WalutaId] [nchar](10) NULL,
	[NazwaDokumentu] [varchar](100) NULL,
	[DataUtworzenia] [datetime] NOT NULL,
	[DataDodania] [datetime] NOT NULL CONSTRAINT [DF_historia_dokumenty_data_dodania]  DEFAULT (dateadd(year,datediff(year,(0),getdate()),(0))),
	[DataWyslaniaDokumentu] [datetime] NULL,
	[Zaplacono] [bit] NOT NULL CONSTRAINT [DF_historia_dokumenty_zaplacono]  DEFAULT ((0)),
	[Uwagi] [varchar](4000) NULL,
	[WartoscNalezna] [decimal](15, 2) NOT NULL CONSTRAINT [DF__historia___warto__7F01C5FD]  DEFAULT ((0)),
	[WartoscNetto] [decimal](15, 2) NOT NULL,
	[WartoscBrutto] [decimal](15, 2) NULL,
	[waluta] [varchar](5) NULL,
	[NazwaPlatnosci] [varchar](50) NULL,
	[TerminPlatnosci] [datetime] NULL,
	[Rezerwacja] [bit] NULL CONSTRAINT [DF__historia___rezer__190BB0C3]  DEFAULT ((1)),
	[Hash] [varchar](100) NULL,
	[NumerObcy] [varchar](500) NULL,
 CONSTRAINT [PK_historia_dokumenty] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]


CREATE TABLE [dbo].[HistoriaDokumentuListPrzewozowy](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[DokumentId] [bigint] NOT NULL,
	[NumerListu] [varchar](300) NOT NULL,
	[Link] [varchar](500) NOT NULL,
 CONSTRAINT [PK_historia_dokumenty_listy_przewozowe] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]


CREATE TABLE [dbo].[HistoriaDokumentuProdukt](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[DokumentId] [int] NOT NULL,
	[KodProduktu] [nvarchar](80) NOT NULL,
	[NazwaProduktu] [nvarchar](255) NOT NULL,
	[Ilosc] [decimal](12, 4) NOT NULL CONSTRAINT [DF_dokumenty_produkty_ilosc]  DEFAULT ((1)),
	[JednostkaMiary] [nvarchar](50) NOT NULL CONSTRAINT [DF__historia___jedno__0AFD888E]  DEFAULT ('szt.'),
	[CenaBrutto] [decimal](15, 2) NOT NULL,
	[CenaNetto] [decimal](15, 2) NOT NULL,
	[WartoscBrutto] [decimal](15, 2) NOT NULL,
	[WartoscNetto] [decimal](10, 2) NOT NULL,
	[Vat] [decimal](15, 2) NOT NULL,
	[WartoscVat] [decimal](15, 2) NOT NULL,
	[Parametry] [xml] NULL,
	[Rabat] [decimal](15, 2) NULL,
	[ProduktId] [int] NULL,
	[Opis] [varchar](8000) NULL,
	[CenaNettoPoRabacie] [decimal](15, 2) NOT NULL CONSTRAINT [DF_historia_dokumenty_produkty_CenaNettoPoRabacie_1]  DEFAULT ((0)),
	[CenaBruttoPoRabacie] [decimal](15, 2) NOT NULL CONSTRAINT [DF_historia_dokumenty_produkty_CenaBruttoPoRabacie]  DEFAULT ((0)),
	[Opis2] [varchar](8000) NULL,
	[TypProduktu] [varchar](200) NULL,
 CONSTRAINT [PK_dokumenty_produkty] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]


CREATE TABLE [dbo].[HistoriaWiadomosci](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[TrescWiadomosci] [nvarchar](4000) NOT NULL,
	[Tytul] [nvarchar](4000) NOT NULL,
	[DoKogo] [varchar](8000) NOT NULL,
	[OdKogo] [varchar](8000) NOT NULL,
	[KopiaBCC] [varchar](8000) NULL,
	[WyslijJakoHTML] [bit] NOT NULL,
	[PowiazaneZdarzenie] [int] NOT NULL,
	[DataStworzenia] [datetime] NOT NULL,
	[BylBlad] [bit] NOT NULL,
	[KampaniaId] [int] NULL,
	[BladKomunikat] [varchar](500) NULL,
 CONSTRAINT [PK_HistoriaWiadomosci] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]


CREATE TABLE [dbo].[Jednostka](
	[Id] [int] NOT NULL,
	[Nazwa] [nvarchar](200) NOT NULL,
	[Calkowitoliczowa] [bit] NOT NULL CONSTRAINT [DF_Jednostki_Calkowitoliczowa]  DEFAULT ((0)),
	[Komunikat] [varchar](1000) NULL,
	[Aktywna] [bit] NOT NULL CONSTRAINT [DF_Jednostki_Aktywna]  DEFAULT ((1)),
 CONSTRAINT [PK_Jednostki] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]


CREATE TABLE [dbo].[Jezyk](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Nazwa] [nvarchar](50) NOT NULL,
	[Domyslny] [bit] NOT NULL CONSTRAINT [DF_jezyki_domyslny]  DEFAULT ((0)),
	[Symbol] [varchar](5) NULL,
	[ObrazekId] [int] NULL,
	[DomyslnyDlaTlumaczen] [bit] NOT NULL CONSTRAINT [DF_jezyki_DomyslnyDlaTlumaczen]  DEFAULT ((0)),
	[UkrytyDlaKlienta] [bit] NOT NULL CONSTRAINT [DF_jezyki_UkrytyDlaKlienta]  DEFAULT ((0)),
	[Kultura] [varchar](5) NULL,
 CONSTRAINT [PK_jezyki] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]


CREATE TABLE [dbo].[KatalogSzablon](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Nazwa] [varchar](200) NOT NULL,
	[Opis] [nvarchar](4000) NULL,
	[Aktywny] [bit] NOT NULL CONSTRAINT [DF_szablony_widoczny]  DEFAULT ((1)),
	[PlikSzablonu] [varchar](500) NULL,
	[DozwoloneFormatyWydruku] [varchar](200) NULL CONSTRAINT [DF_szablony_dla_typów]  DEFAULT (''),
	[MaksymalnaLiczbaElementow] [int] NOT NULL CONSTRAINT [DF_szablony_maksymalna_ilosc_elementow]  DEFAULT ((0)),
	[KopiowacWedlugIlosci] [int] NOT NULL CONSTRAINT [DF_szablony_kopiowac_wedlug_ilosci]  DEFAULT ((0)),
	[ParametrTekstowy1] [varchar](100) NULL,
	[ParametrTekstowy2] [varchar](100) NULL,
	[ParametrTekstowy3] [varchar](100) NULL,
	[ParametrTekstowy4] [varchar](100) NULL,
	[ParametrTekstowy5] [varchar](100) NULL,
	[ParametrTekstowy6] [varchar](100) NULL,
	[ParametrTekstowy7] [varchar](100) NULL,
	[Obrazek0Id] [int] NULL,
	[Obrazek1Id] [int] NULL,
	[Obrazek2Id] [int] NULL,
	[Obrazek3Id] [int] NULL,
	[Obrazek4Id] [int] NULL,
	[Obrazek5Id] [int] NULL,
	[PlikSzablonuSpisuTresci] [varchar](80) NULL,
	[DostepnyDla] [varchar](100) NULL,
 CONSTRAINT [PK_szablony] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY],
 CONSTRAINT [IX_KatalogSzablon] UNIQUE NONCLUSTERED 
(
	[Nazwa] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]


CREATE TABLE [dbo].[KategoriaKlienta](
	[Id] [int] NOT NULL,
	[Nazwa] [nvarchar](1000) NOT NULL,
	[Opis] [varchar](500) NULL,
	[Grupa] [varchar](200) NOT NULL,
	[PokazujKlientowi] [bit] NOT NULL CONSTRAINT [DF_Table_1_samodzielny_zapis]  DEFAULT ((0)),
 CONSTRAINT [PK_klienci_kategorie] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

CREATE TABLE [dbo].[KategoriaProduktu](
	[Id] [bigint] NOT NULL,
	[Nazwa] [nvarchar](200) NOT NULL,
	[Widoczna] [bit] NOT NULL CONSTRAINT [DF__kategorie__widoc__03317E3D]  DEFAULT ((0)),
	[ObrazekId] [int] NULL,
	[ParentId] [bigint] NULL,
	[MiniaturaId] [int] NULL,
	[KategoriaTresciSymbol] [varchar](200) NULL,
	[GrupaId] [int] NOT NULL,
	[Opis] [nvarchar](4000) NULL,
	[OpisKrotki] [nvarchar](4000) NULL,
	[Kolejnosc] [int] NOT NULL CONSTRAINT [DF__kategorie__kolej__47A6A41B]  DEFAULT ((0)),
	[PokazujFiltry] [bit] NOT NULL CONSTRAINT [DF__kategorie__pokaz__10373EF7]  DEFAULT ((1)),
	[Link] [nvarchar](4000) NULL,
	[MetaOpis] [nvarchar](4000) NULL,
	[MetaSlowaKluczowe] [nvarchar](4000) NULL,
	[Dostep] [varchar](50) NOT NULL CONSTRAINT [DF_kategorie_dostep_id]  DEFAULT ((2)),
	[OpisNaProdukt] [nvarchar](4000) NULL,
	[KlasaCss] [varchar](4000) NULL,
 CONSTRAINT [kategorie_pk] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]


CREATE TABLE [dbo].[KategoriaSklepu](
	[Id] [bigint] NOT NULL,
	[nazwa] [varchar](50) NOT NULL,
	[PokazywanaNaMapie] [bit] NOT NULL CONSTRAINT [DF_sklepy_kategorie_PokazywanaNaMapie]  DEFAULT ((1)),
	[ObrazekPineskaId] [int] NULL,
	[Automatyczna] [bit] NOT NULL CONSTRAINT [DF_sklepy_kategorie_Automatyczna_1]  DEFAULT ((0)),
 CONSTRAINT [PK_sklepy_kategorie] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]


CREATE TABLE [dbo].[Klient](
	[Id] [bigint] NOT NULL,
	[Nazwa] [varchar](8000) NULL,
	[Symbol] [varchar](100) NOT NULL,
	[Nip] [varchar](100) NULL,
	[Telefon] [varchar](50) NULL,
	[Email] [varchar](255) NULL,
	[HasloZrodlowe] [varchar](80) NULL,
	[OpiekunId] [bigint] NULL,
	[DrugiOpiekunId] [bigint] NULL,
	[PrzedstawicielId] [bigint] NULL,
	[KlientNadrzednyId] [bigint] NULL,
	[JezykId] [int] NULL,
	[KontoPotwierdzajaceId] [bigint] NULL,
	[AdresWysylkiId] [int] NULL,
	[ZastepcaId] [bigint] NULL,
	[ZdjecieId] [int] NULL,
	[PoziomCenowyId] [int] NULL,
	[SubkontoGrupaId] [int] NULL,
	[CenaDetalicznaPoziomID] [int] NULL,
	[KlientEu] [bit] NOT NULL CONSTRAINT [DF_klienci_klient_EU]  DEFAULT ((0)),
	[Eksport] [bit] NOT NULL CONSTRAINT [DF_klienci_eksport]  DEFAULT ((0)),
	[HasloKlienta] [varchar](80) NULL,
	[BlokadaPlatnosci] [bit] NOT NULL CONSTRAINT [DF_klienci_blokada_platnosci]  DEFAULT ((1)),
	[Aktywny] [bit] NOT NULL CONSTRAINT [DF_klienci_aktywny]  DEFAULT ((1)),
	[Rabat] [decimal](10, 2) NOT NULL CONSTRAINT [DF_klienci_rabat]  DEFAULT ((0)),
	[LimitKredytu] [decimal](16, 2) NOT NULL CONSTRAINT [DF__klienci__kredyt___63F8CA06]  DEFAULT ((0)),
	[IloscWykorzystanegoKredytu] [decimal](16, 2) NOT NULL CONSTRAINT [DF__klienci__kredyt___64ECEE3F]  DEFAULT ((0)),
	[IloscPozostalegoKredytu] [decimal](16, 2) NOT NULL CONSTRAINT [DF_Klient_IloscPozostalegoKredytu]  DEFAULT ((0)),
	[PoleTekst1] [varchar](500) NULL,
	[PoleTekst2] [varchar](100) NULL,
	[PoleTekst3] [varchar](100) NULL,
	[PoleTekst4] [varchar](100) NULL,
	[PoleTekst5] [varchar](100) NULL,
	[MagazynDomyslny] [varchar](50) NULL,
	[WalutaId] [bigint] NULL,
	[BlokadaZamowien] [bit] NOT NULL CONSTRAINT [DF__klienci__blokada__7FB5F314]  DEFAULT ('false'),
	[DataOstatniegoLogowania] [datetime] NULL,
	[LimitIlosciZamowienia] [int] NULL CONSTRAINT [DF__klienci__limit_i__5C02A283]  DEFAULT ((0)),
	[LimitWartosciZamowienia] [decimal](10, 2) NULL CONSTRAINT [DF__klienci__limit_w__5CF6C6BC]  DEFAULT ((0)),
	[AdresWysylkiBlokada] [bit] NOT NULL CONSTRAINT [DF__klienci__adres_w__5FD33367]  DEFAULT ((0)),
	[LimitOkres] [int] NOT NULL CONSTRAINT [DF__klienci__limit_o__1AF3F935]  DEFAULT ((1)),
	[AlternatywnyEmail] [varchar](300) NULL,
	[FakturyElektroniczne] [int] NOT NULL CONSTRAINT [DF__klienci__sfera_f__29971E47]  DEFAULT ((0)),
	[MaksymalnaCenaTowaru] [decimal](10, 2) NULL,
	[PelnaOferta] [bit] NOT NULL CONSTRAINT [DF_klienci_pelna_oferta]  DEFAULT ((1)),
	[Gid] [varchar](100) NULL,
	[MinimalnaWartoscZamowienia] [decimal](10, 2) NOT NULL CONSTRAINT [DF__klienci__minimal__06ADD4BD]  DEFAULT ((0)),
	[DataZmianyHasla] [datetime] NOT NULL CONSTRAINT [DF__klienci__data_zm__31ED387D]  DEFAULT (getdate()),
	[Role] [varchar](100) NOT NULL CONSTRAINT [DF_klienci_role]  DEFAULT ((0)),
	[Skype] [varchar](1000) NULL,
	[GaduGadu] [varchar](100) NULL,
	[WidziWszystkich] [bit] NOT NULL CONSTRAINT [DF_klienci_widzi_wszystkich]  DEFAULT ((0)),
	[OdKiedyLiczymyLimit] [datetime] NULL,
	[Opis] [nvarchar](4000) NULL CONSTRAINT [DF_klienci_opis]  DEFAULT (''),
	[SubkontaAdministrator] [varchar](50) NULL,
	[StaleUkrywanieCen] [bit] NOT NULL CONSTRAINT [DF_klienci_StaleUkrywanieCen]  DEFAULT ((0)),
	[KluczSesji] [varchar](100) NULL,
	[DataZmianyKlucza] [datetime] NULL,
	[IndywidualnaStawaVat] [decimal](10, 2) NULL,
	[DataDodatnia] [datetime] NULL CONSTRAINT [DF_klienci_DataDodatnia]  DEFAULT (getdate()),
	[Login] [varchar](200) NULL,
	[CzyWidziStany] [bit] NOT NULL CONSTRAINT [DF_klienci_CzyWidziStany]  DEFAULT ((1)),
	[OsobaPolecajaca] [varchar](255) NULL,
	[PowodBlokady] [varchar](200) NULL,
	[GidIp] [varchar](200) NULL,
	[ZgodaNaNewsletter] [bit] NOT NULL CONSTRAINT [DF_klienci_ZgodaNaNewsletter]  DEFAULT ((1)),
	[DostepneModulyAdmina] [varchar](8000) NULL,
	[WidziPunkty] [bit] NULL,
 CONSTRAINT [PK_Hklienci] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]


CREATE TABLE [dbo].[KlientAdres](
	[Id] [varchar](50) NOT NULL,
	[KlientId] [bigint] NOT NULL,
	[AdresId] [bigint] NOT NULL,
	[TypAdresu] [varchar](50) NOT NULL CONSTRAINT [DF_KlienAdres_TypAdresu]  DEFAULT ('Brak'),
 CONSTRAINT [PK_KlienAdres] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]


CREATE TABLE [dbo].[KlientKategoriaKlienta](
	[Id] [varchar](200) NOT NULL,
	[KlientId] [bigint] NOT NULL,
	[KategoriaKlientaId] [int] NOT NULL,
 CONSTRAINT [PK_klienci_kategorie_2] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]


CREATE TABLE [dbo].[KlientLimitIlosciowy](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[KlientId] [bigint] NOT NULL,
	[ProduktId] [bigint] NOT NULL,
	[Ilosc] [decimal](18, 4) NOT NULL,
	[Od] [datetime] NULL,
	[Do] [datetime] NULL,
 CONSTRAINT [PK_klienci_limity_ilosciowe] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]


CREATE TABLE [dbo].[Konfekcje](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[KlientId] [bigint] NULL,
	[KategoriaKlientowId] [int] NULL,
	[ProduktId] [bigint] NULL,
	[Ilosc] [decimal](10, 2) NOT NULL,
	[Rabat] [decimal](10, 2) NULL,
	[RabatKwota] [decimal](10, 2) NULL,
	[WalutaId] [bigint] NULL,
	[CechaId] [bigint] NULL,
 CONSTRAINT [PK_Konfekcje] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]


CREATE TABLE [dbo].[Koszyk](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[Nazwa] [varchar](100) NOT NULL,
	[KlientId] [bigint] NULL,
	[Typ] [varchar](50) NOT NULL,
	[Aktywny] [bit] NOT NULL CONSTRAINT [DF_koszyki_aktywny]  DEFAULT ((0)),
	[Parametry] [nvarchar](3000) NULL,
	[DataModyfikacji] [datetime] NOT NULL CONSTRAINT [DF__koszyki__data_mo__22951AFD]  DEFAULT (getdate()),
 CONSTRAINT [PK_koszyki] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]


CREATE TABLE [dbo].[KoszykPozycje](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[KoszykId] [bigint] NOT NULL,
	[ProduktId] [bigint] NOT NULL,
	[Ilosc] [decimal](10, 2) NOT NULL CONSTRAINT [DF_Hkoszyki_ilosc]  DEFAULT ((1)),
	[DataDodania] [datetime] NOT NULL CONSTRAINT [DF_koszyki_data_dodania]  DEFAULT (getdate()),
	[DataZmiany] [datetime] NULL,
	[JednostkaId] [int] NULL,
	[PrzedstawicielId] [bigint] NULL,
	[RabatDodatkowy] [decimal](10, 5) NOT NULL CONSTRAINT [DF_koszyki_pozycje_jednostka_przelicznik_cena]  DEFAULT ((1)),
	[WymuszonaCenaNetto] [decimal](10, 2) NULL,
	[PowodDodatkowegoRabatu] [varchar](5000) NULL,
	[DodatkowyRabatRzeczywisty] [decimal](18, 2) NULL,
	[Indywidualizacja] [nvarchar](3000) NULL,
	[DodajaceZadanie] [int] NULL,
	[TypPozycji] [varchar](50) NULL,
	[Hash] [int] NULL CONSTRAINT [DF_koszyki_pozycje_Hash]  DEFAULT ((0)),
 CONSTRAINT [PK_Hkoszyki] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]


CREATE TABLE [dbo].[Kraje](
	[Id] [int] NOT NULL,
	[Nazwa] [varchar](500) NOT NULL,
	[Synchronizowane] [bit] NOT NULL CONSTRAINT [DF_Kraje_Synchronizowane]  DEFAULT ((0)),
	[Widoczny] [bit] NOT NULL,
 CONSTRAINT [PK_Kraje] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]


CREATE TABLE [dbo].[KupowaneIlosci](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[KlientId] [bigint] NOT NULL,
	[ProduktId] [bigint] NOT NULL,
	[Od] [datetime] NULL,
	[Do] [datetime] NULL,
	[DodatkowaIlosc] [decimal](10, 2) NOT NULL,
	[RodzajDokumentu] [varchar](50) NOT NULL,
 CONSTRAINT [PK_KupowaneIlosci] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY],
 CONSTRAINT [IX_KupowaneIlosci] UNIQUE NONCLUSTERED 
(
	[KlientId] ASC,
	[ProduktId] ASC,
	[Do] ASC,
	[Od] ASC,
	[RodzajDokumentu] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, FILLFACTOR = 80) ON [PRIMARY]
) ON [PRIMARY]


CREATE TABLE [dbo].[LogWpis](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Data] [datetime] NOT NULL,
	[Modul] [varchar](255) NOT NULL CONSTRAINT [DF__logi__modul__507BE13E]  DEFAULT ((-1)),
	[Poziom] [varchar](150) NOT NULL CONSTRAINT [DF__logi__poziom__51700577]  DEFAULT ((-1)),
	[Opis] [varchar](max) NULL,
	[Wiadomosc] [varchar](max) NULL,
	[Ex] [varchar](max) NULL,
	[StackTrace] [varchar](max) NULL,
	[Url] [varchar](max) NULL,
	[User] [varchar](255) NULL,
 CONSTRAINT [PK_logi] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]


CREATE TABLE [dbo].[Magazyn](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Symbol] [varchar](50) NOT NULL,
	[Nazwa] [varchar](100) NULL,
	[ImportowacZErp] [bit] NOT NULL,
	[Parametry] [varchar](200) NOT NULL,
 CONSTRAINT [PK_magazyny] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]


CREATE TABLE [dbo].[MaileBledneDoPonownejWysylki](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[Tytul] [nvarchar](max) NOT NULL,
	[Tresc] [nvarchar](max) NOT NULL,
	[DoKogo] [varchar](max) NOT NULL,
	[Bcc] [varchar](max) NULL,
	[RodzajSkrzynki] [varchar](50) NOT NULL,
	[IloscBledow] [int] NOT NULL,
 CONSTRAINT [PK_MaileBledneDoPonownejWysylki] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]


CREATE TABLE [dbo].[Newsletter](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[Nazwa] [varchar](500) NULL,
	[DataWysylki] [datetime] NULL,
	[DataModyfikacji] [datetime] NULL,
	[KategorieKlientaDoWysylki] [varchar](1000) NOT NULL CONSTRAINT [DF_mailing_kampanie_kategoria_kontrahentow_id]  DEFAULT ((0)),
	[DodatkoweAdresyKlientow] [varchar](5000) NOT NULL,
	[AdresyWykluczone] [varchar](5000) NOT NULL,
	[Temat] [nvarchar](500) NOT NULL,
	[Tresc] [text] NOT NULL,
	[Autor] [nvarchar](200) NULL,
	[Aktywna] [bit] NOT NULL CONSTRAINT [DF_mailing_kampanie_zakonczona]  DEFAULT ((0)),
	[WybraneProdukty] [varchar](500) NULL,
	[IloscWyslana] [int] NOT NULL CONSTRAINT [DF_mailing_kampanie_IloscWyslana]  DEFAULT ((0)),
	[IloscWszyscy] [int] NOT NULL CONSTRAINT [DF_mailing_kampanie_IloscWszystkich]  DEFAULT ((0)),
 CONSTRAINT [PK_mailing_kampanie] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

CREATE TABLE [dbo].[NewsletterZapisani](
	[Id] [bigint] IDENTITY (1, 1) NOT NULL,
	[DataZapisania] [datetime] NOT NULL,
	[DataWypisania] [datetime] NULL,
	[AdersIp] [varchar](50) NOT NULL,
	[Email] [varchar](200) NOT NULL,
 CONSTRAINT [PK_NewsletterZapisani] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]


CREATE TABLE [dbo].[PieczatkiSzablony](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[TypId] [int] NULL,
	[Nazwa] [varchar](1024) NULL,
	[SciezkaDoPlikuSzablonuJSON] [varchar](1024) NULL,
	[SciezkaDoPlikuszablonuSVG] [varchar](1024) NULL,
	[Opis] [varchar](max) NULL,
	[Zablokowany] [bit] NULL,
 CONSTRAINT [PK_PieczatkiSzablony] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

CREATE TABLE [dbo].[PieczatkiTyp](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[SymbolTypu] [varchar](100) NULL,
	[Nazwa] [varchar](1000) NULL,
	[OpisHtml] [varchar](max) NULL,
	[DozwoloneKoloryCalejPieczatki] [varchar](1024) NULL,
	[PowiazaneProduktyId] [varchar](max) NULL,
	[Szerokosc_mm] [decimal](10, 0) NULL,
	[Wysokosc_mm] [decimal](10, 0) NULL,
 CONSTRAINT [PK_PieczatkiTyp] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]


CREATE TABLE [dbo].[Plik](
	[Id] [int] NOT NULL,
	[Sciezka] [varchar](1024) NOT NULL,
	[Nazwa] [varchar](1024) NOT NULL,
	[Data] [datetime] NOT NULL,
	[Rozszerzenie] [varchar](2000) NOT NULL,
	[Rozmiar] [int] NOT NULL,
	[RodzajPliku] [varchar](50) NULL,
	[HtmlPrzycisku] [text] NULL,
	[SposobOtwierania] [varchar](50) NULL,
	[SzerokoscOkna] [int] NULL,
	[WysokoscOkna] [int] NULL,
	[KlasaCss] [varchar](300) NULL,
 CONSTRAINT [obrazki_pk] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]


CREATE TABLE [dbo].[PoziomCenowy](
	[Id] [int] NOT NULL,
	[Nazwa] [nvarchar](50) NOT NULL,
	[WalutaId] [bigint] NULL,
 CONSTRAINT [PK_poziomy_cen] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]


CREATE TABLE [dbo].[Produkt](
	[Id] [bigint] NOT NULL,
	[Nazwa] [nvarchar](2000) NOT NULL,
	[Kod] [nvarchar](80) NULL,
	[Widoczny] [bit] NOT NULL CONSTRAINT [DF_Hprodukty_widoczny_na_hurcie]  DEFAULT ((0)),
	[StanMaksymalny] [decimal](10, 2) NOT NULL CONSTRAINT [DF_produkty_stan]  DEFAULT ((0)),
	[StanMin] [decimal](10, 2) NOT NULL CONSTRAINT [DF_produkty_stan_min]  DEFAULT ((0)),
	[IloscWOpakowaniu] [decimal](10, 2) NOT NULL CONSTRAINT [DF_Hprodukty_ilosc_w_opakowaniu]  DEFAULT ((1)),
	[KodKreskowy] [varchar](150) NULL,
	[Vat] [decimal](10, 2) NULL,
	[PKWiU] [varchar](20) NULL,
	[Opis] [nvarchar](4000) NULL,
	[OpisKrotki] [nvarchar](4000) NULL,
	[Opis2] [nvarchar](4000) NULL,
	[OpisKrotki2] [nvarchar](4000) NULL,
	[Opis3] [nvarchar](4000) NULL,
	[OpisKrotki3] [nvarchar](4000) NULL,
	[Opis4] [nvarchar](4000) NULL,
	[OpisKrotki4] [nvarchar](4000) NULL,
	[Opis5] [nvarchar](4000) NULL,
	[OpisKrotki5] [nvarchar](4000) NULL,
	[ObrazekId] [int] NULL,
	[PoleTekst1] [varchar](2000) NULL,
	[PoleTekst2] [varchar](2000) NULL,
	[PoleTekst3] [varchar](2000) NULL,
	[PoleTekst4] [varchar](2000) NULL,
	[PoleTekst5] [varchar](2000) NULL,
	[PoleLiczba1] [decimal](12, 4) NULL,
	[PoleLiczba2] [decimal](12, 4) NULL,
	[PoleLiczba3] [decimal](12, 4) NULL,
	[PoleLiczba4] [decimal](12, 4) NULL,
	[PoleLiczba5] [decimal](12, 4) NULL,
	[Rodzina] [varchar](200) NULL,
	[Ojciec] [bit] NOT NULL CONSTRAINT [DF_produkty_ojciec]  DEFAULT ((0)),
	[Www] [varchar](500) NULL CONSTRAINT [DF__produkty__www__62108194]  DEFAULT (''),
	[Waga] [decimal](10, 3) NOT NULL CONSTRAINT [DF__produkty__waga__6304A5CD]  DEFAULT ((0)),
	[KolumnaLiczba1] [int] NULL,
	[KolumnaLiczba2] [int] NULL,
	[KolumnaLiczba3] [int] NULL,
	[KolumnaLiczba4] [int] NULL,
	[KolumnaLiczba5] [int] NULL,
	[KolumnaTekst1] [varchar](200) NULL,
	[KolumnaTekst2] [varchar](200) NULL,
	[KolumnaTekst3] [varchar](200) NULL,
	[KolumnaTekst4] [varchar](200) NULL,
	[KolumnaTekst5] [varchar](200) NULL,
	[IloscMinimalna] [decimal](10, 2) NOT NULL CONSTRAINT [DF__produkty__ilosc___7B1C2680]  DEFAULT ((1)),
	[PrzedstawicielId] [bigint] NULL,
	[DostepnyDlaWszystkich] [bit] NOT NULL CONSTRAINT [DF_produkty_dostepny_dla_wszystkich]  DEFAULT ((1)),
	[PopupTekst] [varchar](500) NULL,
	[PopupKomunikat] [varchar](200) NULL,
	[Dostawa] [varchar](100) NULL,
	[WymaganeOz] [bit] NOT NULL CONSTRAINT [DF_produkty_wymagane_OZ]  DEFAULT ((1)),
	[Typ] [varchar](50) NOT NULL CONSTRAINT [DF_produkty_Typ]  DEFAULT ('Produkt'),
	[DataDodania] [datetime] NOT NULL CONSTRAINT [DF_produkty_DataDodatnia]  DEFAULT (getdate()),
	[StatusUkryty] [int] NULL,
	[ParametryDostosowywania] [varchar](8000) NULL,
	[VatOdwrotneObciazenie] [bit] NOT NULL CONSTRAINT [DF_produkty_VatOdwrotneObciazenie_1]  DEFAULT ((0)),
	[Objetosc] [decimal](10, 4) NOT NULL CONSTRAINT [DF_produkty_Objetosc]  DEFAULT ((0)),
	[MenagerId] [bigint] NULL,
	[Widocznosc] [varchar](200) NULL,
	[NiePodlegaRabatowaniu] [bit] NOT NULL CONSTRAINT [DF_produkty_NiePodlegaRabatowaniu_1]  DEFAULT ((0)),
	[MetaOpis] [varchar](8000) NULL,
	[MetaSlowaKluczowe] [varchar](8000) NULL,
	[WyslanoMailNowyProdukt] [bit] NOT NULL CONSTRAINT [DF_produkty_WyslanoMailNowyProdukt]  DEFAULT ((1)),
	[LiczbaSztukNaWarstwie] [decimal](10, 4) NULL CONSTRAINT [DF__produkty__Liczba__69678A99]  DEFAULT (NULL),
	[LiczbaSztukNaPalecie] [decimal](10, 4) NULL CONSTRAINT [DF__produkty__Liczba__6A5BAED2]  DEFAULT (NULL),
	[GlebokoscOpakowaniaJednostkowego] [decimal](10, 4) NULL CONSTRAINT [DF__produkty__Glebok__6B4FD30B]  DEFAULT (NULL),
	[SzerokoscOpakowaniaJednostkowego] [decimal](10, 4) NULL CONSTRAINT [DF__produkty__Szerok__6C43F744]  DEFAULT (NULL),
	[WysokoscOpakowaniaJednostkowego] [decimal](10, 4) NULL CONSTRAINT [DF__produkty__Wysoko__6D381B7D]  DEFAULT (NULL),
	[GlebokoscOpakowaniaZbiorczego] [decimal](10, 4) NULL CONSTRAINT [DF__produkty__Glebok__6E2C3FB6]  DEFAULT (NULL),
	[SzerokoscOpakowaniaZbiorczego] [decimal](10, 4) NULL CONSTRAINT [DF__produkty__Szerok__6F2063EF]  DEFAULT (NULL),
	[WysokoscOpakowaniaZbiorczego] [decimal](10, 4) NULL CONSTRAINT [DF__produkty__Wysoko__70148828]  DEFAULT (NULL),
	[CenaWPunktach] [decimal](10, 4) NULL,
 CONSTRAINT [Hprodukty_pk] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]


CREATE TABLE [dbo].[ProduktCecha](
	[Id] [varchar](200) NOT NULL,
	[CechaId] [bigint] NOT NULL,
	[ProduktId] [bigint] NOT NULL,
 CONSTRAINT [PK_cechy_produkty] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY],
 CONSTRAINT [IX_cechy_produkty] UNIQUE NONCLUSTERED 
(
	[CechaId] ASC,
	[ProduktId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]


CREATE TABLE [dbo].[ProduktJednostka](
	[Id] [varchar](200) NOT NULL,
	[ProduktId] [bigint] NOT NULL,
	[JednostkaId] [int] NOT NULL,
	[Podstawowa] [bit] NOT NULL CONSTRAINT [DF_ProduktyJednostki_Podstawowa]  DEFAULT ((1)),
	[PrzelicznikIlosc] [decimal](15, 4) NOT NULL CONSTRAINT [DF_ProduktyJednostki_PrzelicznikIlosc]  DEFAULT ((1)),
 CONSTRAINT [PK_ProduktyJednostki] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]


CREATE TABLE [dbo].[ProduktKategoria](
	[Id] [varchar](50) NOT NULL,
	[ProduktId] [bigint] NOT NULL,
	[KategoriaId] [bigint] NOT NULL,
	[Rodzaj] [int] NOT NULL CONSTRAINT [DF_produkty_kategorie_zrodlowe_rodzaj]  DEFAULT ((1)),
 CONSTRAINT [PK_produkty_kategorie] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY],
 CONSTRAINT [IX_produkty_kategorie] UNIQUE NONCLUSTERED 
(
	[ProduktId] ASC,
	[KategoriaId] ASC,
	[Rodzaj] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]


CREATE TABLE [dbo].[ProduktPlik](
	[Id] [varchar](200) NOT NULL,
	[ProduktId] [bigint] NOT NULL,
	[PlikId] [int] NOT NULL,
	[Glowny] [bit] NOT NULL CONSTRAINT [DF__produkty___czy_d__07020F21]  DEFAULT ((0)),
 CONSTRAINT [PK_produkty_obrazki] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]


CREATE TABLE [dbo].[ProduktStan](
	[Id] [varchar](200) NOT NULL,
	[ProduktId] [bigint] NOT NULL,
	[MagazynId] [int] NOT NULL,
	[Stan] [decimal](10, 2) NOT NULL,
 CONSTRAINT [PK_ProduktStan] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]


CREATE TABLE [dbo].[ProduktUkryty](
	[id] [bigint] NOT NULL,
	[ProduktZrodloId] [bigint] NULL,
	[KlientZrodloId] [bigint] NULL,
	[KategoriaId] [bigint] NULL,
	[Tryb] [varchar](200) NOT NULL CONSTRAINT [DF_produkty_ukryte_tryb_1]  DEFAULT ((0)),
	[PrzedstawicielId] [int] NULL,
	[KategoriaKlientowId] [int] NULL,
	[CechaProduktuId] [int] NULL,
	[Synchronizowane] [bit] NOT NULL CONSTRAINT [DF_produkty_ukryte_Synchronizowane]  DEFAULT ((0)),
 CONSTRAINT [PK_produkty_ukryte_1] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY],
 CONSTRAINT [unikalnosc] UNIQUE NONCLUSTERED 
(
	[KategoriaId] ASC,
	[KategoriaKlientowId] ASC,
	[KlientZrodloId] ASC,
	[ProduktZrodloId] ASC,
	[Tryb] ASC,
	[CechaProduktuId] ASC,
	[PrzedstawicielId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]


CREATE TABLE [dbo].[ProduktyKodyDodatkowe](
	[Id] [int] NOT NULL,
	[ProduktId] [bigint] NOT NULL,
	[Kod] [varchar](500) NULL,
	[Nazwa] [varchar](500) NULL,
	[KlientId] [bigint] NULL,
 CONSTRAINT [PK_ProduktyKodyDodatkowe] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]


CREATE TABLE [dbo].[ProduktyZamienniki](
	[Id] [varchar](200) NOT NULL,
	[ProduktId] [bigint] NOT NULL,
	[ZamiennikId] [bigint] NOT NULL,
 CONSTRAINT [PK_ProduktyZamienniki] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]


CREATE TABLE [dbo].[ProfilKlienta](
	[Id] [varchar](500) NOT NULL,
	[KlientId] [bigint] NULL,
	[Dodatkowe] [varchar](500) NULL,
	[TypUstawienia] [varchar](100) NULL,
	[Wartosc] [nvarchar](500) NOT NULL,
	[Dopisek] [varchar](100) NULL,
 CONSTRAINT [PK_ProfilKlienta] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]


CREATE TABLE [dbo].[PunktyWpisy](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[KlientId] [int] NOT NULL,
	[ZamowienieId] [int] NULL,
	[IloscPunktow] [decimal](10, 2) NOT NULL,
	[Data] [datetime] NOT NULL,
	[Autor] [varchar](50) NOT NULL,
	[Opis] [varchar](2000) NOT NULL,
 CONSTRAINT [PK_PunktyWpisy] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] 


CREATE TABLE [dbo].[Rabat](
	[Id] [varchar](100) NOT NULL,
	[ProduktId] [bigint] NULL,
	[KlientId] [bigint] NULL,
	[PoziomCenyId] [int] NULL,
	[CechaId] [bigint] NULL,
	[WalutaId] [bigint] NULL,
	[TypRabatu] [varchar](50) NOT NULL,
	[TypWartosci] [varchar](20) NOT NULL,
	[OdKiedy] [datetime] NULL,
	[DoKiedy] [datetime] NULL,
	[KategoriaProduktowId] [bigint] NULL,
	[DodanyPrzez] [varchar](80) NULL,
	[KategoriaKlientowId] [int] NULL,
	[Wartosc1] [decimal](10, 2) NULL CONSTRAINT [DF_Hrabaty_wartosc2]  DEFAULT ((0)),
	[Wartosc2] [decimal](10, 2) NULL CONSTRAINT [DF_rabaty_wartosc_roznicowa]  DEFAULT ((0)),
	[Wartosc3] [decimal](10, 2) NULL CONSTRAINT [DF_rabaty_wartosc_procentowa]  DEFAULT ((0)),
	[Wartosc4] [decimal](10, 2) NULL CONSTRAINT [DF_rabaty_wartosc4]  DEFAULT ((0)),
	[Wartosc5] [decimal](10, 2) NULL CONSTRAINT [DF_rabaty_wartosc5]  DEFAULT ((0)),
	[Aktywny] [bit] NOT NULL CONSTRAINT [DF__rabaty__aktywny__7EC1CEDB]  DEFAULT ('true'),
 CONSTRAINT [PK_Hrabaty] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

/****** Object:  Table [dbo].[Region]    Script Date: 2016-02-01 11:44:15 ******/
CREATE TABLE [dbo].[Region](
	[Id] [int] NOT NULL,
	[Nazwa] [varchar](500) NOT NULL,
	[Widoczny] [bit] NOT NULL,
	[KrajId] [int] NOT NULL,
	[Synchronizowane] [bit] NOT NULL,
 CONSTRAINT [PK_Regiony] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]


CREATE TABLE [dbo].[Rejestracja](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Nazwa] [nvarchar](3000) NULL,
	[Email] [nvarchar](100) NOT NULL,
	[Haslo] [nvarchar](100) NULL,
	[Ulica] [nvarchar](100) NULL,
	[Miasto] [nvarchar](100) NULL,
	[Panstwo] [nvarchar](100) NULL,
	[KodPocztowy] [nvarchar](20) NULL,
	[StatusEksportu] [varchar](50) NOT NULL CONSTRAINT [DF_rejestracje_pobrana]  DEFAULT ((0)),
	[Status] [varchar](50) NOT NULL CONSTRAINT [DF_rejestracje_Status]  DEFAULT ((0)),
	[DataRejestracji] [datetime] NOT NULL CONSTRAINT [DF_rejestracje_data_zalozenia]  DEFAULT (getdate()),
	[ImieNazwisko] [nvarchar](150) NULL,
	[NIP] [nvarchar](100) NULL,
	[FakturyElektroniczne] [bit] NOT NULL CONSTRAINT [DF_rejestracje_faktury_elektroniczne]  DEFAULT ((0)),
	[Telefon] [nvarchar](100) NULL,
	[Uwagi] [nvarchar](1000) NULL,
	[Zalacznik1] [nvarchar](3000) NULL,
	[Zalacznik2] [nvarchar](3000) NULL,
	[Zalacznik3] [nvarchar](3000) NULL,
	[HasloJednorazowe] [nvarchar](50) NULL,
	[RodzajDzialalnosci] [nvarchar](100) NULL,
	[WysylkaUlica] [nvarchar](100) NULL,
	[WysylkaKodPocztowy] [nvarchar](100) NULL,
	[WysylkaPanstwo] [nvarchar](100) NULL,
	[WysylkaMiasto] [nvarchar](100) NULL,
	[KlientId] [int] NULL,
	[Polecajcacy] [varchar](2000) NULL,
	[OddzialId] [bigint] NULL,
	[AkceptacjaRegulaminu] [bit] NOT NULL CONSTRAINT [DF_rejestracje_AkceptacjaRegulaminu]  DEFAULT ((1)),
	[AdresIp] [varchar](50) NULL,
	[PrzetwarzanieDanychOsobowych] [bit] NOT NULL CONSTRAINT [DF_rejestracje_PrzetwarzanieDanychOsobowych]  DEFAULT ((1)),
 CONSTRAINT [PK_rejestracje] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

CREATE TABLE [dbo].[Sklep](
	[Id] [int] NOT NULL,
	[Nazwa] [varchar](500) NULL,
	[Aktywny] [bit] NOT NULL CONSTRAINT [DF_sklepy_aktywny]  DEFAULT ((1)),
	[DataUtworzenia] [datetime] NOT NULL,
	[Www] [varchar](800) NULL,
	[ObrazekId] [int] NULL,
	[Opis] [varchar](1000) NULL,
	[AutorId] [bigint] NULL,
	[AutomatyczneKoordynaty] [bit] NOT NULL CONSTRAINT [DF_sklepy_AutomatyczneKoordynaty]  DEFAULT ((1)),
	[KoordynatyZERP] [bit] NOT NULL CONSTRAINT [DF_sklepy_KoordynatyZERP]  DEFAULT ((0)),
	[Siedziba] [bit] NOT NULL CONSTRAINT [DF_sklepy_Siedziba]  DEFAULT ((0)),
	[AdresId] [bigint] NULL,
 CONSTRAINT [PK_sklepy] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]


CREATE TABLE [dbo].[SklepKategoriaSklepu](
	[Id] [varchar](50) NOT NULL,
	[SklepId] [int] NOT NULL,
	[KategoriaSklepuId] [bigint] NOT NULL,
 CONSTRAINT [PK_sklepy_kategorie_polaczenia] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, FILLFACTOR = 80) ON [PRIMARY]
) ON [PRIMARY]

/****** Object:  Table [dbo].[Slajd]    Script Date: 2016-02-01 11:44:15 ******/
CREATE TABLE [dbo].[Slajd](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[Nazwa] [nvarchar](200) NOT NULL,
	[Opis] [nvarchar](2000)  NULL,
	[PlikTlaId] [int] NULL,
	[KolorTla] [varchar](100) NULL,
	[WysokoscTla] [int] NULL
	 CONSTRAINT [PK_Slajd] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]


CREATE TABLE [dbo].[SposobPokazywaniaStanow](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Nazwa] [varchar](100) NOT NULL,
	[DomyslnyMagazynId] [int] NULL,
	[Widocznosc] [varchar](50) NOT NULL,
	[PozycjaLista] [varchar](7000) NULL,
	[PozycjaKarta] [varchar](7000) NULL,
	[KategoriaKlientaMagazyn] [varchar](50) NULL,
	[DozwolonaRolaKlienta] [varchar](7000) NULL,
 CONSTRAINT [PK_sposoby_pokazywania_stanow] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]


CREATE TABLE [dbo].[SposobPokazywaniaStanowRegula](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[SposobId] [int] NOT NULL,
	[Provider] [varchar](200) NOT NULL,
	[Parametry] [varchar](1000) NOT NULL,
	[Kolejnosc] [int] NOT NULL,
	[WynikHtml] [varchar](1000) NOT NULL,
 CONSTRAINT [PK_sposoby_pokazywania_stanow_reguly] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]


CREATE TABLE [dbo].[StatusZamowienia](
	[Id] [int] NOT NULL,
	[Nazwa] [varchar](50) NOT NULL CONSTRAINT [DF_zamowienia_statusy_nazwa]  DEFAULT (''),
	[Importowac] [bit] NOT NULL CONSTRAINT [DF_zamowienia_statusy_importowac]  DEFAULT ((0)),
	[Symbol] [varchar](50) NOT NULL CONSTRAINT [DF_zamowienia_statusy_Symbol]  DEFAULT (''),
	[PobranoErp] [bit] NOT NULL CONSTRAINT [DF_zamowienia_statusy_PobranoErp]  DEFAULT ((0)),
	[Widoczny] [bit] NOT NULL CONSTRAINT [DF_zamowienia_statusy_Widoczna]  DEFAULT ((1)),
	[Kolor] [varchar](200) NULL,
	[KolorCzcionki] [varchar](200) NULL,
	[PowiadomienieZmianaStatusu] [bit] NOT NULL CONSTRAINT [DF_zamowienia_statusy_PowiadomienieZmianaStatusu]  DEFAULT ((0)),
	[PokazujDokumentyStatus] [bit] NOT NULL CONSTRAINT [DF_zamowienia_statusy_PokazujDokumentyStatus]  DEFAULT ((1)),
	[TraktujJakoOferte] [bit] NOT NULL CONSTRAINT [DF_zamowienia_statusy_TraktujJakoOferte]  DEFAULT ((0)),
	[ZawszeZaplacone] [bit] NOT NULL CONSTRAINT [DF_zamowienia_statusy_ZawszeZaplacone]  DEFAULT ((0)),
	[TraktujJakoFaktoring] [bit] NOT NULL CONSTRAINT [DF_zamowienia_statusy_TraktujJakoFaktoring]  DEFAULT ((0)),
 CONSTRAINT [PK_zamowienia_statusy] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]


CREATE TABLE [dbo].[SubkontoGrupa](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Nazwa] [varchar](80) NOT NULL,
	[LimitIlosciZamowien] [int] NULL,
	[LimitWartosciZamowien] [decimal](10, 2) NULL CONSTRAINT [DF_subkonta_grupy_limit_wartosci_zamowien]  DEFAULT ((0)),
	[LimitOkres] [int] NOT NULL CONSTRAINT [DF_subkonta_grupy_limit_okres]  DEFAULT ((1)),
	[Widoczna] [bit] NOT NULL CONSTRAINT [DF_subkonta_grupy_widoczna]  DEFAULT ((1)),
	[KlientId] [int] NULL,
	[LimityLiczoneOdKiedy] [datetime] NULL,
 CONSTRAINT [PK_subkonta_grupy] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]


CREATE TABLE [dbo].[SzablonyEdytorow](
	[Id] [int] NOT NULL,
	[Nazwa] [varchar](max) NOT NULL,
	[Opis] [varchar](max) NOT NULL,
	[Tresc] [varchar](max) NOT NULL,
 CONSTRAINT [PK_szabablony_edytorow] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]


CREATE TABLE [dbo].[Tlumaczenie](
	[Id] [varchar](100) NOT NULL,
	[Typ] [varchar](200) NULL,
	[JezykId] [int] NOT NULL,
	[ObiektId] [varchar](20) NOT NULL CONSTRAINT [DF_slowniki_obiekt_id]  DEFAULT ((0)),
	[Pole] [varchar](4000) NULL,
	[Wpis] [text] NULL,
 CONSTRAINT [PK_slowniki] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]


CREATE TABLE [dbo].[TlumaczeniePole](
	[Id] [varchar](400) NOT NULL,
	[Nazwa] [nvarchar](2000) NOT NULL,
	[Domyslne] [nvarchar](2000) NULL,
 CONSTRAINT [PK_TlumaczeniePole] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]



CREATE TABLE [dbo].[Tresc](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[NadrzednaId] [int] NULL,
	[Nazwa] [nvarchar](500) NOT NULL,
	[Symbol] [varchar](50) NULL,
	[Dostep] [varchar](50) NOT NULL CONSTRAINT [DF_Tresc_Dostep]  DEFAULT ('Wszyscy'),
	[Kolejnosc] [int] NOT NULL CONSTRAINT [DF_Tresc_Kolejnos]  DEFAULT ((0)),
	[Aktywny] [bit] NOT NULL CONSTRAINT [DF_Tresc_Aktywny]  DEFAULT ((1)),
	[AutorId] [int] NULL,
	[MetaOpis] [nvarchar](max) NULL,
	[MetaSlowaKluczowe] [nvarchar](max) NULL,
	[Systemowa] [bit] NOT NULL CONSTRAINT [DF_Tresc_Systemowa]  DEFAULT ((0)),
	[TrescPokazywanaJakoNaglowek] [varchar](50) NULL,
	[TrescPokazywanaJakoStopka] [varchar](50) NULL,
	[TrescPokazywanaJakoLeweMenu] [varchar](50) NULL,
	[PokazujMenu] [bit] NOT NULL CONSTRAINT [DF_Tresc_PokazujMenu]  DEFAULT ((1)),
	[DodatkoweKlasyCss] [varchar](1000) NULL,
	[DodatkoweKlasyCssReczne] [varchar](1000) NULL,
	[TrescPokazywanaJakoReklamaMenu] [varchar](50) NULL,
	[Rola] [varchar](500) NULL,
	[Szerokosc] [int] NOT NULL,
	[LinkAlternatywny] [varchar](50) NULL,
	[LinkAlternatywnyJakoZwyklyTekst] [bit] NULL,
 CONSTRAINT [PK_Tresc] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]




CREATE TABLE [dbo].[TrescKolumna](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[TrescWierszId] [int] NOT NULL,
	[Szerokosc] [int] NOT NULL,
	[KolorTla] [varchar](50) NULL,
	[ObrazekTla] [int] NULL,
	[Kolejnosc] [int] NOT NULL, 
	[DodatkoweKlasyCss]             VARCHAR (1000) CONSTRAINT [DF_TrescPojemnik_RozciagnijCalaSzerokosc] DEFAULT ((0)) NULL,
    [DodatkoweKlasyCssReczne]       VARCHAR (1000) CONSTRAINT [DF_TrescPojemnik_RozciagnijCalaSzerokosc1] DEFAULT ((0)) NULL,
	[RodzajKontrolki] [varchar](1000) NOT NULL,
	[ParametryKontrolkiSpecyficzne] [varchar](max) NULL,
	[Dostep] [varchar](50) NOT NULL,
 CONSTRAINT [PK_TrescPojemnik] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]


CREATE TABLE [dbo].[TrescWiersz](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[TrescId] [int] NOT NULL,
	[Kolejnosc] [int] NOT NULL,
	[ObrazekTla] [int] NULL,
	[KolorTla] [varchar](50) NULL,
	[MarginesGora] [int] NULL,
	[MarginesDol] [int] NULL,
	[RozciagnijCalaSzerokosc] [bit] NOT NULL CONSTRAINT [DF_TrescWiersz_RozciagnijCalaSzerokosc]  DEFAULT ((0)),
	[DodatkoweKlasyCss] [varchar](1000) NULL,
	[DodatkoweKlasyCssReczne] [varchar](1000) NULL,
	[Wyrownanie] [varchar](50) NULL,
	[Dostep] [varchar](50) NOT NULL,
	[Szerokosc] [int] NULL,
	[OpisKontenera] [varchar](1000) NULL,
 CONSTRAINT [PK_TrescWiersz] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]


CREATE TABLE [dbo].[TypMailingu](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Nazwa] [varchar](100) NOT NULL,
	[Aktywny] [bit] NOT NULL,
 CONSTRAINT [PK_TypMailingu] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]


CREATE TABLE [dbo].[UkladKolumn](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[TypDanych] [varchar](300) NOT NULL,
	[Nazwa] [nvarchar](500) NOT NULL,
	[WidoczneKolumny] [varchar](4000) NOT NULL,
 CONSTRAINT [PK_UkladKolumn] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]


CREATE TABLE [dbo].[Ustawienie](
	[Id] [varchar](500) NOT NULL,
	[Nazwa] [varchar](100) NOT NULL,
	[Wartosc] [varchar](5000) NULL,
	[Symbol] [varchar](100) NOT NULL,
	[Widoczne] [bit] NOT NULL CONSTRAINT [DF_ustawienia_widoczne]  DEFAULT ((0)),
	[Grupa] [varchar](200) NOT NULL,
	[Opis] [varchar](5000) NULL,
	[Typ] [varchar](20) NOT NULL CONSTRAINT [DF_ustawienia_typ]  DEFAULT ('string'),
	[Slownik] [varchar](5000) NULL,
	[Multiwartosc] [bit] NOT NULL CONSTRAINT [DF_ustawienia_multiwartosc]  DEFAULT ((0)),
	[NadpisywanyPracownik] [bit] NULL CONSTRAINT [DF__ustawieni__nadpi__4B4D17CD]  DEFAULT ((0)),
	[OddzialId] [int] NULL,
	[OpisGrupy] [varchar](5000) NULL,
	[WartoscDlaNiezalogowanych] [varchar](5000) NULL,
	[Podgrupa] [varchar](200) NULL,
	[PoprzedniaWartosc] [varchar](5000) NULL,
	[PoprzedniaWartoscDlaNiezalogowanych] [varchar](5000) NULL,
	[Dynamiczne] [bit] NOT NULL CONSTRAINT [DF_ustawienia_Dynamiczne]  DEFAULT ((0)),
	[PodGrupaTekstowa] [varchar](200) NULL,
	[WartoscDomyslna] [varchar](500) NULL,
	[WartoscDomyslnaDlaNiezalogowanych] [varchar](500) NULL,
 CONSTRAINT [PK_Ustawienie] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY],
 CONSTRAINT [IX_ustawienia_symbol_pracownik] UNIQUE NONCLUSTERED 
(
	[Symbol] ASC,
	[OddzialId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]


CREATE TABLE [dbo].[Waluta](
	[Id] [bigint] NOT NULL,
	[WalutaErp] [nvarchar](20) NOT NULL,
	[WalutaB2b] [nvarchar](20) NOT NULL,
	[NrKonta] [varchar](100) NULL,
 CONSTRAINT [PK_Waluta] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]


CREATE TABLE [dbo].[WidocznosciTypow](
	[Id] [varchar](200) NOT NULL,
	[ObiektId] [varchar](200) NULL,
	[Typ] [varchar](200) NULL,
	[KategoriaKlientaIdWszystkie] [varchar](200) NOT NULL,
	[Kierunek] [varchar](50) NOT NULL,
	[KategoriaKlientaIdKtorakolwiek] [varchar](200) NOT NULL,
	[Nazwa] [varchar](500) NULL,
 CONSTRAINT [PK_WidocznosciTypow] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]


CREATE TABLE [dbo].[Zadanie](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[NumerElementuSynchronizacji] [int] NULL,
	[ModulFullTypeName] [varchar](500) NULL,
	[OstatnieUruchomienieStart] [datetime] NULL CONSTRAINT [DF_zadania_ostatnieUruchomienie]  DEFAULT (getdate()),
	[OstatnieUruchomienieKoniec] [datetime] NULL,
	[MozeDzialacOdGodziny] [int] NOT NULL CONSTRAINT [DF_zadania_MozeDzialacOdGodziny]  DEFAULT ((0)),
	[MozeDzialacDoGodziny] [int] NOT NULL CONSTRAINT [DF_zadania_MozeDzialacDoGodziny]  DEFAULT ((24)),
	[IleMinutCzekacDoKolejnegoUruchomienia] [int] NOT NULL CONSTRAINT [DF_zadania_IleMinutCzekacDoKolejnegoUruchomienia]  DEFAULT ((60)),
	[Aktywne] [bit] NOT NULL CONSTRAINT [DF_zadania_Aktywne]  DEFAULT ((1)),
	[Parametry] [varchar](8000) NULL,
	[ModulKolejnosc] [int] NOT NULL CONSTRAINT [DF_zadania_kolejnosc]  DEFAULT ((0)),
	[ZadanieNadrzedne] [int] NULL,
	[ModulWymagany] [bit] NOT NULL CONSTRAINT [DF_zadania_ModulWymagany]  DEFAULT ((0)),
	[OddzialId] [bigint] NULL,
	[Centralne] [bit] NOT NULL CONSTRAINT [DF_zadania_Centralne]  DEFAULT ((0)),
	[Usuniente] [bit] NOT NULL CONSTRAINT [DF_zadania_Usuniente]  DEFAULT ((0)),
 CONSTRAINT [PK_zadania] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]


CREATE TABLE [dbo].[Zamowienie](
	[Id] [bigint] NOT NULL,
	[KlientId] [bigint] NOT NULL,
	[DataUtworzenia] [datetime] NULL,
	[StatusId] [int] NOT NULL,
	[Uwagi] [nvarchar](3000) NULL,
	[PoziomCenyId] [int] NULL,
	[WartoscNetto] [decimal](10, 4) NOT NULL CONSTRAINT [DF_zamowienia_WartoscNetto]  DEFAULT ((0)),
	[WartoscBrutto] [decimal](10, 4) NOT NULL CONSTRAINT [DF_zamowienia_WartoscNetto1]  DEFAULT ((0)),
	[MagazynRealizujacy] [varchar](50) NULL,
	[DokumentyId] [varchar](300) NULL CONSTRAINT [DF_zamowienia_IdDokumentu]  DEFAULT (NULL),
	[AdresId] [int] NULL,
	[WalutaId] [bigint] NULL,
	[TerminDostawy] [datetime] NULL,
	[MagazynPodstawowy] [varchar](50) NULL,
	[BladKomunikat] [varchar](4000) NULL,
	[KategoriaZamowienia] [varchar](200) NULL,
	[PracownikSkladajacyId] [bigint] NULL,
	[NazwaPlatnosci] [varchar](200) NULL,
	[NumerZamowieniaKlienta] [varchar](200) NULL,
	[PlatnikNazwa] [nvarchar](500) NULL,
	[PlatnikNip] [varchar](50) NULL,
	[PlatnikUlica] [nvarchar](2000) NULL,
	[PlatnikMiasto] [nvarchar](2000) NULL,
	[PlatnikKodPocztowy] [varchar](50) NULL,
	[PlatnikKraj] [nvarchar](500) NULL,
	[PlatnikTelefon] [varchar](50) NULL,
	[DokumentNazwaSynchronizacja] [varchar](200) NULL,
	[DodatkowePola] [varchar](max) NULL,
	[DefinicjaDokumentuERP] [varchar](20) NULL,
 CONSTRAINT [PK_Hzamowienia] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]


CREATE TABLE [dbo].[ZamowienieProdukt](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[ZamowienieId] [bigint] NOT NULL,
	[ProduktId] [bigint] NOT NULL,
	[Ilosc] [decimal](10, 2) NOT NULL CONSTRAINT [DF_Hzamowienia_produkty_ilosc]  DEFAULT ((1)),
	[CenaNetto] [decimal](10, 4) NOT NULL,
	[CenaBrutto] [decimal](10, 4) NOT NULL CONSTRAINT [DF__zamowieni__cena___5A1A5A11]  DEFAULT ((0)),
	[Jednostka] [varchar](200) NULL,
	[JednostkaPrzelicznik] [decimal](10, 5) NOT NULL CONSTRAINT [DF__zamowieni__jedno__5B0E7E4A]  DEFAULT ((1)),
	[PrzedstawicielId] [bigint] NULL,
	[DataZmiany] [datetime] NULL,
	[Opis] [varchar](2000) NULL,
	[Opis2] [varchar](2000) NULL,
	[TypPozycji] [varchar](200) NULL,
 CONSTRAINT [PK_Hzamowienia_produkty] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]


CREATE TABLE [dbo].[Zdarzenie](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[ModulFullTypeName] [varchar](500) NOT NULL,
	[ParametryWysylania] [varchar](max) NULL,
 CONSTRAINT [PK_zdarzenia] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

SET ANSI_PADDING ON


CREATE NONCLUSTERED INDEX [cechy_atrybut_id] ON [dbo].[Cecha]
(
	[AtrybutId] ASC
)
INCLUDE ( 	[Id],
	[Nazwa],
	[Widoczna],
	[Symbol],
	[Kolejnosc]) WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]

	
CREATE UNIQUE NONCLUSTERED INDEX [IX_flat_ceny] ON [dbo].[FlatCeny]
(
	[KlientId] ASC,
	[ProduktId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, FILLFACTOR = 80) ON [PRIMARY]


CREATE NONCLUSTERED INDEX [klientid] ON [dbo].[HistoriaDokumentu]
(
	[KlientId] ASC,
	[Rodzaj] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]



CREATE NONCLUSTERED INDEX [dok_id_non_clast] ON [dbo].[HistoriaDokumentuProdukt]
(
	[DokumentId] ASC
)
INCLUDE ( 	[Id],
	[KodProduktu],
	[NazwaProduktu],
	[Ilosc],
	[JednostkaMiary],
	[CenaBrutto],
	[CenaNetto],
	[WartoscBrutto],
	[WartoscNetto],
	[Vat],
	[WartoscVat],
	[Parametry],
	[Rabat],
	[ProduktId],
	[Opis],
	[CenaNettoPoRabacie],
	[CenaBruttoPoRabacie],
	[Opis2]) WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]

	
	
CREATE NONCLUSTERED INDEX [klucz_sesji_nonclas] ON [dbo].[Klient]
(
	[KluczSesji] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]


CREATE NONCLUSTERED INDEX [data_index_nonclas] ON [dbo].[LogWpis]
(
	[Data] ASC
)
INCLUDE ( 	[Id]) WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]


CREATE NONCLUSTERED INDEX [index_produkty_rodzina_ojciec] ON [dbo].[Produkt]
(
	[Rodzina] ASC,
	[Ojciec] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]


CREATE NONCLUSTERED INDEX [cechy_id_produktid] ON [dbo].[ProduktCecha]
(
	[CechaId] ASC
)
INCLUDE ( 	[ProduktId]) WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]


CREATE NONCLUSTERED INDEX [kategorieZrodlowe_produktId] ON [dbo].[ProduktKategoria]
(
	[KategoriaId] ASC
)
INCLUDE ( 	[ProduktId]) WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]


CREATE NONCLUSTERED INDEX [idx_pu] ON [dbo].[ProduktUkryty]
(
	[KlientZrodloId] ASC,
	[Tryb] ASC,
	[ProduktZrodloId] ASC,
	[CechaProduktuId] ASC,
	[KategoriaId] ASC,
	[KategoriaKlientowId] ASC,
	[PrzedstawicielId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]


CREATE UNIQUE NONCLUSTERED INDEX [IX_ProduktyZamienniki] ON [dbo].[ProduktyZamienniki]
(
	[ProduktId] ASC,
	[ZamiennikId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]

CREATE UNIQUE NONCLUSTERED INDEX [ids_tresc_symbol]
    ON [dbo].[Tresc]([Symbol] ASC) WHERE ([symbol] IS NOT NULL);

CREATE UNIQUE NONCLUSTERED INDEX [ids_widocznosci_typow_nazwa]
    ON [dbo].[WidocznosciTypow]([Nazwa] ASC) WHERE ([nazwa] IS NOT NULL);

CREATE NONCLUSTERED INDEX [IX_zamowienia_produkty] ON [dbo].[ZamowienieProdukt]
(
	[ZamowienieId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]


CREATE UNIQUE NONCLUSTERED INDEX [IX_zdarzenia] ON [dbo].[Zdarzenie]
(
	[ModulFullTypeName] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
ALTER TABLE [dbo].[FlatCeny] ADD  CONSTRAINT [DF_flat_ceny_cena_netto]  DEFAULT ((0)) FOR [CenaNetto]
ALTER TABLE [dbo].[FlatCeny] ADD  CONSTRAINT [DF_flat_ceny_typ_rabatu]  DEFAULT ((-1)) FOR [TypRabatu]
ALTER TABLE [dbo].[FlatCeny] ADD  CONSTRAINT [DF__flat_ceny__rabat__59C55456]  DEFAULT ((0)) FOR [Rabat]
ALTER TABLE [dbo].[KlientLimitIlosciowy] ADD  CONSTRAINT [DF_klienci_limity_ilosciowe_Ilosc]  DEFAULT ((0)) FOR [Ilosc]
ALTER TABLE [dbo].[MaileBledneDoPonownejWysylki] ADD  CONSTRAINT [DF_MaileBledneDoPonownejWysylki_IloscBledow]  DEFAULT ((0)) FOR [IloscBledow]
ALTER TABLE [dbo].[ProduktyKodyDodatkowe] ADD  CONSTRAINT [DF_ProduktyKodyKreskowe_Podstawowy]  DEFAULT ((0)) FOR [Nazwa]
ALTER TABLE [dbo].[SposobPokazywaniaStanow] ADD  CONSTRAINT [DF_sposoby_pokazywania_stanow_Widocznosc]  DEFAULT ('Wszyscy') FOR [Widocznosc]
ALTER TABLE [dbo].[SposobPokazywaniaStanowRegula] ADD  CONSTRAINT [DF_sposoby_pokazywania_stanow_reguly_kolejnosc]  DEFAULT ((0)) FOR [Kolejnosc]
ALTER TABLE [dbo].[TypMailingu] ADD  CONSTRAINT [DF_TypMailingu_Aktywny]  DEFAULT ((0)) FOR [Aktywny]
ALTER TABLE [dbo].[Adres]  WITH CHECK ADD  CONSTRAINT [FK_Adresy_Kraje] FOREIGN KEY([KrajId])
REFERENCES [dbo].[Kraje] ([Id])
ALTER TABLE [dbo].[Adres] CHECK CONSTRAINT [FK_Adresy_Kraje]
ALTER TABLE [dbo].[Adres]  WITH CHECK ADD  CONSTRAINT [FK_Adresy_Regiony] FOREIGN KEY([RegionId])
REFERENCES [dbo].[Region] ([Id])
ON DELETE SET NULL
ALTER TABLE [dbo].[Adres] CHECK CONSTRAINT [FK_Adresy_Regiony]


ALTER TABLE [dbo].[BlogWpisBlogKategoria]  WITH CHECK ADD  CONSTRAINT [FK_BlogWpisBlogKategoria_BlogKategoria] FOREIGN KEY([BlogKategoriaId])
REFERENCES [dbo].[BlogKategoria] ([Id])
ALTER TABLE [dbo].[BlogWpisBlogKategoria] CHECK CONSTRAINT [FK_BlogWpisBlogKategoria_BlogKategoria]

ALTER TABLE [dbo].[BlogWpisBlogKategoria]  WITH CHECK ADD  CONSTRAINT [FK_BlogWpisBlogKategoria_BlogWpis] FOREIGN KEY([BlogWpisId])
REFERENCES [dbo].[BlogWpis] ([Id]) ON DELETE CASCADE

ALTER TABLE [dbo].[BlogWpisBlogKategoria] CHECK CONSTRAINT [FK_BlogWpisBlogKategoria_BlogWpis]
ALTER TABLE [dbo].[Cecha]  WITH CHECK ADD  CONSTRAINT [FK_cechy_atrybuty] FOREIGN KEY([AtrybutId])
REFERENCES [dbo].[Atrybut] ([Id])
ON DELETE SET NULL
ALTER TABLE [dbo].[Cecha] CHECK CONSTRAINT [FK_cechy_atrybuty]
ALTER TABLE [dbo].[Cecha]  WITH CHECK ADD  CONSTRAINT [FK_cechy_obrazki] FOREIGN KEY([ObrazekId])
REFERENCES [dbo].[Plik] ([Id])
ON UPDATE CASCADE
ON DELETE SET NULL
ALTER TABLE [dbo].[Cecha] CHECK CONSTRAINT [FK_cechy_obrazki]
ALTER TABLE [dbo].[FlatCeny]  WITH CHECK ADD  CONSTRAINT [FK_flat_ceny_produkty] FOREIGN KEY([ProduktId])
REFERENCES [dbo].[Produkt] ([Id])
ON DELETE CASCADE
ALTER TABLE [dbo].[FlatCeny] CHECK CONSTRAINT [FK_flat_ceny_produkty]
ALTER TABLE [dbo].[Grupa]  WITH NOCHECK ADD  CONSTRAINT [FK_grupa_grupa_obrazek] FOREIGN KEY([ObrazekId])
REFERENCES [dbo].[Plik] ([Id])
ALTER TABLE [dbo].[Grupa] CHECK CONSTRAINT [FK_grupa_grupa_obrazek]
ALTER TABLE [dbo].[HistoriaDokumentu]  WITH CHECK ADD  CONSTRAINT [FK_historia_dokumenty_klienci] FOREIGN KEY([KlientId])
REFERENCES [dbo].[Klient] ([Id])
ON DELETE CASCADE
ALTER TABLE [dbo].[HistoriaDokumentu] CHECK CONSTRAINT [FK_historia_dokumenty_klienci]
ALTER TABLE [dbo].[HistoriaDokumentu]  WITH CHECK ADD  CONSTRAINT [FK_historia_dokumenty_zamowienia_statusy] FOREIGN KEY([StatusId])
REFERENCES [dbo].[StatusZamowienia] ([Id])
ON DELETE SET NULL
ALTER TABLE [dbo].[HistoriaDokumentu] CHECK CONSTRAINT [FK_historia_dokumenty_zamowienia_statusy]
ALTER TABLE [dbo].[HistoriaDokumentuListPrzewozowy]  WITH NOCHECK ADD  CONSTRAINT [FK_historia_dokumenty_listy_przewozowe_historia_dokumenty] FOREIGN KEY([DokumentId])
REFERENCES [dbo].[HistoriaDokumentu] ([Id])
ON DELETE CASCADE
ALTER TABLE [dbo].[HistoriaDokumentuListPrzewozowy] CHECK CONSTRAINT [FK_historia_dokumenty_listy_przewozowe_historia_dokumenty]

ALTER TABLE [dbo].[Jezyk]  WITH NOCHECK ADD  CONSTRAINT [FK_jezyki_obrazki] FOREIGN KEY([ObrazekId])
REFERENCES [dbo].[Plik] ([Id])
ON DELETE SET NULL
ALTER TABLE [dbo].[Jezyk] CHECK CONSTRAINT [FK_jezyki_obrazki]

ALTER TABLE [dbo].[KatalogSzablon]  WITH NOCHECK ADD  CONSTRAINT [FK_szablony_obrazki] FOREIGN KEY([Obrazek0Id])
REFERENCES [dbo].[Plik] ([Id])
ON UPDATE CASCADE
ON DELETE CASCADE
ALTER TABLE [dbo].[KatalogSzablon] CHECK CONSTRAINT [FK_szablony_obrazki]


ALTER TABLE [dbo].[KatalogSzablon]  WITH NOCHECK ADD  CONSTRAINT [FK_szablony_obrazki1] FOREIGN KEY([Obrazek1Id])REFERENCES [dbo].[Plik] ([Id])

ALTER TABLE [dbo].[KatalogSzablon] CHECK CONSTRAINT [FK_szablony_obrazki1]
ALTER TABLE [dbo].[KatalogSzablon]  WITH NOCHECK ADD  CONSTRAINT [FK_szablony_obrazki2] FOREIGN KEY([Obrazek2Id])
REFERENCES [dbo].[Plik] ([Id])
ALTER TABLE [dbo].[KatalogSzablon] CHECK CONSTRAINT [FK_szablony_obrazki2]
ALTER TABLE [dbo].[KatalogSzablon]  WITH NOCHECK ADD  CONSTRAINT [FK_szablony_obrazki3] FOREIGN KEY([Obrazek3Id])
REFERENCES [dbo].[Plik] ([Id])
ALTER TABLE [dbo].[KatalogSzablon] CHECK CONSTRAINT [FK_szablony_obrazki3]
ALTER TABLE [dbo].[KatalogSzablon]  WITH NOCHECK ADD  CONSTRAINT [FK_szablony_obrazki4] FOREIGN KEY([Obrazek4Id])
REFERENCES [dbo].[Plik] ([Id])
ALTER TABLE [dbo].[KatalogSzablon] CHECK CONSTRAINT [FK_szablony_obrazki4]
ALTER TABLE [dbo].[KatalogSzablon]  WITH NOCHECK ADD  CONSTRAINT [FK_szablony_obrazki5] FOREIGN KEY([Obrazek5Id])
REFERENCES [dbo].[Plik] ([Id])
ALTER TABLE [dbo].[KatalogSzablon] CHECK CONSTRAINT [FK_szablony_obrazki5]

ALTER TABLE [dbo].[KategoriaProduktu]  WITH NOCHECK ADD  CONSTRAINT [FK_kategorie_grupy] FOREIGN KEY([GrupaId])
REFERENCES [dbo].[Grupa] ([Id])
ON DELETE CASCADE
ALTER TABLE [dbo].[KategoriaProduktu] CHECK CONSTRAINT [FK_kategorie_grupy]
ALTER TABLE [dbo].[KategoriaProduktu]  WITH NOCHECK ADD  CONSTRAINT [FK_kategorie_kategorie] FOREIGN KEY([ParentId])
REFERENCES [dbo].[KategoriaProduktu] ([Id])
ALTER TABLE [dbo].[KategoriaProduktu] CHECK CONSTRAINT [FK_kategorie_kategorie]
ALTER TABLE [dbo].[KategoriaProduktu]  WITH NOCHECK ADD  CONSTRAINT [FK_kategorie_obrazki] FOREIGN KEY([MiniaturaId])
REFERENCES [dbo].[Plik] ([Id])
ALTER TABLE [dbo].[KategoriaProduktu] CHECK CONSTRAINT [FK_kategorie_obrazki]
ALTER TABLE [dbo].[KategoriaProduktu]  WITH NOCHECK ADD  CONSTRAINT [FK_kategorie_Plik] FOREIGN KEY([MiniaturaId])
REFERENCES [dbo].[Plik] ([Id])
ALTER TABLE [dbo].[KategoriaProduktu] CHECK CONSTRAINT [FK_kategorie_Plik]
ALTER TABLE [dbo].[KategoriaProduktu]  WITH NOCHECK ADD  CONSTRAINT [kategorie_fk2] FOREIGN KEY([ObrazekId])
REFERENCES [dbo].[Plik] ([Id])
ON DELETE SET NULL
ALTER TABLE [dbo].[KategoriaProduktu] CHECK CONSTRAINT [kategorie_fk2]

ALTER TABLE [dbo].[KategoriaSklepu]  WITH CHECK ADD  CONSTRAINT [FK_sklepy_kategorie_Plik] FOREIGN KEY([ObrazekPineskaId])
REFERENCES [dbo].[Plik] ([Id])
ALTER TABLE [dbo].[KategoriaSklepu] CHECK CONSTRAINT [FK_sklepy_kategorie_Plik]
ALTER TABLE [dbo].[Klient]  WITH NOCHECK ADD  CONSTRAINT [FK_klienci_jezyki] FOREIGN KEY([JezykId])
REFERENCES [dbo].[Jezyk] ([Id])
ON DELETE SET NULL
ALTER TABLE [dbo].[Klient] CHECK CONSTRAINT [FK_klienci_jezyki]
ALTER TABLE [dbo].[Klient]  WITH NOCHECK ADD  CONSTRAINT [FK_klienci_klienci] FOREIGN KEY([KlientNadrzednyId])
REFERENCES [dbo].[Klient] ([Id])
ALTER TABLE [dbo].[Klient] CHECK CONSTRAINT [FK_klienci_klienci]
ALTER TABLE [dbo].[Klient]  WITH NOCHECK ADD  CONSTRAINT [FK_klienci_klienci1] FOREIGN KEY([KontoPotwierdzajaceId])
REFERENCES [dbo].[Klient] ([Id])
ALTER TABLE [dbo].[Klient] CHECK CONSTRAINT [FK_klienci_klienci1]
ALTER TABLE [dbo].[Klient]  WITH NOCHECK ADD  CONSTRAINT [FK_klienci_klienci2] FOREIGN KEY([PrzedstawicielId])
REFERENCES [dbo].[Klient] ([Id])
ALTER TABLE [dbo].[Klient] CHECK CONSTRAINT [FK_klienci_klienci2]
ALTER TABLE [dbo].[Klient]  WITH CHECK ADD  CONSTRAINT [FK_klienci_Plik] FOREIGN KEY([ZdjecieId])
REFERENCES [dbo].[Plik] ([Id])
ON DELETE SET NULL
ALTER TABLE [dbo].[Klient] CHECK CONSTRAINT [FK_klienci_Plik]
ALTER TABLE [dbo].[Klient]  WITH NOCHECK ADD  CONSTRAINT [FK_klienci_pracownicy1] FOREIGN KEY([OpiekunId])
REFERENCES [dbo].[Klient] ([Id])
ALTER TABLE [dbo].[Klient] CHECK CONSTRAINT [FK_klienci_pracownicy1]
ALTER TABLE [dbo].[Klient]  WITH NOCHECK ADD  CONSTRAINT [FK_klienci_subkonta_grupy] FOREIGN KEY([SubkontoGrupaId])
REFERENCES [dbo].[SubkontoGrupa] ([Id])
ON DELETE SET NULL
ALTER TABLE [dbo].[Klient] CHECK CONSTRAINT [FK_klienci_subkonta_grupy]
ALTER TABLE [dbo].[Klient]  WITH CHECK ADD  CONSTRAINT [FK_Klient_PoziomCenowy] FOREIGN KEY([PoziomCenowyId])
REFERENCES [dbo].[PoziomCenowy] ([Id])
ALTER TABLE [dbo].[Klient] CHECK CONSTRAINT [FK_Klient_PoziomCenowy]
ALTER TABLE [dbo].[Klient]  WITH NOCHECK ADD  CONSTRAINT [FK_pracownicy_drugi_opiekun] FOREIGN KEY([DrugiOpiekunId])
REFERENCES [dbo].[Klient] ([Id])
ALTER TABLE [dbo].[Klient] CHECK CONSTRAINT [FK_pracownicy_drugi_opiekun]
ALTER TABLE [dbo].[Klient]  WITH CHECK ADD  CONSTRAINT [FK_zastepca] FOREIGN KEY([ZastepcaId])
REFERENCES [dbo].[Klient] ([Id])
ALTER TABLE [dbo].[Klient] CHECK CONSTRAINT [FK_zastepca]
ALTER TABLE [dbo].[KlientKategoriaKlienta]  WITH NOCHECK ADD  CONSTRAINT [FK_klienci_kategorie_kategorie_klientow] FOREIGN KEY([KategoriaKlientaId])
REFERENCES [dbo].[KategoriaKlienta] ([Id])
ON UPDATE CASCADE
ON DELETE CASCADE
ALTER TABLE [dbo].[KlientKategoriaKlienta] CHECK CONSTRAINT [FK_klienci_kategorie_kategorie_klientow]
ALTER TABLE [dbo].[KlientLimitIlosciowy]  WITH NOCHECK ADD  CONSTRAINT [FK_klienci_limity_ilosciowe_klienci] FOREIGN KEY([KlientId])
REFERENCES [dbo].[Klient] ([Id])
ON DELETE CASCADE
ALTER TABLE [dbo].[KlientLimitIlosciowy] CHECK CONSTRAINT [FK_klienci_limity_ilosciowe_klienci]
ALTER TABLE [dbo].[KlientLimitIlosciowy]  WITH NOCHECK ADD  CONSTRAINT [FK_klienci_limity_ilosciowe_produkty] FOREIGN KEY([ProduktId])
REFERENCES [dbo].[Produkt] ([Id])
ON DELETE CASCADE
ALTER TABLE [dbo].[KlientLimitIlosciowy] CHECK CONSTRAINT [FK_klienci_limity_ilosciowe_produkty]
ALTER TABLE [dbo].[Konfekcje]  WITH CHECK ADD  CONSTRAINT [FK_Konfekcje_cechy] FOREIGN KEY([CechaId])
REFERENCES [dbo].[Cecha] ([Id])
ON DELETE CASCADE
ALTER TABLE [dbo].[Konfekcje] CHECK CONSTRAINT [FK_Konfekcje_cechy]
ALTER TABLE [dbo].[Konfekcje]  WITH NOCHECK ADD  CONSTRAINT [FK_Konfekcje_kategorie_klientow] FOREIGN KEY([KategoriaKlientowId])
REFERENCES [dbo].[KategoriaKlienta] ([Id])
ON DELETE CASCADE
ALTER TABLE [dbo].[Konfekcje] CHECK CONSTRAINT [FK_Konfekcje_kategorie_klientow]
ALTER TABLE [dbo].[Konfekcje]  WITH NOCHECK ADD  CONSTRAINT [FK_Konfekcje_produkty] FOREIGN KEY([ProduktId])
REFERENCES [dbo].[Produkt] ([Id])
ON DELETE CASCADE
ALTER TABLE [dbo].[Konfekcje] CHECK CONSTRAINT [FK_Konfekcje_produkty]
ALTER TABLE [dbo].[KoszykPozycje]  WITH CHECK ADD  CONSTRAINT [FK_koszyki_pozycje_Jednostki] FOREIGN KEY([JednostkaId])
REFERENCES [dbo].[Jednostka] ([Id])
ON DELETE CASCADE
ALTER TABLE [dbo].[KoszykPozycje] CHECK CONSTRAINT [FK_koszyki_pozycje_Jednostki]
ALTER TABLE [dbo].[KoszykPozycje]  WITH NOCHECK ADD  CONSTRAINT [FK_koszyki_pozycje_koszyki] FOREIGN KEY([KoszykId])
REFERENCES [dbo].[Koszyk] ([Id])
ON DELETE CASCADE
ALTER TABLE [dbo].[KoszykPozycje] CHECK CONSTRAINT [FK_koszyki_pozycje_koszyki]
ALTER TABLE [dbo].[KoszykPozycje]  WITH NOCHECK ADD  CONSTRAINT [FK_koszyki_produkty] FOREIGN KEY([ProduktId])
REFERENCES [dbo].[Produkt] ([Id])
ON DELETE CASCADE
ALTER TABLE [dbo].[KoszykPozycje] CHECK CONSTRAINT [FK_koszyki_produkty]
ALTER TABLE [dbo].[KoszykPozycje]  WITH CHECK ADD  CONSTRAINT [FK_KoszykPozycje_Klient] FOREIGN KEY([PrzedstawicielId])
REFERENCES [dbo].[Klient] ([Id])
ALTER TABLE [dbo].[KoszykPozycje] CHECK CONSTRAINT [FK_KoszykPozycje_Klient]
ALTER TABLE [dbo].[KupowaneIlosci]  WITH NOCHECK ADD  CONSTRAINT [FK_KupowaneIlosci_klienci] FOREIGN KEY([KlientId])
REFERENCES [dbo].[Klient] ([Id])
ON DELETE CASCADE
ALTER TABLE [dbo].[KupowaneIlosci] CHECK CONSTRAINT [FK_KupowaneIlosci_klienci]
ALTER TABLE [dbo].[KupowaneIlosci]  WITH NOCHECK ADD  CONSTRAINT [FK_KupowaneIlosci_produkty] FOREIGN KEY([ProduktId])
REFERENCES [dbo].[Produkt] ([Id])
ON DELETE CASCADE
ALTER TABLE [dbo].[KupowaneIlosci] CHECK CONSTRAINT [FK_KupowaneIlosci_produkty]
ALTER TABLE [dbo].[PieczatkiSzablony]  WITH CHECK ADD  CONSTRAINT [FK_PieczatkiSzablony_PieczatkiTyp] FOREIGN KEY([TypId])
REFERENCES [dbo].[PieczatkiTyp] ([Id])
ALTER TABLE [dbo].[PieczatkiSzablony] CHECK CONSTRAINT [FK_PieczatkiSzablony_PieczatkiTyp]
ALTER TABLE [dbo].[Produkt]  WITH NOCHECK ADD  CONSTRAINT [FK_produkty_obrazki] FOREIGN KEY([ObrazekId])
REFERENCES [dbo].[Plik] ([Id])
ON DELETE SET NULL
ALTER TABLE [dbo].[Produkt] CHECK CONSTRAINT [FK_produkty_obrazki]
ALTER TABLE [dbo].[ProduktCecha]  WITH NOCHECK ADD  CONSTRAINT [FK_cechy_produkty_produkty] FOREIGN KEY([ProduktId])
REFERENCES [dbo].[Produkt] ([Id])
ON DELETE CASCADE
ALTER TABLE [dbo].[ProduktCecha] CHECK CONSTRAINT [FK_cechy_produkty_produkty]
ALTER TABLE [dbo].[ProduktCecha]  WITH CHECK ADD  CONSTRAINT [FK_Hcechy_produkty_Hcechy_produktow] FOREIGN KEY([CechaId])
REFERENCES [dbo].[Cecha] ([Id])
ON DELETE CASCADE
ALTER TABLE [dbo].[ProduktCecha] CHECK CONSTRAINT [FK_Hcechy_produkty_Hcechy_produktow]

ALTER TABLE [dbo].[ProduktJednostka]  WITH NOCHECK ADD  CONSTRAINT [FK_ProduktyJednostki_Jednostki] FOREIGN KEY([JednostkaId])
REFERENCES [dbo].[Jednostka] ([Id])
ON DELETE CASCADE
ALTER TABLE [dbo].[ProduktJednostka] CHECK CONSTRAINT [FK_ProduktyJednostki_Jednostki]

ALTER TABLE [dbo].[ProduktJednostka]  WITH NOCHECK ADD  CONSTRAINT [FK_ProduktyJednostki_produkty] FOREIGN KEY([ProduktId])
REFERENCES [dbo].[Produkt] ([Id])
ON DELETE CASCADE
ALTER TABLE [dbo].[ProduktJednostka] CHECK CONSTRAINT [FK_ProduktyJednostki_produkty]
ALTER TABLE [dbo].[ProduktKategoria]  WITH NOCHECK ADD  CONSTRAINT [FK_produkty_kategorie_zrodlowe_kategorie_zrodlowe] FOREIGN KEY([KategoriaId])
REFERENCES [dbo].[KategoriaProduktu] ([Id])
ON DELETE CASCADE
ALTER TABLE [dbo].[ProduktKategoria] CHECK CONSTRAINT [FK_produkty_kategorie_zrodlowe_kategorie_zrodlowe]
ALTER TABLE [dbo].[ProduktKategoria]  WITH NOCHECK ADD  CONSTRAINT [FK_produkty_kategorie_zrodlowe_produkty] FOREIGN KEY([ProduktId])
REFERENCES [dbo].[Produkt] ([Id])
ON DELETE CASCADE
ALTER TABLE [dbo].[ProduktKategoria] CHECK CONSTRAINT [FK_produkty_kategorie_zrodlowe_produkty]
ALTER TABLE [dbo].[ProduktPlik]  WITH NOCHECK ADD  CONSTRAINT [FK_produkty_obrazki_produkty] FOREIGN KEY([ProduktId])
REFERENCES [dbo].[Produkt] ([Id])
ON DELETE CASCADE
ALTER TABLE [dbo].[ProduktPlik] CHECK CONSTRAINT [FK_produkty_obrazki_produkty]
ALTER TABLE [dbo].[ProduktPlik]  WITH NOCHECK ADD  CONSTRAINT [produkty_obrazki_fk2] FOREIGN KEY([PlikId])
REFERENCES [dbo].[Plik] ([Id])
ON DELETE CASCADE
ALTER TABLE [dbo].[ProduktPlik] CHECK CONSTRAINT [produkty_obrazki_fk2]

ALTER TABLE [dbo].[ProduktStan]  WITH CHECK ADD  CONSTRAINT [FK_produkty_stany_magazyny] FOREIGN KEY([MagazynId])
REFERENCES [dbo].[Magazyn] ([Id])
ON UPDATE CASCADE
ON DELETE CASCADE
ALTER TABLE [dbo].[ProduktStan] CHECK CONSTRAINT [FK_produkty_stany_magazyny]

ALTER TABLE [dbo].[ProduktStan]  WITH CHECK ADD  CONSTRAINT [FK_produkty_stany_produkty] FOREIGN KEY([ProduktId])
REFERENCES [dbo].[Produkt] ([Id])
ON UPDATE CASCADE
ON DELETE CASCADE
ALTER TABLE [dbo].[ProduktStan] CHECK CONSTRAINT [FK_produkty_stany_produkty]
ALTER TABLE [dbo].[ProduktUkryty]  WITH CHECK ADD  CONSTRAINT [FK_produkty_ukryte_kategorie] FOREIGN KEY([KategoriaId])
REFERENCES [dbo].[KategoriaProduktu] ([Id])
ON DELETE CASCADE
ALTER TABLE [dbo].[ProduktUkryty] CHECK CONSTRAINT [FK_produkty_ukryte_kategorie]
ALTER TABLE [dbo].[ProduktUkryty]  WITH CHECK ADD  CONSTRAINT [FK_produkty_ukryte_kategorie_klientow] FOREIGN KEY([KategoriaKlientowId])
REFERENCES [dbo].[KategoriaKlienta] ([Id])
ON DELETE CASCADE
ALTER TABLE [dbo].[ProduktUkryty] CHECK CONSTRAINT [FK_produkty_ukryte_kategorie_klientow]
ALTER TABLE [dbo].[ProduktUkryty]  WITH CHECK ADD  CONSTRAINT [FK_produkty_ukryte_klienci] FOREIGN KEY([KlientZrodloId])
REFERENCES [dbo].[Klient] ([Id])
ON DELETE CASCADE
ALTER TABLE [dbo].[ProduktUkryty] CHECK CONSTRAINT [FK_produkty_ukryte_klienci]
ALTER TABLE [dbo].[ProduktUkryty]  WITH CHECK ADD  CONSTRAINT [FK_produkty_ukryte_produkty] FOREIGN KEY([ProduktZrodloId])
REFERENCES [dbo].[Produkt] ([Id])
ON DELETE CASCADE
ALTER TABLE [dbo].[ProduktUkryty] CHECK CONSTRAINT [FK_produkty_ukryte_produkty]
ALTER TABLE [dbo].[ProduktyKodyDodatkowe]  WITH NOCHECK ADD  CONSTRAINT [FK_ProduktyKodyKreskowe_klienci] FOREIGN KEY([KlientId])
REFERENCES [dbo].[Klient] ([Id])
ON DELETE CASCADE
ALTER TABLE [dbo].[ProduktyKodyDodatkowe] CHECK CONSTRAINT [FK_ProduktyKodyKreskowe_klienci]
ALTER TABLE [dbo].[ProduktyKodyDodatkowe]  WITH NOCHECK ADD  CONSTRAINT [FK_ProduktyKodyKreskowe_produkty] FOREIGN KEY([ProduktId])
REFERENCES [dbo].[Produkt] ([Id])
ON DELETE CASCADE
ALTER TABLE [dbo].[ProduktyKodyDodatkowe] CHECK CONSTRAINT [FK_ProduktyKodyKreskowe_produkty]
ALTER TABLE [dbo].[ProduktyZamienniki]  WITH CHECK ADD  CONSTRAINT [FK_ProduktyZamienniki_produkty] FOREIGN KEY([ProduktId])
REFERENCES [dbo].[Produkt] ([Id])
ON DELETE CASCADE
ALTER TABLE [dbo].[ProduktyZamienniki] CHECK CONSTRAINT [FK_ProduktyZamienniki_produkty]
ALTER TABLE [dbo].[ProduktyZamienniki]  WITH CHECK ADD  CONSTRAINT [FK_ProduktyZamienniki_produkty1] FOREIGN KEY([ZamiennikId])
REFERENCES [dbo].[Produkt] ([Id])
ALTER TABLE [dbo].[ProduktyZamienniki] CHECK CONSTRAINT [FK_ProduktyZamienniki_produkty1]

ALTER TABLE [dbo].[ProfilKlienta]  WITH CHECK ADD  CONSTRAINT [FK_ProfilKlienta_Klient] FOREIGN KEY([KlientId])
REFERENCES [dbo].[Klient] ([Id])
ON DELETE CASCADE
ALTER TABLE [dbo].[ProfilKlienta] CHECK CONSTRAINT [FK_ProfilKlienta_Klient]

ALTER TABLE [dbo].[Rabat]  WITH CHECK ADD  CONSTRAINT [FK_rabaty_cechy] FOREIGN KEY([CechaId])
REFERENCES [dbo].[Cecha] ([Id])
ON DELETE CASCADE
ALTER TABLE [dbo].[Rabat] CHECK CONSTRAINT [FK_rabaty_cechy]
ALTER TABLE [dbo].[Rabat]  WITH NOCHECK ADD  CONSTRAINT [FK_rabaty_kategoria_klientow] FOREIGN KEY([KategoriaKlientowId])
REFERENCES [dbo].[KategoriaKlienta] ([Id])
ON DELETE CASCADE
ALTER TABLE [dbo].[Rabat] CHECK CONSTRAINT [FK_rabaty_kategoria_klientow]
ALTER TABLE [dbo].[Rabat]  WITH NOCHECK ADD  CONSTRAINT [FK_rabaty_kategorie] FOREIGN KEY([KategoriaProduktowId])
REFERENCES [dbo].[KategoriaProduktu] ([Id])
ON DELETE CASCADE
ALTER TABLE [dbo].[Rabat] CHECK CONSTRAINT [FK_rabaty_kategorie]
ALTER TABLE [dbo].[Rabat]  WITH NOCHECK ADD  CONSTRAINT [FK_rabaty_klienci] FOREIGN KEY([KlientId])
REFERENCES [dbo].[Klient] ([Id])
ON DELETE CASCADE
ALTER TABLE [dbo].[Rabat] CHECK CONSTRAINT [FK_rabaty_klienci]
ALTER TABLE [dbo].[Rabat]  WITH NOCHECK ADD  CONSTRAINT [FK_rabaty_produkty] FOREIGN KEY([ProduktId])
REFERENCES [dbo].[Produkt] ([Id])
ON DELETE CASCADE
ALTER TABLE [dbo].[Rabat] CHECK CONSTRAINT [FK_rabaty_produkty]
ALTER TABLE [dbo].[Rabat]  WITH NOCHECK ADD  CONSTRAINT [FK_rabaty_rabaty] FOREIGN KEY([PoziomCenyId])
REFERENCES [dbo].[PoziomCenowy] ([Id])
ON DELETE SET NULL
ALTER TABLE [dbo].[Rabat] CHECK CONSTRAINT [FK_rabaty_rabaty]
ALTER TABLE [dbo].[Rabat]  WITH CHECK ADD  CONSTRAINT [FK_rabaty_Waluta] FOREIGN KEY([WalutaId])
REFERENCES [dbo].[Waluta] ([Id])
ALTER TABLE [dbo].[Rabat] CHECK CONSTRAINT [FK_rabaty_Waluta]
ALTER TABLE [dbo].[Region]  WITH CHECK ADD  CONSTRAINT [FK_Regiony_Kraje] FOREIGN KEY([KrajId])
REFERENCES [dbo].[Kraje] ([Id])
ON DELETE CASCADE
ALTER TABLE [dbo].[Region] CHECK CONSTRAINT [FK_Regiony_Kraje]


ALTER TABLE [dbo].[Sklep]  WITH CHECK ADD  CONSTRAINT [FK_Sklep_Adres] FOREIGN KEY([AdresId])
REFERENCES [dbo].[Adres] ([Id])
ON DELETE SET NULL
ALTER TABLE [dbo].[Sklep] CHECK CONSTRAINT [FK_Sklep_Adres]

ALTER TABLE [dbo].[Sklep]  WITH NOCHECK ADD  CONSTRAINT [FK_sklepy_obrazki] FOREIGN KEY([ObrazekId])
REFERENCES [dbo].[Plik] ([Id])
ON DELETE SET NULL
ALTER TABLE [dbo].[Sklep] CHECK CONSTRAINT [FK_sklepy_obrazki]


ALTER TABLE [dbo].[SklepKategoriaSklepu]  WITH CHECK ADD  CONSTRAINT [FK_sklepy_kategorie_polaczenia_sklepy] FOREIGN KEY([SklepId])
REFERENCES [dbo].[Sklep] ([Id])
ON DELETE CASCADE


ALTER TABLE [dbo].[SklepKategoriaSklepu] CHECK CONSTRAINT [FK_sklepy_kategorie_polaczenia_sklepy]
ALTER TABLE [dbo].[SklepKategoriaSklepu]  WITH CHECK ADD  CONSTRAINT [FK_sklepy_kategorie_polaczenia_sklepy_kategorie] FOREIGN KEY([KategoriaSklepuId])
REFERENCES [dbo].[KategoriaSklepu] ([Id])
ON DELETE CASCADE
ALTER TABLE [dbo].[SklepKategoriaSklepu] CHECK CONSTRAINT [FK_sklepy_kategorie_polaczenia_sklepy_kategorie]
ALTER TABLE [dbo].[SposobPokazywaniaStanowRegula]  WITH NOCHECK ADD  CONSTRAINT [FK_sposoby_pokazywania_stanow_reguly_sposoby_pokazywania_stanow] FOREIGN KEY([SposobId])
REFERENCES [dbo].[SposobPokazywaniaStanow] ([Id])
ON UPDATE CASCADE
ON DELETE CASCADE
ALTER TABLE [dbo].[SposobPokazywaniaStanowRegula] CHECK CONSTRAINT [FK_sposoby_pokazywania_stanow_reguly_sposoby_pokazywania_stanow]
ALTER TABLE [dbo].[Tlumaczenie]  WITH NOCHECK ADD  CONSTRAINT [FK_slowniki_jezyki] FOREIGN KEY([JezykId])
REFERENCES [dbo].[Jezyk] ([Id])
ON DELETE CASCADE
ALTER TABLE [dbo].[Tlumaczenie] CHECK CONSTRAINT [FK_slowniki_jezyki]
ALTER TABLE [dbo].[Tresc]  WITH CHECK ADD  CONSTRAINT [FK_Tresc_Tresc1] FOREIGN KEY([NadrzednaId])
REFERENCES [dbo].[Tresc] ([Id])
ALTER TABLE [dbo].[Tresc] CHECK CONSTRAINT [FK_Tresc_Tresc1]
ALTER TABLE [dbo].[TrescKolumna]  WITH CHECK ADD  CONSTRAINT [FK_TrescKolumna_TrescWiersz] FOREIGN KEY([TrescWierszId])
REFERENCES [dbo].[TrescWiersz] ([Id])
ON DELETE CASCADE
ALTER TABLE [dbo].[TrescKolumna] CHECK CONSTRAINT [FK_TrescKolumna_TrescWiersz]
ALTER TABLE [dbo].[TrescWiersz]  WITH CHECK ADD  CONSTRAINT [FK_TrescWiersz_Tresc] FOREIGN KEY([TrescId])
REFERENCES [dbo].[Tresc] ([Id])
ON DELETE CASCADE
ALTER TABLE [dbo].[TrescWiersz] CHECK CONSTRAINT [FK_TrescWiersz_Tresc]

ALTER TABLE [dbo].[Zadanie]  WITH CHECK ADD  CONSTRAINT [FK_zadania_klienci] FOREIGN KEY([OddzialId])
REFERENCES [dbo].[Klient] ([Id])
ALTER TABLE [dbo].[Zadanie] CHECK CONSTRAINT [FK_zadania_klienci]
ALTER TABLE [dbo].[Zadanie]  WITH NOCHECK ADD  CONSTRAINT [FK_zadania_zadania] FOREIGN KEY([ZadanieNadrzedne])
REFERENCES [dbo].[Zadanie] ([Id])
ALTER TABLE [dbo].[Zadanie] CHECK CONSTRAINT [FK_zadania_zadania]
ALTER TABLE [dbo].[Zamowienie]  WITH CHECK ADD  CONSTRAINT [FK_zamowienia_zamowienia_statusy] FOREIGN KEY([StatusId])
REFERENCES [dbo].[StatusZamowienia] ([Id])
ALTER TABLE [dbo].[Zamowienie] CHECK CONSTRAINT [FK_zamowienia_zamowienia_statusy]
ALTER TABLE [dbo].[ZamowienieProdukt]  WITH CHECK ADD  CONSTRAINT [FK_Hzamowienia_produkty_Hzamowienia] FOREIGN KEY([ZamowienieId])
REFERENCES [dbo].[Zamowienie] ([Id])
ON DELETE CASCADE
ALTER TABLE [dbo].[ZamowienieProdukt] CHECK CONSTRAINT [FK_Hzamowienia_produkty_Hzamowienia]
ALTER TABLE [dbo].[ZamowienieProdukt]  WITH CHECK ADD  CONSTRAINT [FK_zamowienia_produkty_produkty] FOREIGN KEY([ProduktId])
REFERENCES [dbo].[Produkt] ([Id])
ON DELETE CASCADE
ALTER TABLE [dbo].[ZamowienieProdukt] CHECK CONSTRAINT [FK_zamowienia_produkty_produkty]
ALTER TABLE [dbo].[Konfekcje]  WITH NOCHECK ADD  CONSTRAINT [CK_Konfekcje] CHECK  (([Rabat] IS NOT NULL OR [RabatKwota] IS NOT NULL))
ALTER TABLE [dbo].[Konfekcje] CHECK CONSTRAINT [CK_Konfekcje]
ALTER TABLE [dbo].[Konfekcje]  WITH NOCHECK ADD  CONSTRAINT [CK_Konfekcje_1] CHECK  (([Rabat] IS NOT NULL AND [RabatKwota] IS NULL OR [Rabat] IS NULL AND [RabatKwota] IS NOT NULL))
ALTER TABLE [dbo].[Konfekcje] CHECK CONSTRAINT [CK_Konfekcje_1]

--DECLARE @source_schema sysname, @source_name sysname
--SET @source_schema = N'dbo'
--DECLARE #hinstance CURSOR LOCAL fast_forward
--FOR  SELECT name  FROM [sys].[tables]  WHERE SCHEMA_NAME(schema_id) = @source_schema  AND is_ms_shipped = 0    
--OPEN #hinstance
--FETCH #hinstance INTO @source_name
 
--WHILE (@@fetch_status <> -1)
--	BEGIN
--	 EXEC [sys].[sp_cdc_enable_table]
--	  @source_schema
--	  ,@source_name
--	  ,@role_name = NULL
--	  ,@supports_net_changes = 1
   
--	 FETCH #hinstance INTO @source_name
--	END
 
--CLOSE #hinstance
--DEALLOCATE #hinstance