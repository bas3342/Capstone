<%@ Page Title="View Courses" Language="VB" MasterPageFile="~/Site.Master"
    AutoEventWireup="true" CodeBehind="ViewCourses.aspx.vb"
    Inherits="StudentInformationSystem.ViewCourses" %>

<asp:Content ID="MainContent" ContentPlaceHolderID="MainContent" runat="server">
    <div class="container mt-4">
        <h2>Course Enrollment</h2>

        <div class="mb-3">
            <label for="ddlStudents">Select Student:</label>
            <asp:DropDownList ID="ddlStudents" runat="server" CssClass="form-control" AppendDataBoundItems="true">
                <asp:ListItem Text="-- Select Student --" Value="" />
            </asp:DropDownList>
        </div>

        <div class="mb-3">
            <label for="ddlCourses">Select Course:</label>
            <asp:DropDownList ID="ddlCourses" runat="server" CssClass="form-control" AppendDataBoundItems="true">
                <asp:ListItem Text="-- Select Course --" Value="" />
            </asp:DropDownList>
        </div>

        <div class="mb-3">
            <label for="txtEnrollmentDate">Enrollment Date:</label>
            <asp:TextBox ID="txtEnrollmentDate" runat="server" CssClass="form-control" TextMode="Date" />
        </div>

        <asp:Button ID="btnEnroll" runat="server" Text="Enroll" CssClass="btn btn-primary" OnClick="btnEnroll_Click" />
        <br /><br />
        <asp:Label ID="lblMessage" runat="server" CssClass="text-danger" Visible="false" />

        <hr />

        <h3>Enrolled Students</h3>
        <asp:GridView ID="gvEnrollments" runat="server" CssClass="table table-bordered table-striped" AutoGenerateColumns="True" />
    </div>
</asp:Content>
