--exec sys.sp_cdc_disable_table @source_schema='dbo',	@source_name= 'Zamowienie', @capture_instance= 'dbo_Zamowienie';

delete from ZamowienieDokumenty;
delete from ZamowienieProdukt;
delete from zamowienie;


--exec sys.sp_cdc_enable_table @source_schema='dbo',	@source_name= 'Zamowienie', @capture_instance= 'dbo_Zamowienie', @role_name = NULL, @supports_net_changes = 1;


