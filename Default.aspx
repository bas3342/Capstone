<%@ Page Language="VB" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Default.aspx.vb" Inherits="StudentInformationSystem._Default" %>



<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
    <style>
        .hero-section {
            background-image: url('https://images.unsplash.com/photo-1596495577886-d920f1fb7238?ixlib=rb-4.0.3&auto=format&fit=crop&w=1600&q=80');
            background-size: cover;
            background-position: center;
            height: 80vh;
            color: white;
            display: flex;
            align-items: center;
            justify-content: center;
            text-align: center;
            border-radius: 10px;
        }

        .hero-text h1 {
            font-size: 3rem;
            font-weight: bold;
            text-shadow: 2px 2px 6px rgba(0,0,0,0.5);
        }

        .hero-text p {
            font-size: 1.25rem;
            text-shadow: 1px 1px 4px rgba(0,0,0,0.4);
        }
    </style>

    <div class="container mt-4">
        <div class="hero-section">
            <div class="hero-text">
                <h1>Student Information System</h1>
                <p>Manage your students quickly and efficiently</p>

                <!-- ✅ NEW Manage Students BUTTON -->
                <div class="mt-4">
                    <asp:HyperLink ID="hlManageStudents" runat="server" 
                        NavigateUrl="~/ManageStudents.aspx" 
                        CssClass="btn btn-light btn-lg">
                        Go to Manage Students
                    </asp:HyperLink>
                </div>

            </div>
        </div>
    </div>
</asp:Content>



