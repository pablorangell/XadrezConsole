using System;
using System.IO;
using System.Text.Json;
using XadrezGame.Xadrez;
using XadrezGame.Tabuleiro;
using XadrezGame.Player;
using System.Collections.Generic;

namespace XadrezGame.Tabuleiro
{
    class PartidaXadrez 
    {
        public Tabuleiro tab { get; private set; }
        public int turno { get; private set; }
        public Cor currentPlayer { get; private set; }
        public Player.Player currentPlayerConnected { get; private set; }
        public List<Player.Player> players { get; private set; }
        public Player.Player connectedPlayer01 { get; private set; }
        public Player.Player connectedPlayer02 { get; private set; }
        public bool connected { get; private set; }
        public bool terminada { get; private set; }
        private HashSet<Peca> pecas;
        private HashSet<Peca> capturadas;
        public bool xeque { get; private set; }
        public Peca vulneravelEnPassant { get; private set; }
        public string arquivo = "players.json";

        public PartidaXadrez() 
        {
            tab = new Tabuleiro(8, 8);
            turno = 1;
            currentPlayer = Cor.Branco;
            FileInfo files = new FileInfo(arquivo);
            if (files.Length == 0)
            {
                players = new List<Player.Player>();
            }
            else
            {
                string deserialize = File.ReadAllText(arquivo);
                players = JsonSerializer.Deserialize<List<Player.Player>>(deserialize)!;
            }
            terminada = false;
            xeque = false;
            vulneravelEnPassant = null;
            pecas = new HashSet<Peca>();
            capturadas = new HashSet<Peca>();
            colocarPecas();
        }

        public void Register(string login, string password, string name)
        {
            if(players.Exists(x => x.Login == login))
            {
                throw new PartidaException("Login já existente. TENTE NOVAMENTE!");
            }
            else
            {
                Player.Player player = new Player.Player(login, password, name);
                players.Add(player);
                var jsonOptions = new JsonSerializerOptions { WriteIndented = true };
                string serialize = JsonSerializer.Serialize(players, jsonOptions);
                File.WriteAllText(arquivo, serialize);
            }
        }

        public bool Login(string login, string password)
        {
            bool connectAccount = players.Exists(x => x.Login == login && x.Password == password);
            if (connectAccount)
            {
                if (connectedPlayer01 == null)
                {
                    connectedPlayer01 = players.Find(x => x.Login == login && x.Password == password);
                    currentPlayerConnected = connectedPlayer01;
                }
                else
                {
                    if(players.Find(x => x.Login == login && x.Password == password) == connectedPlayer01)
                    {
                        throw new PartidaException("Já existe uma conta conectada com esse login!");
                    }
                    else
                    {
                        connectedPlayer02 = players.Find(x => x.Login == login && x.Password == password);
                        connected = true;
                    }
                }
                return false;
            }
            return false;
        }

        public Peca executaMovimento(Posicao origem, Posicao destino) 
        {
            Peca p = tab.retirarPeca(origem);
            p.incrementarQuantidadeMovimento();
            Peca pecaCapturada = tab.retirarPeca(destino);
            tab.colocarPeca(p, destino);
            if (pecaCapturada != null) 
            {
                capturadas.Add(pecaCapturada);
            }

            // #jogadaespecial roque pequeno
            if (p is Rei && destino.coluna == origem.coluna + 2) 
            {
                Posicao origemT = new Posicao(origem.linha, origem.coluna + 3);
                Posicao destinoT = new Posicao(origem.linha, origem.coluna + 1);
                Peca T = tab.retirarPeca(origemT);
                T.incrementarQuantidadeMovimento();
                tab.colocarPeca(T, destinoT);
            }

            // #jogadaespecial roque grande
            if (p is Rei && destino.coluna == origem.coluna - 2) 
            {
                Posicao origemT = new Posicao(origem.linha, origem.coluna - 4);
                Posicao destinoT = new Posicao(origem.linha, origem.coluna - 1);
                Peca T = tab.retirarPeca(origemT);
                T.incrementarQuantidadeMovimento();
                tab.colocarPeca(T, destinoT);
            }

            // #jogadaespecial en passant
            if (p is Peao) 
            {
                if (origem.coluna != destino.coluna && pecaCapturada == null) 
                {
                    Posicao posP;
                    if (p.cor == Cor.Branco) 
                    {
                        posP = new Posicao(destino.linha + 1, destino.coluna);
                    }
                    else 
                    {
                        posP = new Posicao(destino.linha - 1, destino.coluna);
                    }
                    pecaCapturada = tab.retirarPeca(posP);
                    capturadas.Add(pecaCapturada);
                }
            }

            return pecaCapturada;
        }

