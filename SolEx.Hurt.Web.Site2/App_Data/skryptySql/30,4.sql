--if NOT exists (select column_name from INFORMATION_SCHEMA.columns where table_name = 'TrescKolumna' and column_name = 'OpisKontenera') ALTER TABLE [TrescKolumna]  ADD OpisKontenera [varchar](1000) NULL;
DECLARE @sql VARCHAR(8000)
DECLARE @tblName VARCHAR(100)
DECLARE @colname VARCHAR(100)
DECLARE @dbType VARCHAR(100)
DECLARE @cdcInst VARCHAR(100)
DECLARE @cdctblName VARCHAR(100)
DECLARE @cdcTemptblName VARCHAR(100)

	SET @tblName = 'TrescKolumna'
	SET @colname = 'OpisKontenera'
	SET @dbType = '[varchar](1000) NULL'

if NOT exists (select column_name from INFORMATION_SCHEMA.columns where table_name = @tblName and column_name = @colname) 
begin
SET @cdcInst = 'dbo_' + @tblName

	---------wyłaczenia monitirowania tabeli
	--exec sys.sp_cdc_disable_table @source_schema='dbo', @source_name= @tblName, @capture_instance= @cdcInst

	------------------------------dodawanie kolumny do tabeli
	EXEC('ALTER TABLE dbo.'+ @tblName +' ADD '+ @colname + ' '+ @dbType  )  
	      
	-----------włączenie monitorowania tabeli
	----EXEC sys.sp_cdc_enable_table @source_schema = 'dbo',@source_name = @tblName,@role_name = NULL, @supports_net_changes = 1
end


	SET @tblName = 'KoszykPozycje'
	SET @colname = 'WymuszonaCenaNettoModul'
	SET @dbType = '[decimal](10,2) NULL'

--if NOT exists (select column_name from INFORMATION_SCHEMA.columns where table_name = 'KoszykPozycje' and column_name = 'WymuszonaCenaNettoModul') ALTER TABLE [KoszykPozycje]  ADD WymuszonaCenaNettoModul [decimal](10,2) NULL;
if NOT exists (select column_name from INFORMATION_SCHEMA.columns where table_name = @tblName and column_name = @colname) 
begin
SET @cdcInst = 'dbo_' + @tblName

	---------wyłaczenia monitirowania tabeli
	--exec sys.sp_cdc_disable_table @source_schema='dbo', @source_name= @tblName, @capture_instance= @cdcInst

	------------------------------dodawanie kolumny do tabeli
	EXEC('ALTER TABLE dbo.'+ @tblName +' ADD '+ @colname + ' '+ @dbType  )  
	      
	-----------włączenie monitorowania tabeli
	----EXEC sys.sp_cdc_enable_table @source_schema = 'dbo',@source_name = @tblName,@role_name = NULL, @supports_net_changes = 1
end


	SET @tblName = 'KoszykPozycje'
	SET @colname = 'WymuszonaCenaNettoPrzedstawiciel'
	SET @dbType = '[decimal](10,2) NULL'

--if NOT exists (select column_name from INFORMATION_SCHEMA.columns where table_name = 'KoszykPozycje' and column_name = 'WymuszonaCenaNettoPrzedstawiciel') ALTER TABLE [KoszykPozycje]  ADD WymuszonaCenaNettoPrzedstawiciel [decimal](10,2) NULL;
if NOT exists (select column_name from INFORMATION_SCHEMA.columns where table_name = @tblName and column_name = @colname) 
begin
SET @cdcInst = 'dbo_' + @tblName

	---------wyłaczenia monitirowania tabeli
	--exec sys.sp_cdc_disable_table @source_schema='dbo', @source_name= @tblName, @capture_instance= @cdcInst

	------------------------------dodawanie kolumny do tabeli
	EXEC('ALTER TABLE dbo.'+ @tblName +' ADD '+ @colname + ' '+ @dbType  )  
	      
	-----------włączenie monitorowania tabeli
	----EXEC sys.sp_cdc_enable_table @source_schema = 'dbo',@source_name = @tblName,@role_name = NULL, @supports_net_changes = 1
