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
        
        IntegracaoBaseDeDados integracaoBaseDeDados = new IntegracaoBaseDeDados();        
        ManipulaArquivo manipulaArquivo = new ManipulaArquivo();        
        string arquivoNome = Arquivo.LocalArquivo;
        
        protected void Page_Load(object sender, EventArgs e)
        {
            
        }     
        
        protected void BtnGO_Click(object sender, EventArgs e)
        {           
            string instrucaoSql = code.Value;
            Regex regex = new Regex(@"\b(?i)(DROP|CREATE|ALTER|MODIFY|GRANT|REVOKE|REBUILD|REORGANIZE|RECOMPILE)");
            Match match = regex.Match(instrucaoSql);            

            try
            {
                if (instrucaoSql != "")
                {
                    DataTable dataTable = new DataTable();
                    string query = code.Value.ToString();
                    dataTable = integracaoBaseDeDados.RetornaDadosDaConsulta(query);
                    divHtml.InnerHtml = integracaoBaseDeDados.ExportaTabelaHtml(dataTable);
                }
            }
            catch (SqlException ex)
            {
                StringBuilder mensagemDeErro = new StringBuilder();
                for (int i = 0; i < ex.Errors.Count; i++)
                {
                    mensagemDeErro.Append("Message: " + ex.Errors[i].Message);
                }
                lblError.ForeColor = System.Drawing.Color.Red;
                lblError.Text = mensagemDeErro.ToString();
            }
        }
        
        protected void BtnSave_Click(Object sender, EventArgs e)
        {
            string sql = code.Value;
            Regex regex = new Regex(@"\b(?i)(DROP|CREATE|ALTER|MODIFY|GRANT|REVOKE|REBUILD|REORGANIZE|RECOMPILE)");
            Match match = regex.Match(sql);            

            try
            {
                if (sql != "")
                {
                    string nomeArquivo = inpNomeArquivo.Value;
                    string extensaoArquivo = ".txt";
                    nomeArquivo += extensaoArquivo;
                    DataTable dataTable = new DataTable();
                    char delimitador = ';';
                    string query = code.Value.ToString();
                    dataTable = integracaoBaseDeDados.RetornaDadosDaConsulta(query);
                    manipulaArquivo.SalvaArquivo(nomeArquivo, extensaoArquivo, delimitador, dataTable);
                }
            }
            catch (SqlException ex)
            {
                StringBuilder mensagemDeErro = new StringBuilder();
                for (int i = 0; i < ex.Errors.Count; i++)
                {
                    mensagemDeErro.Append("Message: " + ex.Errors[i].Message);
                }
                lblError.ForeColor = System.Drawing.Color.Red;
                lblError.Text = mensagemDeErro.ToString();
            }            
        }

        protected void BtnUpLoadFile_Click(Object sender, EventArgs e)
        {            
            String savePath = @"c:\temp\uploads\";
            if (UpLoad.HasFile)
            {
                string arquivo = UpLoad.FileName;
                savePath += arquivo;
                UpLoad.SaveAs(savePath);                
                Arquivo.LocalArquivo = savePath;                                                
                manipulaArquivo.CriaTabelaInsereArquivo(Arquivo.LocalArquivo, Arquivo.Nome, Arquivo.Extensao, ',');
                lblMsg.Text = "Arquivo carregado: " + arquivoNome;
                lblMsg.Visible = true;
            }
        }

        protected void BtnClear_Click(Object sender, EventArgs e)
        {
            try
            {
                integracaoBaseDeDados.LimpaBase(Arquivo.Nome);
            }
            catch (SqlException ex)
            {
                StringBuilder mensagemDeErro = new StringBuilder();
                for (int i = 0; i < ex.Errors.Count; i++)
                {
                    mensagemDeErro.Append("Message: " + ex.Errors[i].Message);
                }
                lblError.ForeColor = System.Drawing.Color.Red;
                lblError.Text = mensagemDeErro.ToString();
            }
            
            divHtml.InnerHtml = "";
            code.Value = "";
            inpNomeArquivo.Value = "";
            lblError.Text = "";
            lblMsg.Visible = false;
            lblError.Visible = false;
        }
    }
}