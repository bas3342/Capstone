<%@ Page Title="Login" Language="vb" AutoEventWireup="false" MasterPageFile="~/Site.Master" CodeBehind="Login.aspx.vb" Inherits="StudentInformationSystem.Login1" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <div class="container mt-5" style="max-width: 400px;">
        <h3 class="mb-4">Login</h3>

        <asp:Label ID="LblMessage" runat="server" CssClass="text-danger" Visible="false" />

        <div class="form-group">
            <label>Email</label>
            <asp:TextBox ID="TxtEmail" runat="server" CssClass="form-control" />
        </div>

        <div class="form-group mt-3">
            <label>Password</label>
            <asp:TextBox ID="TxtPassword" runat="server" TextMode="Password" CssClass="form-control" />
        </div>

        <asp:Button ID="BtnLogin" runat="server" Text="Login" CssClass="btn btn-primary mt-4" OnClick="BtnLogin_Click" />
    </div>
</asp:Content>
