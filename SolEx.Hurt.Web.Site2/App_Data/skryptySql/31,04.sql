IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='MiejsceKosztow' AND xtype='U')
begin

	CREATE TABLE [dbo].[MiejsceKosztow](
		[Id] [bigint] IDENTITY(1,1) NOT NULL,
		[Nazwa] [nvarchar](200) NOT NULL,
		[KlientId] [int] NOT NULL,
	 CONSTRAINT [PK_MiejscaKosztow] PRIMARY KEY CLUSTERED 
	(
		[Id] ASC
	)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
	) ON [PRIMARY]

	--EXEC [sys].[sp_cdc_enable_table] N'dbo','MiejsceKosztow', @role_name = NULL, @supports_net_changes = 1
end

IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='SzablonAkceptacji' AND xtype='U')
begin


CREATE TABLE [dbo].[SzablonAkceptacji](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[Nazwa] [nvarchar](200) NULL,
	[Tworca] [int] NOT NULL,
 CONSTRAINT [PK_SzablonAkceptacji] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

	--EXEC [sys].[sp_cdc_enable_table] N'dbo','SzablonAkceptacji', @role_name = NULL, @supports_net_changes = 1
end

IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='SzablonAkceptacjiPoziomy' AND xtype='U')
begin

CREATE TABLE [dbo].[SzablonAkceptacjiPoziomy](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[SzablonAkceptacjiId] [int] NOT NULL,
	[Klienci] [varchar](1000) NOT NULL,
	[Poziom] [int] NOT NULL,
 CONSTRAINT [PK_SzablonAkceptacjiPoziomy] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

	--EXEC [sys].[sp_cdc_enable_table] N'dbo','SzablonAkceptacjiPoziomy', @role_name = NULL, @supports_net_changes = 1
end


IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='SzablonLimitow' AND xtype='U')
begin

	CREATE TABLE [dbo].[SzablonLimitow](
		[Id] [bigint] IDENTITY(1,1) NOT NULL,
		[Nazwa] [nvarchar](500) NOT NULL,
		[IloscZamowien] [int] NULL,
		[WartoscZamowien] [decimal](10, 2) NULL,
		[IloscMiesiecy] [int] NOT NULL,
		[OdKiedy] [datetime] NULL,
		[Tworca] [int] NOT NULL,
		[MaksymalnaCenaTowaru] [decimal](10, 2) NULL,
	 CONSTRAINT [PK_SzablonLimitow] PRIMARY KEY CLUSTERED 
	(
		[Id] ASC
	)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
	) ON [PRIMARY]

	--EXEC [sys].[sp_cdc_enable_table] N'dbo','SzablonLimitow', @role_name = NULL, @supports_net_changes = 1
end


------------------------------zmiany tabela SubkontoGrupa --------------------------------------------------

if exists ( SELECT * FROM sys.default_constraints WHERE object_id = OBJECT_ID('DF_subkonta_grupy_limit_wartosci_zamowien') AND parent_object_id = OBJECT_ID('SubkontoGrupa')) 
	ALTER TABLE SubkontoGrupa DROP CONSTRAINT DF_subkonta_grupy_limit_wartosci_zamowien;

if exists ( SELECT * FROM sys.default_constraints WHERE object_id = OBJECT_ID('DF_subkonta_grupy_limit_okres') AND parent_object_id = OBJECT_ID('SubkontoGrupa')) 
	ALTER TABLE SubkontoGrupa DROP CONSTRAINT DF_subkonta_grupy_limit_okres;

if exists ( SELECT * FROM sys.default_constraints WHERE object_id = OBJECT_ID('DF_subkonta_grupy_widoczna') AND parent_object_id = OBJECT_ID('SubkontoGrupa')) 
	ALTER TABLE SubkontoGrupa DROP CONSTRAINT DF_subkonta_grupy_widoczna;



DECLARE @tblName VARCHAR(100)  --zmienna do nazwy tabeli
DECLARE @colname VARCHAR(100)  --zmienna do nazwy kolumny 
DECLARE @cdcInst VARCHAR(100)
DECLARE @dbType VARCHAR(100)
DECLARE @Type VARCHAR(100)

SET @tblName = 'SubkontoGrupa'

SET @cdcInst = 'dbo_' + @tblName
---------wyłaczenia monitirowania tabeli
--exec sys.sp_cdc_disable_table @source_schema='dbo', @source_name= @tblName, @capture_instance= @cdcInst

SET @colname = 'LimitIlosciZamowien'
if exists (select column_name from INFORMATION_SCHEMA.columns where table_name = @tblName and column_name = @colname) 
begin
	------------------------------Usunięcie kolumny z tabeli
	EXEC('ALTER TABLE dbo.'+ @tblName +' DROP COLUMN '+ @colname  )  	      
end

SET @colname = 'LimitOkres'
if exists (select column_name from INFORMATION_SCHEMA.columns where table_name = @tblName and column_name = @colname) 
begin
	------------------------------Usunięcie kolumny z tabeli
	EXEC('ALTER TABLE dbo.'+ @tblName +' DROP COLUMN '+ @colname  )  	      
