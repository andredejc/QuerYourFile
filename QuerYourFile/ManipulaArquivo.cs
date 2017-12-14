using System;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;

namespace QuerYourFile {
    public class ManipulaArquivo {
        StringBuilder stringCriaTabela = new StringBuilder();
        IntegracaoBaseDeDados integracaoBaseDeDados = IntegracaoBaseDeDados.GetInstancia();

        /// <summary>
        /// Cria a tabela a partir das colunas do arquivo e insere nessa tabela:
        /// </summary>
        /// <param name="caminhoArquivo"></param>
        /// <param name="nomeArquivo"></param>
        /// <param name="extensao"></param>
        /// <param name="delimitador"></param>
        /// <param name="temCabecalho"></param>
        public void CriaTabelaInsereArquivo(string caminhoArquivo, string nomeArquivo, string extensao, char delimitador, bool temCabecalho) {
            string schema = "[dbo]";
            string tabela = schema + "." + "[" + nomeArquivo + "]";

            if (extensao == Resources.ResourceFile.txt || extensao == Resources.ResourceFile.csv || extensao == Resources.ResourceFile.dat) {
                string linha = null;
                string[] colunas = null;
                stringCriaTabela.Append("IF OBJECT_ID('" + tabela + "','U') IS NOT NULL DROP TABLE " + tabela + " CREATE TABLE " + tabela + "(" + "\n");
                string createTable = null;

                using (DataTable dataTable = new DataTable()) {
                    using (StreamReader reader = new StreamReader(caminhoArquivo, System.Text.Encoding.GetEncoding(1252))) {
                        linha = reader.ReadLine();
                        colunas = linha.Split(delimitador);
                        // Se o arquivo não tiver cabeçalho, cria um cabeçalho genérico para utilização nas consultas:
                        if (temCabecalho) {
                            foreach (string coluna in colunas) {
                                dataTable.Columns.Add(coluna, typeof(String));
                                stringCriaTabela.Append("[" + coluna + "]" + " VARCHAR(500)," + "\n");
                            }
                        } else {
                            int contador = 1;
                            foreach (string coluna in colunas) {
                                dataTable.Columns.Add(coluna, typeof(String));
                                stringCriaTabela.Append("[Coluna_" + contador + "]" + " VARCHAR(500)," + "\n");
                                contador++;
                            }
                        }

                        while (!reader.EndOfStream) {
                            string[] linhaArquivo = reader.ReadLine().Split(delimitador);
                            dataTable.Rows.Add(linhaArquivo);
                        }
                    }

                    createTable = stringCriaTabela.ToString();
                    createTable = createTable.Substring(0, createTable.Length - 2) + ")";
                    integracaoBaseDeDados.ExecutaInstrucaoSql(createTable);
                    integracaoBaseDeDados.InsereDados(tabela, dataTable);
                }
            }
        }

        /// <summary>
        /// Valida o tamanho do arquivo que não pode ser maior que 10mb.
        /// </summary>
        /// <param name="arquivoLocal"></param>
        /// <returns></returns>
        public bool ValidaTamanhoArquivo(string arquivoLocal) {
            FileInfo infoDoArquivo = new FileInfo(arquivoLocal);
            float tamanhoArquivo = (infoDoArquivo.Length / 1024f) / 1024f;
            if (tamanhoArquivo > 10) {
                return false;
            } else {
                return true;
            }
        }

        /// <summary>
        /// Valida se o delimitador passado como parâmetro é válido é válido, comparando entre as cinco primeiras linhas do arquivo:
        /// </summary>
        /// <param name="caminhoArquivo"></param>
        /// <param name="delimitador"></param>
        /// <returns></returns>
        public bool ValidaDelimitador(string caminhoArquivo, char delimitador) {            
            string linha = null;
            int contador = 1;
            int contaDelimitador = 0;
            int contaDelimitadorCompara = 0;

            using (StreamReader streamReader = new StreamReader(caminhoArquivo, Encoding.GetEncoding(1252))) {
                linha = streamReader.ReadLine();
                contaDelimitador = linha.Split(delimitador).Count() - 1;
                contaDelimitadorCompara = linha.Split(delimitador).Count() - 1;

                while (!streamReader.EndOfStream) {
                    if (contaDelimitadorCompara != contaDelimitador) {
                        return false;
                    }

                    contaDelimitadorCompara = contaDelimitador;
                    linha = streamReader.ReadLine();
                    contaDelimitador = linha.Split(delimitador).Count() - 1;
                    contador++;
                    if (contador == 5) {
                        return true;
                    }
                }
                return false;
            }
        }               

        /// <summary>
        /// Exclui o arquivo corrente.
        /// </summary>
        /// <param name="caminhoArquivo"></param>
        public void ExcluiArquivo(string caminhoArquivo) {
            File.Delete(caminhoArquivo);
        }       
    }
}