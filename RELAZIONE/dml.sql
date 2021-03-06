/* Script SQL per il popolamento delle tabelle
 * IMPORTANTE! 
 * Inserire il path della repository (per inserimento immagini)
 * In tutti i prodotti con immagine
 * Esempio:
 * Openrowset( Bulk 'C:/Users/windpw/Desktop/scuola/app web/AppWebProj/resources/water.png', Single_Blob) as image
  */

DECLARE @ROOT VARCHAR(500) = 'C:\Users\windpw\Desktop\scuola\app web\appwebcs-master';
DECLARE @SQL VARCHAR(MAX);

/***** inserisce nuovo prodotto  con immagine ******/
SET @SQL = 'INSERT INTO [dbo].[Prodotto]
           ([titolo]
           ,[descrizione]
           ,[prezzo]
           ,[sconto]
           ,[disponibile]
		   ,[immagine])

	SELECT ''Acqua''
           ,''Bottiglia da 50 ml in plastica. Naturale''
           ,1
           ,0
           ,''no''
		   , BulkColumn FROM Openrowset( Bulk ''' + @ROOT + '/resources/water.png'', Single_Blob) as image'

EXEC(@SQL);


/***** inserisce nuovo prodotto  con immagine ******/
SET @SQL = 'INSERT INTO [dbo].[Prodotto]
           ([titolo]
           ,[descrizione]
           ,[prezzo]
           ,[sconto]
           ,[disponibile]
		   ,[immagine])

	SELECT ''Raspberry''
           ,''Raspberry PI 3''
           ,33.99
           ,4
           ,''si''
		   , BulkColumn FROM Openrowset( Bulk ''' + @ROOT + '/resources/raspberry.png'', Single_Blob) as image'
           
EXEC(@SQL);


/***** inserisce nuovo prodotto  con immagine ******/
SET @SQL = 'INSERT INTO [dbo].[Prodotto]
           ([titolo]
           ,[descrizione]
           ,[prezzo]
           ,[sconto]
           ,[disponibile]
		   ,[immagine])

	SELECT ''Abbonamento Netflix''
           ,''Abbonamento Neflix mensile 4 schermi''
           ,11.99
           ,0
           ,''si''
		   , BulkColumn FROM Openrowset( Bulk ''' + @ROOT + '/resources/netflix.png'', Single_Blob) as image'
           
EXEC(@SQL);


/***** inserisce nuovo prodotto  con immagine ******/
SET @SQL = 'INSERT INTO [dbo].[Prodotto]
           ([titolo]
           ,[descrizione]
           ,[prezzo]
           ,[sconto]
           ,[disponibile]
		   ,[immagine])

	SELECT ''Chitarra Elettrica''
           ,''Fender Stratocaster Colore Nero 6 corde''
           ,164.99
           ,0
           ,''si''
		   , BulkColumn FROM Openrowset( Bulk ''' + @ROOT + '/resources/guitar.png'', Single_Blob) as image'
           
EXEC(@SQL);


/***** inserisce nuovo prodotto  con immagine ******/
SET @SQL = 'INSERT INTO [dbo].[Prodotto]
           ([titolo]
           ,[descrizione]
           ,[prezzo]
           ,[sconto]
           ,[disponibile]
		   ,[immagine])

	SELECT ''Fotocamera Digitale''
           ,''Nikon D5500 + Nikkor 18-140 VR Fotocamera Reflex Digitale, 24,2 Megapixel, LCD Touchscreen regolabile, Wi-Fi incorporato, SD 8GB 200x Premium Lexar''
           ,164.99
           ,0
           ,''si''
		   , BulkColumn 
		   FROM Openrowset( Bulk ''' + @ROOT + '/resources/Nikon.png'', Single_Blob) as image'
EXEC(@SQL);


/***** inserisce nuovo prodotto  con immagine ******/
SET @SQL = 'INSERT INTO [dbo].[Prodotto]
           ([titolo]
           ,[descrizione]
           ,[prezzo]
           ,[sconto]
           ,[disponibile]
		   ,[immagine])

	SELECT ''GTX 1080''
           ,''Asus GeForce STRIX-GTX1080-A8G-GAMING Scheda Grafica da Gaming, 8 GB GDDR5X, PCI Express 3.0, Nero''
           ,759.00
           ,152.00
           ,''no''
		   , BulkColumn 
		   FROM Openrowset( Bulk ''' + @ROOT + '/resources/gtx1080.png'', Single_Blob) as image'
           
EXEC(@SQL);


/***** inserisce nuovo prodotto  con immagine ******/
SET @SQL = 'INSERT INTO [dbo].[Prodotto]
           ([titolo]
           ,[descrizione]
           ,[prezzo]
           ,[sconto]
           ,[disponibile]
		   ,[immagine])

	SELECT ''TP-Link TL-WDN4800 Scheda di Rete Wireless''
           ,''TP-Link TL-WDN4800 Scheda di Rete Wireless Dualband N 900 Mbps PCIe, 450 Mbps/2.4 Ghz & 450 Mbps/5 Ghz, 3 Antenne Esterne, Crittografia WPA/WPA2, Semplice Configurazione''
           ,37.09
           ,12.00
           ,''si''
		   , BulkColumn FROM Openrowset( Bulk ''' + @ROOT + '/resources/tplinkwifi.png'', Single_Blob) as image'
           
EXEC(@SQL);


/***** inserisce nuovo prodotto  con immagine ******/
SET @SQL = 'INSERT INTO [dbo].[Prodotto]
           ([titolo]
           ,[descrizione]
           ,[prezzo]
           ,[sconto]
           ,[disponibile]
		   ,[immagine])

	SELECT ''HP X3500 Mouse Wireless, Nero''
           ,''Sensore ottico e 3 pulsanti, Ricevitore USB wireless a 2,4 GHz. Nella confezione: mouse wireless HP X3500, due batterie AA, software e documentazione, scheda di garanzia ''
           ,12.90
           ,0
           ,''si''
		   , BulkColumn 
		   FROM Openrowset( Bulk ''' + @ROOT + '/resources/hpx3500.jpg'', Single_Blob) as image'
           
EXEC(@SQL);


/***** inserisce nuovo prodotto  con immagine ******/
SET @SQL = 'INSERT INTO [dbo].[Prodotto]
           ([titolo]
           ,[descrizione]
           ,[prezzo]
           ,[sconto]
           ,[disponibile]
		   ,[immagine])

	SELECT ''Yankee candle 1093707E''
           ,''Yankee candle 1093707E Vanilla Cupcake Candele in giara grande, Vetro, Giallo, 9.9x9.8x15.4 cm''
           ,17.99
           ,2.02
           ,''no''
		   , BulkColumn 
		   FROM Openrowset( Bulk ''' + @ROOT + '/resources/yankeecandle_.jpg'', Single_Blob) as image'
           
EXEC(@SQL);



/***** inserisce nuovo prodotto  con immagine ******/
SET @SQL = 'INSERT INTO [dbo].[Prodotto]
           ([titolo]
           ,[descrizione]
           ,[prezzo]
           ,[sconto]
           ,[disponibile]
		   ,[immagine])

	SELECT ''Power Bank''
           ,''20000mAh Caricabatterie Portatile Power Bank Batteria Esterna 3 porte USB''
           ,19.99
           ,0
           ,''si''
		   , BulkColumn 
		   FROM Openrowset( Bulk ''' + @ROOT + '/resources/powerbank.png'', Single_Blob) as image'
           
EXEC(@SQL);


/***** inserisce nuovo prodotto  con immagine ******/
SET @SQL = 'INSERT INTO [dbo].[Prodotto]
           ([titolo]
           ,[descrizione]
           ,[prezzo]
           ,[sconto]
           ,[disponibile]
		   ,[immagine])

	SELECT ''DVD''
           ,''DVD-R 16x Speed 4,7GB , confezione da 25''
           ,52.05
           ,40.58
           ,''si''
		   , BulkColumn 
		   FROM Openrowset( Bulk ''' + @ROOT + '/resources/dvd.png'', Single_Blob) as image'
           
EXEC(@SQL);


/****** FINE IMMAGINI  ******/




/****** inserisce utente ******/
USE [Progetto]
GO

INSERT INTO [dbo].[Utente]
           ([username]
           ,[password]
		   ,[ruolo])
     VALUES
           ('marcoRossi@user.upo', '1234', 'user')
GO

USE [Progetto]
GO

INSERT INTO [dbo].[Utente]
           ([username]
           ,[password]
		   ,[ruolo])
     VALUES
           ('francoNeri@admin.upo', 'asdf', 'admin')
GO





/***** inserisce nuovo prodotto  ******/
USE [Progetto]
GO

INSERT INTO [dbo].[Prodotto]
           ([titolo]
           ,[descrizione]
           ,[prezzo]
           ,[sconto]
           ,[disponibile])
     VALUES
           ('Notebook ASUS'
           ,'ASUS K550VX-GO405T 2.8GHz 500GB HDD i7-7700HQ Computer Portatile Notebook 15.6" 1366 x 768Pixel, Acciaio Inossidabile, Grigio '
           ,'500'
           ,'10'
           ,'si')
GO


/****** inserisce nuovo ordine ******/
USE [Progetto]
GO

INSERT INTO [dbo].[Ordine]
           ([CD_UTENTE]
           ,[dt_inserimento]
           ,[totale]
           ,[stato])
     VALUES
           (1
           ,'2017-08-02'
           ,'490'
           ,'processed')
GO







/****** join ordine e prodotti ******/
USE [Progetto]
GO

INSERT INTO [dbo].[Ordine_Prodotto]
           ([CD_ORDINE]
           ,[CD_PRODOTTO]
           ,[quantita])
     VALUES
           (1
           ,1
           ,1)
GO

USE [Progetto]
GO

INSERT INTO [dbo].[Ordine_Prodotto]
           ([CD_ORDINE]
           ,[CD_PRODOTTO]
           ,[quantita])
     VALUES
           (1
           ,2
           ,1)
GO



/***** una select per gli ordini ******/
USE [Progetto]
GO

select Utente.username, Ordine.CD_ORDINE, Ordine.totale, Prodotto.titolo from 
	ordine join Utente on Ordine.CD_UTENTE = Utente.CD_UTENTE
	join Ordine_Prodotto on Ordine.CD_ORDINE = Ordine_Prodotto.CD_ORDINE
	join prodotto on prodotto.CD_PRODOTTO = Ordine_Prodotto.CD_PRODOTTO
GO
