
if exists (select column_name from INFORMATION_SCHEMA.columns where table_name = 'ProduktCecha' ) 
begin

--exec sys.sp_cdc_disable_table @source_schema='dbo',	@source_name= [ProduktCecha], @capture_instance= 'dbo_ProduktCecha'
DROP TABLE [ProduktCecha];

CREATE TABLE [dbo].[ProduktCecha](
	[Id] [bigint] NOT NULL,
	[CechaId] [bigint] NOT NULL,
	[ProduktId] [bigint] NOT NULL,
 CONSTRAINT [PK_cechy_produkty] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY],
 CONSTRAINT [IX_cechy_produkty] UNIQUE NONCLUSTERED 
(
	[CechaId] ASC,
	[ProduktId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]



ALTER TABLE [dbo].[ProduktCecha]  WITH NOCHECK ADD  CONSTRAINT [FK_cechy_produkty_produkty] FOREIGN KEY([ProduktId])
REFERENCES [dbo].[Produkt] ([Id])
ON DELETE CASCADE


ALTER TABLE [dbo].[ProduktCecha] CHECK CONSTRAINT [FK_cechy_produkty_produkty]


ALTER TABLE [dbo].[ProduktCecha]  WITH CHECK ADD  CONSTRAINT [FK_Hcechy_produkty_Hcechy_produktow] FOREIGN KEY([CechaId])
REFERENCES [dbo].[Cecha] ([Id])
ON DELETE CASCADE


ALTER TABLE [dbo].[ProduktCecha] CHECK CONSTRAINT [FK_Hcechy_produkty_Hcechy_produktow]

end