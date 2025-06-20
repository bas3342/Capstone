Imports System.Net.Http
Imports System.Net.Http.Headers
Imports System.Threading.Tasks
Imports System.Web.Script.Serialization
Imports System.Configuration

Public Class StudentDashboard
    Inherits System.Web.UI.Page

    Private serializer As New JavaScriptSerializer()

    Protected Async Sub Page_Load(ByVal sender As Object, ByVal e As EventArgs) Handles Me.Load
        If Not IsPostBack Then
            Dim email As String = Session("userEmail")?.ToString()
            If String.IsNullOrEmpty(email) Then
                Response.Redirect("Login.aspx")
                Return
            End If

            Dim baseUrl As String = ConfigurationManager.AppSettings("SupabaseUrl")
            Dim apiKey As String = ConfigurationManager.AppSettings("SupabaseKey")



            Using client As New HttpClient()
                client.BaseAddress = New Uri(baseUrl & "/rest/v1/")
                client.DefaultRequestHeaders.Add("apikey", apiKey)
                client.DefaultRequestHeaders.Authorization = New AuthenticationHeaderValue("Bearer", apiKey)

                Dim studentId = Await GetStudentIdByEmail(client, email)
                If studentId Is Nothing Then
                    litEnrolledCourses.Text = "❌ Student not found."
                    Return
                End If

                Await LoadCourses(client)
                Await LoadEnrolledCourses(client, studentId)
            End Using
        End If
    End Sub

    Private Async Function GetStudentIdByEmail(client As HttpClient, email As String) As Task(Of String)
        Dim response = Await client.GetAsync($"Students?email=eq.{Uri.EscapeDataString(email)}&select=id")
        Dim json = Await response.Content.ReadAsStringAsync()
        Dim students = serializer.Deserialize(Of List(Of Dictionary(Of String, Object)))(json)
        If students.Count > 0 Then
            Return students(0)("id").ToString()
        End If
        Return Nothing
    End Function

    Private Async Function LoadCourses(client As HttpClient) As Task
        Dim response = Await client.GetAsync("Courses?select=course_id,course_name")
        Dim json = Await response.Content.ReadAsStringAsync()



        If response.IsSuccessStatusCode Then
            Dim courses = serializer.Deserialize(Of List(Of Dictionary(Of String, Object)))(json)
            ddlCourses.Items.Clear()
            ddlCourses.Items.Add(New ListItem("-- Select a course --", ""))
            For Each course In courses
                ddlCourses.Items.Add(New ListItem(course("course_name").ToString(), course("course_id").ToString()))
            Next
        Else
            ddlCourses.Items.Clear()
            ddlCourses.Items.Add(New ListItem("⚠ Failed to load courses", ""))
        End If
    End Function

    Private Async Function LoadEnrolledCourses(client As HttpClient, studentId As String) As Task
        Dim response = Await client.GetAsync($"Enrollments?student_id=eq.{studentId}&select=course_id")
        Dim json = Await response.Content.ReadAsStringAsync()
        Dim enrollments = serializer.Deserialize(Of List(Of Dictionary(Of String, Object)))(json)

        If enrollments.Count = 0 Then
            litEnrolledCourses.Text = "You are not enrolled in any courses."
            Return
        End If

        Dim output As String = "<ul>"
        For Each enrollment In enrollments
            Dim courseId = enrollment("course_id").ToString()
            Dim courseResponse = Await client.GetAsync($"Courses?course_id=eq.{courseId}&select=course_name")
            Dim courseJson = Await courseResponse.Content.ReadAsStringAsync()
            Dim courses = serializer.Deserialize(Of List(Of Dictionary(Of String, Object)))(courseJson)
            If courses.Count > 0 Then
                output &= "<li>" & courses(0)("course_name").ToString() & "</li>"
            End If
        Next
        output &= "</ul>"
        litEnrolledCourses.Text = output
    End Function

    Protected Async Sub btnEnroll_Click(sender As Object, e As EventArgs)
        lblMessage.Visible = True

        If ddlCourses.SelectedValue = "" Then
            lblMessage.Text = "⚠ Please select a course."
            lblMessage.ForeColor = Drawing.Color.Red
            Return
        End If

        Dim email As String = Session("userEmail")?.ToString()
        If String.IsNullOrEmpty(email) Then
            lblMessage.Text = "⚠ Session expired. Please log in again."
            Return
        End If

        Dim baseUrl As String = ConfigurationManager.AppSettings("SupabaseUrl")
        Dim apiKey As String = ConfigurationManager.AppSettings("SupabaseKey")

        Using client As New HttpClient()
            client.BaseAddress = New Uri(baseUrl & "/rest/v1/")
            client.DefaultRequestHeaders.Add("apikey", apiKey)
            client.DefaultRequestHeaders.Authorization = New AuthenticationHeaderValue("Bearer", apiKey)

            Dim studentId = Await GetStudentIdByEmail(client, email)
            If studentId Is Nothing Then
                lblMessage.Text = "❌ Student not found."
                Return
            End If

            Dim checkUrl = $"Enrollments?student_id=eq.{studentId}&course_id=eq.{ddlCourses.SelectedValue}"
            Dim checkResponse = Await client.GetAsync(checkUrl)
            Dim checkJson = Await checkResponse.Content.ReadAsStringAsync()
            Dim existing = serializer.Deserialize(Of List(Of Object))(checkJson)

            If existing.Count > 0 Then
                lblMessage.Text = "⚠ Already enrolled."
                lblMessage.ForeColor = Drawing.Color.Orange
                Return
            End If

            Dim enrollment = New Dictionary(Of String, Object) From {
                {"student_id", studentId},
                {"course_id", ddlCourses.SelectedValue},
                {"enrollment_date", DateTime.Now.ToString("yyyy-MM-dd")}
            }

            Dim content = New StringContent(serializer.Serialize(enrollment), System.Text.Encoding.UTF8, "application/json")
            Dim apiResponse = Await client.PostAsync("Enrollments", content)
            If apiResponse.IsSuccessStatusCode Then
                lblMessage.Text = "✅ Enrolled successfully!"
                lblMessage.ForeColor = Drawing.Color.Green
                Response.Redirect(Request.RawUrl)  ' ✅ works now

            Else
                lblMessage.Text = "❌ Enrollment failed."
                lblMessage.ForeColor = Drawing.Color.Red
            End If
        End Using
    End Sub
End Class
