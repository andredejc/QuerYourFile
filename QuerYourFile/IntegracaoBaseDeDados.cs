using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Web;

namespace QuerYourFile {
    public class IntegracaoBaseDeDados {

        private SqlConnection RetornaConexaoBase() {
            SqlConnection sqlConnection = new SqlConnection(URLdeConexao.GetURL());
            sqlConnection.Open();
            return sqlConnection;
        }

        public void ExecutaInstrucaoSql(string instrucaoSql) {
            using (SqlCommand sqlCommand = new SqlCommand(instrucaoSql)) {
                sqlCommand.Connection = RetornaConexaoBase();
                sqlCommand.ExecuteNonQuery();
            }
        }

        public int ExecutaInstrucaoSqlRetornaDado(string instrucaoSql) {
            int retornaInteiro;
            using (SqlCommand sqlCommand = new SqlCommand(instrucaoSql)) {
                sqlCommand.CommandType = CommandType.Text;
                sqlCommand.Connection = RetornaConexaoBase();
                retornaInteiro = (int)sqlCommand.ExecuteScalar();
            }
            return retornaInteiro;
        }     

        public void InsereDados(string tabela, DataTable dataTable) {
            using (SqlBulkCopy sqlBulkCopy = new SqlBulkCopy(URLdeConexao.GetURL())) {
                sqlBulkCopy.DestinationTableName = tabela;
                sqlBulkCopy.WriteToServer(dataTable);
            }
        }

        public DataTable RetornaDadosDaConsulta(string instrucaoSql) {
            using (SqlCommand sqlCommand = new SqlCommand(instrucaoSql)) {
                using (SqlDataAdapter sqlDataAdapter = new SqlDataAdapter()) {
                    sqlCommand.Connection = RetornaConexaoBase();
                    sqlDataAdapter.SelectCommand = sqlCommand;
                    using (DataTable dataTable = new DataTable()) {
                        sqlDataAdapter.Fill(dataTable);
                        return dataTable;
                    }
                }
            }
        }

        public void LimpaBase(string nomeTabela) {
            string schema = "dbo";
            string schemaNomeTabela = "[" + schema + "].[" + nomeTabela + "]";
            string dropTabelaDaBase;
            dropTabelaDaBase = @"IF EXISTS( SELECT a.name,b.name 
                                 FROM sys.objects as a INNER JOIN sys.schemas as b
                                    ON a.schema_id = b.schema_id 
                                 WHERE b.name = '" + schema + "' AND a.name = '" + nomeTabela + "') BEGIN DROP TABLE " + schemaNomeTabela + " END ";

            using (SqlCommand sqlCommand = new SqlCommand(dropTabelaDaBase.ToString())) {
                sqlCommand.Connection = RetornaConexaoBase();
                sqlCommand.ExecuteNonQuery();
            }
        }

    }
}