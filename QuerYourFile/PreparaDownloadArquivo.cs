using System;
using System.Data;
using System.IO;

namespace QuerYourFile {
    public class PreparaDownloadArquivo {

        ManipulaArquivoTXT manipulaArquivoTXT = new ManipulaArquivoTXT();
        ManipulaArquivoXLSX manipulaArquivoXLSX = new ManipulaArquivoXLSX();
        /// <summary>
        /// Prepara a estrutura do arquivo para o dowload
        /// </summary>
        /// <param name="tipoArquivoSaida"></param>
        /// <param name="tipoDelimitadorSaida"></param>
        /// <param name="caminhoCompletoArquivo"></param>
        /// <param name="extensaoArquivo"></param>
        /// <param name="dataTable"></param>
        /// <returns></returns>
        public string GeraArquivoDownload(string tipoArquivoSaida, string tipoDelimitadorSaida, string caminhoCompletoArquivo, string extensaoArquivo, DataTable dataTable) {
            string contentType;
            char delimitador;            
            if (tipoArquivoSaida == Resources.ResourceFile.listXlsx) {
                manipulaArquivoXLSX.SalvaArquivoXLSX(caminhoCompletoArquivo, dataTable);
                contentType = Resources.ResourceFile.contentTypeExcel;
            } else if (tipoArquivoSaida == Resources.ResourceFile.listCsv) {
                contentType = Resources.ResourceFile.contentTypeCsv;
                manipulaArquivoTXT.SalvaArquivoTxt(caminhoCompletoArquivo, extensaoArquivo, dataTable);
            } else {
                if (tipoDelimitadorSaida == Resources.ResourceFile.listVirgula) {
                    delimitador = Convert.ToChar(Resources.ResourceFile.delimVirgula);
                } else if (tipoDelimitadorSaida == Resources.ResourceFile.listPontoVirgula) {
                    delimitador = Convert.ToChar(Resources.ResourceFile.delimPontoVirgula);
                } else {
                    delimitador = '\t';                    
                }
                contentType = Resources.ResourceFile.contentTypeTxt;
                manipulaArquivoTXT.SalvaArquivoTxt(caminhoCompletoArquivo, extensaoArquivo, delimitador, dataTable);
            }
            return contentType;
        }
        /// <summary>
        /// Faz o download do arquivo no diretório Downloads
        /// </summary>
        /// <param name="caminhoCompletoArquivo"></param>
        /// <param name="contentType"></param>
        public void FazDownloadArquivo(string caminhoCompletoArquivo, string contentType) {
            System.Web.HttpResponse response = System.Web.HttpContext.Current.Response;
            FileInfo infoArquivo = new FileInfo(caminhoCompletoArquivo);
            response.Clear();
            response.ClearHeaders();
            response.ClearContent();
            response.AddHeader(Resources.ResourceFile.contentDisposition, Resources.ResourceFile.attachmentFilename + infoArquivo.Name);
            response.AddHeader(Resources.ResourceFile.contentLength, infoArquivo.Length.ToString());
            response.ContentType = contentType;
            response.Flush();
            response.TransmitFile(infoArquivo.FullName);
            response.Flush();
            response.End();

        }
    }
}