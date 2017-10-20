using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Web;

namespace QuerYourFile
{
    public class IntegracaoBaseDeDados
    {
        StringDeConexao stringDeConexao = new StringDeConexao();

        public void ExecutaInstrucaoSql(string instrucaoSql)
        {
            using (SqlConnection sqlConnection = new SqlConnection(stringDeConexao.GetStringDeConexao()))
            {
                using (SqlCommand sqlCommand = new SqlCommand(instrucaoSql))
                {
                    sqlCommand.Connection = sqlConnection;
                    sqlCommand.Connection.Open();
                    sqlCommand.ExecuteNonQuery();
                }
            }
        }

        public void InsereDados(string tabela, DataTable dataTable)
        {
            using (SqlBulkCopy sqlBulkCopy = new SqlBulkCopy(stringDeConexao.GetStringDeConexao()))
            {
                sqlBulkCopy.DestinationTableName = tabela;
                sqlBulkCopy.WriteToServer(dataTable);
            }
        }
        
        public DataTable RetornaDadosDaConsulta(string select)
        {
            using (SqlConnection sqlConnection = new SqlConnection(stringDeConexao.GetStringDeConexao()))
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

        public void LimpaBase(string nomeTabela)
        {
            string schema = "dbo";
            string schemaNomeTabela = "[" + schema + "].[" + nomeTabela + "]";
            StringBuilder dropTabelaDaBase = new StringBuilder();
            dropTabelaDaBase.Append(" IF EXISTS( ");
            dropTabelaDaBase.Append(" SELECT a.name,b.name ");
            dropTabelaDaBase.Append(" FROM sys.objects as a	");
            dropTabelaDaBase.Append(" INNER JOIN sys.schemas as b ");
            dropTabelaDaBase.Append(" ON a.schema_id = b.schema_id ");
            dropTabelaDaBase.Append(" WHERE b.name = '" + schema + "' AND a.name = '" + nomeTabela + "' ) ");
            dropTabelaDaBase.Append(" BEGIN DROP TABLE " + schemaNomeTabela + " END ");

            using (SqlConnection sqlConnection = new SqlConnection(stringDeConexao.GetStringDeConexao()))
            {
                using (SqlCommand sqlCommand = new SqlCommand(dropTabelaDaBase.ToString(), sqlConnection))
                {
                    sqlCommand.Connection.Open();
                    sqlCommand.ExecuteNonQuery();
                }
            }
        }

        public string ExportaTabelaHtml(DataTable dataTable)
        {
            if (dataTable != null)
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
                return html.ToString();
            }
            else
            {
                return "";
            }
        }
    }
}