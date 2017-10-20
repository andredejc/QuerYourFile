using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace QuerYourFile
{
    public static class Arquivo
    {
        static string localArquivo;
        public static string LocalArquivo
        {
            get { return localArquivo; }
            set { localArquivo = value; }
        }

        static string nome;
        public static string Nome
        {
            get { return Path.GetFileNameWithoutExtension(localArquivo); }
            set { nome = value; }
        }

        public static string Extensao
        {
            get { return Path.GetExtension(localArquivo); }
        }

        public static char delimitador { get; set; }
    }
}