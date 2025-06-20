<%@ Page Title="Student Dashboard"
    Language="VB"
    MasterPageFile="~/Site.Master"
    AutoEventWireup="true"
    Async="true"
    CodeBehind="StudentDashboard.aspx.vb"
    Inherits="StudentInformationSystem.StudentDashboard" %>

<asp:Content ID="MainContent" ContentPlaceHolderID="MainContent" runat="server">
    <h2>📚 Enrolled Courses</h2>
    <asp:Literal ID="litEnrolledCourses" runat="server" />

    <h2 class="mt-4">➕ Enroll in a New Course</h2>
    <asp:DropDownList ID="ddlCourses" runat="server" CssClass="form-control mb-3" />
    <asp:Button ID="btnEnroll" runat="server" Text="Enroll" CssClass="btn btn-primary" OnClick="btnEnroll_Click" />
    <asp:Label ID="lblMessage" runat="server" CssClass="d-block mt-2" ForeColor="Red" />

    <hr />
    <asp:Label ID="lblDebug" runat="server" ForeColor="Gray" />
</asp:Content>
