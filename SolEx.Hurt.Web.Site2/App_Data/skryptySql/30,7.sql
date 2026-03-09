DECLARE @sql VARCHAR(8000)
DECLARE @tblName VARCHAR(100)
DECLARE @colname VARCHAR(100)
DECLARE @dbType VARCHAR(100)
DECLARE @cdcInst VARCHAR(100)
DECLARE @cdctblName VARCHAR(100)
DECLARE @cdcTemptblName VARCHAR(100)

SET @tblName = 'KategoriaProduktu'
SET @colname = 'Tekst1'
SET @dbType = 'nvarchar(4000) NULL'--[bit] NOT NULL CONSTRAINT [DF_Slajd_CzyPokazywacTytul]  DEFAULT ((0))
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

SET @colname = 'Tekst2'
SET @dbType = 'nvarchar(4000) NULL'--[bit] NOT NULL CONSTRAINT [DF_Slajd_CzyPokazywacTytul]  DEFAULT ((0))
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


SET @colname = 'Tekst3'
SET @dbType = 'nvarchar(4000) NULL'--[bit] NOT NULL CONSTRAINT [DF_Slajd_CzyPokazywacTytul]  DEFAULT ((0))
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


SET @colname = 'Tekst4'
SET @dbType = 'nvarchar(4000) NULL'--[bit] NOT NULL CONSTRAINT [DF_Slajd_CzyPokazywacTytul]  DEFAULT ((0))
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

SET @colname = 'Tekst5'
SET @dbType = 'nvarchar(4000) NULL'--[bit] NOT NULL CONSTRAINT [DF_Slajd_CzyPokazywacTytul]  DEFAULT ((0))
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


/* Pol liczbowe do idZdjecia */


SET @colname = 'ZdjecieId1'
if NOT exists (select column_name from INFORMATION_SCHEMA.columns where table_name = @tblName and column_name = @colname) 
begin
SET @dbType = 'int NULL'--[bit] NOT NULL CONSTRAINT [DF_Slajd_CzyPokazywacTytul]  DEFAULT ((0))
 
SET @cdcInst = 'dbo_' + @tblName

	---------wyłaczenia monitirowania tabeli
	--exec sys.sp_cdc_disable_table @source_schema='dbo', @source_name= @tblName, @capture_instance= @cdcInst

	------------------------------dodawanie kolumny do tabeli
	EXEC('ALTER TABLE dbo.'+ @tblName +' ADD '+ @colname + ' '+ @dbType  )  
--Dodanie kluczy obcych
EXEC('ALTER TABLE [dbo].['+@tblName+']  WITH NOCHECK ADD  CONSTRAINT [FK_'+@tblName+'_Plik_'+ @colname +'] FOREIGN KEY('+@colname+') REFERENCES [dbo].[Plik] ([Id])')
EXEC('ALTER TABLE [dbo].['+@tblName+'] CHECK CONSTRAINT [FK_'+@tblName+'_Plik_'+ @colname +']')
	      
	-----------włączenie monitorowania tabeli
	--EXEC sys.sp_cdc_enable_table @source_schema = 'dbo',@source_name = @tblName,@role_name = NULL, @supports_net_changes = 1
end


SET @colname = 'ZdjecieId2'
if NOT exists (select column_name from INFORMATION_SCHEMA.columns where table_name = @tblName and column_name = @colname) 
begin
SET @dbType = 'int NULL'--[bit] NOT NULL CONSTRAINT [DF_Slajd_CzyPokazywacTytul]  DEFAULT ((0))
    
SET @cdcInst = 'dbo_' + @tblName

	---------wyłaczenia monitirowania tabeli
	--exec sys.sp_cdc_disable_table @source_schema='dbo', @source_name= @tblName, @capture_instance= @cdcInst

	------------------------------dodawanie kolumny do tabeli
	EXEC('ALTER TABLE dbo.'+ @tblName +' ADD '+ @colname + ' '+ @dbType  )  
--Dodanie kluczy obcych
EXEC('ALTER TABLE [dbo].['+@tblName+']  WITH NOCHECK ADD  CONSTRAINT [FK_'+@tblName+'_Plik_'+ @colname +'] FOREIGN KEY('+@colname+') REFERENCES [dbo].[Plik] ([Id])')
EXEC('ALTER TABLE [dbo].['+@tblName+'] CHECK CONSTRAINT [FK_'+@tblName+'_Plik_'+ @colname +']')

	      
	-----------włączenie monitorowania tabeli
	--EXEC sys.sp_cdc_enable_table @source_schema = 'dbo',@source_name = @tblName,@role_name = NULL, @supports_net_changes = 1
