Imports System.Configuration
Imports System.Data
Imports System.Net.Http
Imports System.Net.Http.Headers
Imports System.Threading.Tasks
Imports System.Web.Script.Serialization

Public Class ManageStudents
    Inherits System.Web.UI.Page

    Private serializer As New JavaScriptSerializer()

    Protected Async Sub Page_Load(ByVal sender As Object, ByVal e As EventArgs) Handles Me.Load
        If Not IsPostBack Then
            Await LoadStudents()
        End If
    End Sub

    Protected Async Sub btnCreate_Click(sender As Object, e As EventArgs)
        If String.IsNullOrWhiteSpace(txtFirstName.Text) OrElse
           String.IsNullOrWhiteSpace(txtLastName.Text) OrElse
           String.IsNullOrWhiteSpace(txtEmail.Text) OrElse
           String.IsNullOrWhiteSpace(txtEnrollDate.Text) Then

            lblMessage.Text = "All fields are required."
            lblMessage.ForeColor = Drawing.Color.Red
            Return
        End If

        Dim student = New Dictionary(Of String, Object) From {
            {"first_name", txtFirstName.Text},
            {"last_name", txtLastName.Text},
            {"email", txtEmail.Text},
            {"enrollment_date", DateTime.Parse(txtEnrollDate.Text).ToString("yyyy-MM-dd")}
        }

        Dim baseUrl As String = ConfigurationManager.AppSettings("SupabaseUrl")
        Dim apiKey As String = ConfigurationManager.AppSettings("SupabaseKey")

        Using client As New HttpClient()
            client.BaseAddress = New Uri(baseUrl & "/rest/v1/")
            client.DefaultRequestHeaders.Add("apikey", apiKey)
            client.DefaultRequestHeaders.Authorization = New AuthenticationHeaderValue("Bearer", apiKey)

            Dim content = New StringContent(serializer.Serialize(student), System.Text.Encoding.UTF8, "application/json")
            Dim response = Await client.PostAsync("Students", content)

            If response.IsSuccessStatusCode Then
                lblMessage.Text = "Student added successfully."
                lblMessage.ForeColor = Drawing.Color.Green
                ClearForm()
                Await LoadStudents()
            Else
                lblMessage.Text = "Error adding student."
                lblMessage.ForeColor = Drawing.Color.Red
            End If
        End Using
    End Sub

    Private Async Function LoadStudents() As Task
        Dim baseUrl As String = ConfigurationManager.AppSettings("SupabaseUrl")
        Dim apiKey As String = ConfigurationManager.AppSettings("SupabaseKey")

        Using client As New HttpClient()
            client.BaseAddress = New Uri(baseUrl & "/rest/v1/")
            client.DefaultRequestHeaders.Add("apikey", apiKey)
            client.DefaultRequestHeaders.Authorization = New AuthenticationHeaderValue("Bearer", apiKey)

            Dim response = Await client.GetAsync("Students?select=*")
            Dim json = Await response.Content.ReadAsStringAsync()

            Dim students = serializer.Deserialize(Of List(Of Dictionary(Of String, Object)))(json)
            Dim dt As New DataTable()
            If students.Count > 0 Then
                For Each key In students(0).Keys
                    dt.Columns.Add(key)
                Next
                For Each student In students
                    Dim row = dt.NewRow()
                    For Each key In student.Keys
                        row(key) = student(key)
                    Next
                    dt.Rows.Add(row)
                Next
            End If

            gvStudents.DataSource = dt
            gvStudents.DataBind()
        End Using
    End Function

    Protected Sub gvStudents_RowEditing(sender As Object, e As GridViewEditEventArgs)
        gvStudents.EditIndex = e.NewEditIndex
        LoadStudents().Wait()
    End Sub

    Protected Sub gvStudents_RowCancelingEdit(sender As Object, e As GridViewCancelEditEventArgs)
        gvStudents.EditIndex = -1
        LoadStudents().Wait()
    End Sub

    Protected Sub gvStudents_RowUpdating(sender As Object, e As GridViewUpdateEventArgs)
        Dim row As GridViewRow = gvStudents.Rows(e.RowIndex)
        Dim id As String = gvStudents.DataKeys(e.RowIndex).Value.ToString()

        Dim firstName As String = CType(row.Cells(1).Controls(0), TextBox).Text
        Dim lastName As String = CType(row.Cells(2).Controls(0), TextBox).Text
        Dim email As String = CType(row.Cells(3).Controls(0), TextBox).Text
        Dim enrollDate As String = CType(row.Cells(4).Controls(0), TextBox).Text

        Dim updatedStudent = New Dictionary(Of String, Object) From {
            {"first_name", firstName},
            {"last_name", lastName},
            {"email", email},
            {"enrollment_date", enrollDate}
        }

        Dim baseUrl As String = ConfigurationManager.AppSettings("SupabaseUrl")
        Dim apiKey As String = ConfigurationManager.AppSettings("SupabaseKey")

        Using client As New HttpClient()
            client.BaseAddress = New Uri(baseUrl & "/rest/v1/")
            client.DefaultRequestHeaders.Add("apikey", apiKey)
            client.DefaultRequestHeaders.Authorization = New AuthenticationHeaderValue("Bearer", apiKey)

            Dim content = New StringContent(serializer.Serialize(updatedStudent), System.Text.Encoding.UTF8, "application/json")
            client.DefaultRequestHeaders.Add("Prefer", "return=minimal")
            Dim request = New HttpRequestMessage(New HttpMethod("PATCH"), $"Students?id=eq.{id}") With {
    .Content = content
}
            Dim response = client.SendAsync(request).Result


            gvStudents.EditIndex = -1
            LoadStudents().Wait()
        End Using
    End Sub

    Protected Sub gvStudents_RowDeleting(sender As Object, e As GridViewDeleteEventArgs)
        Dim id As String = gvStudents.DataKeys(e.RowIndex).Value.ToString()

        Dim baseUrl As String = ConfigurationManager.AppSettings("SupabaseUrl")
        Dim apiKey As String = ConfigurationManager.AppSettings("SupabaseKey")

        Using client As New HttpClient()
            client.BaseAddress = New Uri(baseUrl & "/rest/v1/")
            client.DefaultRequestHeaders.Add("apikey", apiKey)
            client.DefaultRequestHeaders.Authorization = New AuthenticationHeaderValue("Bearer", apiKey)

            Dim response = client.DeleteAsync($"Students?id=eq.{id}").Result
            LoadStudents().Wait()
        End Using
    End Sub

    Private Sub ClearForm()
        txtFirstName.Text = ""
        txtLastName.Text = ""
        txtEmail.Text = ""
        txtEnrollDate.Text = ""
    End Sub
End Class
