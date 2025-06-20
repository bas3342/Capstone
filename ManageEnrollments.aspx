<%@ Page Title="Manage Enrollments"
    Language="VB"
    Async="true"
    AutoEventWireup="true"
    MasterPageFile="~/Site.Master"
    CodeBehind="ManageEnrollments.aspx.vb"
    Inherits="StudentInformationSystem.ManageEnrollments" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <div class="container mt-4">
        <h2>Manage Enrollments</h2>

        <asp:Label ID="lblMessage" runat="server" CssClass="text-danger mt-2 d-block" />

        <asp:GridView ID="gvEnrollments" runat="server"
                      AutoGenerateColumns="False"
                      CssClass="table table-bordered"
                      DataKeyNames="enrollment_id"
                      OnRowDeleting="gvEnrollments_RowDeleting">
            <Columns>
                <asp:BoundField DataField="enrollment_id" HeaderText="ID" />
                <asp:BoundField DataField="student_name" HeaderText="Student" />
                <asp:BoundField DataField="course_name" HeaderText="Course" />
                <asp:BoundField DataField="enrollment_date" HeaderText="Date" />
                <asp:CommandField ShowDeleteButton="True" />
            </Columns>
        </asp:GridView>
    </div>
</asp:Content>
