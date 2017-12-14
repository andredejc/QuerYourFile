using System;

namespace QuerYourFile {
    public class ExecutaUploadArquivo {
        ManipulaArquivo manipulaArquivo = new ManipulaArquivo();

        /// <summary>
        /// Faz o upload do arquivo para a base de dados.
        /// </summary>
        /// <param name="arquivoLocal"></param>
        /// <param name="valorDelimitador"></param>
        /// <param name="arquivoNome"></param>
        /// <param name="arquivoExtensao"></param>
        /// <param name="valorTemCabecalho"></param>
        public void UploadArquivo(string arquivoLocal, string valorDelimitador, string arquivoNome, string arquivoExtensao, bool valorTemCabecalho) {
            char delimitador;            
            if (valorDelimitador == Resources.ResourceFile.listVirgula.ToString()) {
                delimitador = Convert.ToChar(Resources.ResourceFile.delimVirgula);
            } else if (valorDelimitador == Resources.ResourceFile.listPontoVirgula) {
                delimitador = Convert.ToChar(Resources.ResourceFile.delimPontoVirgula); ;
            } else {
                delimitador = '\t';
            }

            if (manipulaArquivo.ValidaDelimitador(arquivoLocal, delimitador)) {
                manipulaArquivo.CriaTabelaInsereArquivo(arquivoLocal, arquivoNome, arquivoExtensao, delimitador, valorTemCabecalho);
            } else {
                throw new Exception(Resources.ResourceFile.msgDelimitadorInvalido);
            }
        }
    }
}