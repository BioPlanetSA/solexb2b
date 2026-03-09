

DECLARE @tblName VARCHAR(100)  --zmienna do nazwy tabeli
DECLARE @colname VARCHAR(100)  --zmienna do nazwy kolumny 
DECLARE @dbType VARCHAR(100)	-- znienna do typu kolumny 
DECLARE @cdcInst VARCHAR(100)


SET @tblName = 'Klient'


	SET @cdcInst = 'dbo_' + @tblName

	---------wyłaczenia monitirowania tabeli
	--exec sys.sp_cdc_disable_table @source_schema='dbo', @source_name= @tblName, @capture_instance= @cdcInst

	------------------------------Zmiana kolumny w tabeli
	ALTER TABLE [dbo].[Klient]  WITH CHECK ADD  CONSTRAINT [FK_Klient_Waluty] FOREIGN KEY([WalutaId]) REFERENCES [dbo].[Waluta] ([Id])

	 
	-----------włączenie monitorowania tabeli
	--EXEC sys.sp_cdc_enable_table @source_schema = 'dbo',@source_name = @tblName,@role_name = NULL, @supports_net_changes = 1



