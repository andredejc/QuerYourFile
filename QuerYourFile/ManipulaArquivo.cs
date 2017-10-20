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

namespace QuerYourFile
{
    public class ManipulaArquivo
    {
        StringDeConexao stringDeConexao = new StringDeConexao();
        StringBuilder stringCriaTabela = new StringBuilder();
        IntegracaoBaseDeDados integracao = new IntegracaoBaseDeDados();

        public void CriaTabelaInsereArquivo(string caminhoArquivo, string nomeArquivo, string extensao, char delimitador)
        {
            string schema = "[dbo]";
            string tabela = schema + "." + "[" + nomeArquivo + "]";

            if (extensao == ".txt" || extensao == ".csv")
            {
                string linha = null;
                string[] colunas = null;

                stringCriaTabela.Append("IF OBJECT_ID('" + tabela + "','U') IS NOT NULL DROP TABLE " + tabela + " CREATE TABLE " + tabela + "(" + "\n");
                string createTable = null;

                using (DataTable dataTable = new DataTable())
                {
                    using (StreamReader reader = new StreamReader(caminhoArquivo, Encoding.GetEncoding(1252)))
                    {
                        linha = reader.ReadLine();
                        colunas = linha.Split(delimitador);

                        foreach (string coluna in colunas)
                        {
                            dataTable.Columns.Add(coluna, typeof(String));
                            stringCriaTabela.Append("[" + coluna + "]" + " VARCHAR(500)," + "\n");
                        }

                        while (!reader.EndOfStream)
                        {
                            string[] linhaArquivo = reader.ReadLine().Split(delimitador);
                            dataTable.Rows.Add(linhaArquivo);
                        }
                    }

                    createTable = stringCriaTabela.ToString();
                    createTable = createTable.Substring(0, createTable.Length - 2) + ")";

                    integracao.ExecutaInstrucaoSql(createTable);
                    integracao.InsereDados(tabela, dataTable);

                }
            }
            else if (extensao == ".xls" || extensao == ".xlsx" || extensao == ".xlsb")
            {
                stringCriaTabela.Append("IF OBJECT_ID('" + tabela + "','U') IS NOT NULL DROP TABLE " + tabela + " CREATE TABLE " + tabela + "(" + "\n");
                string createTable = null;
                List<string> lista = new List<string>();
                string excelConnectionString = @"Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + caminhoArquivo + "; Extended Properties='Excel 12.0; HDR=YES'";

                Excel.Application app = null;
                Excel.Workbooks books = null;
                Excel.Workbook book = null;
                Excel.Worksheet sheet = null;

                app = new Excel.Application();
                books = app.Workbooks;
                book = app.Workbooks.Open(caminhoArquivo, 0, false, 5, "", "", false, Excel.XlPlatform.xlWindows, "", true, false, 0, true, false, false);
                sheet = book.Worksheets[1] as Excel.Worksheet;
                string planilha = sheet.Name;

                string celula;
                Excel.Range range = sheet.UsedRange;
                int rangeColunas = range.Columns.Count;

                for (int i = 1; i <= rangeColunas; i++)
                {
                    Regex regex = new Regex(@"(?!^\d+$)^.+$"); // Regex que valida se o nome da coluna não é apenas números.
                    Match match = regex.Match((range.Cells[1, i] as Excel.Range).Value2.ToString());

                    if (match.Success)
                    {
                        celula = (string)(range.Cells[1, i] as Excel.Range).Value2;
                        stringCriaTabela.Append("[" + celula + "]" + " VARCHAR(250)," + "\n");
                        lista.Add(celula);
                    }
                }

                createTable = stringCriaTabela.ToString();
                createTable = createTable.Substring(0, createTable.Length - 2) + ")";

                integracao.ExecutaInstrucaoSql(createTable);

                // Insere os dados    
                int count = 0;
                using (OleDbConnection oleDbConnection = new OleDbConnection(excelConnectionString))
                {
                    oleDbConnection.Open();
                    using (OleDbCommand oleDbCommand = new OleDbCommand(@"SELECT * FROM [" + planilha + "$]", oleDbConnection))
                    {
                        OleDbDataReader oleDbDataReader;
                        oleDbDataReader = oleDbCommand.ExecuteReader();
                        using (SqlBulkCopy sqlBulkCopy = new SqlBulkCopy(stringDeConexao.GetStringDeConexao()) { DestinationTableName = tabela, BulkCopyTimeout = 0 })
                        {
                            foreach (string cel in lista)
                            {
                                sqlBulkCopy.ColumnMappings.Add(count, cel);
                                count++;
                            }
                            sqlBulkCopy.WriteToServer(oleDbDataReader);
                        }
                    }
                }

                book.Close(true, Type.Missing, Type.Missing);
                app.Quit();
                KillProcessoExcelEspecifico(caminhoArquivo);
            }

        }        

        public void SalvaArquivo(string nomeArquivo, string extensaoArquivo, char delimitador, DataTable queryDataTable)
        {
            string caminhoSalvaArquivo = @"C:\Temp\" + nomeArquivo;
            int contadorDeColunas = 1;
            int totalDeColunas = queryDataTable.Columns.Count;
            StringBuilder montaArquivo = new StringBuilder();

            foreach (DataColumn coluna in queryDataTable.Columns)
            {
                montaArquivo.Append(coluna.ColumnName);
                if (contadorDeColunas != totalDeColunas)
                {
                    montaArquivo.Append(delimitador);
                }
                contadorDeColunas++;
            }

            montaArquivo.AppendLine();
            contadorDeColunas = 1;

            foreach (DataRow linhas in queryDataTable.Rows)
            {
                foreach (DataColumn coluna in queryDataTable.Columns)
                {
                    montaArquivo.Append(linhas[coluna.ColumnName]);
                    if (contadorDeColunas != totalDeColunas)
                    {
                        montaArquivo.Append(delimitador);
                    }
                    contadorDeColunas++;
                }

                montaArquivo.AppendLine();
                contadorDeColunas = 1;
            }

            File.WriteAllText(caminhoSalvaArquivo, montaArquivo.ToString());
        }

        private static void KillProcessoExcelEspecifico(string excelFileName)
        {
            string nomeArquivo = Path.GetFileNameWithoutExtension(excelFileName);
            var processes = from p in Process.GetProcessesByName("EXCEL")
                            select p;

            foreach (var process in processes)
            {
                if (process.MainWindowTitle == "Microsoft Excel - " + nomeArquivo || process.MainWindowTitle == "" || process.MainWindowTitle == null)
                {
                    process.Kill();
                }
            }
        }
    }
}