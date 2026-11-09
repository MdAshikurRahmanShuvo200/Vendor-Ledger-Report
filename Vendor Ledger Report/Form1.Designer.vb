<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class Form1
    Inherits System.Windows.Forms.Form

    'Form overrides dispose to clean up the component list.
    <System.Diagnostics.DebuggerNonUserCode()> _
    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
        Try
            If disposing AndAlso components IsNot Nothing Then
                components.Dispose()
            End If
        Finally
            MyBase.Dispose(disposing)
        End Try
    End Sub

    'Required by the Windows Form Designer
    Private components As System.ComponentModel.IContainer

    'NOTE: The following procedure is required by the Windows Form Designer
    'It can be modified using the Windows Form Designer.  
    'Do not modify it using the code editor.
    <System.Diagnostics.DebuggerStepThrough()> _
    Private Sub InitializeComponent()
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(Form1))
        Me.MainPanel = New System.Windows.Forms.Panel()
        Me.Label2 = New System.Windows.Forms.Label()
        Me.Summary = New System.Windows.Forms.RadioButton()
        Me.Semi = New System.Windows.Forms.RadioButton()
        Me.Details = New System.Windows.Forms.RadioButton()
        Me.Label1 = New System.Windows.Forms.Label()
        Me.btnShowReport = New System.Windows.Forms.Button()
        Me.ToCustomerIDLabel = New System.Windows.Forms.Label()
        Me.FromCustomerIDLabel = New System.Windows.Forms.Label()
        Me.cmbFromVendorID = New System.Windows.Forms.ComboBox()
        Me.dtpToDate = New System.Windows.Forms.DateTimePicker()
        Me.btnRefresh = New System.Windows.Forms.Button()
        Me.cmbToVendorID = New System.Windows.Forms.ComboBox()
        Me.ToDateLabel = New System.Windows.Forms.Label()
        Me.dtpFromDate = New System.Windows.Forms.DateTimePicker()
        Me.FromDateLabel = New System.Windows.Forms.Label()
        Me.MainPanel.SuspendLayout()
        Me.SuspendLayout()
        '
        'MainPanel
        '
        Me.MainPanel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.MainPanel.Controls.Add(Me.Label2)
        Me.MainPanel.Controls.Add(Me.Summary)
        Me.MainPanel.Controls.Add(Me.Semi)
        Me.MainPanel.Controls.Add(Me.Details)
        Me.MainPanel.Controls.Add(Me.Label1)
        Me.MainPanel.Controls.Add(Me.btnShowReport)
        Me.MainPanel.Controls.Add(Me.ToCustomerIDLabel)
        Me.MainPanel.Controls.Add(Me.FromCustomerIDLabel)
        Me.MainPanel.Controls.Add(Me.cmbFromVendorID)
        Me.MainPanel.Controls.Add(Me.dtpToDate)
        Me.MainPanel.Controls.Add(Me.btnRefresh)
        Me.MainPanel.Controls.Add(Me.cmbToVendorID)
        Me.MainPanel.Controls.Add(Me.ToDateLabel)
        Me.MainPanel.Controls.Add(Me.dtpFromDate)
        Me.MainPanel.Controls.Add(Me.FromDateLabel)
        Me.MainPanel.ForeColor = System.Drawing.SystemColors.ActiveCaptionText
        Me.MainPanel.Location = New System.Drawing.Point(75, 26)
        Me.MainPanel.Margin = New System.Windows.Forms.Padding(30)
        Me.MainPanel.Name = "MainPanel"
        Me.MainPanel.Padding = New System.Windows.Forms.Padding(20)
        Me.MainPanel.Size = New System.Drawing.Size(968, 452)
        Me.MainPanel.TabIndex = 33
        '
        'Label2
        '
        Me.Label2.BackColor = System.Drawing.SystemColors.Menu
        Me.Label2.Font = New System.Drawing.Font("Microsoft Sans Serif", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label2.Location = New System.Drawing.Point(7, 243)
        Me.Label2.Name = "Label2"
        Me.Label2.Size = New System.Drawing.Size(215, 38)
        Me.Label2.TabIndex = 35
        Me.Label2.Text = "Report Type"
        '
        'Summary
        '
        Me.Summary.AutoSize = True
        Me.Summary.Font = New System.Drawing.Font("Microsoft Sans Serif", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Summary.Location = New System.Drawing.Point(526, 248)
        Me.Summary.Name = "Summary"
        Me.Summary.Size = New System.Drawing.Size(139, 33)
        Me.Summary.TabIndex = 34
        Me.Summary.TabStop = True
        Me.Summary.Text = "Summary"
        Me.Summary.UseVisualStyleBackColor = True
        '
        'Semi
        '
        Me.Semi.AutoSize = True
        Me.Semi.Font = New System.Drawing.Font("Microsoft Sans Serif", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Semi.Location = New System.Drawing.Point(346, 248)
        Me.Semi.Name = "Semi"
        Me.Semi.Size = New System.Drawing.Size(174, 33)
        Me.Semi.TabIndex = 33
        Me.Semi.TabStop = True
        Me.Semi.Text = "Semi Details"
        Me.Semi.UseVisualStyleBackColor = True
        '
        'Details
        '
        Me.Details.AutoSize = True
        Me.Details.Font = New System.Drawing.Font("Microsoft Sans Serif", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Details.Location = New System.Drawing.Point(228, 248)
        Me.Details.Name = "Details"
        Me.Details.Size = New System.Drawing.Size(112, 33)
        Me.Details.TabIndex = 32
        Me.Details.TabStop = True
        Me.Details.Text = "Details"
        Me.Details.UseVisualStyleBackColor = True
        '
        'Label1
        '
        Me.Label1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.Label1.Font = New System.Drawing.Font("Microsoft Sans Serif", 14.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label1.Location = New System.Drawing.Point(353, 20)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(291, 43)
        Me.Label1.TabIndex = 16
        Me.Label1.Text = "AP Vendor Ledger"
        '
        'btnShowReport
        '
        Me.btnShowReport.Anchor = System.Windows.Forms.AnchorStyles.None
        Me.btnShowReport.Font = New System.Drawing.Font("Microsoft Sans Serif", 13.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.btnShowReport.Location = New System.Drawing.Point(353, 348)
        Me.btnShowReport.Name = "btnShowReport"
        Me.btnShowReport.Size = New System.Drawing.Size(203, 51)
        Me.btnShowReport.TabIndex = 31
        Me.btnShowReport.Text = "Show Report"
        Me.btnShowReport.UseVisualStyleBackColor = True
        '
        'ToCustomerIDLabel
        '
        Me.ToCustomerIDLabel.BackColor = System.Drawing.SystemColors.Menu
        Me.ToCustomerIDLabel.Font = New System.Drawing.Font("Microsoft Sans Serif", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.ToCustomerIDLabel.Location = New System.Drawing.Point(496, 188)
        Me.ToCustomerIDLabel.Name = "ToCustomerIDLabel"
        Me.ToCustomerIDLabel.Size = New System.Drawing.Size(215, 38)
        Me.ToCustomerIDLabel.TabIndex = 23
        Me.ToCustomerIDLabel.Text = "To Vendor ID"
        '
        'FromCustomerIDLabel
        '
        Me.FromCustomerIDLabel.BackColor = System.Drawing.SystemColors.Menu
        Me.FromCustomerIDLabel.Font = New System.Drawing.Font("Microsoft Sans Serif", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.FromCustomerIDLabel.Location = New System.Drawing.Point(7, 188)
        Me.FromCustomerIDLabel.Name = "FromCustomerIDLabel"
        Me.FromCustomerIDLabel.Size = New System.Drawing.Size(215, 38)
        Me.FromCustomerIDLabel.TabIndex = 22
        Me.FromCustomerIDLabel.Text = "From Vendor ID"
        '
        'cmbFromVendorID
        '
        Me.cmbFromVendorID.DropDownHeight = 165
        Me.cmbFromVendorID.Font = New System.Drawing.Font("Microsoft Sans Serif", 13.5!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.cmbFromVendorID.ForeColor = System.Drawing.SystemColors.InactiveCaptionText
        Me.cmbFromVendorID.FormattingEnabled = True
        Me.cmbFromVendorID.IntegralHeight = False
        Me.cmbFromVendorID.Location = New System.Drawing.Point(228, 188)
        Me.cmbFromVendorID.MaxDropDownItems = 3
        Me.cmbFromVendorID.Name = "cmbFromVendorID"
        Me.cmbFromVendorID.Size = New System.Drawing.Size(200, 39)
        Me.cmbFromVendorID.TabIndex = 24
        '
        'dtpToDate
        '
        Me.dtpToDate.CalendarForeColor = System.Drawing.SystemColors.MenuHighlight
        Me.dtpToDate.CalendarTitleBackColor = System.Drawing.SystemColors.ControlText
        Me.dtpToDate.CalendarTitleForeColor = System.Drawing.SystemColors.MenuHighlight
        Me.dtpToDate.CustomFormat = "dd-MMM-yyyy"
        Me.dtpToDate.Font = New System.Drawing.Font("Microsoft Sans Serif", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.dtpToDate.Format = System.Windows.Forms.DateTimePickerFormat.Custom
        Me.dtpToDate.Location = New System.Drawing.Point(733, 144)
        Me.dtpToDate.Name = "dtpToDate"
        Me.dtpToDate.Size = New System.Drawing.Size(200, 35)
        Me.dtpToDate.TabIndex = 21
        '
        'btnRefresh
        '
        Me.btnRefresh.FlatAppearance.BorderColor = System.Drawing.SystemColors.ButtonShadow
        Me.btnRefresh.FlatAppearance.MouseOverBackColor = System.Drawing.Color.White
        Me.btnRefresh.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.btnRefresh.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.btnRefresh.ForeColor = System.Drawing.SystemColors.ActiveBorder
        Me.btnRefresh.Image = CType(resources.GetObject("btnRefresh.Image"), System.Drawing.Image)
        Me.btnRefresh.Location = New System.Drawing.Point(869, 35)
        Me.btnRefresh.Margin = New System.Windows.Forms.Padding(0)
        Me.btnRefresh.Name = "btnRefresh"
        Me.btnRefresh.Padding = New System.Windows.Forms.Padding(5)
        Me.btnRefresh.Size = New System.Drawing.Size(65, 65)
        Me.btnRefresh.TabIndex = 17
        Me.btnRefresh.UseVisualStyleBackColor = True
        '
        'cmbToVendorID
        '
        Me.cmbToVendorID.DropDownHeight = 165
        Me.cmbToVendorID.Font = New System.Drawing.Font("Microsoft Sans Serif", 13.5!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.cmbToVendorID.ForeColor = System.Drawing.SystemColors.InactiveCaptionText
        Me.cmbToVendorID.FormattingEnabled = True
        Me.cmbToVendorID.IntegralHeight = False
        Me.cmbToVendorID.Location = New System.Drawing.Point(733, 188)
        Me.cmbToVendorID.MaxDropDownItems = 3
        Me.cmbToVendorID.Name = "cmbToVendorID"
        Me.cmbToVendorID.Size = New System.Drawing.Size(200, 39)
        Me.cmbToVendorID.TabIndex = 25
        '
        'ToDateLabel
        '
        Me.ToDateLabel.BackColor = System.Drawing.SystemColors.Menu
        Me.ToDateLabel.Font = New System.Drawing.Font("Microsoft Sans Serif", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.ToDateLabel.Location = New System.Drawing.Point(496, 144)
        Me.ToDateLabel.Name = "ToDateLabel"
        Me.ToDateLabel.Size = New System.Drawing.Size(215, 38)
        Me.ToDateLabel.TabIndex = 20
        Me.ToDateLabel.Text = "To Date"
        '
        'dtpFromDate
        '
        Me.dtpFromDate.CalendarForeColor = System.Drawing.SystemColors.MenuHighlight
        Me.dtpFromDate.CustomFormat = "dd-MMM-yyyy"
        Me.dtpFromDate.Font = New System.Drawing.Font("Microsoft Sans Serif", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.dtpFromDate.Format = System.Windows.Forms.DateTimePickerFormat.Custom
        Me.dtpFromDate.Location = New System.Drawing.Point(228, 144)
        Me.dtpFromDate.Name = "dtpFromDate"
        Me.dtpFromDate.Size = New System.Drawing.Size(200, 35)
        Me.dtpFromDate.TabIndex = 18
        '
        'FromDateLabel
        '
        Me.FromDateLabel.BackColor = System.Drawing.SystemColors.Menu
        Me.FromDateLabel.Font = New System.Drawing.Font("Microsoft Sans Serif", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.FromDateLabel.Location = New System.Drawing.Point(7, 144)
        Me.FromDateLabel.Name = "FromDateLabel"
        Me.FromDateLabel.Size = New System.Drawing.Size(215, 38)
        Me.FromDateLabel.TabIndex = 19
        Me.FromDateLabel.Text = "From Date"
        '
        'Form1
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(9.0!, 20.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(1118, 505)
        Me.Controls.Add(Me.MainPanel)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog
        Me.MaximizeBox = False
        Me.MinimizeBox = False
        Me.Name = "Form1"
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen
        Me.Text = "Vendor Ledger Report"
        Me.MainPanel.ResumeLayout(False)
        Me.MainPanel.PerformLayout()
        Me.ResumeLayout(False)

    End Sub

    Friend WithEvents MainPanel As Panel
    Friend WithEvents Label1 As Label
    Friend WithEvents btnShowReport As Button
    Friend WithEvents ToCustomerIDLabel As Label
    Friend WithEvents FromCustomerIDLabel As Label
    Friend WithEvents cmbFromVendorID As ComboBox
    Friend WithEvents dtpToDate As DateTimePicker
    Friend WithEvents btnRefresh As Button
    Friend WithEvents cmbToVendorID As ComboBox
    Friend WithEvents ToDateLabel As Label
    Friend WithEvents dtpFromDate As DateTimePicker
    Friend WithEvents FromDateLabel As Label
    Friend WithEvents Summary As RadioButton
    Friend WithEvents Semi As RadioButton
    Friend WithEvents Details As RadioButton
    Friend WithEvents Label2 As Label
End Class
