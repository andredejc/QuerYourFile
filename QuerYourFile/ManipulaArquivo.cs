using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.Data.SqlClient;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using Excel = Microsoft.Office.Interop.Excel;


namespace QuerYourFile {
    public class ManipulaArquivo {

        StringBuilder stringCriaTabela = new StringBuilder();
        IntegracaoBaseDeDados integracaoBase = new IntegracaoBaseDeDados();

        // Método CriaTabelaInsereArquivo para arquivos texto:
        public void CriaTabelaInsereArquivo(string caminhoArquivo, string nomeArquivo, string extensao, char delimitador, string temCabecalho) {
            string schema = "[dbo]";
            string tabela = schema + "." + "[" + nomeArquivo + "]";

            if (extensao == ".txt" || extensao == ".csv" || extensao == ".dat") {
                string linha = null;
                string[] colunas = null;
                stringCriaTabela.Append("IF OBJECT_ID('" + tabela + "','U') IS NOT NULL DROP TABLE " + tabela + " CREATE TABLE " + tabela + "(" + "\n");
                string createTable = null;

                using (DataTable dataTable = new DataTable()) {
                    using (StreamReader reader = new StreamReader(caminhoArquivo, Encoding.GetEncoding(1252))) {
                        linha = reader.ReadLine();
                        colunas = linha.Split(delimitador);

                        if (temCabecalho == "yes") {
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
                    integracaoBase.ExecutaInstrucaoSql(createTable);
                    integracaoBase.InsereDados(tabela, dataTable);
                }
            }
        }

        // Valida se o delimitador passado é válido, comparando entre as cinco primeiras linhas do arquivo:
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

        public void SalvaArquivoXLSX(string caminhoCompletoArquivo, DataTable queryDataTable) {
            Excel.Application app = null;
            Excel.Workbook book = null;
            Excel.Worksheet sheet = null;

            DataSet dataSet = new DataSet();
            dataSet.Tables.Add(queryDataTable);

            // Total de colunas e linhas da query:
            int totalColunas = dataSet.Tables[0].Columns.Count + 1;
            int totalLinhas = dataSet.Tables[0].Rows.Count + 1;

            app = new Excel.Application();
            app.DisplayAlerts = false;
            book = app.Workbooks.Add(Type.Missing);
            sheet = book.Worksheets[1] as Excel.Worksheet;

            // Pega o range e transforma o formato das celulas em texto para não haver perda de dados:
            Excel.Range range;
            Excel.Range rangeCelulas;
            Excel.Range rangeColunas;

            rangeCelulas = sheet.Cells;
            rangeColunas = rangeCelulas[totalLinhas, totalColunas] as Excel.Range;
            range = sheet.get_Range("A1", rangeColunas);
            range.NumberFormat = "@";

            int contadorDeColunas = 1;
            int contadorDeLinhas = 2;

            // Adiciona as colunas do arquivo:                        
            foreach (DataColumn column in dataSet.Tables[0].Columns) {
                sheet.Cells[1, contadorDeColunas] = column.ToString();
                contadorDeColunas++;
            }
            contadorDeColunas = 1;

            // Adiciona as linhas do arquivo:
            foreach (DataRow row in dataSet.Tables[0].Rows) {
                foreach (DataColumn column in dataSet.Tables[0].Columns) {
                    sheet.Cells[contadorDeLinhas, contadorDeColunas] = row[column];
                    contadorDeColunas++;
                }
                contadorDeColunas = 1;
                contadorDeLinhas++;
            }

            book.SaveAs(caminhoCompletoArquivo, Excel.XlFileFormat.xlOpenXMLWorkbook, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Excel.XlSaveAsAccessMode.xlNoChange, Excel.XlSaveConflictResolution.xlUserResolution, Type.Missing, Type.Missing, Type.Missing, Type.Missing);
            book.Close(true, Type.Missing, Type.Missing);
            app.Quit();

            EncerraProcessoExcelEspecifico(caminhoCompletoArquivo);
        }

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
        // Método sobrecarregado para carga de arquivo tipo CSV pois o delimitador é fixo:
        public void SalvaArquivoTxt(string caminhoCompletoArquivo, string extensaoArquivo, DataTable queryDataTable) {
            char delimitador = ',';
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

        public void ExcluiArquivo(string caminhoArquivo) {
            File.Delete(caminhoArquivo);
        }

        private static void EncerraProcessoExcelEspecifico(string arquivoExcel) {
            string nomeArquivo = Path.GetFileNameWithoutExtension(arquivoExcel);
            var processos = from p in Process.GetProcessesByName("EXCEL") select p;

            foreach (var processo in processos) {
                if (processo.MainWindowTitle == "Microsoft Excel - " + nomeArquivo || processo.MainWindowTitle == "" || processo.MainWindowTitle == null) {
                    processo.Kill();
                }
            }
        }
    }
}