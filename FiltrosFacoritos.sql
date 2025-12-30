ALTER TABLE dbo.FiltrosFavoritos
ADD Nome NVARCHAR(100) NOT NULL DEFAULT 'Sem Nome',
    FiltrosJson NVARCHAR(MAX) NULL;