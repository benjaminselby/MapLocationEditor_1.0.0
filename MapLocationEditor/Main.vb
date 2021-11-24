Imports System.Data.SqlClient
Imports System.Configuration
Imports System.ComponentModel

Public Class Main

    Private locationRectangle As Rectangle
    Private startPoint As Point
    Private isDragging As Boolean
    Private imageScaleFactor As Double
    Private imagePosition As Point
    Private imageDisplaySize As Size
    Private oldImagePosition As Point
    Private oldImageDisplaySize As Size
    Private oldMousePosition As Point


    Public Sub New()

        ' This call is required by the designer.
        InitializeComponent()

        mapPictureBox.CreateGraphics.SmoothingMode = Drawing2D.SmoothingMode.AntiAlias

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

        If mapPictureBox.Image IsNot Nothing Then

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
            If mapPictureBox.Image.Width / mapPictureBox.Image.Height > mapPictureBox.ClientSize.Width / mapPictureBox.ClientSize.Height Then

                ' Image fits box horizontally and is centred vertically.
                imageScaleFactor = mapPictureBox.ClientSize.Width / mapPictureBox.Image.Width
                imagePosition.X = 0
                imagePosition.Y = (mapPictureBox.ClientSize.Height - mapPictureBox.Image.Height * imageScaleFactor) \ 2

            Else

                ' Image fits box vertically and is centred horizontally.
                imageScaleFactor = mapPictureBox.ClientSize.Height / mapPictureBox.Image.Height
                imagePosition.X = (mapPictureBox.ClientSize.Width - mapPictureBox.Image.Width * imageScaleFactor) \ 2
                imagePosition.Y = 0

            End If

            imageDisplaySize.Width = mapPictureBox.Image.Width * imageScaleFactor
            imageDisplaySize.Height = mapPictureBox.Image.Height * imageScaleFactor

        End If

    End Sub


    ' =====================================================================================================
    ' EVENT HANDLERS
    ' =====================================================================================================


    Private Sub mapPictureBox_Resize(sender As Object, e As EventArgs) Handles mapPictureBox.Resize

        oldImagePosition = imagePosition

        setImageScalingValues()

        ' Resize the current location rectangle (if there is one).
        If locationRectangle.Size <> New Size(0, 0) And oldImageDisplaySize <> New Size(0, 0) Then

            Dim horizScale As Double = imageDisplaySize.Width / oldImageDisplaySize.Width
            Dim vertScale As Double = imageDisplaySize.Height / oldImageDisplaySize.Height

            locationRectangle.X = (locationRectangle.X - oldImagePosition.X) * horizScale + imagePosition.X
            locationRectangle.Y = (locationRectangle.Y - oldImagePosition.Y) * vertScale + imagePosition.Y
            locationRectangle.Width = locationRectangle.Width * horizScale
            locationRectangle.Height = locationRectangle.Height * vertScale

        End If

        oldImageDisplaySize = imageDisplaySize

    End Sub


    Private Sub mapPictureBox_Paint(sender As Object, e As PaintEventArgs) Handles mapPictureBox.Paint

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

            locationRectangle.X = Math.Min(e.X, startPoint.X)
            locationRectangle.Y = Math.Min(e.Y, startPoint.Y)
            locationRectangle.Width = Math.Abs(startPoint.X - e.X)
            locationRectangle.Height = Math.Abs(startPoint.Y - e.Y)

            ' The invalidation area needs to cover the current rectangle area, but also include 
            ' the previous mouse pointer location. Without doing this, rapid mouse movements will leave
            ' orphaned drawing elements. 
            ' The invalidation area is also padded by 10 pixels each way to make sure it covers
            ' thick borders. This could arguably be improved by detecting the current drawing pen width
            ' but that's probably overkill at this point. 

            Dim invalidateRect As Rectangle
            invalidateRect.X = Math.Min(oldMousePosition.X, locationRectangle.X) - 10
            invalidateRect.Y = Math.Min(oldMousePosition.Y, locationRectangle.Y) - 10
            invalidateRect.Width = Math.Abs(
                Math.Max(oldMousePosition.X, Math.Max(e.X, startPoint.X)) - invalidateRect.X) + 20
            invalidateRect.Height = Math.Abs(
                Math.Max(oldMousePosition.Y, Math.Max(e.Y, startPoint.Y)) - invalidateRect.Y) + 20

            mapPictureBox.Invalidate(invalidateRect)

            oldMousePosition = e.Location

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
            Dim locationXProp As Double = (locationRectangle.X - imagePosition.X) / imageDisplaySize.Width
            Dim locationYProp As Double = (locationRectangle.Y - imagePosition.Y) / imageDisplaySize.Height
            Dim locationWidthProp As Double = locationRectangle.Width / imageDisplaySize.Width
            Dim locationHeightProp As Double = locationRectangle.Height / imageDisplaySize.Height

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

        ' Get the co-ordinates of a location box (if one has already been saved to the database) and display. 

        If RoomNamesCbx.Text <> "" Then

            Using synergyConn As New SqlConnection(ConfigurationManager.ConnectionStrings("synergy").ToString)
                Using getLocationCmd As New SqlCommand(
                                ConfigurationManager.AppSettings("GetLocationSql"), synergyConn)

                    getLocationCmd.Parameters.AddWithValue("Location", RoomNamesCbx.Text)
                    synergyConn.Open()

                    Using getLocationRdr As SqlDataReader = getLocationCmd.ExecuteReader

                        If getLocationRdr.HasRows Then

                            getLocationRdr.Read()
                            locationRectangle.X = imagePosition.X + CDbl(getLocationRdr("X")) * imageDisplaySize.Width
                            locationRectangle.Y = imagePosition.Y + CDbl(getLocationRdr("Y")) * imageDisplaySize.Height
                            locationRectangle.Width = CDbl(getLocationRdr("Width")) * imageDisplaySize.Width
                            locationRectangle.Height = CDbl(getLocationRdr("Height")) * imageDisplaySize.Height

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
