Imports System.Net.Http
Imports System.Net.Http.Headers
Imports System.Web.Script.Serialization

Public Class EnrollmentChart
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As EventArgs) Handles Me.Load
        If Not IsPostBack Then
            InjectChartData()
        End If
    End Sub

    Private Async Sub InjectChartData()
        Dim baseUrl As String = ConfigurationManager.AppSettings("SupabaseUrl").Replace("/rest/v1/Students", "")
        Dim apiKey As String = ConfigurationManager.AppSettings("SupabaseKey")

        Using client As New HttpClient()
            client.BaseAddress = New Uri(baseUrl)
            client.DefaultRequestHeaders.Add("apikey", apiKey)
            client.DefaultRequestHeaders.Authorization = New AuthenticationHeaderValue("Bearer", apiKey)

            Dim supabaseResponse = Await client.GetAsync("/rest/v1/Enrollments?select=course_id,courses(course_name)")
            If Not supabaseResponse.IsSuccessStatusCode Then
                Response.Write("<script>alert('Failed to load enrollment data.')</script>")
                Return
            End If

            Dim json = Await supabaseResponse.Content.ReadAsStringAsync()
            Dim serializer As New JavaScriptSerializer()
            Dim enrollments = serializer.Deserialize(Of List(Of Dictionary(Of String, Object)))(json)

            ' Count enrollments per course name
            Dim courseCounts As New Dictionary(Of String, Integer)()
            For Each record In enrollments
                If record.ContainsKey("courses") Then
                    Dim course = CType(record("courses"), Dictionary(Of String, Object))
                    Dim courseName = course("course_name").ToString()
                    If courseCounts.ContainsKey(courseName) Then
                        courseCounts(courseName) += 1
                    Else
                        courseCounts(courseName) = 1
                    End If
                End If
            Next

            ' Prepare JS data arrays
            Dim labels = New List(Of String)(courseCounts.Keys)
            Dim data = New List(Of Integer)(courseCounts.Values)

            Dim jsScript = $"
                <script>
                    var ctx = document.getElementById('enrollmentChart').getContext('2d');
                    new Chart(ctx, {{
                        type: 'bar',
                        data: {{
                            labels: {New JavaScriptSerializer().Serialize(labels)},
                            datasets: [{{
                                label: 'Enrollments per Course',
                                data: {New JavaScriptSerializer().Serialize(data)},
                                backgroundColor: 'rgba(54, 162, 235, 0.6)',
                                borderColor: 'rgba(54, 162, 235, 1)',
                                borderWidth: 1
                            }}]
                        }},
                        options: {{
                            scales: {{
                                y: {{ beginAtZero: true }}
                            }}
                        }}
                    }});
                </script>
            "
            ClientScript.RegisterStartupScript(Me.GetType(), "chartScript", jsScript)
        End Using
    End Sub
End Class
