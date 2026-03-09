DECLARE @cdcInst VARCHAR(100)
DECLARE @tblName VARCHAR(100)
DECLARE @colname VARCHAR(100)
DECLARE @dbType VARCHAR(100)
DECLARE @newColname VARCHAR(100)

SET @tblName = 'Magazyn'
SET @colname = 'Id'

SET @cdcInst = 'dbo_' + @tblName
	--exec sys.sp_cdc_disable_table @source_schema='dbo', @source_name= @tblName, @capture_instance= @cdcInst

	if exists ( SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID('FK_produkty_stany_magazyny') AND parent_object_id = OBJECT_ID('ProduktStan')) ALTER TABLE ProduktStan DROP CONSTRAINT FK_produkty_stany_magazyny;
	Drop table Magazyn

	CREATE TABLE [dbo].[Magazyn](
	[Id] [int] NOT NULL,
	[Symbol] [varchar](50) NOT NULL,
	[Nazwa] [varchar](100) NULL,
	[ImportowacZErp] [bit] NOT NULL,
	[Parametry] [varchar](200) NOT NULL,
	 CONSTRAINT [PK_magazyny] PRIMARY KEY CLUSTERED 
	(
		[Id] ASC
	)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
	) ON [PRIMARY]

	--trzeba usunac aby mozna bylo ustawic klucz obcy
	delete from ProduktStan

	ALTER TABLE [dbo].[ProduktStan]  WITH CHECK ADD  CONSTRAINT [FK_produkty_stany_magazyny] FOREIGN KEY([MagazynId])
REFERENCES [dbo].[Magazyn] ([Id])

ALTER TABLE [dbo].[ProduktStan] CHECK CONSTRAINT [FK_produkty_stany_magazyny]


	--EXEC sys.sp_cdc_enable_table @source_schema = 'dbo',@source_name = @tblName,@role_name = NULL, @supports_net_changes = 1