end


SET @colname = 'ZdjecieId3'
if NOT exists (select column_name from INFORMATION_SCHEMA.columns where table_name = @tblName and column_name = @colname) 
begin
SET @dbType = 'int NULL'--[bit] NOT NULL CONSTRAINT [DF_Slajd_CzyPokazywacTytul]  DEFAULT ((0))
SET @cdcInst = 'dbo_' + @tblName

	---------wyłaczenia monitirowania tabeli
	--exec sys.sp_cdc_disable_table @source_schema='dbo', @source_name= @tblName, @capture_instance= @cdcInst

	------------------------------dodawanie kolumny do tabeli
	EXEC('ALTER TABLE dbo.'+ @tblName +' ADD '+ @colname + ' '+ @dbType  )  
--Dodanie kluczy obcych
EXEC('ALTER TABLE [dbo].['+@tblName+']  WITH NOCHECK ADD  CONSTRAINT [FK_'+@tblName+'_Plik_'+ @colname +'] FOREIGN KEY('+@colname+') REFERENCES [dbo].[Plik] ([Id])')
EXEC('ALTER TABLE [dbo].['+@tblName+'] CHECK CONSTRAINT [FK_'+@tblName+'_Plik_'+ @colname +']')

	      
	-----------włączenie monitorowania tabeli
	--EXEC sys.sp_cdc_enable_table @source_schema = 'dbo',@source_name = @tblName,@role_name = NULL, @supports_net_changes = 1
end


SET @colname = 'ZdjecieId4'
if NOT exists (select column_name from INFORMATION_SCHEMA.columns where table_name = @tblName and column_name = @colname) 
begin
SET @dbType = 'int NULL'--[bit] NOT NULL CONSTRAINT [DF_Slajd_CzyPokazywacTytul]  DEFAULT ((0))
SET @cdcInst = 'dbo_' + @tblName

	---------wyłaczenia monitirowania tabeli
	--exec sys.sp_cdc_disable_table @source_schema='dbo', @source_name= @tblName, @capture_instance= @cdcInst

	------------------------------dodawanie kolumny do tabeli
	EXEC('ALTER TABLE dbo.'+ @tblName +' ADD '+ @colname + ' '+ @dbType  )  
--Dodanie kluczy obcych
EXEC('ALTER TABLE [dbo].['+@tblName+']  WITH NOCHECK ADD  CONSTRAINT [FK_'+@tblName+'_Plik_'+ @colname +'] FOREIGN KEY('+@colname+') REFERENCES [dbo].[Plik] ([Id])')
EXEC('ALTER TABLE [dbo].['+@tblName+'] CHECK CONSTRAINT [FK_'+@tblName+'_Plik_'+ @colname +']')

	      
	-----------włączenie monitorowania tabeli
	--EXEC sys.sp_cdc_enable_table @source_schema = 'dbo',@source_name = @tblName,@role_name = NULL, @supports_net_changes = 1
end


SET @colname = 'ZdjecieId5'
if NOT exists (select column_name from INFORMATION_SCHEMA.columns where table_name = @tblName and column_name = @colname) 
begin
 
SET @cdcInst = 'dbo_' + @tblName

	---------wyłaczenia monitirowania tabeli
	--exec sys.sp_cdc_disable_table @source_schema='dbo', @source_name= @tblName, @capture_instance= @cdcInst

	------------------------------dodawanie kolumny do tabeli
	EXEC('ALTER TABLE dbo.'+ @tblName +' ADD '+ @colname + ' '+ @dbType  )  
--Dodanie kluczy obcych   
EXEC('ALTER TABLE [dbo].['+@tblName+']  WITH NOCHECK ADD  CONSTRAINT [FK_'+@tblName+'_Plik_'+ @colname +'] FOREIGN KEY('+@colname+') REFERENCES [dbo].[Plik] ([Id])')
EXEC('ALTER TABLE [dbo].['+@tblName+'] CHECK CONSTRAINT [FK_'+@tblName+'_Plik_'+ @colname +']')
     

	      
	-----------włączenie monitorowania tabeli
	--EXEC sys.sp_cdc_enable_table @source_schema = 'dbo',@source_name = @tblName,@role_name = NULL, @supports_net_changes = 1
end

ALTER TABLE [BlogWpis] ALTER COLUMN [AutorId] [bigint] NULL
UPDATE [BlogWpis] SET [AutorId] = NULL WHERE [AutorId] = 0

