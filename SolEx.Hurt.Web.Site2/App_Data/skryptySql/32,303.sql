
IF EXISTS (SELECT * FROM sys.tables where name = 'ZamowienieDokumenty')
BEGIN
	--exec sys.sp_cdc_disable_table @source_schema='dbo',	@source_name= 'ZamowienieDokumenty', @capture_instance='dbo_ZamowienieDokumenty'
	
	Drop table [dbo].[ZamowienieDokumenty]

	CREATE TABLE [dbo].[ZamowienieDokumentyERP](
		[Id] [bigint] NOT NULL,
		[IdZamowienia] [bigint] NOT NULL,
		[IdDokumentu] [bigint] NOT NULL,
		[NazwaERP] [varchar](200) NULL,
	 CONSTRAINT [PK_ZamowienieDokumenty] PRIMARY KEY CLUSTERED 
	(
		[Id] ASC
	)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
	) ON [PRIMARY]


	ALTER TABLE [dbo].[ZamowienieDokumentyERP]  WITH CHECK ADD  CONSTRAINT [FK_ZamowieniaDokumentyERP_Zamowienie] FOREIGN KEY([IdZamowienia]) REFERENCES [dbo].[Zamowienie] ([Id]) ON DELETE CASCADE

	--EXEC sys.sp_cdc_enable_table @source_schema = 'dbo',@source_name ='ZamowienieDokumentyERP',@role_name = NULL, @supports_net_changes = 1
END