end

SET @colname = 'LimitWartosciZamowien'
if exists (select column_name from INFORMATION_SCHEMA.columns where table_name = @tblName and column_name = @colname) 
begin
	------------------------------Usunięcie kolumny z tabeli
	EXEC('ALTER TABLE dbo.'+ @tblName +' DROP COLUMN '+ @colname  )  	      
end

SET @colname = 'LimityLiczoneOdKiedy'
if exists (select column_name from INFORMATION_SCHEMA.columns where table_name = @tblName and column_name = @colname) 
begin
	------------------------------Usunięcie kolumny z tabeli
	EXEC('ALTER TABLE dbo.'+ @tblName +' DROP COLUMN '+ @colname  )  	      
end

SET @colname = 'Widoczna'
if exists (select column_name from INFORMATION_SCHEMA.columns where table_name = @tblName and column_name = @colname) 
begin
	------------------------------Usunięcie kolumny z tabeli
	EXEC('ALTER TABLE dbo.'+ @tblName +' DROP COLUMN '+ @colname  )  	      
end

SET @colname = 'SzablonAkceptacjiId'
SET @dbType = 'bigint NULL'--[bit] NOT NULL CONSTRAINT [DF_Slajd_CzyPokazywacTytul]  DEFAULT ((0)) -- tym z domyslna wartością 
if NOT exists (select column_name from INFORMATION_SCHEMA.columns where table_name = @tblName and column_name = @colname) 
begin

	------------------------------dodawanie kolumny do tabeli
	EXEC('ALTER TABLE dbo.'+ @tblName +' ADD '+ @colname + ' '+ @dbType  )  

	--Dodanie kluczy obcych
	EXEC('ALTER TABLE [dbo].[SubkontoGrupa] ADD CONSTRAINT [FK_SubkontoGrupa_SzablonAkceptacji] FOREIGN KEY ([SzablonAkceptacjiId]) REFERENCES [dbo].[SzablonAkceptacji] ([Id]) ON DELETE SET NULL')
	EXEC('ALTER TABLE [dbo].[SubkontoGrupa] CHECK CONSTRAINT [FK_SubkontoGrupa_SzablonAkceptacji]')
end

SET @colname = 'SzablonLimitowId'
SET @dbType = 'bigint NULL'--[bit] NOT NULL CONSTRAINT [DF_Slajd_CzyPokazywacTytul]  DEFAULT ((0)) -- tym z domyslna wartością 
if NOT exists (select column_name from INFORMATION_SCHEMA.columns where table_name = @tblName and column_name = @colname) 
begin
	------------------------------dodawanie kolumny do tabeli
	EXEC('ALTER TABLE dbo.'+ @tblName +' ADD '+ @colname + ' '+ @dbType  )  
	--Dodanie kluczy obcych
	EXEC('ALTER TABLE [dbo].[SubkontoGrupa] ADD CONSTRAINT [FK_SubkontoGrupa_SzablonLimitow] FOREIGN KEY ([SzablonLimitowId]) REFERENCES [dbo].[SzablonLimitow] ([Id]) ON DELETE SET NULL')
	EXEC('ALTER TABLE [dbo].[SubkontoGrupa] CHECK CONSTRAINT [FK_SubkontoGrupa_SzablonLimitow]')
end

-----------włączenie monitorowania tabeli
--EXEC sys.sp_cdc_enable_table @source_schema = 'dbo',@source_name = @tblName,@role_name = NULL, @supports_net_changes = 1



-------------zmiany tabela Klienci -----------------------------
	if exists ( SELECT * FROM sys.default_constraints WHERE object_id = OBJECT_ID('DF__klienci__limit_i__5C02A283') AND parent_object_id = OBJECT_ID('Klient')) 
	ALTER TABLE Klient DROP CONSTRAINT DF__klienci__limit_i__5C02A283;

	if exists ( SELECT * FROM sys.default_constraints WHERE object_id = OBJECT_ID('DF__klienci__limit_o__1AF3F935') AND parent_object_id = OBJECT_ID('Klient')) 
	ALTER TABLE Klient DROP CONSTRAINT DF__klienci__limit_o__1AF3F935;

	if exists ( SELECT * FROM sys.default_constraints WHERE object_id = OBJECT_ID('DF__klienci__limit_w__5CF6C6BC') AND parent_object_id = OBJECT_ID('Klient')) 
	ALTER TABLE Klient DROP CONSTRAINT DF__klienci__limit_w__5CF6C6BC;

	if exists ( SELECT * FROM sys.default_constraints WHERE object_id = OBJECT_ID('DF__klienci__minimal__06ADD4BD') AND parent_object_id = OBJECT_ID('Klient')) 
	ALTER TABLE Klient DROP CONSTRAINT DF__klienci__minimal__06ADD4BD;

	if exists ( SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID('FK_klienci_klienci1') AND parent_object_id = OBJECT_ID('Klient')) 
	ALTER TABLE Klient DROP CONSTRAINT FK_klienci_klienci1;

SET @tblName = 'Klient'

