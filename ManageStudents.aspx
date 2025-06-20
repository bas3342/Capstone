<%@ Page Title="Manage Students" Language="vb" Async="true" AutoEventWireup="true" MasterPageFile="~/Site.Master" CodeBehind="ManageStudents.aspx.vb" Inherits="StudentInformationSystem.ManageStudents" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <div class="container mt-4">
        <h2>Manage Students</h2>

        <div class="mb-3">
            <label for="txtFirstName">First Name</label>
            <asp:TextBox ID="txtFirstName" runat="server" CssClass="form-control" />
        </div>
        <div class="mb-3">
            <label for="txtLastName">Last Name</label>
            <asp:TextBox ID="txtLastName" runat="server" CssClass="form-control" />
        </div>
        <div class="mb-3">
            <label for="txtEmail">Email</label>
            <asp:TextBox ID="txtEmail" runat="server" CssClass="form-control" />
        </div>
        <div class="mb-3">
            <label for="txtEnrollDate">Enrollment Date</label>
            <asp:TextBox ID="txtEnrollDate" runat="server" CssClass="form-control" TextMode="Date" />
        </div>

        <asp:Button ID="btnCreate" runat="server" Text="Add Student" CssClass="btn btn-primary" OnClick="btnCreate_Click" />
        <asp:Label ID="lblMessage" runat="server" CssClass="text-danger mt-2 d-block" Visible="False" />

        <hr />

        <asp:GridView ID="gvStudents" runat="server"
                      AutoGenerateColumns="False"
                      CssClass="table table-striped table-bordered"
                      DataKeyNames="id"
                      OnRowEditing="gvStudents_RowEditing"
                      OnRowCancelingEdit="gvStudents_RowCancelingEdit"
                      OnRowUpdating="gvStudents_RowUpdating"
                      OnRowDeleting="gvStudents_RowDeleting">
            <Columns>
                <asp:BoundField DataField="id" HeaderText="ID" ReadOnly="True" />
                <asp:BoundField DataField="first_name" HeaderText="First Name" />
                <asp:BoundField DataField="last_name" HeaderText="Last Name" />
                <asp:BoundField DataField="email" HeaderText="Email" />
                <asp:BoundField DataField="enrollment_date" HeaderText="Enrollment Date" />
                <asp:CommandField ShowEditButton="True" ShowDeleteButton="True" />
            </Columns>
        </asp:GridView>
    </div>
</asp:Content>