        public void desfazMovimento(Posicao origem, Posicao destino, Peca pecaCapturada)
        {
            Peca p = tab.retirarPeca(destino);
            p.decrementarQuantidadeMovimento();
            if (pecaCapturada != null) 
            {
                tab.colocarPeca(pecaCapturada, destino);
                capturadas.Remove(pecaCapturada);
            }
            tab.colocarPeca(p, origem);

            // #jogadaespecial roque pequeno
            if (p is Rei && destino.coluna == origem.coluna + 2) 
            {
                Posicao origemT = new Posicao(origem.linha, origem.coluna + 3);
                Posicao destinoT = new Posicao(origem.linha, origem.coluna + 1);
                Peca T = tab.retirarPeca(destinoT);
                T.decrementarQuantidadeMovimento();
                tab.colocarPeca(T, origemT);
            }

            // #jogadaespecial roque grande
            if (p is Rei && destino.coluna == origem.coluna - 2) 
            {
                Posicao origemT = new Posicao(origem.linha, origem.coluna - 4);
                Posicao destinoT = new Posicao(origem.linha, origem.coluna - 1);
                Peca T = tab.retirarPeca(destinoT);
                T.decrementarQuantidadeMovimento();
                tab.colocarPeca(T, origemT);
            }

            // #jogadaespecial en passant
            if (p is Peao) 
            {
                if (origem.coluna != destino.coluna && pecaCapturada == vulneravelEnPassant) 
                {
                    Peca peao = tab.retirarPeca(destino);
                    Posicao posP;
                    if (p.cor == Cor.Branco) 
                    {
                        posP = new Posicao(3, destino.coluna);
                    }
                    else {
                        posP = new Posicao(4, destino.coluna);
                    }
                    tab.colocarPeca(peao, posP);
                }
            }
        }

        public void realizaJogada(Posicao origem, Posicao destino) 
        {
            Peca pecaCapturada = executaMovimento(origem, destino);

            if (estaEmXeque(currentPlayer)) 
            {
                desfazMovimento(origem, destino, pecaCapturada);
                throw new TabuleiroException("Você não pode se colocar em xeque!");
            }

            Peca p = tab.peca(destino);

            // #jogadaespecial promocao
            if (p is Peao) 
            {
                if ((p.cor == Cor.Branco && destino.linha == 0) || (p.cor == Cor.Preto && destino.linha == 7)) 
                {
                    p = tab.retirarPeca(destino);
                    pecas.Remove(p);
                    Peca dama = new Dama(tab, p.cor);
                    tab.colocarPeca(dama, destino);
                    pecas.Add(dama);
                }
            }

            if (estaEmXeque(adversaria(currentPlayer))) 
            {
                xeque = true;
            }
            else 
            {
                xeque = false;
            }

            if (testeXequemate(adversaria(currentPlayer))) 
            {
                terminada = true;
            }
            else 
            {
                turno++;
                mudaJogador();
            }

            // #jogadaespecial en passant

            if (p is Peao && (destino.linha == origem.linha - 2 || destino.linha == origem.linha + 2)) 
            {
                vulneravelEnPassant = p;
            }
            else 
            {
                vulneravelEnPassant = null;
            }
        }

        public void validarPosicaoDeOrigem(Posicao pos) 
        {
            if (tab.peca(pos) == null) 
            {
                throw new TabuleiroException("Não existe peça na posição de origem escolhida!");
            }
            if (currentPlayer != tab.peca(pos).cor) 
            {
                throw new TabuleiroException("A peça de origem escolhida não é sua!");
            }
            if (!tab.peca(pos).existemMovimentosPossiveis()) 
            {
                throw new TabuleiroException("Não há movimentos possíveis para a peça de origem escolhida!");
            }
        }

        public void validarPosicaoDeDestino(Posicao origem, Posicao destino) 
        {
            if (!tab.peca(origem).movimentosPossiveis(destino)) 
            {
                throw new TabuleiroException("Posição de destino inválida!");
            }
        }

        private void mudaJogador() {
            if (currentPlayer == Cor.Branco) 
            {
                currentPlayer = Cor.Preto;
            }
            else 
            {
                currentPlayer = Cor.Branco;
            }
        }

        public HashSet<Peca> pecasCapturadas(Cor cor) 
        {
            HashSet<Peca> aux = new HashSet<Peca>();
            foreach (Peca x in capturadas) 
            {
                if (x.cor == cor) 
                {
                    aux.Add(x);
                }
            }
            return aux;
        }

        public HashSet<Peca> pecasEmJogo(Cor cor) 
        {
            HashSet<Peca> aux = new HashSet<Peca>();
            foreach (Peca x in pecas) 
            {
                if (x.cor == cor) 
                {
                    aux.Add(x);
                }
            }
            aux.ExceptWith(pecasCapturadas(cor));
            return aux;
        }

