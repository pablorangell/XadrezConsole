using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace XadrezGame.Tabuleiro
{
    abstract class Peca
    {
        public Posicao posicao { get; set; }
        public Cor cor { get; set; }
        public int quantidadeMovimentos { get; protected set; }
        public Tabuleiro tab { get; protected set; }

        public Peca(Tabuleiro tab, Cor cor)
        {
            this.posicao = null;
            this.cor = cor;
            this.quantidadeMovimentos = 0;
            this.tab = tab;
        }

        public void incrementarQuantidadeMovimento()
        {
            quantidadeMovimentos++;
        }

        public void decrementarQuantidadeMovimento()
        {
            quantidadeMovimentos--;
        }

        public bool existemMovimentosPossiveis()
        {
            bool[,] mat = movimentosPossiveis();
            for (int i = 0; i < tab.linhas; i++)
            {
                for (int j = 0; j < tab.colunas; j++)
                {
                    if (mat[i, j] == true)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        public bool movimentosPossiveis(Posicao pos)
        {
            return movimentosPossiveis()[pos.linha, pos.coluna];
        }

        public abstract bool[,] movimentosPossiveis();
    }
}