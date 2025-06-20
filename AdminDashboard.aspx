<%@ Page Title="Admin Dashboard" Language="vb" AutoEventWireup="true" MasterPageFile="~/Site.Master" CodeBehind="AdminDashboard.aspx.vb" Inherits="StudentInformationSystem.AdminDashboard" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <h2>Welcome, Admin!</h2>

    <div class="row">
        <!-- Manage Students -->
        <div class="col-md-4">
            <asp:Button ID="btnManageStudents" runat="server" Text="📋 Manage Students" CssClass="btn btn-primary btn-block mb-2" PostBackUrl="~/ManageStudents.aspx" />
        </div>

        <!-- View Courses -->
        <div class="col-md-4">
            <asp:Button ID="btnViewCourses" runat="server" Text="📚 View Courses" CssClass="btn btn-success btn-block mb-2" PostBackUrl="~/ViewCourses.aspx" />
        </div>

        <!-- Enrollment Chart -->
        <div class="col-md-4">
            <asp:Button ID="btnEnrollmentChart" runat="server" Text="📈 View Enrollment Chart" CssClass="btn btn-info btn-block mb-2" PostBackUrl="~/EnrollmentChart.aspx" />
        </div>

        <!-- Manage Courses -->
        <div class="col-md-4">
            <asp:Button ID="btnManageCourses" runat="server" Text="🛠 Manage Courses" CssClass="btn btn-warning btn-block mb-2" PostBackUrl="~/ManageCourses.aspx" />
        </div>

        <!-- Manage Enrollments -->
        <div class="col-md-4">
            <asp:Button ID="btnManageEnrollments" runat="server" Text="🧾 Manage Enrollments" CssClass="btn btn-warning btn-block mb-2" PostBackUrl="~/ManageEnrollments.aspx" />
        </div>
    </div>
</asp:Content>
