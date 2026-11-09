USE BNBDAT;
GO

CREATE OR ALTER VIEW vw_AP_VENDOR_TRANSACTION
AS

WITH AllAccountSets AS
(
    SELECT
        ACCTSET AS IDACCTSET,
        TEXTDESC
    FROM APRAS
),
CalculatedLedger AS
(
    SELECT
        Ledger.SourceTable,
        Ledger.TransactionDate,
        Ledger.FISCYR AS [Periodic Year],
        Ledger.FISCPER AS [Periodic Month],
        Ledger.TransactionType,
        Ledger.IDVEND AS [Vendor ID],
        Ledger.DOC_NUMBER AS [DOC NUMBER],
        Ledger.AppliedDocument AS [Applied Document],
        vend.VENDNAME AS [Vendor Name],

        COALESCE(
            NULLIF(LTRIM(RTRIM(Ledger.IDACCTSET)), ''),
            vend.IDACCTSET
        ) AS [Account Set ID],

        vend.IDACCTSET AS [Vendor Main Account Set ID],

        COALESCE(
            NULLIF(LTRIM(RTRIM(Ledger.IDACCTSET)), ''),
            vend.IDACCTSET
        ) AS [Acct Set ID Detail],

        Ledger.CNTBTCH AS [Batch Number],
        Ledger.CNTITEM AS [Entry No],
        Ledger.SourceCode,
        Ledger.DocNumber AS [Document Number],
        Ledger.Description AS [Entry Description],
        Ledger.AuditUser AS [Audit User],
        Ledger.BankCode,
        bank.NAME AS [Bank Name],

        Ledger.Invoice,
        Ledger.DebitNote AS [Debit Note],
        Ledger.CreditNote AS [Credit Note],
        Ledger.Interest,
        Ledger.Payment,
        Ledger.Prepayment,
        Ledger.ApplyDocument AS [Apply Document],
        Ledger.MiscPayment AS [Misc. Payment],
        Ledger.Adjustment,

        ISNULL(Ledger.Invoice, 0)
          + ISNULL(Ledger.DebitNote, 0)
          + ISNULL(Ledger.Interest, 0)
          - ISNULL(Ledger.CreditNote, 0)
          - ISNULL(Ledger.Payment, 0)
          - ISNULL(Ledger.Prepayment, 0)
          - ISNULL(Ledger.ApplyDocument, 0)
          - ISNULL(Ledger.MiscPayment, 0)
          + ISNULL(Ledger.Adjustment, 0)
          AS [Transaction Amount],

        /* OPENING BALANCE: ORDER BY ekhon fully deterministic (closing er sathe ek-i) */
        COALESCE(
            SUM(
                ISNULL(Ledger.Invoice, 0)
                  + ISNULL(Ledger.DebitNote, 0)
                  + ISNULL(Ledger.Interest, 0)
                  - ISNULL(Ledger.CreditNote, 0)
                  - ISNULL(Ledger.Payment, 0)
                  - ISNULL(Ledger.Prepayment, 0)
                  - ISNULL(Ledger.ApplyDocument, 0)
                  - ISNULL(Ledger.MiscPayment, 0)
                  + ISNULL(Ledger.Adjustment, 0)
            ) OVER (
                PARTITION BY Ledger.IDVEND
                ORDER BY
                    Ledger.TransactionDate,
                    Ledger.CNTBTCH,
                    Ledger.CNTITEM,
                    Ledger.SortSeq,
                    Ledger.DOC_NUMBER,
                    /* NEW: stable tie-breakers */
                    Ledger.SourceTable,
                    Ledger.AppliedDocument,
                    Ledger.DocNumber,
                    Ledger.TransactionType,
                    (
                        ISNULL(Ledger.Invoice, 0)
                          + ISNULL(Ledger.DebitNote, 0)
                          + ISNULL(Ledger.Interest, 0)
                          - ISNULL(Ledger.CreditNote, 0)
                          - ISNULL(Ledger.Payment, 0)
                          - ISNULL(Ledger.Prepayment, 0)
                          - ISNULL(Ledger.ApplyDocument, 0)
                          - ISNULL(Ledger.MiscPayment, 0)
                          + ISNULL(Ledger.Adjustment, 0)
                    )
                ROWS BETWEEN UNBOUNDED PRECEDING AND 1 PRECEDING
            ),
            0
        ) AS [Opening Balance],

        CASE
            WHEN (
                ISNULL(Ledger.Invoice, 0)
                  + ISNULL(Ledger.DebitNote, 0)
                  + ISNULL(Ledger.Interest, 0)
                  - ISNULL(Ledger.CreditNote, 0)
                  - ISNULL(Ledger.Payment, 0)
                  - ISNULL(Ledger.Prepayment, 0)
                  - ISNULL(Ledger.ApplyDocument, 0)
                  - ISNULL(Ledger.MiscPayment, 0)
                  + ISNULL(Ledger.Adjustment, 0)
            ) < 0
            THEN ABS(
                ISNULL(Ledger.Invoice, 0)
                  + ISNULL(Ledger.DebitNote, 0)
                  + ISNULL(Ledger.Interest, 0)
                  - ISNULL(Ledger.CreditNote, 0)
                  - ISNULL(Ledger.Payment, 0)
                  - ISNULL(Ledger.Prepayment, 0)
                  - ISNULL(Ledger.ApplyDocument, 0)
                  - ISNULL(Ledger.MiscPayment, 0)
                  + ISNULL(Ledger.Adjustment, 0)
            )
            ELSE 0
        END AS [Debit],

        CASE
            WHEN (
                ISNULL(Ledger.Invoice, 0)
                  + ISNULL(Ledger.DebitNote, 0)
                  + ISNULL(Ledger.Interest, 0)
                  - ISNULL(Ledger.CreditNote, 0)
                  - ISNULL(Ledger.Payment, 0)
                  - ISNULL(Ledger.Prepayment, 0)
                  - ISNULL(Ledger.ApplyDocument, 0)
                  - ISNULL(Ledger.MiscPayment, 0)
                  + ISNULL(Ledger.Adjustment, 0)
            ) > 0
            THEN (
                ISNULL(Ledger.Invoice, 0)
                  + ISNULL(Ledger.DebitNote, 0)
                  + ISNULL(Ledger.Interest, 0)
                  - ISNULL(Ledger.CreditNote, 0)
                  - ISNULL(Ledger.Payment, 0)
                  - ISNULL(Ledger.Prepayment, 0)
                  - ISNULL(Ledger.ApplyDocument, 0)
                  - ISNULL(Ledger.MiscPayment, 0)
                  + ISNULL(Ledger.Adjustment, 0)
            )
            ELSE 0
        END AS [Credit],

        /* CLOSING AMOUNT: Opening er sathe hubohu ek-i ORDER BY */
        SUM(
            ISNULL(Ledger.Invoice, 0)
              + ISNULL(Ledger.DebitNote, 0)
              + ISNULL(Ledger.Interest, 0)
              - ISNULL(Ledger.CreditNote, 0)
              - ISNULL(Ledger.Payment, 0)
              - ISNULL(Ledger.Prepayment, 0)
              - ISNULL(Ledger.ApplyDocument, 0)
              - ISNULL(Ledger.MiscPayment, 0)
              + ISNULL(Ledger.Adjustment, 0)
        ) OVER (
            PARTITION BY Ledger.IDVEND
            ORDER BY
                Ledger.TransactionDate,
                Ledger.CNTBTCH,
                Ledger.CNTITEM,
                Ledger.SortSeq,
                Ledger.DOC_NUMBER,
                /* NEW: stable tie-breakers */
                Ledger.SourceTable,
                Ledger.AppliedDocument,
                Ledger.DocNumber,
                Ledger.TransactionType,
                (
                    ISNULL(Ledger.Invoice, 0)
                      + ISNULL(Ledger.DebitNote, 0)
                      + ISNULL(Ledger.Interest, 0)
                      - ISNULL(Ledger.CreditNote, 0)
                      - ISNULL(Ledger.Payment, 0)
                      - ISNULL(Ledger.Prepayment, 0)
                      - ISNULL(Ledger.ApplyDocument, 0)
                      - ISNULL(Ledger.MiscPayment, 0)
                      + ISNULL(Ledger.Adjustment, 0)
                )
            ROWS BETWEEN UNBOUNDED PRECEDING AND CURRENT ROW
        ) AS [Closing Amount]

    FROM
    (
        /* 1. APIBH: Invoice / Debit Note / Credit Note / Interest */
        SELECT
            'APIBH' AS SourceTable,
            1 AS SortSeq,
            CONVERT(
                DATE,
                CONVERT(VARCHAR(8), CAST(inv.DATEINVC AS BIGINT)),
                112
            ) AS TransactionDate,
            inv.FISCYR,
            inv.FISCPER,

            CASE inv.TEXTTRX
                WHEN 1 THEN 'Invoice'
                WHEN 2 THEN 'Debit Note'
                WHEN 3 THEN 'Credit Note'
                WHEN 4 THEN 'Interest'
                ELSE 'Invoice'
            END AS TransactionType,

            inv.IDVEND AS IDVEND,
            inv.IDINVC AS DOC_NUMBER,
            CAST(' ' AS VARCHAR(22)) AS AppliedDocument,
            inv.CNTBTCH AS CNTBTCH,
            inv.CNTITEM AS CNTITEM,

            CASE inv.TEXTTRX
                WHEN 1 THEN 'AP-IN'
                WHEN 2 THEN 'AP-DN'
                WHEN 3 THEN 'AP-CR'
                WHEN 4 THEN 'AP-IT'
                ELSE 'AP-IN'
            END AS SourceCode,

            inv.IDINVC AS DocNumber,
            inv.INVCDESC AS Description,
            CAST(NULL AS VARCHAR(8)) AS BankCode,
            inv.IDACCTSET,

            CASE WHEN inv.TEXTTRX = 1 THEN inv.AMTINVCTOT END AS Invoice,
            CASE WHEN inv.TEXTTRX = 2 THEN inv.AMTINVCTOT END AS DebitNote,
            CASE WHEN inv.TEXTTRX = 3 THEN inv.AMTINVCTOT END AS CreditNote,
            CASE WHEN inv.TEXTTRX = 4 THEN inv.AMTINVCTOT END AS Interest,

            CAST(NULL AS DECIMAL(19,3)) AS Payment,
            CAST(NULL AS DECIMAL(19,3)) AS Prepayment,
            CAST(NULL AS DECIMAL(19,3)) AS ApplyDocument,
            CAST(NULL AS DECIMAL(19,3)) AS MiscPayment,
            CAST(NULL AS DECIMAL(19,3)) AS Adjustment,

            inv.AUDTUSER AS AuditUser

        FROM APIBH AS inv
        INNER JOIN APIBC AS ibc
            ON ibc.CNTBTCH = inv.CNTBTCH
        WHERE ISNULL(inv.ERRBATCH, 0) <= 0
          AND ibc.BTCHSTTS = 3

        UNION ALL

        /* 2. APOBL: Prepayment */
        SELECT
            'APOBL' AS SourceTable,
            1 AS SortSeq,
            CONVERT(
                DATE,
                CONVERT(VARCHAR(8), CAST(obl.DATEINVC AS BIGINT)),
                112
            ) AS TransactionDate,
            obl.FISCYR,
            obl.FISCPER,
            'Prepayment' AS TransactionType,
            obl.IDVEND AS IDVEND,
            obl.IDINVC AS DOC_NUMBER,
            ISNULL(NULLIF(LTRIM(RTRIM(obl.IDRMIT)), ''), 'N/A') AS AppliedDocument,
            obl.CNTBTCH AS CNTBTCH,
            obl.CNTITEM AS CNTITEM,
            'AP-PI' AS SourceCode,
            obl.IDINVC AS DocNumber,
            obl.DESCINVC AS Description,
            obl.IDBANK AS BankCode,
            obl.IDACCTSET,

            CAST(NULL AS DECIMAL(19,3)) AS Invoice,
            CAST(NULL AS DECIMAL(19,3)) AS DebitNote,
            CAST(NULL AS DECIMAL(19,3)) AS CreditNote,
            CAST(NULL AS DECIMAL(19,3)) AS Interest,
            CAST(NULL AS DECIMAL(19,3)) AS Payment,
            ABS(obl.AMTINVCHC) AS Prepayment,
            CAST(NULL AS DECIMAL(19,3)) AS ApplyDocument,
            CAST(NULL AS DECIMAL(19,3)) AS MiscPayment,
            CAST(NULL AS DECIMAL(19,3)) AS Adjustment,

            obl.AUDTUSER AS AuditUser

        FROM APOBL AS obl
        WHERE CAST(obl.TXTTRXTYPE AS VARCHAR(10)) = '10'

        UNION ALL

        /* 3. APTCR: Payment / Prepayment / Apply Document / Misc Payment / Adjustment
              (PERFECT version logic, kono change nai) */
        SELECT
            'APTCR' AS SourceTable,
            2 AS SortSeq,
            CONVERT(
                DATE,
                CONVERT(VARCHAR(8), CAST(rcpt.DATERMIT AS BIGINT)),
                112
            ) AS TransactionDate,
            rcpt.FISCYR,
            rcpt.FISCPER,

            CASE rcpt.RMITTYPE
                WHEN 1 THEN 'Payment'
                WHEN 2 THEN 'Prepayment Remittance'
                WHEN 3 THEN 'Apply Document'
                WHEN 4 THEN 'Misc. Payment'
                WHEN 5 THEN 'Adjustment'
                ELSE 'Payment'
            END AS TransactionType,

            rcpt.IDVEND AS IDVEND,

            /* DOC NUMBER from APTCP.CNTADJ (fallback to old value if no match) */
            CASE
                WHEN rcpt.RMITTYPE = 5
                THEN CONCAT(
                        'ADJ-',   /* prefix for Adjustment only */
                        COALESCE(
                            NULLIF(LTRIM(RTRIM(CAST(pcp.CNTADJ AS VARCHAR(22)))), ''),
                            ISNULL(NULLIF(rcpt.DOCNBR, ''), 'N/A')
                        )
                     )
                ELSE COALESCE(
                        NULLIF(LTRIM(RTRIM(CAST(pcp.CNTADJ AS VARCHAR(22)))), ''),
                        ISNULL(NULLIF(rcpt.DOCNBR, ''), 'N/A')
                     )
            END AS DOC_NUMBER,

            /* Applied Document from APTCP.IDINVC (fallback to old value if no match) */
            COALESCE(
                NULLIF(LTRIM(RTRIM(pcp.IDINVC)), ''),
                ISNULL(NULLIF(rcpt.IDRMIT, ''), 'N/A')
            ) AS AppliedDocument,

            rcpt.CNTBTCH AS CNTBTCH,
            rcpt.CNTENTR AS CNTITEM,

            CASE rcpt.RMITTYPE
                WHEN 1 THEN 'AP-PY'
                WHEN 2 THEN 'AP-PI'
                WHEN 3 THEN 'AP-AD'
                WHEN 4 THEN 'AP-MP'
                WHEN 5 THEN 'AP-AD'
                ELSE 'AP-PY'
            END AS SourceCode,

            rcpt.DOCNBR AS DocNumber,
            rcpt.TEXTRMIT AS Description,
            rcpt.IDBANK AS BankCode,
            rcpt.IDACCTSET,

            CAST(NULL AS DECIMAL(19,3)) AS Invoice,
            CAST(NULL AS DECIMAL(19,3)) AS DebitNote,
            CAST(NULL AS DECIMAL(19,3)) AS CreditNote,
            CAST(NULL AS DECIMAL(19,3)) AS Interest,

            /* ORIGINAL (calculation er jonno): Prepayment Remittance er value ekhane thakbe */
            CASE
                WHEN rcpt.RMITTYPE = 1 THEN rcpt.AMTRMIT
                WHEN rcpt.RMITTYPE = 2 THEN -1 * rcpt.AMTRMIT
            END AS Payment,

            CAST(NULL AS DECIMAL(19,3)) AS Prepayment,

            CASE WHEN rcpt.RMITTYPE = 3 THEN rcpt.AMTRMIT END AS ApplyDocument,
            CASE WHEN rcpt.RMITTYPE = 4 THEN rcpt.AMTRMIT END AS MiscPayment,

            CASE
                WHEN rcpt.RMITTYPE = 5
                THEN COALESCE(
                    NULLIF(rcpt.AMTADJTCUR, 0),
                    NULLIF(rcpt.AMTRMIT, 0),
                    0
                )
            END AS Adjustment,

            rcpt.AUDTUSER AS AuditUser

        FROM APTCR AS rcpt

        /* APTCP lookup only (TOP 1) so APTCR transactions never duplicate */
        OUTER APPLY
        (
            SELECT TOP (1)
                cp.IDINVC,
                cp.CNTADJ
            FROM APTCP AS cp
            WHERE cp.IDVEND  = rcpt.IDVEND
              AND cp.CNTBTCH = rcpt.CNTBTCH
              AND cp.CNTRMIT = rcpt.CNTENTR
            ORDER BY cp.CNTADJ, cp.IDINVC
        ) AS pcp

        WHERE ISNULL(rcpt.ERRBATCH, 0) <= 0
          AND EXISTS (
                SELECT 1
                FROM APBTA AS b
                WHERE b.CNTBTCH = rcpt.CNTBTCH
                  AND b.PAYMTYPE = rcpt.BTCHTYPE
                  AND b.PAYMTYPE IN ('AD', 'PY')
                  AND b.BATCHSTAT = 3
              )
          AND (
                rcpt.RMITTYPE <> 2
                OR EXISTS (
                    SELECT 1
                    FROM APOBP AS y
                    WHERE y.IDVEND = rcpt.IDVEND
                      AND y.IDINVC = rcpt.DOCNBR
                      AND y.TRXTYPE = 59
                )
              )
          AND NOT (
                rcpt.RMITTYPE = 2
                AND EXISTS (
                    SELECT 1
                    FROM APOBP AS x
                    WHERE x.IDVEND = rcpt.IDVEND
                      AND x.IDINVC = rcpt.DOCNBR
                      AND x.TRXTYPE = 81
                )
              )

        UNION ALL

        /* 4. APOBP: Existing logic + Prepayment Reversed */
        SELECT
            'APOBP' AS SourceTable,
            2 AS SortSeq,
            CONVERT(
                DATE,
                CONVERT(VARCHAR(8), CAST(p.DATEBUS AS BIGINT)),
                112
            ) AS TransactionDate,
            CAST(LEFT(CAST(p.DATEBUS AS VARCHAR(8)), 4) AS INT) AS FISCYR,
            CAST(SUBSTRING(CAST(p.DATEBUS AS VARCHAR(8)), 5, 2) AS INT) AS FISCPER,

            CASE p.TRXTYPE
                WHEN 43 THEN 'Credit Applied (CT)'
                WHEN 44 THEN 'Credit Note Applied (CF)'
                WHEN 53 THEN 'Payment Reversed'
                WHEN 83 THEN 'Payment-Invoice Reversed'
                WHEN 59 THEN 'Prepayment Reversed'
                ELSE 'Reversal'
            END AS TransactionType,

            p.IDVEND AS IDVEND,
            ISNULL(NULLIF(p.IDINVC, ''), 'N/A') AS DOC_NUMBER,
            ISNULL(NULLIF(p.IDRMIT, ''), 'N/A') AS AppliedDocument,
            p.CNTBTCH AS CNTBTCH,
            p.CNTITEM AS CNTITEM,

            CASE p.TRXTYPE
                WHEN 43 THEN 'AP-CT'
                WHEN 44 THEN 'AP-CF'
                ELSE 'AP-VR'
            END AS SourceCode,

            p.IDINVC AS DocNumber,

            CASE
                WHEN p.TRXTYPE IN (43, 44)
                    THEN 'Credit Note Application'
                WHEN p.TRXTYPE = 59
                    THEN 'Prepayment Reversed'
                ELSE 'Payment Reversal'
            END AS Description,

            p.IDBANK AS BankCode,
            CAST(NULL AS VARCHAR(6)) AS IDACCTSET,

            CAST(NULL AS DECIMAL(19,3)) AS Invoice,
            CAST(NULL AS DECIMAL(19,3)) AS DebitNote,
            CAST(NULL AS DECIMAL(19,3)) AS CreditNote,
            CAST(NULL AS DECIMAL(19,3)) AS Interest,
            CAST(NULL AS DECIMAL(19,3)) AS Payment,
            CAST(NULL AS DECIMAL(19,3)) AS Prepayment,
            CAST(NULL AS DECIMAL(19,3)) AS ApplyDocument,
            CAST(NULL AS DECIMAL(19,3)) AS MiscPayment,

            /* Preserve original sign for all APOBP transaction types */
            p.AMTPAYMHC AS Adjustment,

            p.AUDTUSER AS AuditUser

        FROM APOBP AS p
        WHERE p.TRXTYPE IN (43, 44)
           OR p.TRXTYPE = 53
           OR (
                p.TRXTYPE = 83
                AND p.IDINVC LIKE 'LC%'
           )
           OR (
                p.TRXTYPE = 59
                AND p.CNTSEQNCE = 3
           )

    ) AS Ledger

    LEFT JOIN APVEN AS vend
        ON Ledger.IDVEND = vend.VENDORID

    LEFT JOIN BKACCT AS bank
        ON Ledger.BankCode = bank.BANK
),

