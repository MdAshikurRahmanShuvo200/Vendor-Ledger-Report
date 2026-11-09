Imports System.Configuration
Imports System.Data.SqlClient
Imports System.Drawing
' Needed for Windows API calls (DllImport, Marshal, StructLayout)
Imports System.Runtime.InteropServices

Public Class Form1

    ' App.config theke connection string ("SageDBConn")
    Private ReadOnly connString As String = ConfigurationManager.ConnectionStrings("SageDBConn").ConnectionString

    Private Const VIEW_NAME As String = "vw_AP_VENDOR_TRANSACTION"

    ' Width of the Vendor ID column in the three-column dropdown
    Private Const ID_COLUMN_WIDTH As Integer = 80

    ' ADDED: Width of the Vendor Name column (the Acc ID column starts after ID + Name)
    ' Acc ID column gets the remaining space: 480 - 130 - 230 = 120px
    Private Const NAME_COLUMN_WIDTH As Integer = 300

    ' Total width of the dropdown list
    Private Const DROPDOWN_WIDTH As Integer = 480

    ' =========================================================================
    ' WINDOWS API DECLARATIONS (used to position the dropdown list window)
    ' =========================================================================

    ' Rectangle structure used by the Windows API
    <StructLayout(LayoutKind.Sequential)>
    Private Structure RECT
        Public Left As Integer
        Public Top As Integer
        Public Right As Integer
        Public Bottom As Integer
    End Structure

    ' Structure that holds the handles of the parts of a ComboBox (edit box, dropdown list, etc.)
    <StructLayout(LayoutKind.Sequential)>
    Private Structure COMBOBOXINFO
        Public cbSize As Integer
        Public rcItem As RECT
        Public rcButton As RECT
        Public stateButton As Integer
        Public hwndCombo As IntPtr
        Public hwndEdit As IntPtr
        Public hwndList As IntPtr
    End Structure

    ' Structure Windows passes with the WM_WINDOWPOSCHANGING message (new position/size of a window)
    <StructLayout(LayoutKind.Sequential)>
    Private Structure WINDOWPOS
        Public hwnd As IntPtr
        Public hwndInsertAfter As IntPtr
        Public x As Integer
        Public y As Integer
        Public cx As Integer
        Public cy As Integer
        Public flags As UInteger
    End Structure

    ' Gets the handle of the dropdown list window of a ComboBox
    <DllImport("user32.dll")>
    Private Shared Function GetComboBoxInfo(hwndCombo As IntPtr, ByRef pcbi As COMBOBOXINFO) As Boolean
    End Function

    ' Message sent to a window just BEFORE its position/size changes (we can modify the new position here)
    Private Const WM_WINDOWPOSCHANGING As Integer = &H46

    ' Flag meaning "the window is not being moved" (in that case we must not touch X)
    Private Const SWP_NOMOVE As UInteger = &H2UI

    ' Hooks into the dropdown list window of the ToVendorID ComboBox.
    ' Whenever Windows tries to place the list, X is replaced with TargetLeft,
    ' so the list appears directly under FromVendorID without any visible jump.
    Private Class ListMover
        Inherits NativeWindow

        ' Screen X position where the list must appear
        Public TargetLeft As Integer

        Protected Overrides Sub WndProc(ByRef m As Message)
            If m.Msg = WM_WINDOWPOSCHANGING Then
                Dim pos As WINDOWPOS = DirectCast(Marshal.PtrToStructure(m.LParam, GetType(WINDOWPOS)), WINDOWPOS)

                ' Only change X when the window is actually being moved
                If (pos.flags And SWP_NOMOVE) = 0UI Then
                    pos.x = TargetLeft
                    Marshal.StructureToPtr(pos, m.LParam, False)
                End If
            End If

            MyBase.WndProc(m)
        End Sub
    End Class

    ' One mover instance for the ToVendorID dropdown list
    Private ReadOnly toListMover As New ListMover()

    ' Form load howar somoy
    Private Sub Form1_Load(sender As Object, e As EventArgs) Handles MyBase.Load

        ' Configure both vendor ComboBoxes to draw three columns (ID, Name, Acc ID) manually
        SetupVendorComboBox(cmbFromVendorID)
        SetupVendorComboBox(cmbToVendorID)

        LoadVendorIDsFromView()
        SetDefaultDates()
        Details.Checked = True   ' Default Report Type
    End Sub

    ' =========================================================================
    ' THREE-COLUMN VENDOR DROPDOWN SETUP
    ' =========================================================================

    ' Prepares a ComboBox so it can draw a header row and three columns (Vendor ID, Vendor Name, Acc ID)
    Private Sub SetupVendorComboBox(ByVal cmb As ComboBox)
        ' Owner draw lets us paint each item ourselves (needed for multiple columns)
        cmb.DrawMode = DrawMode.OwnerDrawFixed
        ' Keep typing enabled so the user can still type a Vendor ID
        cmb.DropDownStyle = ComboBoxStyle.DropDown
        ' Make the dropdown list wide enough to show all columns
        cmb.DropDownWidth = DROPDOWN_WIDTH
        ' Item height; slightly taller rows look better
        cmb.ItemHeight = 22
        ' Show about 10 rows in the dropdown at once
        cmb.MaxDropDownItems = 10

        ' Avoid attaching the same handler twice if this method is called again
        RemoveHandler cmb.DrawItem, AddressOf VendorCombo_DrawItem
        AddHandler cmb.DrawItem, AddressOf VendorCombo_DrawItem

        ' Adjust the dropdown width/position every time it opens
        RemoveHandler cmb.DropDown, AddressOf VendorCombo_DropDown
        AddHandler cmb.DropDown, AddressOf VendorCombo_DropDown

        ' Release the list hook when the dropdown closes
        RemoveHandler cmb.DropDownClosed, AddressOf VendorCombo_DropDownClosed
        AddHandler cmb.DropDownClosed, AddressOf VendorCombo_DropDownClosed
    End Sub

    ' Makes BOTH Vendor dropdowns open at the same left position as the FromVendorID ComboBox.
    ' For ToVendorID, the list window is hooked so its X position is replaced before it is shown.
    Private Sub VendorCombo_DropDown(sender As Object, e As EventArgs)
        Dim cmb As ComboBox = DirectCast(sender, ComboBox)

        ' Screen X position of the FromVendorID ComboBox (the reference position)
        Dim targetLeft As Integer = cmbFromVendorID.PointToScreen(Point.Empty).X

        ' Keep the dropdown inside the screen: shrink the width if it would go past the right edge
        Dim availableWidth As Integer = Screen.FromControl(cmb).WorkingArea.Right - targetLeft
        cmb.DropDownWidth = Math.Max(cmb.Width, Math.Min(DROPDOWN_WIDTH, availableWidth))

        ' Only the ToVendorID dropdown needs to be moved (From is already in the right place)
        If cmb Is cmbToVendorID Then
            Dim info As New COMBOBOXINFO()
            info.cbSize = Marshal.SizeOf(info)

            If GetComboBoxInfo(cmb.Handle, info) AndAlso info.hwndList <> IntPtr.Zero Then
                ' Hook the list window (only if it is not already hooked)
                If toListMover.Handle <> info.hwndList Then
                    If toListMover.Handle <> IntPtr.Zero Then toListMover.ReleaseHandle()
                    toListMover.AssignHandle(info.hwndList)
                End If

                ' Tell the hook where the list must appear
                toListMover.TargetLeft = targetLeft
            End If
        End If
    End Sub

    ' Removes the hook when the ToVendorID dropdown closes
    Private Sub VendorCombo_DropDownClosed(sender As Object, e As EventArgs)
        If sender Is cmbToVendorID AndAlso toListMover.Handle <> IntPtr.Zero Then
            toListMover.ReleaseHandle()
        End If
    End Sub

    ' CHANGED: Custom paint routine for every row in the Vendor dropdown (now draws 3 columns)
    Private Sub VendorCombo_DrawItem(sender As Object, e As DrawItemEventArgs)
        Dim cmb As ComboBox = DirectCast(sender, ComboBox)

        ' Nothing to draw if there is no valid item index
        If e.Index < 0 OrElse e.Index >= cmb.Items.Count Then Return

        Dim item As VendorItem = TryCast(cmb.Items(e.Index), VendorItem)
        If item Is Nothing Then Return

        ' Draw the normal background (highlight when hovered/selected)
        e.DrawBackground()

        Dim textBrush As Brush = SystemBrushes.WindowText
        Dim itemFont As Font = e.Font

        ' Header row (index 0): bold text and grey background so it looks like a column heading
        If item.IsHeader Then
            Using bg As New SolidBrush(Color.LightGray)
                e.Graphics.FillRectangle(bg, e.Bounds)
            End Using
            itemFont = New Font(e.Font, FontStyle.Bold)
        ElseIf (e.State And DrawItemState.Selected) = DrawItemState.Selected Then
            ' Use highlight text color for a selected data row
            textBrush = SystemBrushes.HighlightText
        End If

        ' ADDED: Format that keeps text on one line and adds "..." if it is longer than its column,
        ' so a long Vendor Name never overlaps the Acc ID column
        Using fmt As New StringFormat()
            fmt.Trimming = StringTrimming.EllipsisCharacter
            fmt.FormatFlags = StringFormatFlags.NoWrap

            ' ADDED: Vertical offset so the text stays centered in the row
            Dim textY As Single = e.Bounds.Y + (e.Bounds.Height - itemFont.Height) / 2.0F

            ' Column 1: Vendor ID
            Dim idRect As New RectangleF(e.Bounds.X + 2, textY, ID_COLUMN_WIDTH - 4, itemFont.Height)
            e.Graphics.DrawString(item.VendorID, itemFont, textBrush, idRect, fmt)

            ' Column 2: Vendor Name (starts after the ID column)
            Dim nameRect As New RectangleF(e.Bounds.X + ID_COLUMN_WIDTH, textY, NAME_COLUMN_WIDTH - 6, itemFont.Height)
            e.Graphics.DrawString(item.VendorName, itemFont, textBrush, nameRect, fmt)

            ' ADDED: Column 3: Acc ID (starts after the ID + Name columns)
            Dim accLeft As Single = e.Bounds.X + ID_COLUMN_WIDTH + NAME_COLUMN_WIDTH
            Dim accWidth As Single = Math.Max(20, e.Bounds.Right - accLeft - 4)
            Dim accRect As New RectangleF(accLeft, textY, accWidth, itemFont.Height)
            e.Graphics.DrawString(item.AccountSetID, itemFont, textBrush, accRect, fmt)
        End Using

        ' Dispose the bold font only if we created it
        If item.IsHeader Then itemFont.Dispose()

        e.DrawFocusRectangle()
    End Sub

    ' Small helper class that holds one dropdown row (ID + Name + Acc ID)
    Private Class VendorItem
        Public Property VendorID As String
        Public Property VendorName As String
        ' ADDED: Account Set ID shown in the third column (header label is "Acc ID")
        Public Property AccountSetID As String
        ' True only for the heading row at the top of the dropdown
        Public Property IsHeader As Boolean

        ' IMPORTANT: ToString returns only the ID, so the textbox shows just the ID after selection
        Public Overrides Function ToString() As String
            Return VendorID
        End Function
    End Class

    ' Shobcheye purono transaction date ber kore From Date set kora
    Private Sub SetDefaultDates()
        Try
            Using conn As New SqlConnection(connString)
                Dim query As String = "SELECT MIN([TransactionDate]) FROM " & VIEW_NAME & " WHERE [TransactionDate] IS NOT NULL"
                Dim cmd As New SqlCommand(query, conn)

                conn.Open()
                Dim val As Object = cmd.ExecuteScalar()

                If val IsNot DBNull.Value AndAlso val IsNot Nothing Then
                    dtpFromDate.Value = Convert.ToDateTime(val).Date
                Else
                    dtpFromDate.Value = New Date(1990, 1, 1)
                End If
            End Using
        Catch ex As Exception
            dtpFromDate.Value = New Date(1990, 1, 1)
        End Try

        dtpToDate.Value = DateTime.Today
    End Sub

    ' CHANGED: View theke unique Vendor ID, Vendor Name AND Account Set ID load kora (A-Z)
    Private Sub LoadVendorIDsFromView()
        Try
            Using conn As New SqlConnection(connString)
                ' CHANGED: Query now also returns [Account Set ID]; MAX is used so each Vendor ID appears only once
                Dim query As String = "SELECT [Vendor ID], MAX([Vendor Name]) AS [Vendor Name], MAX([Account Set ID]) AS [Account Set ID] " &
                                      "FROM " & VIEW_NAME & " " &
                                      "WHERE [Vendor ID] IS NOT NULL AND [Vendor ID] <> '' " &
                                      "GROUP BY [Vendor ID] " &
                                      "ORDER BY [Vendor ID] ASC"
                Dim cmd As New SqlCommand(query, conn)

                conn.Open()
                Dim reader As SqlDataReader = cmd.ExecuteReader()

                cmbFromVendorID.Items.Clear()
                cmbToVendorID.Items.Clear()

                ' CHANGED: Header row now has three headings: Vendor ID | Vendor Name | Acc ID
                cmbFromVendorID.Items.Add(New VendorItem With {.VendorID = "Vend ID", .VendorName = "Vendor Name", .AccountSetID = "Acc ID", .IsHeader = True})
                cmbToVendorID.Items.Add(New VendorItem With {.VendorID = "Vend ID", .VendorName = "Vendor Name", .AccountSetID = "Acc ID", .IsHeader = True})

                While reader.Read()
                    Dim vendId As String = reader("Vendor ID").ToString().Trim()

                    ' Read Vendor Name safely (it may be NULL in the database)
                    Dim vendName As String = If(reader("Vendor Name") Is DBNull.Value, "", reader("Vendor Name").ToString().Trim())

                    ' ADDED: Read Account Set ID safely (it may be NULL in the database)
                    Dim accSet As String = If(reader("Account Set ID") Is DBNull.Value, "", reader("Account Set ID").ToString().Trim())

                    ' Create separate item objects so each ComboBox has its own copy
                    cmbFromVendorID.Items.Add(New VendorItem With {.VendorID = vendId, .VendorName = vendName, .AccountSetID = accSet, .IsHeader = False})
                    cmbToVendorID.Items.Add(New VendorItem With {.VendorID = vendId, .VendorName = vendName, .AccountSetID = accSet, .IsHeader = False})
                End While

                reader.Close()
            End Using
        Catch ex As Exception
            MessageBox.Show("Failed to load Vendor IDs: " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

    ' ComboBox faka thakle database theke MIN / MAX value niye ashe
    Private Function GetFirstOrLastValue(ByVal columnName As String, ByVal isMin As Boolean) As String
        Dim result As String = ""
        Dim aggregate As String = If(isMin, "MIN", "MAX")
        Dim query As String = $"SELECT {aggregate}([{columnName}]) FROM {VIEW_NAME} WHERE [{columnName}] IS NOT NULL AND [{columnName}] <> ''"

        Try
            Using conn As New SqlConnection(connString)
                Dim cmd As New SqlCommand(query, conn)
                conn.Open()
                Dim val As Object = cmd.ExecuteScalar()
                If val IsNot DBNull.Value AndAlso val IsNot Nothing Then
                    result = val.ToString().Trim()
                End If
            End Using
        Catch ex As Exception
            result = If(isMin, " ", "ZZZZZZZZZZ")
        End Try

        Return result
    End Function

    ' Returns only the Vendor ID from a vendor ComboBox (works with VendorItem objects or typed text).
    ' If the header row is selected it is treated as empty, so the report does not filter by "Vendor ID".
    Private Function GetComboValue(cmb As ComboBox) As String
        Dim selected As VendorItem = TryCast(cmb.SelectedItem, VendorItem)

        If selected IsNot Nothing Then
            ' Ignore the header row if somehow selected
            If selected.IsHeader Then Return String.Empty
            Return selected.VendorID.Trim()
        End If

        ' Otherwise use whatever the user typed manually
        Return cmb.Text.Trim()
    End Function

    ' Refresh button
    Private Sub btnRefresh_Click(sender As Object, e As EventArgs) Handles btnRefresh.Click
        cmbFromVendorID.Text = String.Empty
        cmbToVendorID.Text = String.Empty
        cmbFromVendorID.SelectedIndex = -1
        cmbToVendorID.SelectedIndex = -1

        LoadVendorIDsFromView()
        SetDefaultDates()

        Details.Checked = True

        MessageBox.Show("Data Refreshed and Filters Reset Successfully!", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information)
    End Sub

    ' Show Report button
    Private Sub btnShowReport_Click(sender As Object, e As EventArgs) Handles btnShowReport.Click

        ' 1. Date validation
        If dtpFromDate.Value.Date > dtpToDate.Value.Date Then
            MessageBox.Show("From Date cannot be greater than To Date.", "Invalid Date", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Return
        End If

        ' 2. Report Type ber kora (Crystal Report parameter list-er value-r sathe hubohu mil thakte hobe)
        Dim reportType As String
        If Details.Checked Then
            reportType = "Detail"
        ElseIf Semi.Checked Then
            reportType = "Semi Detail"
        ElseIf Summary.Checked Then
            reportType = "Summary"
        Else
            MessageBox.Show("Please select a Report Type.", "Report Type", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Return
        End If

        Dim rptForm As New frmReportViewer()

        ' 3. Dates (ToDate = din-er shesh muhurto 23:59:59)
        rptForm.FromDate = dtpFromDate.Value.Date
        rptForm.ToDate = dtpToDate.Value.Date.AddDays(1).AddSeconds(-1)

        ' 4. Vendor ID (faka thakle MIN/MAX)
        ' GetComboValue returns only the Vendor ID even though items hold ID + Name + Acc ID
        Dim fromVendText As String = GetComboValue(cmbFromVendorID)
        Dim toVendText As String = GetComboValue(cmbToVendorID)

        If String.IsNullOrEmpty(fromVendText) Then
            rptForm.FromVendorID = GetFirstOrLastValue("Vendor ID", True)
        Else
            rptForm.FromVendorID = fromVendText
        End If

        If String.IsNullOrEmpty(toVendText) Then
            rptForm.ToVendorID = GetFirstOrLastValue("Vendor ID", False)
        Else
            rptForm.ToVendorID = toVendText
        End If

        ' 5. Report Type
        rptForm.ReportType = reportType

        rptForm.ShowDialog()
    End Sub

End Class