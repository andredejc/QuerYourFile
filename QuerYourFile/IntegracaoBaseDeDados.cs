using System.Data;
using System.Data.SqlClient;
using System.Text.RegularExpressions;

namespace QuerYourFile {
    sealed class IntegracaoBaseDeDados {
        /// <summary>
        /// Definição do padrão Singleton:
        /// </summary>
        private static readonly IntegracaoBaseDeDados instancia = new IntegracaoBaseDeDados();
        private IntegracaoBaseDeDados() { }
        public static IntegracaoBaseDeDados GetInstancia() {
            return instancia;
        }

        URLdeConexao url = URLdeConexao.GetInstancia();

        /// <summary>
        /// Executa uma instrução SQL.
        /// </summary>
        /// <param name="instrucaoSql"></param>
        public void ExecutaInstrucaoSql(string instrucaoSql) {
            using (SqlConnection sqlConnection = new SqlConnection(url.GetURL())) {
                sqlConnection.Open();
                using (SqlCommand sqlCommand = new SqlCommand(instrucaoSql)) {
                    sqlCommand.Connection = sqlConnection;
                    sqlCommand.ExecuteNonQuery();
                }
            }            
        }

        /// <summary>
        /// Executa uma instrução SQL e retorna um inteiro que representa a quantidade de registros manipulados pela instrução.
        /// </summary>
        /// <param name="instrucaoSql"></param>
        /// <returns></returns>
        public int ExecutaInstrucaoSqlRetornaDado(string instrucaoSql) {
            int retornaInteiro;
            using (SqlConnection sqlConnection = new SqlConnection(url.GetURL())) {
                sqlConnection.Open();
                using (SqlCommand sqlCommand = new SqlCommand(instrucaoSql)) {
                    sqlCommand.CommandType = CommandType.Text;
                    sqlCommand.Connection = sqlConnection;
                    retornaInteiro = (int)sqlCommand.ExecuteScalar();
                }
            }            
            return retornaInteiro;
        }     

        /// <summary>
        /// Insere os dados na tabela passada como parâmetro.
        /// </summary>
        /// <param name="tabela"></param>
        /// <param name="dataTable"></param>
        public void InsereDados(string tabela, DataTable dataTable) {
            using (SqlBulkCopy sqlBulkCopy = new SqlBulkCopy(url.GetURL())) {
                sqlBulkCopy.DestinationTableName = tabela;
                sqlBulkCopy.WriteToServer(dataTable);
            }
        }

        /// <summary>
        /// Retorna os dados de uma instrução SQL.
        /// </summary>
        /// <param name="instrucaoSql"></param>
        /// <returns></returns>
        public DataTable RetornaDadosDaConsulta(string instrucaoSql) {
            using (SqlConnection sqlConnection = new SqlConnection(url.GetURL())) {
                sqlConnection.Open();
                using (SqlCommand sqlCommand = new SqlCommand(instrucaoSql)) {
                    using (SqlDataAdapter sqlDataAdapter = new SqlDataAdapter()) {
                        sqlCommand.Connection = sqlConnection;
                        sqlDataAdapter.SelectCommand = sqlCommand;
                        using (DataTable dataTable = new DataTable()) {
                            sqlDataAdapter.Fill(dataTable);
                            return dataTable;
                        }
                    }
                }
            }            
        }

        /// <summary>
        /// Exclui da base de dados a tabela passada como parâmetro.
        /// </summary>
        /// <param name="nomeTabela"></param>
        public void LimpaBase(string nomeTabela) {
            string schema = "dbo";
            string schemaNomeTabela = "[" + schema + "].[" + nomeTabela + "]";
            string dropTabelaDaBase;
            dropTabelaDaBase = @"IF EXISTS( SELECT a.name,b.name 
                                 FROM sys.objects as a INNER JOIN sys.schemas as b
                                    ON a.schema_id = b.schema_id 
                                 WHERE b.name = '" + schema + "' AND a.name = '" + nomeTabela + "') BEGIN DROP TABLE " + schemaNomeTabela + " END ";

            using (SqlConnection sqlConnection = new SqlConnection(url.GetURL())) {
                sqlConnection.Open();
                using (SqlCommand sqlCommand = new SqlCommand(dropTabelaDaBase.ToString())) {
                    sqlCommand.Connection = sqlConnection;
                    sqlCommand.ExecuteNonQuery();
                }
            }
        }

        /// <summary>
        /// Valida se a instrução SQL passada como parâmetro é uma consulta válida.
        /// </summary>
        /// <param name="instrucaoSql"></param>
        /// <returns></returns>
        public Match ValidaInstrucaoSql(string instrucaoSql) {
            Regex regex = new Regex(@"\b(?i)(DROP|CREATE|ALTER|MODIFY|GRANT|REVOKE|REBUILD|REORGANIZE|RECOMPILE)");
            Match match = regex.Match(instrucaoSql);
            return match;
        } 

        /// <summary>
        /// Faz a validação da instrução no editor.
        /// </summary>
        /// <param name="editorSql"></param>
        /// <returns></returns>
        public bool ValidaEditorSql(string editorSql) {
            if (editorSql != Resources.ResourceFile.stringVazia && !ValidaInstrucaoSql(editorSql).Success) {
                return true;
            } else {
                return false;
            }

        }
    }
}