SET @cdcInst = 'dbo_' + @tblName
---------wyłaczenia monitirowania tabeli
--exec sys.sp_cdc_disable_table @source_schema='dbo', @source_name= @tblName, @capture_instance= @cdcInst
SET @colname = 'KontoPotwierdzajaceId'
if exists (select column_name from INFORMATION_SCHEMA.columns where table_name = @tblName and column_name = @colname) 
begin
	------------------------------Usunięcie kolumny z tabeli
	EXEC('ALTER TABLE dbo.'+ @tblName +' DROP COLUMN '+ @colname  )  	      
end

SET @colname = 'LimitIlosciZamowienia'
if exists (select column_name from INFORMATION_SCHEMA.columns where table_name = @tblName and column_name = @colname) 
begin
	------------------------------Usunięcie kolumny z tabeli
	EXEC('ALTER TABLE dbo.'+ @tblName +' DROP COLUMN '+ @colname  )  	      
end
SET @colname = 'LimitWartosciZamowienia'
if exists (select column_name from INFORMATION_SCHEMA.columns where table_name = @tblName and column_name = @colname) 
begin
	------------------------------Usunięcie kolumny z tabeli
	EXEC('ALTER TABLE dbo.'+ @tblName +' DROP COLUMN '+ @colname  )  	      
end
SET @colname = 'LimitOkres'
if exists (select column_name from INFORMATION_SCHEMA.columns where table_name = @tblName and column_name = @colname) 
begin
	------------------------------Usunięcie kolumny z tabeli
	EXEC('ALTER TABLE dbo.'+ @tblName +' DROP COLUMN '+ @colname  )  	      
end
SET @colname = 'OdKiedyLiczymyLimit'
if exists (select column_name from INFORMATION_SCHEMA.columns where table_name = @tblName and column_name = @colname) 
begin
	------------------------------Usunięcie kolumny z tabeli
	EXEC('ALTER TABLE dbo.'+ @tblName +' DROP COLUMN '+ @colname  )  	      
end


SET @colname = 'MinimalnaWartoscZamowienia'
SET @Type = 'DECIMAL (10, 2) NULL'
------------------------------Zmiana kolumny do tabeli
EXEC('ALTER TABLE dbo.'+ @tblName +' ALTER COLUMN '+ @colname+' '+@Type   ) 



SET @colname = 'SzablonLimitowId'
SET @dbType = 'bigint NULL'--[bit] NOT NULL CONSTRAINT [DF_Slajd_CzyPokazywacTytul]  DEFAULT ((0)) -- tym z domyslna wartością 

if NOT exists (select column_name from INFORMATION_SCHEMA.columns where table_name = @tblName and column_name = @colname) 
begin

	------------------------------dodawanie kolumny do tabeli
	EXEC('ALTER TABLE dbo.'+ @tblName +' ADD '+ @colname + ' '+ @dbType  )  

	--Dodanie kluczy obcych
	EXEC('ALTER TABLE [dbo].['+ @tblName +'] ADD CONSTRAINT [FK_Klient_SzablonLimitow] FOREIGN KEY ([SzablonLimitowId]) REFERENCES [dbo].[SzablonLimitow] ([Id]) ON DELETE SET NULL')
	EXEC('ALTER TABLE [dbo].['+ @tblName +'] CHECK CONSTRAINT [FK_Klient_SzablonLimitow]')
end

SET @colname = 'MiejsceKosztowId'
SET @dbType = 'bigint NULL'--[bit] NOT NULL CONSTRAINT [DF_Slajd_CzyPokazywacTytul]  DEFAULT ((0)) -- tym z domyslna wartością 

if NOT exists (select column_name from INFORMATION_SCHEMA.columns where table_name = @tblName and column_name = @colname) 
begin

	------------------------------dodawanie kolumny do tabeli
	EXEC('ALTER TABLE dbo.'+ @tblName +' ADD '+ @colname + ' '+ @dbType  )  
	--Dodanie kluczy obcych
	EXEC('ALTER TABLE [dbo].['+ @tblName +'] ADD CONSTRAINT [FK_Klient_Klient] FOREIGN KEY ([MiejsceKosztowId]) REFERENCES [dbo].[MiejsceKosztow] ([Id]) ON DELETE SET NULL')
	EXEC('ALTER TABLE [dbo].['+ @tblName +'] CHECK CONSTRAINT [FK_Klient_Klient]')
end

SET @colname = 'SzablonAkceptacjiId'
SET @dbType = 'bigint NULL'--[bit] NOT NULL CONSTRAINT [DF_Slajd_CzyPokazywacTytul]  DEFAULT ((0)) -- tym z domyslna wartością 

if NOT exists (select column_name from INFORMATION_SCHEMA.columns where table_name = @tblName and column_name = @colname) 
begin

	------------------------------dodawanie kolumny do tabeli
	EXEC('ALTER TABLE dbo.'+ @tblName +' ADD '+ @colname + ' '+ @dbType  )  
	
end
-----------włączenie monitorowania tabeli
--EXEC sys.sp_cdc_enable_table @source_schema = 'dbo',@source_name = @tblName,@role_name = NULL, @supports_net_changes = 1
