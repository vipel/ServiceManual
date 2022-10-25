USE [ServiceDev]
GO

/****** Object:  Table [dbo].[service_task]    Script Date: 25 Oct 2022 17.49.23 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[service_task](
	[Id] [int] NOT NULL,
	[FactoryDeviceId] [int] NOT NULL,
	[Created] [datetime] NULL,
	[Description] [nvarchar](4000) NULL,
	[Criticality] [int] NULL,
	[State] [int] NULL,
 CONSTRAINT [PK_service_task] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO

