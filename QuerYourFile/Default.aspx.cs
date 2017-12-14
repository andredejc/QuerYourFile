using System;
using System.Data;
using System.Data.SqlClient;
using System.IO;


namespace QuerYourFile {
    public partial class Default : System.Web.UI.Page
    {        
        IntegracaoBaseDeDados integracaoBaseDeDados = IntegracaoBaseDeDados.GetInstancia();
        PreparaDownloadArquivo preparaDownloadArquivo = PreparaDownloadArquivo.GetInstancia();
        GeraMensagem geraMensagem = GeraMensagem.GetInstancia();
        TabelaHTML tabelaHtml = TabelaHTML.GetInstancia();
        ManipulaArquivo manipulaArquivo = new ManipulaArquivo();

        protected void Page_Load(object sender, EventArgs e)
        {
            lblError.Text = Resources.ResourceFile.stringVazia;
            StatusLabel.Text = Resources.ResourceFile.stringVazia;
            btn_Vai.Text = Resources.ResourceFile.btn_Vai;
            btn_Limpa.Text = Resources.ResourceFile.btn_limpa;
            lbl_Cabecalho.InnerText = Resources.ResourceFile.lbl_cabecalho;
            leg_ArquivoEntrada.InnerText = Resources.ResourceFile.leg_ArquivoEntrada;
            leg_ArquivoSaida.InnerText = Resources.ResourceFile.leg_ArquivoSaida;
            lbl_DelimitadorEntrada.InnerText = Resources.ResourceFile.lbl_Delimitador;
            lbl_DelimitadorSaida.InnerText = Resources.ResourceFile.lbl_Delimitador;
            lbl_TipoSaida.InnerText = Resources.ResourceFile.lbl_TipoSaida;            
        }

        protected void BtnSalva_Click(Object sender, EventArgs e) {            
            string caminhoCompletoArquivo = Server.MapPath(Resources.ResourceFile.strDownloadArquivo);
            try {
                if (integracaoBaseDeDados.ValidaEditorSql(editorSql.Value)) {
                    string nomeArquivo = inp_nomeArquivo.Value;
                    string extensaoArquivo = Resources.ResourceFile.ponto + listaTipoArquivoSaida.SelectedValue;
                    caminhoCompletoArquivo += nomeArquivo + extensaoArquivo;                    
                    string query = editorSql.Value.ToString();
                    DataTable dataTable = integracaoBaseDeDados.RetornaDadosDaConsulta(editorSql.Value.ToString());
                    string contentType = preparaDownloadArquivo.GeraArquivoDownload(listaTipoArquivoSaida.SelectedValue, listaDelimitadorSaida.SelectedValue, caminhoCompletoArquivo, extensaoArquivo, dataTable);
                    preparaDownloadArquivo.FazDownloadArquivo(caminhoCompletoArquivo, contentType);
                } else {
                    lblError.ForeColor = System.Drawing.Color.Red;
                    lblError.Text = Resources.ResourceFile.msgQueryValida;
                }
            }
            catch (SqlException sqlException) {
                lblError.ForeColor = System.Drawing.Color.Red;
                lblError.Text = geraMensagem.GeraMensagemSql(sqlException);
            }
            finally {
                manipulaArquivo.ExcluiArquivo(caminhoCompletoArquivo);
            }
        }

        protected void BtnLimpa_Click(object sender, EventArgs e) {
            try {
                integracaoBaseDeDados.LimpaBase((string)(Session[Resources.ResourceFile.sessionArquivoNome]));                
            }
            catch (SqlException sqlException) {
                lblError.ForeColor = System.Drawing.Color.Red;
                lblError.Text = geraMensagem.GeraMensagemSql(sqlException);
            }
            finally {
                editorSql.Value = Resources.ResourceFile.stringVazia;
                lblError.Text = Resources.ResourceFile.stringVazia;
                lblError.Visible = false;
                divHtml.InnerHtml = Resources.ResourceFile.stringVazia;
                StatusLabel.Text = Resources.ResourceFile.stringVazia;
                inp_nomeArquivo.Value = Resources.ResourceFile.stringVazia;
            }            
        }

        protected void BtnVai_Click(Object sender, EventArgs e) {            
            try {
                if (integracaoBaseDeDados.ValidaEditorSql(editorSql.Value)) {
                    DataTable dataTable = new DataTable();                    
                    dataTable = integracaoBaseDeDados.RetornaDadosDaConsulta(editorSql.Value.ToString());
                    divHtml.InnerHtml = tabelaHtml.ExportaTabelaHtml(dataTable);
                } else {                    
                    lblError.ForeColor = System.Drawing.Color.Red;
                    lblError.Text = Resources.ResourceFile.msgQueryValida;
                }
            }
            catch (SqlException sqlException) {                
                lblError.ForeColor = System.Drawing.Color.Red;
                lblError.Text = geraMensagem.GeraMensagemSql(sqlException);
            }
        }

        protected void BtnUploadArquivo_Click(Object sender, EventArgs e) {
            ExecutaUploadArquivo executaUploadArquivo = new ExecutaUploadArquivo();            
            try {                
                if (UploadDeArquivos.HasFile) {
                    try {
                        string arquivoNomeExtensao = Path.GetFileName(UploadDeArquivos.FileName);
                        string arquivoLocal = Server.MapPath(Resources.ResourceFile.srtUploadArquivo) + arquivoNomeExtensao;
                        string arquivoNome = Path.GetFileNameWithoutExtension(arquivoLocal);
                        string arquivoExtensao = Path.GetExtension(arquivoLocal);                        
                        Session[Resources.ResourceFile.sessionArquivoLocal] = arquivoLocal;
                        Session[Resources.ResourceFile.sessionArquivoNome] = arquivoNome;                        
                        UploadDeArquivos.SaveAs(arquivoLocal);                        
                        bool valorTemCabecalho = sel_TemCabecalho.SelectedValue == Resources.ResourceFile.stringYes ? true : false ;                                               

                        if (manipulaArquivo.ValidaTamanhoArquivo(arquivoLocal)) {                            
                            executaUploadArquivo.UploadArquivo(arquivoLocal, sel_delimitadorEntrada.SelectedValue, arquivoNome, arquivoExtensao, valorTemCabecalho);
                            StatusLabel.ForeColor = System.Drawing.Color.Green;
                            StatusLabel.Text = Resources.ResourceFile.msgArquivoCarregado;
                            manipulaArquivo.ExcluiArquivo(arquivoLocal);                            
                        } else {
                            lblError.ForeColor = System.Drawing.Color.Red;
                            lblError.Text = Resources.ResourceFile.msgTamanhoArquivo;
                        }
                    }
                    catch (Exception ex) {
                        StatusLabel.Text = Resources.ResourceFile.msgArquivoNaoCarregado + ex.Message;
                    }
                }
            }
            catch (Exception ex) {
                throw ex;
            }
        }

        protected void listaTipoArquivoSaida_Selected(object sender, EventArgs e) {
            if (listaTipoArquivoSaida.SelectedValue == Resources.ResourceFile.listXlsx || listaTipoArquivoSaida.SelectedValue == Resources.ResourceFile.listCsv) {
                listaDelimitadorSaida.Enabled = false; ;
            } else {
                listaDelimitadorSaida.Enabled = true;
            }
        }
    }
}