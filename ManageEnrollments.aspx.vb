Imports System.Configuration
Imports System.Net.Http
Imports System.Net.Http.Headers
Imports System.Threading.Tasks
Imports System.Web.Script.Serialization

Public Class ManageEnrollments
    Inherits System.Web.UI.Page

    Private serializer As New JavaScriptSerializer()

    Public Class EnrollmentDisplay
        Public Property enrollment_id As Integer
        Public Property student_name As String
        Public Property course_name As String
        Public Property enrollment_date As String
    End Class

    Protected Async Sub Page_Load(ByVal sender As Object, ByVal e As EventArgs) Handles Me.Load
        If Not IsPostBack Then
            Await LoadEnrollments()
        End If
    End Sub

    Private Async Function LoadEnrollments() As Task
        Dim baseUrl As String = ConfigurationManager.AppSettings("SupabaseUrl")
        Dim apiKey As String = ConfigurationManager.AppSettings("SupabaseKey")

        Using client As New HttpClient()
            client.BaseAddress = New Uri(baseUrl & "/rest/v1/")
            client.DefaultRequestHeaders.Add("apikey", apiKey)
            client.DefaultRequestHeaders.Authorization = New AuthenticationHeaderValue("Bearer", apiKey)

            ' Step 1: Get raw enrollment data
            Dim enrollmentsJson = Await (Await client.GetAsync("Enrollments?select=*")).Content.ReadAsStringAsync()
            Dim enrollments = serializer.Deserialize(Of List(Of Dictionary(Of String, Object)))(enrollmentsJson)

            ' Step 2: Get all students
            Dim studentsJson = Await (Await client.GetAsync("Students?select=id,first_name,last_name")).Content.ReadAsStringAsync()
            Dim students = serializer.Deserialize(Of List(Of Dictionary(Of String, Object)))(studentsJson).ToDictionary(Function(s) s("id").ToString())

            ' Step 3: Get all courses
            Dim coursesJson = Await (Await client.GetAsync("Courses?select=course_id,course_name")).Content.ReadAsStringAsync()
            Dim courses = serializer.Deserialize(Of List(Of Dictionary(Of String, Object)))(coursesJson).ToDictionary(Function(c) c("course_id").ToString())

            ' Step 4: Merge into display data
            Dim displayList As New List(Of EnrollmentDisplay)()
            For Each item In enrollments
                Dim studentId = item("student_id").ToString()
                Dim courseId = item("course_id").ToString()

                Dim studentName = If(students.ContainsKey(studentId),
                                 students(studentId)("first_name") & " " & students(studentId)("last_name"),
                                 "Unknown")

                Dim courseName = If(courses.ContainsKey(courseId),
                                courses(courseId)("course_name").ToString(),
                                "Unknown")

                displayList.Add(New EnrollmentDisplay With {
                .enrollment_id = Convert.ToInt32(item("enrollment_id")),
                .student_name = studentName,
                .course_name = courseName,
                .enrollment_date = item("enrollment_date").ToString()
            })
            Next

            gvEnrollments.DataSource = displayList
            gvEnrollments.DataBind()
        End Using
    End Function


    Protected Async Sub gvEnrollments_RowDeleting(sender As Object, e As GridViewDeleteEventArgs)
        Dim enrollmentId = gvEnrollments.DataKeys(e.RowIndex).Value.ToString()
        Dim baseUrl As String = ConfigurationManager.AppSettings("SupabaseUrl")
        Dim apiKey As String = ConfigurationManager.AppSettings("SupabaseKey")

        Using client As New HttpClient()
            client.BaseAddress = New Uri(baseUrl & "/rest/v1/")
            client.DefaultRequestHeaders.Add("apikey", apiKey)
            client.DefaultRequestHeaders.Authorization = New AuthenticationHeaderValue("Bearer", apiKey)

            Dim response = Await client.DeleteAsync($"Enrollments?enrollment_id=eq.{enrollmentId}")
            If response.IsSuccessStatusCode Then
                lblMessage.Text = "✅ Enrollment deleted."
            Else
                lblMessage.Text = "❌ Failed to delete enrollment."
            End If

            Await LoadEnrollments()
        End Using
    End Sub
End Class
