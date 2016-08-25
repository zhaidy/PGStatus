<%@ Page Title="Home Page" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true"
    CodeBehind="Default.aspx.cs" Inherits="PokemonStatus._Default" Async="true" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
    <script type="text/javascript">
        var prm = Sys.WebForms.PageRequestManager.getInstance();
        // Add initializeRequest and endRequest
        prm.add_initializeRequest(prm_InitializeRequest);
        prm.add_endRequest(prm_EndRequest);

        // Called when async postback begins
        function prm_InitializeRequest(sender, args) {
            $("<div class='wrapper'><div class='ball ball-1'></div><div class='ball ball-2'></div><div class='ball ball-3'></div></div>").appendTo(document.body);
        }

        // Called when async postback ends
        function prm_EndRequest(sender, args) {
            $('.wrapper').remove();
            Scroll();
            Responsive();
        }

        $(document).ready(function () {
            var table = $('.table');
            var div = table.parent().addClass('table-responsive-vertical shadow-z-1');
        });

        function Responsive() {
            var table = $('.table');
            var div = table.parent().addClass('table-responsive-vertical shadow-z-1');
        }

        function Scroll() {
            $('html,body').animate({
                scrollTop: $("#divMain").offset().top - 70
            }, 'slow');
        }
    </script>
    <asp:Panel ID="LoginPanel" runat="server">
        <div class="panel panel-default panel-profile" align="center">
            <div class="panel-heading" style="background-image: url(Images/718257.jpg); background-position: center;
                position: relative; height: 0; padding-bottom: 40%;">
            </div>
            <div class="panel-body">
                <div class="form-inline">
                    <div class="form-group">
                        <asp:TextBox ID="UserName" runat="server" Width="270px" CssClass="form-control"></asp:TextBox>
                    </div>
                    <div class="form-group">
                        <asp:TextBox ID="Password" runat="server" Width="270px" TextMode="Password" CssClass="form-control"></asp:TextBox>
                    </div>
                    <div class="checkbox">
                        <label>
                            <asp:CheckBox ID="chkRememberMe" runat="server" />
                            Remember me</label>
                    </div>
                    <asp:Button ID="GoogleSubmit" runat="server" Text="Google Login" OnClick="GoogleSubmit_Click"
                        CssClass="btn btn-success"></asp:Button>
                    <asp:Button ID="PtcSubmit" runat="server" Text="Ptc Login" OnClick="PtcSubmit_Click"
                        CssClass="btn btn-primary"></asp:Button>
                </div>
            </div>
        </div>
    </asp:Panel>
    <asp:Panel ID="bodyPanel" runat="server" DefaultButton="btnFilter">
        <asp:UpdatePanel ID="upTable" runat="server">
            <ContentTemplate>
                <asp:Label ID="lab" runat="server" Font-Size="X-Large" Text="" ForeColor="#CC0000"></asp:Label>
                <asp:Label ID="lblLoginMethod" runat="server" Text="" Visible="false"></asp:Label>
                <div class="container-fluid" id="divMain">
                    <% if (lblLoginMethod.Text.Length > 0) %>
                    <% { %>
                    <div class="panel panel-default">
                        <div class="panel-body">
                            <div class="form-inline">
                                <asp:TextBox ID="txtFilter" runat="server" CssClass="form-control" placeholder="Pokemon Name / CP"></asp:TextBox>
                                <asp:LinkButton ID="btnFilter" runat="server" CssClass="btn btn-primary-outline"
                                    OnClick="btnFilter_Click"><span class="icon icon-cycle"></span> Go!</asp:LinkButton>
                            </div>
                        </div>
                    </div>
                    <asp:GridView ID="ASPxGridView1" runat="server" AutoGenerateColumns="false" CssClass="table table-hover table-mc-light-blue"
                        GridLines="None" AllowSorting="true" OnSorting="ASPxGridView1_Sorting" OnRowDataBound="ASPxGridView1_RowDataBound">
                        <Columns>
                            <asp:BoundField HeaderText="ID" DataField="ID" SortExpression="ID" />
                            <asp:BoundField HeaderText="PokemonCN" DataField="PokemonCN" SortExpression="PokemonCN" />
                            <asp:BoundField HeaderText="Pokemon" DataField="Pokemon" SortExpression="Pokemon" />
                            <asp:BoundField HeaderText="CreationTime" DataField="CreationTime" SortExpression="CreationTime" />
                            <asp:BoundField HeaderText="LV" DataField="LV" SortExpression="LV" />
                            <asp:BoundField HeaderText="CP" DataField="CP" SortExpression="CP" />
                            <asp:BoundField HeaderText="MaxCP" DataField="MaxCP" SortExpression="MaxCP" />
                            <asp:BoundField HeaderText="ATK" DataField="ATK" SortExpression="ATK" />
                            <asp:BoundField HeaderText="DEF" DataField="DEF" SortExpression="DEF" />
                            <asp:BoundField HeaderText="STA" DataField="STA" SortExpression="STA" />
                            <asp:BoundField HeaderText="CPPerfection" DataField="CPPerfection" DataFormatString="{0:F2}%"
                                SortExpression="CPPerfection" />
                            <asp:BoundField HeaderText="IVPerfection" DataField="IVPerfection" DataFormatString="{0:F2}%"
                                SortExpression="IVPerfection" />
                        </Columns>
                    </asp:GridView>
                    <% } %>
                </div>
            </ContentTemplate>
            <Triggers>
                <asp:AsyncPostBackTrigger ControlID="GoogleSubmit" />
                <asp:AsyncPostBackTrigger ControlID="PtcSubmit" />
                <asp:AsyncPostBackTrigger ControlID="btnFilter" />
            </Triggers>
        </asp:UpdatePanel>
    </asp:Panel>
</asp:Content>
