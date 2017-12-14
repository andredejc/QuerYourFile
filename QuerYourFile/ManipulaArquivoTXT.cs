using System;
using System.Data;
using System.IO;
using System.Text;

namespace QuerYourFile {
    sealed class ManipulaArquivoTXT {
        /// <summary>
        /// Definição do padrão Singleton:
        /// </summary>
        private static readonly ManipulaArquivoTXT instancia = new ManipulaArquivoTXT();
        private ManipulaArquivoTXT() { }
        public static ManipulaArquivoTXT GetInstancia() {
            return instancia;
        }

        /// <summary>
        /// Salva arquivo de texto no formato .csv
        /// </summary>
        /// <param name="caminhoCompletoArquivo"></param>
        /// <param name="extensaoArquivo"></param>
        /// <param name="queryDataTable"></param>
        public void SalvaArquivoTxt(string caminhoCompletoArquivo, string extensaoArquivo, DataTable queryDataTable) {
            char delimitador = Convert.ToChar(Resources.ResourceFile.delimVirgula);
            int contadorDeColunas = 1;
            int totalDeColunas = queryDataTable.Columns.Count;
            StringBuilder montaArquivo = new StringBuilder();

            foreach (DataColumn coluna in queryDataTable.Columns) {
                montaArquivo.Append(coluna.ColumnName);
                if (contadorDeColunas != totalDeColunas) {
                    montaArquivo.Append(delimitador);
                }
                contadorDeColunas++;
            }
            montaArquivo.AppendLine();
            contadorDeColunas = 1;

            foreach (DataRow linha in queryDataTable.Rows) {
                foreach (DataColumn coluna in queryDataTable.Columns) {
                    montaArquivo.Append(linha[coluna.ColumnName]);

                    if (contadorDeColunas != totalDeColunas) {
                        montaArquivo.Append(delimitador);
                    }
                    contadorDeColunas++;
                }
                montaArquivo.AppendLine();
                contadorDeColunas = 1;
            }

            File.WriteAllText(caminhoCompletoArquivo, montaArquivo.ToString());
        }

        /// <summary>
        /// Método sobrecarregado que salva arquivos de texto mas não .csv
        /// </summary>
        /// <param name="caminhoCompletoArquivo"></param>
        /// <param name="extensaoArquivo"></param>
        /// <param name="delimitador"></param>
        /// <param name="queryDataTable"></param>
        public void SalvaArquivoTxt(string caminhoCompletoArquivo, string extensaoArquivo, char delimitador, DataTable queryDataTable) {
            int contadorDeColunas = 1;
            int totalDeColunas = queryDataTable.Columns.Count;
            StringBuilder montaArquivo = new StringBuilder();

            foreach (DataColumn coluna in queryDataTable.Columns) {
                montaArquivo.Append(coluna.ColumnName);
                if (contadorDeColunas != totalDeColunas) {
                    montaArquivo.Append(delimitador);
                }
                contadorDeColunas++;
            }
            montaArquivo.AppendLine();
            contadorDeColunas = 1;

            foreach (DataRow linhas in queryDataTable.Rows) {
                foreach (DataColumn coluna in queryDataTable.Columns) {
                    montaArquivo.Append(linhas[coluna.ColumnName]);
                    if (contadorDeColunas != totalDeColunas) {
                        montaArquivo.Append(delimitador);
                    }
                    contadorDeColunas++;
                }
                montaArquivo.AppendLine();
                contadorDeColunas = 1;
            }

            File.WriteAllText(caminhoCompletoArquivo, montaArquivo.ToString());
        }
    }
}