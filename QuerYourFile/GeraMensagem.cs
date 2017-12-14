using System.Data.SqlClient;
using System.Text;

namespace QuerYourFile {
    sealed class GeraMensagem {
        /// <summary>
        /// Definição do padrão Singleton:
        /// </summary>
        private static readonly GeraMensagem instancia = new GeraMensagem();
        private GeraMensagem() { }
        public static GeraMensagem GetInstancia() {
            return instancia;
        }

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