Public Class Form1

    Dim CurrentSelectedMonitor As MonitorBox

    Private Sub Form1_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Dim largestHeight As Integer = 0
        For i As Integer = 0 To SystemInformation.MonitorCount - 1
            Dim nMonitor As New MonitorBox
            nMonitor.Size = New Size((15 / 100) * Screen.AllScreens(i).WorkingArea.Width, (15 / 100) * Screen.AllScreens(i).WorkingArea.Height)
            nMonitor.MonitorSize = New Size(Screen.AllScreens(i).WorkingArea.Width, Screen.AllScreens(i).WorkingArea.Height)
            nMonitor.MonitorName = Screen.AllScreens(i).DeviceName.ToString
            nMonitor.MonitorWorkingArea = Screen.AllScreens(i).WorkingArea.Location
            AddHandler nMonitor.Click, AddressOf MonitorBoxClicked
            AddHandler nMonitor.DoubleClick, AddressOf MonitorBoxDoubleClicked
            FlowLayoutPanel1.Controls.Add(nMonitor)
            If Screen.AllScreens(i).WorkingArea.Height > largestHeight Then
                largestHeight = Screen.AllScreens(i).WorkingArea.Height
            End If
        Next
        FlowLayoutPanel1.Size = New Size(FlowLayoutPanel1.Width, ((15 / 100) * largestHeight) + 8)
        GroupBox1.Location = New Point(12, FlowLayoutPanel1.Location.Y + FlowLayoutPanel1.Height + 7)
        Button2.Location = New Point(337, GroupBox1.Location.Y + GroupBox1.Size.Height + 4)
        Me.Size = New Size(Width, Button2.Location.Y + Button2.Height + 47)
    End Sub

    Public Function ScaleImage(ByVal OldImage As Image, ByVal TargetHeight As Integer, ByVal TargetWidth As Integer) As System.Drawing.Image
        Dim NewHeight As Integer = TargetHeight
        Dim NewWidth As Integer = NewHeight / OldImage.Height * OldImage.Width
        If NewWidth > TargetWidth Then
            NewWidth = TargetWidth
            NewHeight = NewWidth / OldImage.Width * OldImage.Height
        End If
        Return New Bitmap(OldImage, NewWidth, NewHeight)
    End Function

    Public Sub MonitorBoxClicked(sender As Object, e As EventArgs)
        Dim curMonitor As MonitorBox = DirectCast(sender, MonitorBox)
        CurrentSelectedMonitor = curMonitor
        TextBox1.Text = curMonitor.MonitorName
        TextBox2.Text = curMonitor.MonitorSize.Width.ToString & "x" & curMonitor.MonitorSize.Height.ToString
        TextBox3.Text = curMonitor.MonitorWallpaper
    End Sub

    Public Sub MonitorBoxDoubleClicked(sender As Object, e As EventArgs)
        Dim curMonitor As MonitorBox = DirectCast(sender, MonitorBox)
        CurrentSelectedMonitor = curMonitor
        TextBox1.Text = curMonitor.MonitorName
        TextBox2.Text = curMonitor.MonitorSize.Width.ToString & "x" & curMonitor.MonitorSize.Height.ToString
        TextBox3.Text = curMonitor.MonitorWallpaper

        Button1.PerformClick()
    End Sub

    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        Dim ofd As New OpenFileDialog
        If ofd.ShowDialog = Windows.Forms.DialogResult.OK Then
            Dim nowImg As Image = Image.FromFile(ofd.FileName)
            CurrentSelectedMonitor.Image = Image.FromFile(ofd.FileName)
            CurrentSelectedMonitor.MonitorWallpaper = ofd.FileName.ToString
            TextBox3.Text = ofd.FileName
        End If
    End Sub

    Private Sub Button2_Click(sender As Object, e As EventArgs) Handles Button2.Click
        Dim genWallpaperImage As New Bitmap(SystemInformation.VirtualScreen.Width, SystemInformation.VirtualScreen.Height)
        For Each ctrl As Control In FlowLayoutPanel1.Controls
            If TypeOf ctrl Is MonitorBox Then
                Dim checkMon As MonitorBox = DirectCast(ctrl, MonitorBox)
                If String.IsNullOrEmpty(checkMon.MonitorWallpaper) Then
                    Dim result = MessageBox.Show(String.Format("Monitor {0} Is missing a wallpaper, If you choose to continue monitor {0} will have a black background!", checkMon.MonitorName.ToString), "Monitor Missing!", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Error)
                    If result = DialogResult.Cancel Then
                    ElseIf result = DialogResult.No Then
                    ElseIf result = DialogResult.Yes Then
                        GoTo genWallpaper
                    End If
                Else
genWallpaper:
                    Dim g As Graphics = Graphics.FromImage(genWallpaperImage)
                    g.Clear(Color.Black)

                    For Each nMon As MonitorBox In FlowLayoutPanel1.Controls
                        g.DrawImage(Image.FromFile(nMon.MonitorWallpaper), New Rectangle(nMon.MonitorWorkingArea.X, nMon.MonitorWorkingArea.Y, nMon.MonitorSize.Width, nMon.MonitorSize.Height))
                    Next

                End If
            End If
        Next

        Dim sfd As New SaveFileDialog
        If sfd.ShowDialog = Windows.Forms.DialogResult.OK Then
            genWallpaperImage.Save(sfd.FileName)
        End If
    End Sub
End Class

Public Class MonitorBox
    Inherits PictureBox

    Public _MonitorName As String
    Public _MontiorSize As Size
    Public _MonitorWallpaper As String
    Public _MonitorWorkingArea As Point

    Public Property MonitorWorkingArea As Point
        Get
            Return _MonitorWorkingArea
        End Get
        Set(value As Point)
            _MonitorWorkingArea = value
        End Set
    End Property

    Public Property MonitorWallpaper As String
        Get
            Return _MonitorWallpaper
        End Get
        Set(value As String)
            _MonitorWallpaper = value
        End Set
    End Property
    Public Property MonitorName As String
        Get
            Return _MonitorName
        End Get
        Set(value As String)
            _MonitorName = value
        End Set
    End Property
    Public Property MonitorSize As Size
        Get
            Return _MontiorSize
        End Get
        Set(value As Size)
            _MontiorSize = value
        End Set
    End Property

    Sub New()
        SizeMode = PictureBoxSizeMode.StretchImage
        BorderStyle = Windows.Forms.BorderStyle.FixedSingle
    End Sub
End Class