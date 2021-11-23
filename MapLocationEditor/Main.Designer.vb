<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class Main
    Inherits System.Windows.Forms.Form

    'Form overrides dispose to clean up the component list.
    <System.Diagnostics.DebuggerNonUserCode()>
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
    <System.Diagnostics.DebuggerStepThrough()>
    Private Sub InitializeComponent()
        Me.mapPictureBox = New System.Windows.Forms.PictureBox()
        Me.Label1 = New System.Windows.Forms.Label()
        Me.SaveBtn = New System.Windows.Forms.Button()
        Me.RoomNamesCbx = New System.Windows.Forms.ComboBox()
        Me.messageLbl = New System.Windows.Forms.Label()
        CType(Me.mapPictureBox, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'mapPictureBox
        '
        Me.mapPictureBox.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
            Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.mapPictureBox.Image = Global.Graphics_1.My.Resources.Resources.College_Map_August_2021
        Me.mapPictureBox.Location = New System.Drawing.Point(10, 35)
        Me.mapPictureBox.Name = "mapPictureBox"
        Me.mapPictureBox.Size = New System.Drawing.Size(1083, 775)
        Me.mapPictureBox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom
        Me.mapPictureBox.TabIndex = 0
        Me.mapPictureBox.TabStop = False
        '
        'Label1
        '
        Me.Label1.AutoSize = True
        Me.Label1.Font = New System.Drawing.Font("Segoe UI", 9.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point)
        Me.Label1.Location = New System.Drawing.Point(12, 9)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(93, 15)
        Me.Label1.TabIndex = 2
        Me.Label1.Text = "Location Name:"
        Me.Label1.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        '
        'SaveBtn
        '
        Me.SaveBtn.Location = New System.Drawing.Point(291, 5)
        Me.SaveBtn.Name = "SaveBtn"
        Me.SaveBtn.Size = New System.Drawing.Size(75, 23)
        Me.SaveBtn.TabIndex = 3
        Me.SaveBtn.Text = "Save"
        Me.SaveBtn.UseVisualStyleBackColor = True
        '
        'RoomNamesCbx
        '
        Me.RoomNamesCbx.FormattingEnabled = True
        Me.RoomNamesCbx.Location = New System.Drawing.Point(112, 5)
        Me.RoomNamesCbx.Name = "RoomNamesCbx"
        Me.RoomNamesCbx.Size = New System.Drawing.Size(173, 23)
        Me.RoomNamesCbx.TabIndex = 4
        '
        'messageLbl
        '
        Me.messageLbl.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.messageLbl.Location = New System.Drawing.Point(372, 5)
        Me.messageLbl.Name = "messageLbl"
        Me.messageLbl.Size = New System.Drawing.Size(719, 23)
        Me.messageLbl.TabIndex = 5
        Me.messageLbl.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        '
        'Form1
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(7.0!, 15.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(1103, 818)
        Me.Controls.Add(Me.messageLbl)
        Me.Controls.Add(Me.RoomNamesCbx)
        Me.Controls.Add(Me.SaveBtn)
        Me.Controls.Add(Me.Label1)
        Me.Controls.Add(Me.mapPictureBox)
        Me.Name = "Form1"
        Me.Text = "Form1"
        CType(Me.mapPictureBox, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub

    Friend WithEvents mapPictureBox As PictureBox
    Friend WithEvents Label1 As Label
    Friend WithEvents SaveBtn As Button
    Friend WithEvents RoomNamesCbx As ComboBox
    Friend WithEvents messageLbl As Label
End Class