ALTER TABLE [dbo].[BlogWpis]  WITH NOCHECK ADD  CONSTRAINT [FK_BlogWpis_Autor] FOREIGN KEY([AutorId]) REFERENCES [dbo].[Klient] ([Id]) ON DELETE SET NULL
ALTER TABLE [dbo].[BlogWpis] CHECK CONSTRAINT [FK_BlogWpis_Autor]

if NOT exists ( SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID('FK_BlogWpisPlik_ZdjecieId1') AND parent_object_id = OBJECT_ID('BlogWpis'))
begin
	ALTER TABLE [dbo].[BlogWpis]  WITH NOCHECK ADD  CONSTRAINT [FK_BlogWpisPlik_ZdjecieId1] FOREIGN KEY([ZdjecieId1]) REFERENCES [dbo].[Plik] ([Id])
	ALTER TABLE [dbo].[BlogWpis] CHECK CONSTRAINT [FK_BlogWpisPlik_ZdjecieId1]
end
if NOT exists ( SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID('FK_BlogWpisPlik_ZdjecieId2') AND parent_object_id = OBJECT_ID('BlogWpis'))
begin
	ALTER TABLE [dbo].[BlogWpis]  WITH NOCHECK ADD  CONSTRAINT [FK_BlogWpisPlik_ZdjecieId2] FOREIGN KEY([ZdjecieId2]) REFERENCES [dbo].[Plik] ([Id])
	ALTER TABLE [dbo].[BlogWpis] CHECK CONSTRAINT [FK_BlogWpisPlik_ZdjecieId2]
end
if NOT exists ( SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID('FK_BlogWpisPlik_ZdjecieId3') AND parent_object_id = OBJECT_ID('BlogWpis'))
begin
	ALTER TABLE [dbo].[BlogWpis]  WITH NOCHECK ADD  CONSTRAINT [FK_BlogWpisPlik_ZdjecieId3] FOREIGN KEY([ZdjecieId3]) REFERENCES [dbo].[Plik] ([Id])
	ALTER TABLE [dbo].[BlogWpis] CHECK CONSTRAINT [FK_BlogWpisPlik_ZdjecieId3]
end
if NOT exists ( SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID('FK_BlogWpisPlik_ZdjecieId4') AND parent_object_id = OBJECT_ID('BlogWpis'))
begin
	ALTER TABLE [dbo].[BlogWpis]  WITH NOCHECK ADD  CONSTRAINT [FK_BlogWpisPlik_ZdjecieId4] FOREIGN KEY([ZdjecieId4]) REFERENCES [dbo].[Plik] ([Id])
	ALTER TABLE [dbo].[BlogWpis] CHECK CONSTRAINT [FK_BlogWpisPlik_ZdjecieId4]
end
if NOT exists ( SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID('FK_BlogWpisPlik_ZdjecieId5') AND parent_object_id = OBJECT_ID('BlogWpis'))
begin
	ALTER TABLE [dbo].[BlogWpis]  WITH NOCHECK ADD  CONSTRAINT [FK_BlogWpisPlik_ZdjecieId5] FOREIGN KEY([ZdjecieId5]) REFERENCES [dbo].[Plik] ([Id])
	ALTER TABLE [dbo].[BlogWpis] CHECK CONSTRAINT [FK_BlogWpisPlik_ZdjecieId5]
end


--alter table TrescWiersz add AcordionNazwa nvarchar(500) 
SET @tblName = 'TrescWiersz'
SET @colname = 'AcordionNazwa'
SET @dbType = 'nvarchar(500)'--[bit] NOT NULL CONSTRAINT [DF_Slajd_CzyPokazywacTytul]  DEFAULT ((0)) -- tym z domyslna wartością 

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

--alter table TrescWiersz add AcordionZwiniety bit not null default(0);
SET @tblName = 'TrescWiersz'
SET @colname = 'AcordionZwiniety'
SET @dbType = 'bit not null CONSTRAINT [DF_Tresc_AcordionZwiniety]  default(0)'--[bit] NOT NULL CONSTRAINT [DF_Slajd_CzyPokazywacTytul]  DEFAULT ((0)) -- tym z domyslna wartością 

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

if exists (select column_name from INFORMATION_SCHEMA.columns where table_name = 'TrescWiersz' and column_name = 'Wyrownanie') ALTER TABLE TrescWiersz  DROP COLUMN  [Wyrownanie];

if exists (select column_name from INFORMATION_SCHEMA.columns where table_name = 'KategoriaProduktu' and column_name = 'OpisKrotki') ALTER TABLE KategoriaProduktu  DROP COLUMN  [OpisKrotki];