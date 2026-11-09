Imports System.Configuration
Imports System.Data.SqlClient
Imports CrystalDecisions.CrystalReports.Engine
Imports CrystalDecisions.Shared

Public Class frmReportViewer

    ' Form1 theke ashbe
    Public FromDate As Date
    Public ToDate As Date
    Public FromVendorID As String
    Public ToVendorID As String
    Public Property ReportType As String

    ' Apnar Reports folder (development PC)
    Private Const REPORTS_FOLDER As String = "C:\Users\ASUS\source\repos\Vendor Ledger Report_FInal\Vendor Ledger Report\Reports"

    Private Sub frmReportViewer_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Try
            ' 1. Report document
            Dim rptDoc As New ReportDocument()

            ' 2. Report file khuje ber kora
            Dim reportFileName As String = "Updated_Vendor_Ledger.rpt"

            Dim candidates As String() = {
                IO.Path.Combine(REPORTS_FOLDER, reportFileName),
                IO.Path.Combine(Application.StartupPath, reportFileName),
                IO.Path.Combine(Application.StartupPath, "..", "..", "Reports", reportFileName),
                IO.Path.Combine(Application.StartupPath, "..", "..", "..", "Reports", reportFileName)
            }

            Dim reportPath As String = Nothing
            For Each p As String In candidates
                If IO.File.Exists(p) Then
                    reportPath = IO.Path.GetFullPath(p)
                    Exit For
                End If
            Next

            If reportPath Is Nothing Then
                MessageBox.Show("Crystal Report file not found!" & vbCrLf &
                                "Please make sure '" & reportFileName & "' is inside:" & vbCrLf & REPORTS_FOLDER,
                                "Path Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
                Exit Sub
            End If

            ' 3. Report load
            rptDoc.Load(reportPath)

            ' 4. Database login (App.config er "SageDBConn" theke)
            Dim sb As New SqlConnectionStringBuilder(ConfigurationManager.ConnectionStrings("SageDBConn").ConnectionString)

            Dim connectionInfo As New ConnectionInfo()
            connectionInfo.ServerName = sb.DataSource
            connectionInfo.DatabaseName = sb.InitialCatalog
            If sb.IntegratedSecurity Then
                connectionInfo.IntegratedSecurity = True
            Else
                connectionInfo.UserID = sb.UserID
                connectionInfo.Password = sb.Password
            End If

            ' 4a. Main report-er table/view
            ApplyLogin(rptDoc, connectionInfo)

            ' 4b. Subreport (companyinfo etc.) er table/view
            For Each subRpt As ReportDocument In rptDoc.Subreports
                ApplyLogin(subRpt, connectionInfo)
            Next

            ' 5. Parameters (Crystal Report-er naam-er sathe hubohu mil)
            rptDoc.SetParameterValue("FromDate", FromDate)
            rptDoc.SetParameterValue("ToDate", ToDate)
            rptDoc.SetParameterValue("FromVendorID", If(String.IsNullOrEmpty(FromVendorID), "", FromVendorID))
            rptDoc.SetParameterValue("ToVendorID", If(String.IsNullOrEmpty(ToVendorID), "ZZZZZZZZZZ", ToVendorID))
            rptDoc.SetParameterValue("ReportType", If(String.IsNullOrEmpty(ReportType), "Detail", ReportType))

            ' Parameter panel hide + value reuse
            CrystalReportViewer1.ShowParameterPanelButton = False
            CrystalReportViewer1.ReuseParameterValuesOnRefresh = True

            ' 6. Viewer-e report assign
            CrystalReportViewer1.ReportSource = rptDoc
            CrystalReportViewer1.RefreshReport()

            ' 7. Maximized
            Me.WindowState = FormWindowState.Maximized

        Catch ex As Exception
            MessageBox.Show("Error loading report: " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

    ' Ekta ReportDocument (main ba subreport) er sob table-e login apply kore
    Private Sub ApplyLogin(ByVal doc As ReportDocument, ByVal connInfo As ConnectionInfo)
        For Each table As Table In doc.Database.Tables
            Dim logOn As TableLogOnInfo = table.LogOnInfo
            logOn.ConnectionInfo = connInfo
            table.ApplyLogOnInfo(logOn)
        Next
    End Sub

End Class