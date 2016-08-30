<%@ Page Title="Pokemon Go Status" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true"
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
            //Responsive();
        }

        $(document).ready(function () {
            //var table = $('.table');
            //var div = table.parent().addClass('table-responsive-vertical shadow-z-1');
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

        function RenameConfirm() {
            if (confirm("Only rename 10 pokemons at one time, pokemons already with a nickname will not be renamed."))
                return true;
            else return false;
        }

        function TransferConfirm() {
            if (confirm("Only transfer 10 pokemons at one time !"))
                return true;
            else return false;
        }
    </script>
    <script type="text/javascript" src="http://ajax.googleapis.com/ajax/libs/jquery/1.8.3/jquery.min.js"></script>
    <script type="text/javascript">
        $("[id*=chkHeader]").live("click", function () {
            var chkHeader = $(this);
            var grid = $(this).closest("table");
            $("input[type=checkbox]", grid).each(function () {
                if (chkHeader.is(":checked")) {
                    $(this).attr("checked", "checked");
                } else {
                    $(this).removeAttr("checked");
                }
            });
        });
        $("[id*=chkRow]").live("click", function () {
            var grid = $(this).closest("table");
            var chkHeader = $("[id*=chkHeader]", grid);
            if (!$(this).is(":checked")) {
                chkHeader.removeAttr("checked");
            } else {
                if ($("[id*=chkRow]", grid).length == $("[id*=chkRow]:checked", grid).length) {
                    chkHeader.attr("checked", "checked");
                }
            }
        });
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
                <div class="growl growl-static" style="width: 100%; display: none;" runat="server"
                    id="msg">
                    <div id="msg2" runat="server" class="alert alert-dark alert-dismissable" role="alert">
                        <button type="button" class="close" data-dismiss="alert" aria-label="Close">
                            <span aria-hidden="true">×</span>
                        </button>
                        <asp:Label ID="lblMessage" runat="server"></asp:Label>
                    </div>
                </div>
                <asp:Label ID="lab" runat="server" Font-Size="X-Large" Text="" ForeColor="#CC0000"></asp:Label>
                <asp:Label ID="lblLoginMethod" runat="server" Text="" Visible="false"></asp:Label>
                <asp:Label ID="lblFilter" runat="server" Text="" Visible="false"></asp:Label>
                <div id="divMain">
                    <% if (lblLoginMethod.Text.Length > 0) %>
                    <% { %>
                    <div class="panel panel-default">
                        <div class="panel-body">
                            <div class="col-sm-9">
                                <div class="form-inline">
                                    <asp:TextBox ID="txtFilter" runat="server" CssClass="form-control" placeholder="Pokemon Name / CP"></asp:TextBox>
                                    <asp:LinkButton ID="btnFilter" runat="server" CssClass="btn btn-success-outline"
                                        OnClick="btnFilter_Click"><span class="icon icon-cycle"></span> Go!</asp:LinkButton>
                                    <asp:LinkButton ID="btnRename" runat="server" CssClass="btn btn-primary-outline"
                                        OnClick="btnRename_Click" OnClientClick="return RenameConfirm();"><span class="icon icon-pencil"></span> Rename</asp:LinkButton>
                                </div>
                            </div>
                            <div class="col-sm-3 text-right">
                                <asp:LinkButton ID="btnTransfer" runat="server" CssClass="btn btn-danger-outline"
                                    OnClick="btnTransfer_Click" OnClientClick="return TransferConfirm();"><span class="icon icon-cross"></span> Transfer</asp:LinkButton>
                            </div>
                        </div>
                    </div>
                    <div class="table-responsive">
                        <asp:GridView ID="ASPxGridView1" runat="server" AutoGenerateColumns="false" CssClass="table table-hover table-condensed"
                            GridLines="None" AllowSorting="true" OnSorting="ASPxGridView1_Sorting" OnRowDataBound="ASPxGridView1_RowDataBound"
                            DataKeyNames="PID">
                            <Columns>
                                <%--<asp:BoundField HeaderText="ID" DataField="ID" SortExpression="ID" />--%>
                                <asp:TemplateField>
                                    <HeaderTemplate>
                                        <asp:CheckBox ID="chkHeader" runat="server" />
                                    </HeaderTemplate>
                                    <ItemTemplate>
                                        <asp:CheckBox ID="chkRow" runat="server" />
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <%--<asp:BoundField HeaderText="PokemonCN" DataField="PokemonCN" SortExpression="PokemonCN" />--%>
                                <asp:BoundField HeaderText="Pokemon" DataField="Pokemon" SortExpression="Pokemon" />
                                <asp:BoundField HeaderText="NickName" DataField="NickName" SortExpression="NickName" />
                                <asp:BoundField HeaderText="CreationTime" DataField="CreationTime" SortExpression="CreationTime" />
                                <asp:BoundField HeaderText="LV" DataField="LV" SortExpression="LV" />
                                <asp:BoundField HeaderText="CP" DataField="CP" SortExpression="CP" />
                                <asp:BoundField HeaderText="MaxCP" DataField="MaxCP" SortExpression="MaxCP" />
                                <asp:BoundField HeaderText="ATK" DataField="ATK" SortExpression="ATK" />
                                <asp:BoundField HeaderText="DEF" DataField="DEF" SortExpression="DEF" />
                                <asp:BoundField HeaderText="STA" DataField="STA" SortExpression="STA" />
                                <asp:BoundField HeaderText="Move 1" DataField="Move1" SortExpression="Move1" />
                                <asp:BoundField HeaderText="Move 2" DataField="Move2" SortExpression="Move2" />
                                <asp:BoundField HeaderText="CPPerfection" DataField="CPPerfection" DataFormatString="{0:F2}%"
                                    SortExpression="CPPerfection" />
                                <asp:BoundField HeaderText="IVPerfection" DataField="IVPerfection" DataFormatString="{0:F2}%"
                                    SortExpression="IVPerfection" />
                            </Columns>
                        </asp:GridView>
                    </div>
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
