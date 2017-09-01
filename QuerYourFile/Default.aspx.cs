using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Microsoft.SqlServer.TransactSql.ScriptDom;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;


namespace QuerYourFile
{
    public partial class Default : System.Web.UI.Page
    {
        string parseQuery = null;

        protected void Page_Load(object sender, EventArgs e)
        {
            
        }     
        
        protected void BtnGO_Click(object sender, EventArgs e)
        {
            string sql = code.Value;
            Regex regex = new Regex(@"\b(?i)(DROP|CREATE|ALTER|MODIFY|GRANT|REVOKE|REBUILD|REORGANIZE|RECOMPILE)");
            Match match = regex.Match(sql);

            AcessoDados acessoDados = new AcessoDados();

            try
            {
                if (sql != "")
                {
                    if (match.Success)
                    {
                        parseQuery = null;
                    }
                    else
                    {
                        TSql100Parser parser = new TSql100Parser(false);
                        TSqlFragment fragment;

                        IList<ParseError> errors;
                        fragment = parser.Parse(new StringReader(sql), out errors);
                        if (errors != null && errors.Count > 0)
                        {
                            List<string> errorList = new List<string>();
                            foreach (var error in errors)
                            {
                                errorList.Add(error.Message);
                            }
                        }
                        else
                        {
                            DataTable dataTable = new DataTable();
                            string query = code.Value.ToString();
                            dataTable = acessoDados.GetData(query);
                            divHtml.InnerHtml = acessoDados.ExportaHtml(dataTable);
                        }
                    }
                }
            }
            catch (Exception ex)
            {                
                if(ex.ToString().Contains("Error Number:208"))
                {
                    lblError.ForeColor = System.Drawing.Color.Red;
                    lblError.Text = "Nome do arquivo inválido!";
                }
                else
                {
                    lblError.Text = ex.ToString();
                }                
            }
        }

        protected void custom_Validador(object source, ServerValidateEventArgs e)
        {
            if (parseQuery == null)
            {
                e.IsValid = true;
            }
            else
            {
                e.IsValid = false;
            }
        }

        protected void BtnClear_Click(Object sender, EventArgs e)
        {
            divHtml.InnerHtml = "";
            code.Value = "";
            lblError.Text = "";
            lblMsg.Visible = false;
            lblError.Visible = false;
        }

        protected void BtnSave_Click(Object sender, EventArgs e)
        {            
            
        }

        protected void BtnUpLoadFile_Click(Object sender, EventArgs e)
        {
            AcessoDados acessoDados = new AcessoDados();
            String savePath = @"c:\temp\uploads\";

            if (UpLoad.HasFile)
            {
                string arquivo = UpLoad.FileName;
                savePath += arquivo;
                UpLoad.SaveAs(savePath);

                acessoDados.GetDataArquivo(savePath);
                lblMsg.Visible = true;
            }
        }
    }
}