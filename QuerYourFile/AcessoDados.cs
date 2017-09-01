using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Text;

namespace QuerYourFile
{
    public class AcessoDados
    {
        const string connectionString = @"Password=P@ssw0rd;Persist Security Info=True;User ID=sa;Data Source=localhost;Initial Catalog=Testes;";

        // Insere no banco os dados dos arquivos:
        public void GetDataArquivo(string arquivo)
        {
            string extensao = Path.GetExtension(arquivo);
            string tabela = "[" + Path.GetFileNameWithoutExtension(arquivo) + "]";
            string linha = null;
            string[] colunas = null;
            char delimitador = ',';
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.Append("IF OBJECT_ID('" + tabela + "','U') IS NOT NULL DROP TABLE " + tabela + " CREATE TABLE " + tabela + "(" + "\n");
            string createTable = null;

            using (DataTable dataTable = new DataTable())
            {
                using (StreamReader reader = new StreamReader(arquivo, Encoding.GetEncoding(1252)))
                {
                    linha = reader.ReadLine();
                    colunas = linha.Split(delimitador);

                    foreach (string coluna in colunas)
                    {
                        dataTable.Columns.Add(coluna, typeof(String));
                        stringBuilder.Append("[" + coluna + "]" + " VARCHAR(500)," + "\n");
                    }

                    while (!reader.EndOfStream)
                    {
                        string[] linhaArquivo = reader.ReadLine().Split(delimitador);
                        dataTable.Rows.Add(linhaArquivo);
                    }
                }

                createTable = stringBuilder.ToString();
                createTable = createTable.Substring(0, createTable.Length - 2) + ")";

                using (SqlConnection sqlConnection = new SqlConnection(connectionString))
                {
                    using (SqlCommand sqlCommand = new SqlCommand(createTable))
                    {
                        sqlCommand.Connection = sqlConnection;
                        sqlCommand.Connection.Open();
                        sqlCommand.ExecuteNonQuery();

                        using (SqlBulkCopy sqlBulkCopy = new SqlBulkCopy(sqlConnection))
                        {
                            sqlBulkCopy.DestinationTableName = tabela;
                            sqlBulkCopy.WriteToServer(dataTable);
                        }

                    }
                }
            }
        }
        // Retorna a query dos arquivos:
        public DataTable GetData(string select)
        {
            try
            {
                using (SqlConnection sqlConnection = new SqlConnection(connectionString))
                {
                    using (SqlCommand sqlCommand = new SqlCommand(select))
                    {
                        using (SqlDataAdapter sqlDataAdapter = new SqlDataAdapter())
                        {
                            sqlCommand.Connection = sqlConnection;
                            sqlDataAdapter.SelectCommand = sqlCommand;
                            using (DataTable dataTable = new DataTable())
                            {
                                sqlDataAdapter.Fill(dataTable);
                                return dataTable;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        // Gera a tabela HTML a partir da consulta:
        public string ExportaHtml(DataTable dataTable)
        {
            StringBuilder html = new StringBuilder();

            html.Append("<div class='container'>");
            html.Append("<table>");

            foreach (DataColumn column in dataTable.Columns)
            {
                html.Append("<th>");
                html.Append(column.ColumnName);
                html.Append("</th>");
            }

            html.Append("</tr></thead><tbody>");

            foreach (DataRow row in dataTable.Rows)
            {
                html.Append("<tr>");
                foreach (DataColumn column in dataTable.Columns)
                {
                    html.Append("<td>");
                    html.Append(row[column.ColumnName]);
                    html.Append("</td>");
                }
                html.Append("</tr>");
            }
            html.Append("</tbody></div></table></body></html>");

            string htmlBody = html.ToString();
            return htmlBody;
        }
    }
}