Imports System.Net.Http
Imports System.Net.Http.Headers
Imports System.Web.Script.Serialization

Public Class ViewCourses
    Inherits System.Web.UI.Page

    Private client As HttpClient
    Private baseUrl As String
    Private apiKey As String

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As EventArgs) Handles Me.Load
        baseUrl = ConfigurationManager.AppSettings("SupabaseUrl").Replace("/rest/v1/Students", "")
        apiKey = ConfigurationManager.AppSettings("SupabaseKey")

        client = New HttpClient()
        client.BaseAddress = New Uri(baseUrl)
        client.DefaultRequestHeaders.Add("apikey", apiKey)
        client.DefaultRequestHeaders.Authorization = New AuthenticationHeaderValue("Bearer", apiKey)

        If Not IsPostBack Then
            LoadStudents()
            LoadCourses()
            LoadEnrollments()
        End If
    End Sub


    Private Sub LoadStudents()
        Dim response = client.GetAsync("/rest/v1/Students?select=id,first_name,last_name").Result
        If response.IsSuccessStatusCode Then
            Dim json = response.Content.ReadAsStringAsync().Result
            Dim serializer As New JavaScriptSerializer()
            Dim students = serializer.Deserialize(Of List(Of Dictionary(Of String, Object)))(json)

            ddlStudents.DataSource = students.Select(Function(s) New With {
                .id = s("id"),
                .full_name = s("first_name") & " " & s("last_name")
            }).ToList()

            ddlStudents.DataTextField = "full_name"
            ddlStudents.DataValueField = "id"
            ddlStudents.DataBind()
        End If
    End Sub

    Private Sub LoadCourses()
        Dim response = client.GetAsync("/rest/v1/Courses?select=course_id,course_name").Result
        If response.IsSuccessStatusCode Then
            Dim json = response.Content.ReadAsStringAsync().Result
            Dim serializer As New JavaScriptSerializer()
            Dim courses = serializer.Deserialize(Of List(Of Dictionary(Of String, Object)))(json)

            ddlCourses.DataSource = courses.Select(Function(c) New With {
            .course_name = c("course_name"),
            .course_id = c("course_id")
        }).ToList()

            ddlCourses.DataTextField = "course_name"
            ddlCourses.DataValueField = "course_id"
            ddlCourses.DataBind()
        End If
    End Sub


    Private Sub LoadEnrollments()
        Dim url = "/rest/v1/Enrollments?select=enrollment_id,student:Students(first_name,last_name),course:Courses(course_name),enrollment_date&order=enrollment_date.desc"
        Dim response = client.GetAsync(url).Result
        If response.IsSuccessStatusCode Then
            Dim json = response.Content.ReadAsStringAsync().Result
            Dim serializer As New JavaScriptSerializer()
            Dim data = serializer.Deserialize(Of List(Of Dictionary(Of String, Object)))(json)

            Dim table As New DataTable()
            table.Columns.Add("enrollment_id")
            table.Columns.Add("student")
            table.Columns.Add("course_name")
            table.Columns.Add("enrollment_date")

            For Each row In data
                Dim student = row("student")
                Dim course = row("course")
                table.Rows.Add(row("enrollment_id"),
                               student("first_name") & " " & student("last_name"),
                               course("course_name"),
                               row("enrollment_date"))
            Next

            gvEnrollments.DataSource = table
            gvEnrollments.DataBind()
        End If
    End Sub

    Protected Sub btnEnroll_Click(sender As Object, e As EventArgs)
        If ddlStudents.SelectedValue = "" OrElse ddlCourses.SelectedValue = "" OrElse txtEnrollmentDate.Text = "" Then
            lblMessage.Text = "❌ All fields are required."
            lblMessage.CssClass = "text-danger"
            lblMessage.Visible = True
            Return
        End If

        Dim data As New Dictionary(Of String, Object) From {
            {"student_id", Convert.ToInt32(ddlStudents.SelectedValue)},
            {"course_id", Convert.ToInt32(ddlCourses.SelectedValue)},
            {"enrollment_date", DateTime.Parse(txtEnrollmentDate.Text).ToString("yyyy-MM-dd")}
        }

        Dim serializer As New JavaScriptSerializer()
        Dim jsonData = serializer.Serialize(New Object() {data})
        Dim content = New StringContent(jsonData, System.Text.Encoding.UTF8, "application/json")

        Dim response = client.PostAsync("/rest/v1/Enrollments", content).Result

        If response.IsSuccessStatusCode Then
            lblMessage.Text = "✅ Enrollment successful."
            lblMessage.CssClass = "text-success"
            lblMessage.Visible = True
            LoadEnrollments()
        Else
            lblMessage.Text = "❌ Error: " & response.StatusCode.ToString()
            lblMessage.CssClass = "text-danger"
            lblMessage.Visible = True
        End If
    End Sub
End Class
