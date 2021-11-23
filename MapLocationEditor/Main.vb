Imports System.Data.SqlClient
Imports System.Configuration
Imports System.ComponentModel

Public Class Main

    Private locationRectangle As Rectangle
    Private startPoint As Point
    Private isDragging As Boolean
    Private mapImage As Image
    Private imageScaleFactor As Double
    Private imageXpos As Integer
    Private imageYpos As Integer
    Private imageDisplayWidth As Integer
    Private imageDisplayHeight As Integer

    Public Sub New()

        ' This call is required by the designer.
        InitializeComponent()

        mapPictureBox.CreateGraphics.SmoothingMode = Drawing2D.SmoothingMode.AntiAlias
        mapImage = mapPictureBox.Image

        ' Load the room name combobox with a list of room IDs as they appear in the database timetable. 
        Using synergyConn As New SqlConnection(ConfigurationManager.ConnectionStrings("synergy").ToString)
            Using getRoomsListCmd As New SqlCommand(ConfigurationManager.AppSettings("GetRoomsListSql"), synergyConn)

                synergyConn.Open()

                Using getRoomsListReader As SqlDataReader = getRoomsListCmd.ExecuteReader()

                    While getRoomsListReader.Read()
                        RoomNamesCbx.Items.Add(getRoomsListReader("Room"))
                    End While

                End Using
            End Using
        End Using

    End Sub


    ' =====================================================================================================
    ' FUNCTIONS
    ' =====================================================================================================


    Private Sub setImageScalingValues()

        If mapImage IsNot Nothing Then

            ' The picure box image is scaled to fit into the picture box. 
            ' This means that the image's new dimensions will depend on the shape of the image 
            ' relative to the shape of the picture box (which varies as the user resizes the form). 
            ' The image will fit either horizontally or vertically, and the 'non-fitting' dimension
            ' will have some excess padding around the image.
            '
            ' Also, the image will be zoomed by the same scaling factor both horizontally and vertically. 


            ' Compute a Width to Height ratio for both the image and the picture box.
            ' Comparison of the two ratios indicates how the image will be resized to fit
            ' into the picture box (assuming Zoom mode).
            If mapImage.Width / mapImage.Height > mapPictureBox.ClientSize.Width / mapPictureBox.ClientSize.Height Then

                ' Image fits box horizontally and is centred vertically.
                imageScaleFactor = mapPictureBox.ClientSize.Width / mapImage.Width
                imageXpos = 0
                imageYpos = (mapPictureBox.ClientSize.Height - mapImage.Height * imageScaleFactor) \ 2

            Else

                ' Image fits box vertically and is centred horizontally.
                imageScaleFactor = mapPictureBox.ClientSize.Height / mapImage.Height
                imageXpos = (mapPictureBox.ClientSize.Width - mapImage.Width * imageScaleFactor) \ 2
                imageYpos = 0

            End If

            imageDisplayWidth = mapImage.Width * imageScaleFactor
            imageDisplayHeight = mapImage.Height * imageScaleFactor

        End If

    End Sub


    ' =====================================================================================================
    ' EVENT HANDLERS
    ' =====================================================================================================


    Private Sub mapPictureBox_Resize(sender As Object, e As EventArgs) Handles mapPictureBox.Resize

        setImageScalingValues()

    End Sub


    Private Sub mapPictureBox_Paint(sender As Object, e As PaintEventArgs) Handles mapPictureBox.Paint

        setImageScalingValues()

        If locationRectangle <> Nothing Then

            Using myPen As New Pen(Color.Red, 3)
                e.Graphics.DrawRectangle(myPen, locationRectangle.X, locationRectangle.Y,
                        locationRectangle.Width, locationRectangle.Height)
            End Using

        End If

    End Sub


    Private Sub mapPictureBox_MouseDown(sender As Object, e As MouseEventArgs) Handles mapPictureBox.MouseDown
        locationRectangle = New Rectangle(e.X, e.Y, 1, 1)
        startPoint = New Point(e.X, e.Y)
        isDragging = True
        mapPictureBox.Invalidate()
    End Sub


    Private Sub mapPictureBox_MouseMove(sender As Object, e As MouseEventArgs) Handles mapPictureBox.MouseMove

        If isDragging Then

            locationRectangle.X = If(e.X < startPoint.X, e.X, startPoint.X)
            locationRectangle.Width = Math.Abs(startPoint.X - e.X)

            locationRectangle.Y = If(e.Y < startPoint.Y, e.Y, startPoint.Y)
            locationRectangle.Height = Math.Abs(startPoint.Y - e.Y)

            mapPictureBox.Invalidate(New Rectangle(
                locationRectangle.X - 10, locationRectangle.Y - 10,
                locationRectangle.Width + 100, locationRectangle.Height + 100))

        End If

    End Sub


    Private Sub mapPictureBox_MouseUp(sender As Object, e As MouseEventArgs) Handles mapPictureBox.MouseUp
        startPoint = Nothing
        isDragging = False
    End Sub


    Private Sub SaveBtn_Click(sender As Object, e As EventArgs) Handles SaveBtn.Click

        If RoomNamesCbx.Text = "" Then

            MessageBox.Show("No room ID selected. Changes will not be saved.")
            Return

        Else

            ' Location and size of the drawn rectangle are recorded as proportions of the image area
            ' so they can be re-drawn regardless of the image rendering size. 
            Dim locationXProp As Double = (locationRectangle.X - imageXpos) / imageDisplayWidth
            Dim locationYProp As Double = (locationRectangle.Y - imageYpos) / imageDisplayHeight
            Dim locationWidthProp As Double = locationRectangle.Width / imageDisplayWidth
            Dim locationHeightProp As Double = locationRectangle.Height / imageDisplayHeight

            ' Save rectangle proportions to database. 
            Using synergyConn As New SqlConnection(ConfigurationManager.ConnectionStrings("synergy").ToString)
                Using saveLocationCmd As New SqlCommand(
                            ConfigurationManager.AppSettings("SaveLocationProc"), synergyConn)

                    saveLocationCmd.CommandType = CommandType.StoredProcedure
                    saveLocationCmd.Parameters.AddWithValue("Location", RoomNamesCbx.Text)
                    saveLocationCmd.Parameters.AddWithValue("X", locationXProp)
                    saveLocationCmd.Parameters.AddWithValue("Y", locationYProp)
                    saveLocationCmd.Parameters.AddWithValue("Width", locationWidthProp)
                    saveLocationCmd.Parameters.AddWithValue("Height", locationHeightProp)

                    synergyConn.Open()
                    saveLocationCmd.ExecuteNonQuery()

                End Using
            End Using

            messageLbl.Text = String.Format("Saved room [{0}] location as: {1:0.0000}, {2:0.0000}, {3:0.0000}, {4:0.0000}",
                    RoomNamesCbx.Text,
                    locationXProp,
                    locationYProp,
                    locationWidthProp,
                    locationHeightProp)

        End If

    End Sub

    Private Sub RoomNamesCbx_SelectedIndexChanged(sender As Object, e As EventArgs) _
            Handles RoomNamesCbx.SelectedIndexChanged

        ' Get the co-ordinates of a location box (if one has already been saved) to the database and display. 

        If RoomNamesCbx.Text <> "" Then

            Using synergyConn As New SqlConnection(ConfigurationManager.ConnectionStrings("synergy").ToString)
                Using getLocationCmd As New SqlCommand(
                                ConfigurationManager.AppSettings("GetLocationSql"), synergyConn)

                    getLocationCmd.Parameters.AddWithValue("Location", RoomNamesCbx.Text)
                    synergyConn.Open()

                    Using getLocationRdr As SqlDataReader = getLocationCmd.ExecuteReader

                        If getLocationRdr.HasRows Then

                            getLocationRdr.Read()
                            locationRectangle.X = imageXpos + CDbl(getLocationRdr("X")) * imageDisplayWidth
                            locationRectangle.Y = imageYpos + CDbl(getLocationRdr("Y")) * imageDisplayHeight
                            locationRectangle.Width = CDbl(getLocationRdr("Width")) * imageDisplayWidth
                            locationRectangle.Height = CDbl(getLocationRdr("Height")) * imageDisplayHeight

                        Else

                            locationRectangle = Nothing

                        End If

                    End Using
                End Using
            End Using

            mapPictureBox.Invalidate()

        End If
    End Sub


End Class
