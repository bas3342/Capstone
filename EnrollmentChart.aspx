<%@ Page Title="Enrollment Chart" Language="VB" MasterPageFile="~/Site.Master"
    AutoEventWireup="true" CodeBehind="EnrollmentChart.aspx.vb"
    Inherits="StudentInformationSystem.EnrollmentChart" Async="true" %>


<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <h2>📈 Course Enrollment Chart</h2>
    <canvas id="enrollmentChart" width="800" height="400"></canvas>
    <script src="https://cdn.jsdelivr.net/npm/chart.js"></script>
    <script runat="server">
        Protected Sub Page_Load(sender As Object, e As EventArgs)
            ' Data will be injected into JS in the code-behind
        End Sub
    </script>
</asp:Content>

