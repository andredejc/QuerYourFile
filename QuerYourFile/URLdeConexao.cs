using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace QuerYourFile {
    public class URLdeConexao {
        public static string urlDeConexao = @"Password=P@ssw0rd;Persist Security Info=True;User ID=sa;Data Source=localhost;Initial Catalog=Testes;";
        public static string GetURL() {
            return urlDeConexao;
        }
    }
}