/* unique account sets per vendor, comma separated */
VendorAcctSets AS
(
    SELECT
        v.[Vendor ID],
        STUFF((
            SELECT DISTINCT ', ' + x.[Acct Set ID Detail]
            FROM CalculatedLedger AS x
            WHERE x.[Vendor ID] = v.[Vendor ID]
              AND NULLIF(LTRIM(RTRIM(x.[Acct Set ID Detail])), '') IS NOT NULL
            FOR XML PATH(''), TYPE
        ).value('.', 'NVARCHAR(MAX)'), 1, 2, '') AS [Vendor Account Set List],

        (
            SELECT COUNT(DISTINCT LTRIM(RTRIM(x.[Acct Set ID Detail])))
            FROM CalculatedLedger AS x
            WHERE x.[Vendor ID] = v.[Vendor ID]
              AND NULLIF(LTRIM(RTRIM(x.[Acct Set ID Detail])), '') IS NOT NULL
        ) AS [NoOfAccS]
    FROM (SELECT DISTINCT [Vendor ID] FROM CalculatedLedger) AS v
),

/* AP-IN invoice + AP-PI prepayment rows per vendor/document
   Used by Early Payment Info for both invoice and prepayment applications */
InvDates AS
(
    SELECT
        z.[Vendor ID],
        z.[DOC NUMBER],
        z.InvBatch,
        z.InvEntry,
        z.InvDate,
        z.InvSourceCode
    FROM
    (
        SELECT
            LTRIM(RTRIM(x.[Vendor ID]))   AS [Vendor ID],
            LTRIM(RTRIM(x.[DOC NUMBER]))  AS [DOC NUMBER],
            x.[Batch Number]              AS InvBatch,
            x.[Entry No]                  AS InvEntry,
            CAST(x.TransactionDate AS DATE) AS InvDate,
            x.SourceCode                  AS InvSourceCode,
            ROW_NUMBER() OVER (
                PARTITION BY LTRIM(RTRIM(x.[Vendor ID])), LTRIM(RTRIM(x.[DOC NUMBER]))
                ORDER BY
                    CASE WHEN x.SourceCode = 'AP-IN' THEN 0 ELSE 1 END,
                    x.TransactionDate, x.[Batch Number], x.[Entry No]
            ) AS rn
        FROM CalculatedLedger AS x
        WHERE x.SourceCode IN ('AP-IN', 'AP-PI', 'AP-CR')
          AND NULLIF(LTRIM(RTRIM(x.[DOC NUMBER])), '') IS NOT NULL
    ) AS z
    WHERE z.rn = 1
)
SELECT
    comp.CONAME AS [Company Name],
    comp.LEGALNAME AS [Company Legal Name],
    RTRIM(
        ISNULL(comp.ADDR01, '') + ' ' +
        ISNULL(comp.ADDR02, '') + ' ' +
        ISNULL(comp.ADDR03, '') + ' ' +
        ISNULL(comp.CITY, '') + ' ' +
        ISNULL(comp.COUNTRY, '')
    ) AS [Company Address],

    cal.SourceTable AS [Source Table],
    cal.TransactionDate,
    cal.[Periodic Year],
    cal.[Periodic Month],
    CONCAT(
        cal.[Periodic Year],
        '-',
        RIGHT('0' + CAST(cal.[Periodic Month] AS VARCHAR(2)), 2)
    ) AS [Fiscal Period],

    cal.TransactionType,
    cal.[Vendor ID],
    cal.[DOC NUMBER],
    cal.[Applied Document],
    COALESCE(cal.[Vendor Name], '') AS [Vendor Name],
    acctset.IDACCTSET AS [Account Set ID],
    acctset.TEXTDESC AS [Account Set Name],

    cal.[Acct Set ID Detail],
    txnset.TEXTDESC AS [Transaction Account Set Name],
    cal.[Batch Number],
    cal.[Entry No],
    cal.SourceCode AS [Source Code],

    CASE
        WHEN cal.SourceCode = 'AP-IN'
        THEN DATEDIFF(DAY, cal.TransactionDate, CAST(GETDATE() AS DATE))
        ELSE NULL
    END AS [Days Over],

    vas.[Vendor Account Set List],
    vas.[NoOfAccS],

    cal.[Document Number],
    cal.[Entry Description],
    cal.[Audit User],
    cal.BankCode,
    cal.[Bank Name],
    cal.Invoice,
    cal.[Debit Note],
    cal.[Credit Note],
    cal.Interest,

    /* DISPLAY ONLY: Prepayment Remittance row e Payment column e dekhabe na */
    CASE
        WHEN cal.TransactionType = 'Prepayment Remittance' THEN NULL
        ELSE cal.Payment
    END AS Payment,

    /* DISPLAY ONLY: Prepayment Remittance er value Prepayment column e MINUS hishebe dekhabe.
       Closing/Opening/Transaction Amount e kono effect nai (source column unchanged) */
    CASE
        WHEN cal.TransactionType = 'Prepayment Remittance' THEN -ABS(cal.Payment)
        ELSE cal.Prepayment
    END AS Prepayment,

    cal.[Apply Document],
    cal.[Misc. Payment],
    cal.Adjustment,
    cal.[Transaction Amount],
    cal.[Opening Balance],
    cal.Debit,
    cal.Credit,
    cal.[Closing Amount],

    /* Early Payment Info:
       AP-IN = IN Batch-Entry, Date
       AP-PI = PP Batch-Entry, Date (Prepayment) */
    CASE
        WHEN cal.SourceCode IN ('AP-AD', 'AP-PY')
         AND invd.InvDate IS NOT NULL
        THEN CONCAT(
                 CASE
                     WHEN invd.InvSourceCode = 'AP-PI' THEN 'PP '
                     WHEN invd.InvSourceCode = 'AP-CR' THEN 'CN '
                     ELSE 'IN '
                 END,
                 CAST(invd.InvBatch AS VARCHAR(20)), '-',
                 CAST(invd.InvEntry AS VARCHAR(20)), ', ',
                 RIGHT('0' + CAST(DAY(invd.InvDate) AS VARCHAR(2)), 2),
                 '-',
                 LEFT(DATENAME(MONTH, invd.InvDate), 3),
                 '-',
                 RIGHT(CAST(YEAR(invd.InvDate) AS VARCHAR(4)), 2)
             )
        ELSE '---'
    END AS [Early Payment Info],

    /* Invoice Date (only real invoice AP-IN, only when payment date is before invoice date) */
    CASE
        WHEN cal.SourceCode IN ('AP-AD', 'AP-PY')
         AND invd.InvSourceCode = 'AP-IN'
         AND invd.InvDate IS NOT NULL
         AND CAST(cal.TransactionDate AS DATE) < invd.InvDate
        THEN invd.InvDate
        ELSE NULL
    END AS [Invoice Date],

    /* Payment Number = [Document Number] as-is from database, AP-PY only (else NULL) */
    CASE
        WHEN cal.SourceCode = 'AP-PY'
        THEN cal.[Document Number]
        ELSE NULL
    END AS [Payment Number],

    /* TransactionType-wise amount columns (all DECIMAL(19,3) numbers)
       TransactionType match korle [Transaction Amount] boshbe, nahole 0.000 */
    CAST(CASE WHEN cal.TransactionType = 'Credit Note Applied (CF)'
              THEN ISNULL(cal.[Transaction Amount], 0) ELSE 0 END AS DECIMAL(19,3)) AS [Amt - Credit Note Applied (CF)],

    CAST(CASE WHEN cal.TransactionType = 'Prepayment'
              THEN ISNULL(cal.[Transaction Amount], 0) ELSE 0 END AS DECIMAL(19,3)) AS [Amt - Prepayment],

    CAST(CASE WHEN cal.TransactionType = 'Payment'
              THEN ISNULL(cal.[Transaction Amount], 0) ELSE 0 END AS DECIMAL(19,3)) AS [Amt - Payment],

    CAST(CASE WHEN cal.TransactionType IS NULL
              THEN ISNULL(cal.[Transaction Amount], 0) ELSE 0 END AS DECIMAL(19,3)) AS [Amt - NULL],

    CAST(CASE WHEN cal.TransactionType = 'Prepayment Reversed'
              THEN ISNULL(cal.[Transaction Amount], 0) ELSE 0 END AS DECIMAL(19,3)) AS [Amt - Prepayment Reversed],

    CAST(CASE WHEN cal.TransactionType = 'Prepayment Remittance'
              THEN ISNULL(cal.[Transaction Amount], 0) ELSE 0 END AS DECIMAL(19,3)) AS [Amt - Prepayment Remittance],

    CAST(CASE WHEN cal.TransactionType = 'Credit Note'
              THEN ISNULL(cal.[Transaction Amount], 0) ELSE 0 END AS DECIMAL(19,3)) AS [Amt - Credit Note],

    CAST(CASE WHEN cal.TransactionType = 'Invoice'
              THEN ISNULL(cal.[Transaction Amount], 0) ELSE 0 END AS DECIMAL(19,3)) AS [Amt - Invoice],

    CAST(CASE WHEN cal.TransactionType = 'Adjustment'
              THEN ISNULL(cal.[Transaction Amount], 0) ELSE 0 END AS DECIMAL(19,3)) AS [Amt - Adjustment],

    CAST(CASE WHEN cal.TransactionType = 'Payment Reversed'
              THEN ISNULL(cal.[Transaction Amount], 0) ELSE 0 END AS DECIMAL(19,3)) AS [Amt - Payment Reversed],

    CAST(CASE WHEN cal.TransactionType = 'Credit Applied (CT)'
              THEN ISNULL(cal.[Transaction Amount], 0) ELSE 0 END AS DECIMAL(19,3)) AS [Amt - Credit Applied (CT)],

    CAST(CASE WHEN cal.TransactionType = 'Debit Note'
              THEN ISNULL(cal.[Transaction Amount], 0) ELSE 0 END AS DECIMAL(19,3)) AS [Amt - Debit Note]

FROM AllAccountSets acctset
LEFT JOIN CalculatedLedger cal
    ON acctset.IDACCTSET = cal.[Account Set ID]
LEFT JOIN AllAccountSets txnset
    ON txnset.IDACCTSET = cal.[Acct Set ID Detail]
LEFT JOIN VendorAcctSets vas
    ON vas.[Vendor ID] = cal.[Vendor ID]
LEFT JOIN InvDates invd
    ON  invd.[Vendor ID]  = LTRIM(RTRIM(cal.[Vendor ID]))
    AND invd.[DOC NUMBER] = LTRIM(RTRIM(cal.[Applied Document]))
    AND cal.SourceCode IN ('AP-AD', 'AP-PY')
CROSS JOIN CSCOM comp;
GO




 SELECT *
FROM vw_AP_VENDOR_TRANSACTION
where [TransactionType]='Prepayment Remittance' and [Batch Number]=1188

 SELECT *
FROM vw_AP_VENDOR_TRANSACTION
where [Vendor ID]=100004;