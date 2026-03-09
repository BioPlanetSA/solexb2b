
DECLARE @tblName VARCHAR(100)  --zmienna do nazwy tabeli
DECLARE @colname VARCHAR(100)  --zmienna do nazwy kolumny 
DECLARE @dbType VARCHAR(100)	-- znienna do typu kolumny 
DECLARE @cdcInst VARCHAR(100)


SET @tblName = 'ProduktUkryty'
SET @colname = 'CechaProduktuId'
SET @dbType = 'bigint NULL'

if exists ( SELECT * FROM sys.objects WHERE object_id = OBJECT_ID('unikalnosc') AND parent_object_id = OBJECT_ID('ProduktUkryty')) ALTER TABLE ProduktUkryty DROP CONSTRAINT unikalnosc;
IF EXISTS (SELECT * FROM sysindexes WHERE name = 'idx_pu') DROP INDEX idx_pu ON dbo.ProduktUkryty;

if exists (select column_name from INFORMATION_SCHEMA.columns where table_name = @tblName and column_name = @colname) 
begin


	SET @cdcInst = 'dbo_' + @tblName

	---------wyłaczenia monitirowania tabeli
	--exec sys.sp_cdc_disable_table @source_schema='dbo', @source_name= @tblName, @capture_instance= @cdcInst
	-- wyłączam constrainty

	------------------------------Zmiana kolumny w tabeli
	EXEC('ALTER TABLE dbo.'+ @tblName +'  ALTER COLUMN  '+ @colname + ' '+ @dbType  )  

	EXEC('CREATE NONCLUSTERED INDEX [idx_pu] ON [dbo].[ProduktUkryty](	[KlientZrodloId] ASC, [Tryb] ASC,	[ProduktZrodloId] ASC,	[CechaProduktuId] ASC,	[KategoriaId] ASC,	[KategoriaKlientowId] ASC,	[PrzedstawicielId] ASC)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY] ')
    EXEC('ALTER TABLE dbo.'+ @tblName +'  ADD CONSTRAINT [unikalnosc] UNIQUE NONCLUSTERED (	[KategoriaId] ASC,	[KategoriaKlientowId] ASC,	[KlientZrodloId] ASC,	[ProduktZrodloId] ASC,	[Tryb] ASC,	[CechaProduktuId] ASC,	[PrzedstawicielId] ASC)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]')
	-----------włączenie monitorowania tabeli
	--EXEC sys.sp_cdc_enable_table @source_schema = 'dbo',@source_name = @tblName,@role_name = NULL, @supports_net_changes = 1

end