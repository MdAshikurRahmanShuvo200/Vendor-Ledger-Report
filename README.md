# 📖 AP Vendor Ledger Reporting Application

An enterprise-grade **Accounts Payable (AP) Vendor Ledger Reporting & Financial Analytics Solution** built using **VB.NET (WinForms)**, **SAP Crystal Reports**, and **Microsoft SQL Server (T-SQL)**. This application extracts, computes, and renders detailed vendor transaction history, running balances, opening/closing amounts, and multi-formatted reports for financial auditing and analysis.

---

## 🖼️ Application Interface & Report Previews

### 1. Main Selection & Filter Interface (VB.NET GUI)
The desktop application provides an interactive control panel enabling users to filter ledger records by date ranges, vendor IDs, and choose between three distinct reporting layouts (**Details**, **Semi Details**, and **Summary**).

![Vendor Ledger Main UI]<img width="1440" height="702" alt="Vendor-Ledger-Report" src="https://github.com/user-attachments/assets/e4ab51fa-1ad8-4181-b3aa-12d7ff12ada0" />


---

### 2. Multi-Format Crystal Reports Outputs

* **Detail Report Format (`AP Vendor Ledger Report (Detail)`):**
  Displays complete transaction lines sorted by monthly periods with running balance calculations and full audit trail metadata.
  
  ![AP Vendor Ledger Detail Report]<img width="863" height="528" alt="Vendor-Ledger-Report1" src="https://github.com/user-attachments/assets/0fab0454-bccf-466f-9258-08197a915d5e" />


* **Period Total & Monthly Breakdown:**
  Provides structured monthly running totals, adjustment details, payment reversals, and account set rollups.
  
  ![AP Vendor Ledger Period Total Details]<img width="943" height="563" alt="Vendor-Ledger-Report2" src="https://github.com/user-attachments/assets/3a057909-7740-497d-8d2d-1a046f71f752" />


* **Semi Detail Report Format (`AP Vendor Ledger Report (Semi Detail)`):**
  Grouped by vendor with comprehensive transaction descriptions, applied documents, debit/credit breakdowns, and period totals.
  
  ![AP Vendor Ledger Semi Detail Report]<img width="938" height="569" alt="Vendor-Ledger-Report3" src="https://github.com/user-attachments/assets/4bcf615c-ac9e-4cdf-9b93-a80a615110eb" />


* **Summary Report Format (`AP Vendor Ledger Report (Summary)`):**
  High-level executive overview summarizing total transaction counts, opening balances, total debit/credit amounts, and net closing balances per vendor.
  
  ![AP Vendor Ledger Summary Report]<img width="934" height="560" alt="Vendor-Ledger-Report4" src="https://github.com/user-attachments/assets/9ea46fde-d373-4a9b-9dc3-231fa577fa7f" />


---

## 🌟 Key Capabilities & Features

* **3-in-1 Dynamic Report Views**:
  * **Details**: Full transactional line items with date ranges and running balances.
  * **Semi Details**: Grouped transactional audit details with payment apply mappings.
  * **Summary**: Executive-level vendor balance and movement summary.

* **Deterministic Running Balance Calculation**: 
  Uses SQL T-SQL Window Functions (`SUM() OVER (PARTITION BY ... ORDER BY ...)`) with multi-level tie-breakers to calculate accurate **Opening** and **Closing Balances** per transaction line.

* **Full AP Lifecycle Transaction Support**:
  * Invoices (`AP-IN`)
  * Debit Notes (`AP-DN`) & Credit Notes (`AP-CR`)
  * Payments (`AP-PY`) & Prepayments (`AP-PI`)
  * Apply Documents (`AP-AD`) & Misc Payments (`AP-MP`)
  * Adjustments & Payment/Prepayment Reversals (`AP-VR`)

* **Early Payment & Linkage Tracking**: Dynamic lookup matching payment remittances with original invoice/prepayment batch and entry numbers.

* **Multi-Account Set Integration**: Aggregates vendor account sets (`IDACCTSET`) for comprehensive sub-ledger reconciliation.

---

## 🛠️ Technology Stack

| Component | Technology / Library |
| :--- | :--- |
| **Language** | Visual Basic .NET (VB.NET) |
| **Framework** | .NET Framework 4.8 |
| **UI Framework** | Windows Forms (WinForms) |
| **Reporting Engine** | SAP Crystal Reports (`CrystalDecisions.CrystalReports.Engine`) |
| **Database** | Microsoft SQL Server (T-SQL CTEs, Window Functions, Views) |
| **IDE** | Visual Studio 2022 |

---

## 🗄️ Database View Schema (`vw_AP_VENDOR_TRANSACTION`)

The report relies on an advanced SQL Server View that joins `APIBH`, `APOBL`, `APTCR`, `APOBP`, `APVEN`, `BKACCT`, and `CSCOM` tables with deterministic sequence ordering.

