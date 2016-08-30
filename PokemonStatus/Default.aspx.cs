using PokemonGo.RocketAPI;
using PokemonGo.RocketAPI.Enums;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.Security;
using System.Text;
using POGOProtos.Networking.Responses;

namespace PokemonStatus
{
    public partial class _Default : Page
    {
        Image sortImage = new Image();
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                if (Request.Cookies["UserName"] != null && Request.Cookies["Password"] != null)
                {
                    UserName.Text = Encoding.UTF8.GetString(MachineKey.Unprotect(Convert.FromBase64String(Request.Cookies["UserName"].Value), "UserName"));
                    Password.Attributes.Add("value", Encoding.UTF8.GetString(MachineKey.Unprotect(Convert.FromBase64String(Request.Cookies["Password"].Value), "Password")));
                    chkRememberMe.Checked = true;
                }
                Session["dt"] = null;
                Session["sort"] = null;
                Session["SortExpression"] = null;
            }
            try
            {
                //Add refresh header to refresh the page 60 seconds before session timeout
                Response.AddHeader("Refresh", Convert.ToString((Session.Timeout * 60) - 60));
            }
            catch (Exception)
            {
                throw;
            }
        }

        private async Task ViewPokemons(Inventory _inventory, string filter)
        {
            DataTable t = new DataTable();
            t.Columns.Add("ID", typeof(int));
            t.Columns.Add("PokemonCN", typeof(string));
            t.Columns.Add("Pokemon", typeof(string));
            t.Columns.Add("CreationTime", typeof(DateTime));
            t.Columns.Add("LV", typeof(double));
            t.Columns.Add("CP", typeof(int));
            t.Columns.Add("MaxCP", typeof(int));
            t.Columns.Add("ATK", typeof(int));
            t.Columns.Add("DEF", typeof(int));
            t.Columns.Add("STA", typeof(int));
            t.Columns.Add("Move1", typeof(string));
            t.Columns.Add("Move2", typeof(string));
            t.Columns.Add("CPPerfection", typeof(double));
            t.Columns.Add("IVPerfection", typeof(double));
            t.Columns.Add("PID", typeof(ulong));
            t.Columns.Add("NickName", typeof(string));

            var Pokemons = await _inventory.GetPokemons();
            if (filter.Length > 0)
            {
                Pokemons = Pokemons.Where(p => p.PokemonId.ToString().ToLower().Contains(filter.ToLower()) || p.Cp.ToString() == filter);
            }
            foreach (var Pokemon in Pokemons)
            {
                DataRow newrow = t.NewRow();
                newrow["ID"] = (int)Pokemon.PokemonId;
                newrow["PokemonCN"] = (PokemonNameCN)Pokemon.PokemonId;
                newrow["Pokemon"] = Pokemon.PokemonId;
                DateTime time = DateTime.MinValue;
                TimeZoneInfo timeInfo = TimeZoneInfo.FindSystemTimeZoneById("Singapore Standard Time");
                DateTime startTime = TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1, 0, 0, 0));
                newrow["CreationTime"] = TimeZoneInfo.ConvertTime(startTime, timeInfo).AddMilliseconds(Pokemon.CreationTimeMs);
                newrow["LV"] = PokemonInfo.GetLevel(Pokemon);
                newrow["CP"] = Pokemon.Cp;
                newrow["MaxCP"] = PokemonInfo.CalculateMaxCP(Pokemon);
                newrow["ATK"] = Pokemon.IndividualAttack;
                newrow["DEF"] = Pokemon.IndividualDefense;
                newrow["STA"] = Pokemon.IndividualStamina;
                newrow["Move1"] = Pokemon.Move1.ToString().Replace("Fast", "");
                newrow["Move2"] = Pokemon.Move2;
                newrow["CPPerfection"] = Math.Round(PokemonInfo.CalculatePokemonPerfection(Pokemon), 2);
                newrow["IVPerfection"] = Math.Round(PokemonInfo.CalculatePokemonPerfection2(Pokemon), 2);
                newrow["PID"] = Pokemon.Id;
                newrow["NickName"] = Pokemon.Nickname;
                t.Rows.Add(newrow);
            }
            if (Session["sort"] != null && Session["SortExpression"] != null)
            {
                t.DefaultView.Sort = Session["sort"].ToString();
            }
            else
            {
                Session["sort"] = "CreationTime DESC";
                Session["SortExpression"] = "CreationTime";
                Session["SortDirection"] = "DESC";
                t.DefaultView.Sort = Session["sort"].ToString();
            }
            ASPxGridView1.DataSource = t;
            ASPxGridView1.DataBind();
            Session["dt"] = t;
            ASPxGridView1.HeaderRow.TableSection = TableRowSection.TableHeader;
            if (Session["sort"] != null && Session["SortExpression"] != null)
            {
                if (t.DefaultView.Sort.Contains("DESC"))
                {
                    sortImage.ImageUrl = "~/Images/asc.gif";
                }
                if (t.DefaultView.Sort.Contains("ASC"))
                {
                    sortImage.ImageUrl = "~/Images/desc.gif";
                }
                int columnIndex = 0;
                foreach (DataControlFieldHeaderCell headerCell in ASPxGridView1.HeaderRow.Cells)
                {
                    if (headerCell.ContainingField.SortExpression == Session["SortExpression"].ToString())
                    {
                        columnIndex = ASPxGridView1.HeaderRow.Cells.GetCellIndex(headerCell);
                    }
                }
                ASPxGridView1.HeaderRow.Cells[columnIndex].Controls.Add(sortImage);
            }

        }

        protected void PtcSubmit_Click(object sender, EventArgs e)
        {
            if (chkRememberMe.Checked)
            {
                Response.Cookies["UserName"].Expires = DateTime.Now.AddDays(30);
                Response.Cookies["Password"].Expires = DateTime.Now.AddDays(30);
            }
            else
            {
                Response.Cookies["UserName"].Expires = DateTime.Now.AddDays(-1);
                Response.Cookies["Password"].Expires = DateTime.Now.AddDays(-1);

            }
            Response.Cookies["UserName"].Value = Convert.ToBase64String(MachineKey.Protect(Encoding.UTF8.GetBytes(UserName.Text.Trim()), "UserName"));
            Response.Cookies["Password"].Value = Convert.ToBase64String(MachineKey.Protect(Encoding.UTF8.GetBytes(Password.Text.Trim()), "Password"));
            Response.Cookies["LoginMethod"].Value = "Ptc";
            Ptc(UserName.Text, Password.Text, "");
        }
        async void Ptc(string userName, string password, string filter)
        {

            lab.Text = "";

            Settings _clientSettings;
            Inventory _inventory;
            Client _client;
            try
            {
                _clientSettings = new Settings();
                _clientSettings.AuthType = AuthType.Ptc;
                _clientSettings.PtcUsername = userName;
                _clientSettings.PtcPassword = password;
                _client = new Client(_clientSettings);
                Session["client"] = _client;
                await _client.Login.DoPtcLogin(_clientSettings.PtcUsername, _clientSettings.PtcPassword);
                _inventory = new Inventory(_client);
                await ViewPokemons(_inventory, filter);
                lblLoginMethod.Text = "Ptc";
            }
            catch (Exception ex)
            {
                lab.Text = ex.Message;
                lblLoginMethod.Text = "";
            }
        }

        private async void Google(string userName, string password, string filter)
        {
            lab.Text = "";

            Settings _clientSettings;
            Inventory _inventory;
            Client _client;
            try
            {
                _clientSettings = new Settings();
                _clientSettings.AuthType = AuthType.Google;
                _clientSettings.PtcUsername = userName;
                _clientSettings.PtcPassword = password;
                _client = new Client(_clientSettings);
                Session["client"] = _client;
                await _client.Login.DoGoogleLogin(_clientSettings.PtcUsername, _clientSettings.PtcPassword);
                _inventory = new Inventory(_client);
                await ViewPokemons(_inventory, filter);
                lblLoginMethod.Text = "Google";
            }
            catch (Exception ex)
            {
                lab.Text = ex.Message;
                lblLoginMethod.Text = "";
            }

        }

        protected void GoogleSubmit_Click(object sender, EventArgs e)
        {
            if (chkRememberMe.Checked)
            {
                Response.Cookies["UserName"].Expires = DateTime.Now.AddDays(30);
                Response.Cookies["Password"].Expires = DateTime.Now.AddDays(30);
            }
            else
            {
                Response.Cookies["UserName"].Expires = DateTime.Now.AddDays(-1);
                Response.Cookies["Password"].Expires = DateTime.Now.AddDays(-1);

            }

            Response.Cookies["UserName"].Value = Convert.ToBase64String(MachineKey.Protect(Encoding.UTF8.GetBytes(UserName.Text.Trim()), "UserName"));
            Response.Cookies["Password"].Value = Convert.ToBase64String(MachineKey.Protect(Encoding.UTF8.GetBytes(Password.Text.Trim()), "Password"));

            Google(UserName.Text, Password.Text, "");
        }

        protected void btnFilter_Click(object sender, EventArgs e)
        {
            if (lblLoginMethod.Text == "Ptc" && UserName.Text.Length > 0 && Password.Text.Length > 0)
            {
                Ptc(UserName.Text, Password.Text, txtFilter.Text);
            }
            if (lblLoginMethod.Text == "Google" && UserName.Text.Length > 0 && Password.Text.Length > 0)
            {
                Google(UserName.Text, Password.Text, txtFilter.Text);
            }
            lblFilter.Text = txtFilter.Text;
            txtFilter.Text = "";
            txtFilter.Focus();
            //DataTable dt = Session["dt"] as DataTable;
            //string filter = txtFilter.Text;
            //string expression = "Pokemon like '%" + filter + "%'";
            //dt.DefaultView.RowFilter = expression;
            //ASPxGridView1.DataSource = dt;
            //ASPxGridView1.DataBind();
            //ASPxGridView1.Sort("CreationTime", SortDirection.Descending); ScriptManager.RegisterStartupScript(this, this.GetType(), "tmp2", " var t2 = document.getElementById('" + txtFilter.ClientID + "');t2.focus();t2.value = t2.value;", true);
            //Session["dt"] = dt;
        }
        
        protected void ASPxGridView1_Sorting(object sender, GridViewSortEventArgs e)
        {
            if (Session["dt"] != null)
            {
                DataTable t = Session["dt"] as DataTable;
                if (t != null)
                {
                    t.DefaultView.Sort = e.SortExpression + " " + GetSortDirection(e.SortExpression);
                    ASPxGridView1.DataSource = Session["dt"];
                    ASPxGridView1.DataBind();
                    Session["sort"] = t.DefaultView.Sort;
                    Session["SortExpression"] = e.SortExpression;
                }
                if (t.DefaultView.Sort.Contains("DESC"))
                {
                    sortImage.ImageUrl = "~/Images/asc.gif";
                }
                if (t.DefaultView.Sort.Contains("ASC"))
                {
                    sortImage.ImageUrl = "~/Images/desc.gif";
                }
                int columnIndex = 0;
                foreach (DataControlFieldHeaderCell headerCell in ASPxGridView1.HeaderRow.Cells)
                {
                    if (headerCell.ContainingField.SortExpression == e.SortExpression)
                    {
                        columnIndex = ASPxGridView1.HeaderRow.Cells.GetCellIndex(headerCell);
                    }
                }
                ASPxGridView1.HeaderRow.Cells[columnIndex].Controls.Add(sortImage);
            }
            else
            {
                if (UserName.Text.Length > 0 && Password.Text.Length > 0)
                {

                    if (lblLoginMethod.Text == "Ptc")
                    {
                        Ptc(UserName.Text, Password.Text, txtFilter.Text);
                    }
                    if (lblLoginMethod.Text == "Google")
                    {
                        Google(UserName.Text, Password.Text, txtFilter.Text);
                    }
                }
                else
                {
                    Page.Response.Redirect("Default.aspx");
                }
            }
        }
        private string GetSortDirection(string column)
        {
            string sortDirection = "DESC";
            string sortExpression = Session["SortExpression"] as string;
            if (sortExpression != null)
            {
                if (sortExpression == column)
                {
                    string lastDirection = Session["SortDirection"] as string;
                    if ((lastDirection != null) && (lastDirection == "DESC"))
                    {
                        sortDirection = "ASC";
                    }
                }
            }
            Session["SortDirection"] = sortDirection;
            Session["SortExpression"] = column;
            return sortDirection;
        }

        protected void ASPxGridView1_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            //if (e.Row.RowType == DataControlRowType.DataRow)
            //{
            //    TableCellCollection cell = e.Row.Cells;
            //    cell[1].Attributes.Add("data-title", "PokemonCN");
            //    cell[2].Attributes.Add("data-title", "Pokemon");
            //    cell[3].Attributes.Add("data-title", "CreationTime");
            //    cell[4].Attributes.Add("data-title", "LV");
            //    cell[5].Attributes.Add("data-title", "CP");
            //    cell[6].Attributes.Add("data-title", "MaxCP");
            //    cell[7].Attributes.Add("data-title", "ATK");
            //    cell[8].Attributes.Add("data-title", "DEF");
            //    cell[9].Attributes.Add("data-title", "STA");
            //    cell[10].Attributes.Add("data-title", "Move 1");
            //    cell[11].Attributes.Add("data-title", "Move 2");
            //    cell[12].Attributes.Add("data-title", "CPPerfection");
            //    cell[13].Attributes.Add("data-title", "IVPerfection");
            //    cell[14].Attributes.Add("data-title", "PID");
            //    cell[15].Attributes.Add("data-title", "NickName");
            //}
        }

        protected async void btnRename_Click(object sender, EventArgs e)
        {
            if (Session["client"] != null)
            {
                string filter = lblFilter.Text;
                Inventory inventory = new Inventory((Client)Session["client"]);
                int failCount = 0;
                foreach (GridViewRow row in ASPxGridView1.Rows)
                {
                    if (row.RowType == DataControlRowType.DataRow)
                    {
                        CheckBox chkRow = (row.Cells[0].FindControl("chkRow") as CheckBox);
                        if (chkRow.Checked)
                        {
                            int nickNameIndex = GetColumnIndexByName(row, "NickName");
                            if (row.Cells[nickNameIndex].Text.Length == 0 || row.Cells[nickNameIndex].Text == "&nbsp;")
                            {
                                int pokemonIndex = GetColumnIndexByName(row, "Pokemon");
                                int ivIndex = GetColumnIndexByName(row, "IVPerfection");
                                int rowIndex = row.RowIndex;
                                ulong pid = Convert.ToUInt64(ASPxGridView1.DataKeys[rowIndex].Values["PID"]);
                                string pokemon = row.Cells[pokemonIndex].Text;
                                string ivPerc = row.Cells[ivIndex].Text.Replace("%", "");
                                if (pokemon.Length > 8)
                                {
                                    pokemon = pokemon.Substring(0, 8);
                                }
                                string nickName = (pokemon + ivPerc).Length > 12 ? (pokemon + ivPerc).Substring(0, 12) : (pokemon + ivPerc);
                                try
                                {
                                    await inventory.rename(pid, nickName);
                                }
                                catch (Exception ex)
                                {
                                    failCount++;
                                    showMsg("danger", ex.ToString());
                                }
                            }
                        }
                    }
                    else
                    {
                        string rowtype = row.RowType.ToString();
                    }
                }
                showMsg("success", "Pokemon renamed. " + failCount.ToString() + " failed to rename.");
                if (lblLoginMethod.Text == "Ptc" && UserName.Text.Length > 0 && Password.Text.Length > 0)
                {
                    Ptc(UserName.Text, Password.Text, filter);
                }
                if (lblLoginMethod.Text == "Google" && UserName.Text.Length > 0 && Password.Text.Length > 0)
                {
                    Google(UserName.Text, Password.Text, filter);
                }
                txtFilter.Text = "";
                txtFilter.Focus();
            }
            else
            {
                showMsg("danger", "Session expired, please login again");
            }
        }

        protected async void btnTransfer_Click(object sender, EventArgs e)
        {
            if (Session["client"] != null)
            {
                string filter = lblFilter.Text;
                Inventory inventory = new Inventory((Client)Session["client"]);
                int failCount = 0;
                foreach (GridViewRow row in ASPxGridView1.Rows)
                {
                    if (row.RowType == DataControlRowType.DataRow)
                    {
                        CheckBox chkRow = (row.Cells[0].FindControl("chkRow") as CheckBox);
                        if (chkRow.Checked)
                        {
                            int rowIndex = row.RowIndex;
                            ulong pid = Convert.ToUInt64(ASPxGridView1.DataKeys[rowIndex].Values["PID"]);
                            try
                            {
                                await inventory.transfer(pid);
                            }
                            catch (Exception ex)
                            {
                                failCount++;
                                showMsg("danger", ex.ToString());
                            }
                        }
                    }
                }
                showMsg("success", "Pokemon transfered. " + failCount.ToString() + " failed to transfer.");
                if (lblLoginMethod.Text == "Ptc" && UserName.Text.Length > 0 && Password.Text.Length > 0)
                {
                    Ptc(UserName.Text, Password.Text, filter);
                }
                if (lblLoginMethod.Text == "Google" && UserName.Text.Length > 0 && Password.Text.Length > 0)
                {
                    Google(UserName.Text, Password.Text, filter);
                }
                txtFilter.Text = "";
                txtFilter.Focus();
            }
            else
            {
                showMsg("danger", "Session expired, please login again.");
            }
        }

        protected void showMsg(string color, string text)
        {
            msg.Attributes.Add("style", "display: block; width: 100%;");
            msg2.Attributes.Add("class", "alert alert-dismissable alert-" + color);
            lblMessage.Text = text;
        }
        protected void hideMsg()
        {
            msg.Attributes.Add("style", "display: none");
        }
        protected int GetColumnIndexByName(GridViewRow row, string columnName)
        {
            int columnIndex = 0;
            foreach (DataControlFieldCell cell in row.Cells)
            {
                if (cell.ContainingField is BoundField)
                    if (((BoundField)cell.ContainingField).DataField.Equals(columnName))
                        break;
                columnIndex++; // keep adding 1 while we don't have the correct name
            }
            return columnIndex;
        }
    }
}
