using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace QuerYourFile
{
    public class StringDeConexao
    {
        private string stringDeConexao = @"Password=P@ssw0rd;Persist Security Info=True;User ID=sa;Data Source=localhost;Initial Catalog=Testes;";

        public string GetStringDeConexao()
        {
            return stringDeConexao;
        }
    }
}