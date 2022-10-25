USE [ServiceDev]
GO

/****** Object:  Table [dbo].[factory_device]    Script Date: 25 Oct 2022 17.48.59 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[factory_device](
	[Id] [int] NOT NULL,
	[Name] [nvarchar](20) NULL,
	[Year] [int] NULL,
	[Type] [nvarchar](10) NULL,
 CONSTRAINT [PK_factory_device] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO

