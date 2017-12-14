using System;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Excel = Microsoft.Office.Interop.Excel;

namespace QuerYourFile {
    sealed class ManipulaArquivoXLSX {
        /// <summary>
        /// Definição do padrão Singleton:
        /// </summary>
        private static readonly ManipulaArquivoXLSX instancia = new ManipulaArquivoXLSX();
        private ManipulaArquivoXLSX() { }
        public static ManipulaArquivoXLSX GetInstancia() {
            return instancia;
        }

        /// <summary>
        /// Salva arquivos do tipo .xlsx
        /// </summary>
        /// <param name="caminhoCompletoArquivo"></param>
        /// <param name="queryDataTable"></param>
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
            range = sheet.get_Range(Resources.ResourceFile.rangeExcel, rangeColunas);
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

        /// <summary>
        /// Encerra o processo após manipulação do arquivo.
        /// </summary>
        /// <param name="arquivoExcel"></param>
        private static void EncerraProcessoExcelEspecifico(string arquivoExcel) {
            string nomeArquivo = Path.GetFileNameWithoutExtension(arquivoExcel);
            var processos = from p in Process.GetProcessesByName(Resources.ResourceFile.processoExcel) select p;
            foreach (var processo in processos) {
                if (processo.MainWindowTitle == Resources.ResourceFile.tituloExcel + nomeArquivo || processo.MainWindowTitle == Resources.ResourceFile.stringVazia || processo.MainWindowTitle == null) {
                    processo.Kill();
                }
            }
        }
    }
}