```sql
USE BNBDAT;
GO

CREATE OR ALTER VIEW vw_AP_VENDOR_TRANSACTION
AS
WITH AllAccountSets AS (
    SELECT ACCTSET AS IDACCTSET, TEXTDESC FROM APRAS
),
CalculatedLedger AS (
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
        COALESCE(NULLIF(LTRIM(RTRIM(Ledger.IDACCTSET)), ''), vend.IDACCTSET) AS [Account Set ID],
        Ledger.CNTBTCH AS [Batch Number],
        Ledger.CNTITEM AS [Entry No],
        Ledger.SourceCode,
        Ledger.DocNumber AS [Document Number],
        Ledger.Description AS [Entry Description],
        Ledger.AuditUser AS [Audit User],
        Ledger.BankCode,
        bank.NAME AS [Bank Name],

        -- Amount Breakdown
        ISNULL(Ledger.Invoice, 0)
          + ISNULL(Ledger.DebitNote, 0)
          + ISNULL(Ledger.Interest, 0)
          - ISNULL(Ledger.CreditNote, 0)
          - ISNULL(Ledger.Payment, 0)
          - ISNULL(Ledger.Prepayment, 0)
          - ISNULL(Ledger.ApplyDocument, 0)
          - ISNULL(Ledger.MiscPayment, 0)
          + ISNULL(Ledger.Adjustment, 0) AS [Transaction Amount],

        -- Deterministic Opening Balance Calculation
        COALESCE(
            SUM(
                ISNULL(Ledger.Invoice, 0) + ISNULL(Ledger.DebitNote, 0) + ISNULL(Ledger.Interest, 0)
                - ISNULL(Ledger.CreditNote, 0) - ISNULL(Ledger.Payment, 0) - ISNULL(Ledger.Prepayment, 0)
                - ISNULL(Ledger.ApplyDocument, 0) - ISNULL(Ledger.MiscPayment, 0) + ISNULL(Ledger.Adjustment, 0)
            ) OVER (
                PARTITION BY Ledger.IDVEND
                ORDER BY Ledger.TransactionDate, Ledger.CNTBTCH, Ledger.CNTITEM, Ledger.SortSeq, Ledger.DOC_NUMBER
                ROWS BETWEEN UNBOUNDED PRECEDING AND 1 PRECEDING
            ), 0
        ) AS [Opening Balance],

        -- Deterministic Closing Amount Calculation
        SUM(
            ISNULL(Ledger.Invoice, 0) + ISNULL(Ledger.DebitNote, 0) + ISNULL(Ledger.Interest, 0)
            - ISNULL(Ledger.CreditNote, 0) - ISNULL(Ledger.Payment, 0) - ISNULL(Ledger.Prepayment, 0)
            - ISNULL(Ledger.ApplyDocument, 0) - ISNULL(Ledger.MiscPayment, 0) + ISNULL(Ledger.Adjustment, 0)
        ) OVER (
            PARTITION BY Ledger.IDVEND
            ORDER BY Ledger.TransactionDate, Ledger.CNTBTCH, Ledger.CNTITEM, Ledger.SortSeq, Ledger.DOC_NUMBER
            ROWS BETWEEN UNBOUNDED PRECEDING AND CURRENT ROW
        ) AS [Closing Amount]

    FROM (
        /* Union of Invoices (APIBH), Prepayments (APOBL), Receipts (APTCR), & Reversals (APOBP) */
        SELECT 'APIBH' AS SourceTable, 1 AS SortSeq, ... FROM APIBH
        UNION ALL
        SELECT 'APOBL' AS SourceTable, 1 AS SortSeq, ... FROM APOBL
        UNION ALL
        SELECT 'APTCR' AS SourceTable, 2 AS SortSeq, ... FROM APTCR
        UNION ALL
        SELECT 'APOBP' AS SourceTable, 2 AS SortSeq, ... FROM APOBP
    ) AS Ledger
    LEFT JOIN APVEN vend ON Ledger.IDVEND = vend.VENDORID
    LEFT JOIN BKACCT bank ON Ledger.BankCode = bank.BANK
)
SELECT * FROM CalculatedLedger;
GO
```

---

## 📁 Project Structure

```text
Posting-Journal-Entries/
├── Posting Journals Reports.sln       # Visual Studio Solution File
├── README.md                           # Repository Documentation
└── Vendor Ledger Report/
    ├── App.config                      # Configuration & Database Connection Strings
    ├── Form1.vb                        # Main Form Logic (Date Range, Vendor ID, View Type)
    ├── Form1.Designer.vb               # WinForms Designer Code
    ├── frmReportViewer.vb              # Crystal Reports Viewer Form
    ├── Reports/
    │   └── GL_Posting.rpt              # SAP Crystal Reports Layout File
    └── SQL/
        ├── Vednor Ledger_with_sorting_10Nov.sql   # Core SQL View Script
        ├── Posting Jornals.sql
        └── Company INFO.sql
```

---

## ⚙️ Setup and Installation

1. **Clone the Repository**:
   ```bash
   git clone https://github.com/MdAshikurRahmanShuvo200/Posting-Journal-Entries.git
   cd Posting-Journal-Entries
   ```

2. **Execute Database View**:
   * Open **Microsoft SQL Server Management Studio (SSMS)**.
   * Execute `Vendor Ledger Report/SQL/Vednor Ledger_with_sorting_10Nov.sql` on your ERP database (`BNBDAT`).

3. **Run Application**:
   * Open `Posting Journals Reports.sln` in Visual Studio 2022.
   * Ensure SAP Crystal Reports runtime engine for .NET Framework is installed.
   * Build and run the project (`F5`).

---

## 👨‍💻 Author

**Md. Ashikur Rahman Shuvo**  
*Data Analyst & Software Solutions Developer*  
* GitHub: [@MdAshikurRahmanShuvo200](https://github.com/MdAshikurRahmanShuvo200)
