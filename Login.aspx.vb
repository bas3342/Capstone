Imports System.Net
Imports System.Net.Http
Imports System.Net.Http.Headers
Imports System.Web.Script.Serialization

Public Class Login1
    Inherits System.Web.UI.Page

    Protected Sub BtnLogin_Click(sender As Object, e As EventArgs)
        ServicePointManager.ServerCertificateValidationCallback = Function(_sender, cert, chain, sslPolicyErrors) True

        Dim email As String = TxtEmail.Text.Trim()
        Dim password As String = TxtPassword.Text.Trim()

        If String.IsNullOrEmpty(email) OrElse String.IsNullOrEmpty(password) Then
            LblMessage.Text = "❌ Please enter both email and password."
            LblMessage.Visible = True
            Return
        End If

        Dim baseUrl As String = ConfigurationManager.AppSettings("SupabaseUrl").Replace("/rest/v1/Students", "")
        Dim apiKey As String = ConfigurationManager.AppSettings("SupabaseKey")

        Using client As New HttpClient()
            client.BaseAddress = New Uri(baseUrl)
            client.DefaultRequestHeaders.Add("apikey", apiKey)
            client.DefaultRequestHeaders.Authorization = New AuthenticationHeaderValue("Bearer", apiKey)

            Dim supabaseResponse = client.GetAsync($"/rest/v1/Users?email=eq.{email}&select=email,password,role").Result

            If supabaseResponse.IsSuccessStatusCode Then
                Dim json = supabaseResponse.Content.ReadAsStringAsync().Result
                Dim serializer As New JavaScriptSerializer()
                Dim users = serializer.Deserialize(Of List(Of Dictionary(Of String, Object)))(json)

                If users.Count = 1 AndAlso users(0)("password").ToString() = password Then
                    Session("userEmail") = email
                    Session("userRole") = users(0)("role").ToString()

                    If Session("userRole").ToString().ToLower() = "admin" Then
                        Response.Redirect("AdminDashboard.aspx")
                    Else
                        Response.Redirect("StudentDashboard.aspx")
                    End If
                Else
                    LblMessage.Text = "❌ Invalid email or password."
                    LblMessage.Visible = True
                End If
            Else
                Dim errorContent As String = supabaseResponse.Content.ReadAsStringAsync().Result
                LblMessage.Text = "❌ Supabase error: " & supabaseResponse.StatusCode.ToString() & " - " & errorContent
                LblMessage.Visible = True
            End If
        End Using
    End Sub
End Class
