using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;


namespace QuerYourFile
{
    public partial class Default : System.Web.UI.Page
    {

        ManipulaArquivo manipulaArquivo = new ManipulaArquivo();
        IntegracaoBaseDeDados integracaoBaseDeDados = new IntegracaoBaseDeDados();
        TabelaHTML tabelaHtml = new TabelaHTML();
        Regex regex = new Regex(@"\b(?i)(DROP|CREATE|ALTER|MODIFY|GRANT|REVOKE|REBUILD|REORGANIZE|RECOMPILE)");

        protected void Page_Load(object sender, EventArgs e)
        {
            lblError.Text = "";
            StatusLabel.Text = "";
        }

        protected void BtnSalva_Click(Object sender, EventArgs e) {
            char delimitador;
            Match match = regex.Match(editorSql.Value);
            string caminhoCompletoArquivo = Server.MapPath("~/DownloadDeArquivos/");

            try {
                if (editorSql.Value != "") {
                    string nomeArquivo = inp_nomeArquivo.Value;
                    string extensaoArquivo = "." + listaTipoArquivoSaida.SelectedValue;
                    nomeArquivo += extensaoArquivo;
                    caminhoCompletoArquivo += nomeArquivo;
                    string query = editorSql.Value.ToString();
                    DataTable dataTable = integracaoBaseDeDados.RetornaDadosDaConsulta(editorSql.Value.ToString());
                    string contentType;

                    if (listaTipoArquivoSaida.SelectedValue == "xlsx") {
                        manipulaArquivo.SalvaArquivoXLSX(caminhoCompletoArquivo, dataTable);
                        contentType = "application/vnd.ms-excel";
                    } else if (listaTipoArquivoSaida.SelectedValue == "csv") {
                        contentType = "text/csv";                        
                        manipulaArquivo.SalvaArquivoTxt(caminhoCompletoArquivo, extensaoArquivo, dataTable);

                    } else {
                        if (listaDelimitadorSaida.SelectedValue == "virgula") {
                            delimitador = ',';
                        } else if (listaDelimitadorSaida.SelectedValue == "ponto_virgula") {
                            delimitador = ';';
                        } else {
                            delimitador = '\t';
                        }
                        contentType = "text/plain";

                        manipulaArquivo.SalvaArquivoTxt(caminhoCompletoArquivo, extensaoArquivo, delimitador, dataTable);
                    }

                    System.Web.HttpResponse response = System.Web.HttpContext.Current.Response;
                    FileInfo infoArquivo = new FileInfo(caminhoCompletoArquivo);

                    Response.Clear();
                    Response.ClearHeaders();
                    Response.ClearContent();
                    Response.AddHeader("Content-Disposition", "attachment; filename=" + infoArquivo.Name);
                    Response.AddHeader("Content-Length", infoArquivo.Length.ToString());
                    Response.ContentType = contentType;
                    Response.Flush();
                    Response.TransmitFile(infoArquivo.FullName);
                    Response.Flush();
                    Response.End();

                }
                else {
                    lblError.ForeColor = System.Drawing.Color.Red;
                    lblError.Text = "Digite uma query valida no editor no editor!";
                }
            }
            catch (SqlException ex) {
                StringBuilder mensagemDeErro = new StringBuilder();
                for (int i = 0; i < ex.Errors.Count; i++) {
                    mensagemDeErro.Append("Message: " + ex.Errors[i].Message);
                }
                lblError.ForeColor = System.Drawing.Color.Red;
                lblError.Text = mensagemDeErro.ToString();
            }
            finally {
                manipulaArquivo.ExcluiArquivo(caminhoCompletoArquivo);
            }
        }

        protected void BtnLimpa_Click(object sender, EventArgs e) {
            try {
                integracaoBaseDeDados.LimpaBase((string)(Session["ArquivoNome"]));
            }
            catch (SqlException ex) {
                StringBuilder mensagemDeErro = new StringBuilder();
                for (int i = 0; i < ex.Errors.Count; i++) {
                    mensagemDeErro.Append("Message: " + ex.Errors[i].Message);
                }
                lblError.ForeColor = System.Drawing.Color.Red;
                lblError.Text = mensagemDeErro.ToString();
            }

            editorSql.Value = "";
            lblError.Text = "";
            lblError.Visible = false;
            divHtml.InnerHtml = "";
            StatusLabel.Text = "";
            inp_nomeArquivo.Value = "";
        }

