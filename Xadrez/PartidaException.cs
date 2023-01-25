using System;
using XadrezGame.Xadrez;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace XadrezGame.Xadrez
{
    class PartidaException : Exception
    {
        public PartidaException(string mensagem) : base(mensagem) { }
    }
}