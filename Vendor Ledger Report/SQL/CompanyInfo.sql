
USE BNBDAT;
GO

CREATE OR ALTER VIEW dbo.vw_AP_COMPANY_INFO
AS
SELECT TOP (1)
    comp.CONAME AS [Company Name],
    comp.LEGALNAME AS [Company Legal Name],

    NULLIF(LTRIM(RTRIM(comp.ADDR01)), '')
        AS [Address Line 1],

    NULLIF(LTRIM(RTRIM(comp.ADDR02)), '')
        AS [Address Line 2],

    NULLIF(LTRIM(RTRIM(comp.ADDR03)), '')
        AS [Address Line 3],

    CONCAT(
        NULLIF(LTRIM(RTRIM(comp.ADDR01)), ''),
        CASE
            WHEN NULLIF(LTRIM(RTRIM(comp.ADDR02)), '') IS NOT NULL
            THEN ' ' + LTRIM(RTRIM(comp.ADDR02))
            ELSE ''
        END
    ) AS [Company Address]

FROM dbo.CSCOM AS comp;
GO

-- Test the view
SELECT *
FROM dbo.vw_AP_COMPANY_INFO;
GO
