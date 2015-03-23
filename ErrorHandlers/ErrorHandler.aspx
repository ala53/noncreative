<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ErrorHandler.aspx.cs" Inherits="NonCreative.Error_Handlers.ErrorHandler" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Fragranx: An error has occurred</title>
</head>
<body style="background: #BBB; font-family:sans-serif">
    <form id="form1" runat="server">
        <div>
            <div id="head">
                <h1>Uh oh...</h1>
                <h3 id="ErrorMessage" runat="server"></h3>
            </div>
            <h4>Click <a href="<%= ResolveUrl("~/") %>" runat="server" >here</a> to go home.</h4>
            <h4>Below are the details of ther error if you would like to submit a <a href="https://fragranx.serviceapps.net/submitBugReport.aspx?error=crash">bug report</a>.</h4>
            <div id="Details" runat="server">

            </div>
        </div>
    </form>
</body>
</html>
