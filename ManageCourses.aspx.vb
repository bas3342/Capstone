Imports System.Net.Http
Imports System.Net.Http.Headers
Imports System.Threading.Tasks
Imports System.Web.Script.Serialization
Imports System.Configuration

Public Class Course
    Public Property course_id As Integer
    Public Property course_name As String
    Public Property ects As Integer
    Public Property instructor As String
End Class

Public Class ManageCourses
    Inherits System.Web.UI.Page

    Private serializer As New JavaScriptSerializer()

    Protected Async Sub Page_Load(ByVal sender As Object, ByVal e As EventArgs) Handles Me.Load
        If Not IsPostBack Then
            Await LoadCourses()
        End If
    End Sub

    Protected Async Sub btnAddCourse_Click(sender As Object, e As EventArgs)
        If String.IsNullOrWhiteSpace(txtCourseName.Text) OrElse
           String.IsNullOrWhiteSpace(txtEcts.Text) OrElse
           String.IsNullOrWhiteSpace(txtInstructor.Text) Then

            lblMessage.Text = "All fields are required."
            lblMessage.ForeColor = Drawing.Color.Red
            Return
        End If

        Dim baseUrl As String = ConfigurationManager.AppSettings("SupabaseUrl")
        Dim apiKey As String = ConfigurationManager.AppSettings("SupabaseKey")

        Dim course = New Dictionary(Of String, Object) From {
            {"course_name", txtCourseName.Text},
            {"ects", Convert.ToInt32(txtEcts.Text)},
            {"instructor", txtInstructor.Text}
        }

        Using client As New HttpClient()
            client.BaseAddress = New Uri(baseUrl & "/rest/v1/")
            client.DefaultRequestHeaders.Add("apikey", apiKey)
            client.DefaultRequestHeaders.Authorization = New AuthenticationHeaderValue("Bearer", apiKey)

            Dim content = New StringContent(serializer.Serialize(course), System.Text.Encoding.UTF8, "application/json")
            Dim response = Await client.PostAsync("Courses", content)

            If response.IsSuccessStatusCode Then
                lblMessage.Text = "✅ Course added successfully."
                lblMessage.ForeColor = Drawing.Color.Green
                Await LoadCourses()
                ClearForm()
            Else
                lblMessage.Text = "❌ Failed to add course."
                lblMessage.ForeColor = Drawing.Color.Red
            End If
        End Using
    End Sub

    Private Async Function LoadCourses() As Task
        Dim baseUrl As String = ConfigurationManager.AppSettings("SupabaseUrl")
        Dim apiKey As String = ConfigurationManager.AppSettings("SupabaseKey")

        Using client As New HttpClient()
            client.BaseAddress = New Uri(baseUrl & "/rest/v1/")
            client.DefaultRequestHeaders.Add("apikey", apiKey)
            client.DefaultRequestHeaders.Authorization = New AuthenticationHeaderValue("Bearer", apiKey)

            Dim response = Await client.GetAsync("Courses?select=*")
            Dim json = Await response.Content.ReadAsStringAsync()
            Dim courses = serializer.Deserialize(Of List(Of Course))(json)
            gvCourses.DataSource = courses
            gvCourses.DataBind()



        End Using
    End Function

    Protected Async Sub gvCourses_RowDeleting(sender As Object, e As GridViewDeleteEventArgs)
        Dim courseId = gvCourses.DataKeys(e.RowIndex).Value.ToString()
        Dim baseUrl As String = ConfigurationManager.AppSettings("SupabaseUrl")
        Dim apiKey As String = ConfigurationManager.AppSettings("SupabaseKey")

        Using client As New HttpClient()
            client.BaseAddress = New Uri(baseUrl & "/rest/v1/")
            client.DefaultRequestHeaders.Add("apikey", apiKey)
            client.DefaultRequestHeaders.Authorization = New AuthenticationHeaderValue("Bearer", apiKey)

            Dim response = Await client.DeleteAsync($"Courses?course_id=eq.{courseId}")
            If response.IsSuccessStatusCode Then
                lblMessage.Text = "✅ Course deleted."
                lblMessage.ForeColor = Drawing.Color.Green
            Else
                lblMessage.Text = "❌ Failed to delete course."
                lblMessage.ForeColor = Drawing.Color.Red
            End If
            Await LoadCourses()
        End Using
    End Sub

    Private Sub ClearForm()
        txtCourseName.Text = ""
        txtEcts.Text = ""
        txtInstructor.Text = ""
    End Sub
End Class
