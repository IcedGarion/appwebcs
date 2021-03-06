GO
CREATE DATABASE Progetto;
GO


/****** TABELLA UTENTE ******/
USE [Progetto]
GO

/****** Object:  Table [dbo].[Utente]    Script Date: 08/08/2017 17:03:29 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

SET ANSI_PADDING ON
GO

CREATE TABLE [dbo].[Utente](
	[CD_UTENTE] [int] IDENTITY(1,1) NOT NULL,
	[username] [varchar](50) NOT NULL UNIQUE,
	[password] [varchar](50) NOT NULL,
	[ruolo] [varchar](10) NULL,
 CONSTRAINT [PK_Utente] PRIMARY KEY CLUSTERED 
(
	[CD_UTENTE] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

SET ANSI_PADDING OFF
GO








/****** TABELLA ORDINE ******/
USE [Progetto]
GO

/****** Object:  Table [dbo].[Ordine]    Script Date: 08/08/2017 17:08:35 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

SET ANSI_PADDING ON
GO

CREATE TABLE [dbo].[Ordine](
	[CD_ORDINE] [int] IDENTITY(1,1) NOT NULL,
	[CD_UTENTE] [int] NOT NULL,
	[dt_inserimento] [date] NOT NULL,
	[totale] [float] NOT NULL,
	[stato] [varchar](10),
 CONSTRAINT [PK_Ordine] PRIMARY KEY CLUSTERED 
(
	[CD_ORDINE] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

SET ANSI_PADDING OFF
GO

ALTER TABLE [dbo].[Ordine]  WITH CHECK ADD  CONSTRAINT [FK_Ordine_Utente] FOREIGN KEY([CD_UTENTE])
REFERENCES [dbo].[Utente] ([CD_UTENTE])
GO

ALTER TABLE [dbo].[Ordine] CHECK CONSTRAINT [FK_Ordine_Utente]
GO













/****** TABELLA PRODOTTO ******/
USE [Progetto]
GO

/****** Object:  Table [dbo].[Prodotto]    Script Date: 08/08/2017 17:14:15 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

SET ANSI_PADDING ON
GO

CREATE TABLE [dbo].[Prodotto](
	[CD_PRODOTTO] [int] IDENTITY(1,1) NOT NULL,
	[titolo] [varchar](255) NOT NULL,
	[descrizione] [text] NOT NULL,
	[prezzo] [float] NOT NULL,
	[sconto] [float] NOT NULL,
	[immagine] [varbinary](max) NULL,
	[disponibile] [varchar](10) NOT NULL,
 CONSTRAINT [PK_Prodotto] PRIMARY KEY CLUSTERED 
(
	[CD_PRODOTTO] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO

SET ANSI_PADDING OFF
GO










/****** TABELLA ORDINE_PRODOTTO ******/
USE [Progetto]
GO

/****** Object:  Table [dbo].[Ordine_Prodotto]    Script Date: 08/08/2017 17:17:19 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[Ordine_Prodotto](
	[CD_ORDINE] [int] NOT NULL,
	[CD_PRODOTTO] [int] NOT NULL,
	[quantita] [int] NOT NULL,
 CONSTRAINT [PK_Ordine_Prodotto] PRIMARY KEY CLUSTERED 
(
	[CD_ORDINE] ASC,
	[CD_PRODOTTO] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

ALTER TABLE [dbo].[Ordine_Prodotto]  WITH CHECK ADD  CONSTRAINT [FK_Ordine_Prodotto_Ordine] FOREIGN KEY([CD_ORDINE])
REFERENCES [dbo].[Ordine] ([CD_ORDINE])
GO

ALTER TABLE [dbo].[Ordine_Prodotto] CHECK CONSTRAINT [FK_Ordine_Prodotto_Ordine]
GO

ALTER TABLE [dbo].[Ordine_Prodotto]  WITH CHECK ADD  CONSTRAINT [FK_Ordine_Prodotto_Prodotto] FOREIGN KEY([CD_PRODOTTO])
REFERENCES [dbo].[Prodotto] ([CD_PRODOTTO])
GO

ALTER TABLE [dbo].[Ordine_Prodotto] CHECK CONSTRAINT [FK_Ordine_Prodotto_Prodotto]
GO