using System.Web.Configuration;

namespace QuerYourFile {
    sealed class URLdeConexao {
        /// <summary>
        /// Definição do padrão Singleton:
        /// </summary>
        private static readonly URLdeConexao instancia = new URLdeConexao();
        private string urlDeConexao;
        private URLdeConexao() {
            urlDeConexao = WebConfigurationManager.ConnectionStrings[Resources.ResourceFile.urlDeConexao].ConnectionString;
        }
        public static URLdeConexao GetInstancia() {
            return instancia;
        }

        /// <summary>
        /// Retorna a URL de conexão com a base de dados
        /// </summary>
        /// <returns></returns>
        public string GetURL() {
            return urlDeConexao;
        }
    }
}