end



if exists (select column_name from INFORMATION_SCHEMA.columns where table_name = 'KoszykPozycje' and column_name = 'WymuszonaCenaNetto') ALTER TABLE [KoszykPozycje]  DROP COLUMN  WymuszonaCenaNetto;



if exists ( SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID('FK_zastepca') AND parent_object_id = OBJECT_ID('Klient')) ALTER TABLE Klient DROP CONSTRAINT FK_zastepca;
if exists (select column_name from INFORMATION_SCHEMA.columns where table_name = 'Klient' and column_name = 'ZastepcaId') ALTER TABLE Klient DROP COLUMN  ZastepcaId;

--alter table BlogKategoria add [BlogGrupaId] [int] null
SET @tblName = 'BlogKategoria'
SET @colname = 'BlogGrupaId'
SET @dbType = 'int null'

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


--ALTER TABLE Tresc ADD SposobOtwierania varchar(100)
SET @tblName = 'Tresc'
SET @colname = 'SposobOtwierania'
SET @dbType = 'varchar(100) NULL'

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

SET @tblName = 'TrescWiersz'
SET @colname = 'Marginesy'
SET @dbType = 'varchar(100) NULL'

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

SET @tblName = 'TrescWiersz'
SET @colname = 'Paddingi'
SET @dbType = 'varchar(100) NULL'

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

SET @tblName = 'TrescKolumna'
SET @colname = 'Paddingi'
SET @dbType = 'varchar(100) NULL'

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

SET @tblName = 'TrescKolumna'
SET @colname = 'Marginesy'
SET @dbType = 'varchar(100) NULL'

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

--ALTER TABLE tresc ADD ZawartoscJakoMenu bit
SET @tblName = 'Tresc'
SET @colname = 'ZawartoscJakoMenu'
SET @dbType = '[bit] NOT NULL CONSTRAINT [DF_Tresc_ZawartoscJakoMenu]  DEFAULT ((0))'

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

--alter table trescwiersz drop column marginesgora
if exists (select column_name from INFORMATION_SCHEMA.columns where table_name = 'TrescWiersz' and column_name = 'MarginesGora') ALTER TABLE TrescWiersz  DROP COLUMN  MarginesGora;

--alter table trescwiersz drop column marginesdol
if exists (select column_name from INFORMATION_SCHEMA.columns where table_name = 'TrescWiersz' and column_name = 'MarginesDol') ALTER TABLE TrescWiersz  DROP COLUMN  MarginesDol;

-- alter table tresc drop column LinkAlternatywnyJakoZwyklyTekst
if exists (select column_name from INFORMATION_SCHEMA.columns where table_name = 'Tresc' and column_name = 'LinkAlternatywnyJakoZwyklyTekst') ALTER TABLE Tresc  DROP COLUMN  LinkAlternatywnyJakoZwyklyTekst;



IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='BlogGrupa' AND xtype='U')
begin
	CREATE TABLE [dbo].[BlogGrupa](
		[Id] [int] IDENTITY(1,1) NOT NULL,
		[Nazwa] [varchar](max) NOT NULL,
	 CONSTRAINT [PK_BlogGrupa] PRIMARY KEY CLUSTERED 
	(
		[Id] ASC
	)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
	) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

	ALTER TABLE [dbo].[BlogKategoria]  WITH CHECK ADD  CONSTRAINT [FK_BlogKategoria_BlogGrupa] FOREIGN KEY([BlogGrupaId]) REFERENCES [dbo].[BlogGrupa] ([Id]) ON DELETE CASCADE

	--EXEC [sys].[sp_cdc_enable_table] N'dbo','BlogGrupa', @role_name = NULL, @supports_net_changes = 1
end
