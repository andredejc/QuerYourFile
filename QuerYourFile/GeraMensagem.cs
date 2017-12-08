using System.Data.SqlClient;
using System.Text;

namespace QuerYourFile {
    public class GeraMensagem {
        /// <summary>
        /// Gera uma string a partir de uma SqlException.
        /// </summary>
        /// <param name="sqlException"></param>
        /// <returns></returns>
        public string GeraMensagemSql(SqlException sqlException) {
            StringBuilder mensagemDeErro = new StringBuilder();
            for (int i = 0; i < sqlException.Errors.Count; i++) {
                mensagemDeErro.Append(sqlException.Errors[i].Message);
            }
            return mensagemDeErro.ToString();
        }
    }
}