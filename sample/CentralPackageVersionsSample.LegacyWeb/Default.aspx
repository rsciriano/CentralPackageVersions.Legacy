<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="CentralPackageVersionsSample.LegacyWeb.Default" Async="true" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
<meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
    <title>Legacy web app sample</title>
</head>
<body>
    <form id="form1" runat="server">
        <h1>Legacy web app sample</h1>
        <div>
            <p>This is an example of centrally managing package versions on a legacy ASP.NET web app</p>
            <div>

                <asp:Label ID="Label1" runat="server" Text="PackageId:"></asp:Label>
                <asp:TextBox ID="tbPackageId" runat="server">NuGet.Server.Core</asp:TextBox>
                <asp:Button ID="btnGetInfo" runat="server" OnClick="btnGetInfo_Click" Text="Get info" />

            </div>
            <div><%=this.ProcessMessage %></div>
            <%if (this.PackagePublishInfo != null)
                {%>
            <ul> 
                <li>PackageId: <%=this.PackagePublishInfo.Id %></li>
                <li>Version: <%=this.PackagePublishInfo.Version %></li>
                <li>Published: <%=this.PackagePublishInfo.Published %></li>
            </ul>
            <%}%>

        </div>
    </form>
</body>
</html>
