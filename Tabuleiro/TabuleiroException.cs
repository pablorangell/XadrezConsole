using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace XadrezGame.Tabuleiro
{
    public class TabuleiroException : Exception
    {
        public TabuleiroException(string mensagem) : base(mensagem) { }
    }
}