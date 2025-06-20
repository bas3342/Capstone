<%@ Page Title="Manage Courses" Language="VB" MasterPageFile="~/Site.Master" AutoEventWireup="true" Async="true" CodeBehind="ManageCourses.aspx.vb" Inherits="StudentInformationSystem.ManageCourses" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <div class="container mt-4">
        <h2>Manage Courses</h2>

        <div class="mb-3">
            <label>Course Name</label>
            <asp:TextBox ID="txtCourseName" runat="server" CssClass="form-control" />
        </div>
        <div class="mb-3">
            <label>ECTS</label>
            <asp:TextBox ID="txtEcts" runat="server" CssClass="form-control" />
        </div>
        <div class="mb-3">
            <label>Instructor</label>
            <asp:TextBox ID="txtInstructor" runat="server" CssClass="form-control" />
        </div>

        <asp:Button ID="btnAddCourse" runat="server" Text="Add Course" CssClass="btn btn-primary" OnClick="btnAddCourse_Click" />
        <asp:Label ID="lblMessage" runat="server" CssClass="text-danger mt-2 d-block" />

        <hr />

        <asp:GridView ID="gvCourses" runat="server"
                      AutoGenerateColumns="true"
                      CssClass="table table-striped table-bordered"
                      OnRowDeleting="gvCourses_RowDeleting"
                      DataKeyNames="course_id">
            <Columns>
                <asp:CommandField ShowDeleteButton="true" />
            </Columns>
        </asp:GridView>
    </div>
</asp:Content>
