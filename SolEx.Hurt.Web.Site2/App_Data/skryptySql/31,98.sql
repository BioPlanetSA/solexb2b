

IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='Komunikaty' AND xtype='U')
Begin

CREATE TABLE [dbo].[Komunikaty](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[Nazwa] [nvarchar](200) NOT NULL,
	[Tresc] [nvarchar](max) NOT NULL,
	[OdKiedy] [datetime] NULL,
	[Przycisk] [nvarchar](100) NOT NULL,
	[CyklPokazywania] [nvarchar](100) NOT NULL,
 CONSTRAINT [PK_Komunikaty] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]


end



