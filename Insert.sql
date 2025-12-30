USE [CliCAR];
GO

UPDATE VisitaReserva 
SET ID_Comprador = 'd009a308-c6fd-415e-bfdd-5a13710376fd' 
WHERE ID_Anuncio IN (7, 8, 9);