        protected void BtnVai_Click(Object sender, EventArgs e) {
            Match match = regex.Match(editorSql.Value);
            try {
                if (editorSql.Value != "") {
                    DataTable dataTable = new DataTable();                    
                    dataTable = integracaoBaseDeDados.RetornaDadosDaConsulta(editorSql.Value.ToString());
                    divHtml.InnerHtml = tabelaHtml.ExportaTabelaHtml(dataTable);
                }
            }
            catch (SqlException ex) {
                StringBuilder mensagemDeErro = new StringBuilder();
                for (int i = 0; i < ex.Errors.Count; i++) {
                    mensagemDeErro.Append("Message: " + ex.Errors[i].Message);
                }
                lblError.ForeColor = System.Drawing.Color.Red;
                lblError.Text = mensagemDeErro.ToString();
            }
        }

        protected void BtnUploadArquivo_Click(Object sender, EventArgs e) {
            try {
                if (UploadDeArquivos.HasFile) {
                    try {
                        string arquivoNomeExtensao = Path.GetFileName(UploadDeArquivos.FileName);
                        string arquivoLocal = Server.MapPath("~/UploadDeArquivos/") + arquivoNomeExtensao;
                        string arquivoNome = Path.GetFileNameWithoutExtension(arquivoLocal);
                        string arquivoExtensao = Path.GetExtension(arquivoLocal);
                        string identificadorTabela = Guid.NewGuid().ToString();

                        // Alterado a utilização da classe static Arquivo para sessão do usuário:
                        Session["ArquivoLocal"] = arquivoLocal;
                        Session["ArquivoNome"] = arquivoNome;
                        Session["IdentificadorTabela"] = identificadorTabela;

                        UploadDeArquivos.SaveAs(arquivoLocal);
                        string valorDelimitador = sel_delimitadorEntrada.Value;
                        string valorTemCabecalho = sel_TemCabecalho.Value;
                        char delimitador;

                        FileInfo infoDoArquivo = new FileInfo(arquivoLocal);
                        float tamanhoArquivo = (infoDoArquivo.Length / 1024f) / 1024f;

                        if (tamanhoArquivo > 10) {
                            lblError.ForeColor = System.Drawing.Color.Red;
                            lblError.Text = "Só são aceitos arquivos até 10 MB. Verifique o arquivo!";
                        }
                        else {
                            if (valorDelimitador == "virgula") {
                                delimitador = ',';
                            } else if (valorDelimitador == "pontovirgula") {
                                delimitador = ';';
                            } else {
                                delimitador = '\t';
                            }

                            if (manipulaArquivo.ValidaDelimitador(arquivoLocal, delimitador)) {
                                manipulaArquivo.CriaTabelaInsereArquivo(arquivoLocal, arquivoNome, arquivoExtensao, delimitador, valorTemCabecalho);
                            } else {
                                throw new Exception("Delimitador especificado inválido!");
                            }

                            StatusLabel.ForeColor = System.Drawing.Color.Green;
                            StatusLabel.Text = "O arquivo [ " + arquivoNome + " ] foi carregado!";
                            manipulaArquivo.ExcluiArquivo(arquivoLocal);
                        }
                    }
                    catch (Exception ex) {
                        StatusLabel.Text = "O arquivo não foi carregado! Erro: " + ex.Message;
                    }
                }
            }
            catch (Exception ex) {
                throw ex;
            }
        }

        protected void listaTipoArquivoSaida_Selected(object sender, EventArgs e) {
            if (listaTipoArquivoSaida.SelectedValue == "xlsx" || listaTipoArquivoSaida.SelectedValue == "csv") {
                listaDelimitadorSaida.Enabled = false; ;
            }
            else {
                listaDelimitadorSaida.Enabled = true;
            }
        }
    }
}