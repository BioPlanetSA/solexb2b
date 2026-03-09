
IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='HistoriaDokumentuPlatnosciOnline' AND xtype='U')
begin
	CREATE TABLE [dbo].HistoriaDokumentuPlatnosciOnline(
		Id bigint IDENTITY(1,1) not null,
		IdDokumentu int not null,
		  NazwaDokumentu varchar(max) not null,
		 PlatnikId int not null,
		 DataOperacji datetime not null,
		 IpOperacji  varchar(200) not null,
 Kwota decimal(10,2) not null,
 Status varchar(max)

	 CONSTRAINT [PK_HistoriaDokumentuPlatnosciOnline] PRIMARY KEY CLUSTERED 
	(
		[Id] ASC
	)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
	) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

	
	-- to dajemy jesli ma być zapisywana historia zmian 
	--EXEC [sys].[sp_cdc_enable_table] N'dbo','HistoriaDokumentuPlatnosciOnline', @role_name = NULL, @supports_net_changes = 1
end