        private Cor adversaria(Cor cor) 
        {
            if (cor == Cor.Branco) 
            {
                return Cor.Preto;
            }
            else 
            {
                return Cor.Branco;
            }
        }

        private Peca rei(Cor cor) 
        {
            foreach (Peca x in pecasEmJogo(cor)) 
            {
                if (x is Rei) 
                {
                    return x;
                }
            }
            return null;
        }

        public bool estaEmXeque(Cor cor) 
        {
            Peca R = rei(cor);
            if (R == null) 
            {
                throw new TabuleiroException("Não tem rei da cor " + cor + " no tabuleiro!");
            }
            foreach (Peca x in pecasEmJogo(adversaria(cor))) 
            {
                bool[,] mat = x.movimentosPossiveis();
                if (mat[R.posicao.linha, R.posicao.coluna]) 
                {
                    return true;
                }
            }
            return false;
        }

        public bool testeXequemate(Cor cor) 
        {
            if (!estaEmXeque(cor)) 
            {
                return false;
            }
            foreach (Peca x in pecasEmJogo(cor)) 
            {
                bool[,] mat = x.movimentosPossiveis();
                for (int i=0; i<tab.linhas; i++) 
                {
                    for (int j=0; j<tab.colunas; j++) 
                    {
                        if (mat[i, j]) 
                        {
                            Posicao origem = x.posicao;
                            Posicao destino = new Posicao(i, j);
                            Peca pecaCapturada = executaMovimento(origem, destino);
                            bool testeXeque = estaEmXeque(cor);
                            desfazMovimento(origem, destino, pecaCapturada);
                            if (!testeXeque) 
                            {
                                return false;
                            }
                        }
                    }
                }
            }
            return true;
        }

        public void colocarNovaPeca(char coluna, int linha, Peca peca) 
        {
            tab.colocarPeca(peca, new PosicaoXadrez(coluna, linha).toPosicao());
            pecas.Add(peca);
        }

        private void colocarPecas() 
        {
            colocarNovaPeca('a', 1, new Torre(tab, Cor.Branco));
            colocarNovaPeca('b', 1, new Cavalo(tab, Cor.Branco));
            colocarNovaPeca('c', 1, new Bispo(tab, Cor.Branco));
            colocarNovaPeca('d', 1, new Dama(tab, Cor.Branco));
            colocarNovaPeca('e', 1, new Rei(tab, Cor.Branco, this));
            colocarNovaPeca('f', 1, new Bispo(tab, Cor.Branco));
            colocarNovaPeca('g', 1, new Cavalo(tab, Cor.Branco));
            colocarNovaPeca('h', 1, new Torre(tab, Cor.Branco));
            colocarNovaPeca('a', 2, new Peao(tab, Cor.Branco, this));
            colocarNovaPeca('b', 2, new Peao(tab, Cor.Branco, this));
            colocarNovaPeca('c', 2, new Peao(tab, Cor.Branco, this));
            colocarNovaPeca('d', 2, new Peao(tab, Cor.Branco, this));
            colocarNovaPeca('e', 2, new Peao(tab, Cor.Branco, this));
            colocarNovaPeca('f', 2, new Peao(tab, Cor.Branco, this));
            colocarNovaPeca('g', 2, new Peao(tab, Cor.Branco, this));
            colocarNovaPeca('h', 2, new Peao(tab, Cor.Branco, this));

            colocarNovaPeca('a', 8, new Torre(tab, Cor.Preto));
            colocarNovaPeca('b', 8, new Cavalo(tab, Cor.Preto));
            colocarNovaPeca('c', 8, new Bispo(tab, Cor.Preto));
            colocarNovaPeca('d', 8, new Dama(tab, Cor.Preto));
            colocarNovaPeca('e', 8, new Rei(tab, Cor.Preto, this));
            colocarNovaPeca('f', 8, new Bispo(tab, Cor.Preto));
            colocarNovaPeca('g', 8, new Cavalo(tab, Cor.Preto));
            colocarNovaPeca('h', 8, new Torre(tab, Cor.Preto));
            colocarNovaPeca('a', 7, new Peao(tab, Cor.Preto, this));
            colocarNovaPeca('b', 7, new Peao(tab, Cor.Preto, this));
            colocarNovaPeca('c', 7, new Peao(tab, Cor.Preto, this));
            colocarNovaPeca('d', 7, new Peao(tab, Cor.Preto, this));
            colocarNovaPeca('e', 7, new Peao(tab, Cor.Preto, this));
            colocarNovaPeca('f', 7, new Peao(tab, Cor.Preto, this));
            colocarNovaPeca('g', 7, new Peao(tab, Cor.Preto, this));
            colocarNovaPeca('h', 7, new Peao(tab, Cor.Preto, this));
        }
    }
}
