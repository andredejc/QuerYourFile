using System.Web.Configuration;

namespace QuerYourFile {
    public class URLdeConexao {

        public static string urlDeConexao = WebConfigurationManager.ConnectionStrings[Resources.ResourceFile.urlDeConexao].ConnectionString;

        /// <summary>
        /// Retorna a URL de conexão com a base de dados
        /// </summary>
        /// <returns></returns>
        public static string GetURL() {
            return urlDeConexao;
        }
    }
}