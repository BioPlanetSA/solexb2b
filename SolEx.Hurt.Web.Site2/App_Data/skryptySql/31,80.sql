IF (EXISTS (SELECT *  FROM INFORMATION_SCHEMA.TABLES 
                 WHERE TABLE_SCHEMA = 'dbo' 
                 AND  TABLE_NAME = 'WidocznosciTypow'))
BEGIN


   DROP TABLE [dbo].[WidocznosciTypow];
END


   CREATE TABLE [dbo].[WidocznosciTypow](
	[Id] [bigint] NOT NULL,
	[ObiektId] [bigint] NULL,
	[Typ] [varchar](200) NULL,
	[KategoriaKlientaIdWszystkie] [varchar](400) NOT NULL,
	[Kierunek] [varchar](50) NOT NULL,
	[KategoriaKlientaIdKtorakolwiek] [varchar](400) NOT NULL,
	[Nazwa] [varchar](500) NULL,
 CONSTRAINT [PK_WidocznosciTypow] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY];



