
DECLARE @tblName VARCHAR(100)  --zmienna do nazwy tabeli
DECLARE @colname VARCHAR(100)  --zmienna do nazwy kolumny 
DECLARE @dbType VARCHAR(100)	-- znienna do typu kolumny 
DECLARE @cdcInst VARCHAR(100)

SET @tblName = 'HistoriaDokumentuProdukt'
SET @colname = 'ProduktIdBazowy'
SET @dbType = 'bigint NULL'

if NOT exists (select column_name from INFORMATION_SCHEMA.columns where table_name = @tblName and column_name = @colname) 
begin
	SET @cdcInst = 'dbo_' + @tblName

	---------wyłaczenia monitirowania tabeli
	--exec sys.sp_cdc_disable_table @source_schema='dbo', @source_name= @tblName, @capture_instance= @cdcInst

	------------------------------dodawanie kolumny do tabeli
	EXEC('ALTER TABLE dbo.'+ @tblName +' ADD '+ @colname + ' '+ @dbType  )  
      
	-----------włączenie monitorowania tabeli
	--EXEC sys.sp_cdc_enable_table @source_schema = 'dbo',@source_name = @tblName,@role_name = NULL, @supports_net_changes = 1

end



EXEC('CREATE VIEW vKupowaneIlosci
AS
  select hp.ProduktId, case when pj.Podstawowa=1 then hp.Ilosc  
  else hp.Ilosc * pj.PrzelicznikIlosc end as Ilosc, hd.KlientId,hd.DataUtworzenia as DataZakupu, hd.Rodzaj as RodzajDokumentu
  from HistoriaDokumentuProdukt hp join HistoriaDokumentu hd on hd.Id=hp.DokumentId join ProduktJednostka pj on pj.JednostkaId=hp.JednostkaMiary where 
  pj.ProduktId = case when hp.ProduktIdBazowy is not null then hp.ProduktIdBazowy else hp.ProduktId end 
')


IF (EXISTS (SELECT *  FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA = 'dbo' AND  TABLE_NAME = 'KupowaneIlosci'))
BEGIN

   DROP TABLE [dbo].KupowaneIlosci;

	   CREATE TABLE [dbo].[KupowaneIlosci](
		[Id] [bigint] IDENTITY(1,1) NOT NULL,
		[KlientId] [bigint] NOT NULL,
		[ProduktId] [bigint] NOT NULL,
		[DataZakupu] [datetime] NULL,
		[Ilosc] [decimal](10, 2) NOT NULL,
		[RodzajDokumentu] [varchar](50) NOT NULL,
	 CONSTRAINT [PK_KupowaneIlosci] PRIMARY KEY CLUSTERED 
	(
		[Id] ASC
	)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY],
	 CONSTRAINT [IX_KupowaneIlosci] UNIQUE NONCLUSTERED 
	(
		[KlientId] ASC,
		[ProduktId] ASC,
		[DataZakupu] ASC,
		[RodzajDokumentu] ASC
	)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, FILLFACTOR = 80) ON [PRIMARY]
	) ON [PRIMARY]

	SET ANSI_PADDING OFF
	ALTER TABLE [dbo].[KupowaneIlosci]  WITH NOCHECK ADD  CONSTRAINT [FK_KupowaneIlosci_klienci] FOREIGN KEY([KlientId])REFERENCES [dbo].[Klient] ([Id]) ON DELETE CASCADE

	ALTER TABLE [dbo].[KupowaneIlosci] CHECK CONSTRAINT [FK_KupowaneIlosci_klienci]
	ALTER TABLE [dbo].[KupowaneIlosci]  WITH NOCHECK ADD  CONSTRAINT [FK_KupowaneIlosci_produkty] FOREIGN KEY([ProduktId])REFERENCES [dbo].[Produkt] ([Id])ON DELETE CASCADE
	ALTER TABLE [dbo].[KupowaneIlosci] CHECK CONSTRAINT [FK_KupowaneIlosci_produkty]

End

