
IF EXISTS (SELECT * FROM sysobjects WHERE name='Zdarzenie' AND xtype='U')
BEGIN
--exec sys.sp_cdc_disable_table @source_schema='dbo',	@source_name= 'Zdarzenie', @capture_instance= 'dbo_Zdarzenie';
drop table [Zdarzenie];
END


IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='UstawieniePowiadomienia' AND xtype='U')
begin
	CREATE TABLE [dbo].UstawieniePowiadomienia(
		[Id] [bigint] NOT NULL,
		[ParametryWysylania] [varchar](600) NULL,
		[ZgodaNaZmianyPrzezKlienta] [bit] NOT NULL DEFAULT ((0)),
		CONSTRAINT [PK_ustawieniepowiadomienia] PRIMARY KEY CLUSTERED 
		(
			[Id] ASC
		)
	) 
	
	-- to dajemy jesli ma być zapisywana historia zmian 
	--EXEC [sys].[sp_cdc_enable_table] N'dbo','UstawieniePowiadomienia', @role_name = NULL, @supports_net_changes = 1
end