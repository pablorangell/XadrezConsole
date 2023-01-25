using System;
using XadrezGame.Tabuleiro;
using XadrezGame.Xadrez;
using System.Collections.Generic;

namespace XadrezGame.Tabuleiro
{
    class Tela
    {
        public static void imprimirPartida(PartidaXadrez partida) 
        {
            imprimirTabuleiro(partida.tab);
            Console.WriteLine();
            imprimirPecasCapturadas(partida);
            Console.WriteLine();
            Console.WriteLine("Turno: " + partida.turno);
            if (!partida.terminada) 
            {
                Console.WriteLine("Aguardando jogada: " + partida.currentPlayer + " - " + partida.currentPlayerConnected);
                if (partida.xeque) 
                {
                    Console.WriteLine("XEQUE!");
                }
            }
            else 
            {
                Console.WriteLine("XEQUEMATE!");
                Console.WriteLine("Vencedor: " + partida.currentPlayer + " - " + partida.currentPlayerConnected);
                if (partida.currentPlayerConnected == partida.connectedPlayer01)
                {
                    partida.connectedPlayer01.AddWin();
                }
                else
                {
                    partida.connectedPlayer02.AddWin();
                }
            }
        }

        public static void imprimirPecasCapturadas(PartidaXadrez partida) 
        {
            Console.WriteLine("Peças capturadas:");
            Console.Write("Brancas: ");
            imprimirConjunto(partida.pecasCapturadas(Cor.Branco));
            Console.WriteLine();
            Console.Write("Pretas: ");
            ConsoleColor aux = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.Yellow;
            imprimirConjunto(partida.pecasCapturadas(Cor.Preto));
            Console.ForegroundColor = aux;
            Console.WriteLine();
        }

        public static void imprimirConjunto(HashSet<Peca> conjunto) 
        {
            Console.Write("[");
            foreach (Peca x in conjunto) 
            {
                Console.Write(x + " ");
            }
            Console.Write("]");
        }

        public static void imprimirTabuleiro(Tabuleiro tab) 
        {
            for (int i=0; i<tab.linhas; i++) 
            {
                Console.Write(8 - i + " ");
                for (int j=0; j<tab.colunas; j++) 
                {
                    imprimirPeca(tab.peca(i, j));
                }
                Console.WriteLine();
            }
            Console.WriteLine("  a b c d e f g h");
        }

        public static void imprimirTabuleiro(Tabuleiro tab, bool[,] posicoePossiveis) 
        {

            ConsoleColor fundoOriginal = Console.BackgroundColor;
            ConsoleColor fundoAlterado = ConsoleColor.DarkGray;

            for (int i = 0; i < tab.linhas; i++) 
            {
                Console.Write(8 - i + " ");
                for (int j = 0; j < tab.colunas; j++) 
                {
                    if (posicoePossiveis[i, j]) 
                    {
                        Console.BackgroundColor = fundoAlterado;
                    }
                    else 
                    {
                        Console.BackgroundColor = fundoOriginal;
                    }
                    imprimirPeca(tab.peca(i, j));
                    Console.BackgroundColor = fundoOriginal;
                }
                Console.WriteLine();
            }
            Console.WriteLine("  a b c d e f g h");
            Console.BackgroundColor = fundoOriginal;
        }

        public static PosicaoXadrez lerPosicaoXadrez() 
        {
            string s = Console.ReadLine();
            char coluna = s[0];
            int linha = int.Parse(s[1] + "");
            return new PosicaoXadrez(coluna, linha);
        }

        public static void imprimirPeca(Peca peca) 
        {

            if (peca == null) 
            {
                Console.Write("- ");
            }
            else 
            {
                if (peca.cor == Cor.Branco) 
                {
                    Console.Write(peca);
                }
                else 
                {
                    ConsoleColor aux = Console.ForegroundColor;
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.Write(peca);
                    Console.ForegroundColor = aux;
                }
                Console.Write(" ");
            }
        }

        public static void imprimriLoginECadastro(PartidaXadrez partida)
        {
            int option;
            do
            {
                Console.Clear();
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("======== BEM VINDO AO XADREZ GAME ========");
                Console.WriteLine("========      MENU PRINCIPAL      ========");
                Console.ForegroundColor = ConsoleColor.White;   
                Console.WriteLine("1 - LOGIN");
                Console.WriteLine("2 - CADASTRO");
                Console.WriteLine("3 - Exibir jogadores cadastrados.");
                Console.ForegroundColor = ConsoleColor.Red;
                Console.Write("Digite a opção desejada: ");
                Console.ResetColor();
                option = int.Parse(Console.ReadLine());
                Console.Clear();
                switch(option)
                {
                    case 1:
                        while(!partida.connected)
                        {
                            try
                            {
                                imprimirLogin(partida);
                                Console.Clear();
                            }
                            catch(PartidaException ex)
                            {
                                Console.WriteLine(ex.Message);
                                Console.ForegroundColor = ConsoleColor.Green;
                                Console.WriteLine("Pressione qualquer tecla para continuar...");
                                Console.ResetColor();
                                Console.ReadKey();
                                Console.Clear();
                            }
                        }
                        break;

                    case 2:
                        try
                        {
                            imprimirCadastro(partida);
                            Console.ForegroundColor = ConsoleColor.Green;
                            Console.WriteLine("Cadastro realizado com sucesso!");
                            Console.ForegroundColor = ConsoleColor.White;
                            Console.WriteLine("Pressione qualquer tecla para retornar ao menu anterior...");
                            Console.ResetColor();
                            Console.ReadKey();
                        }
                        catch(PartidaException ex)
                        {
                            Console.WriteLine(ex.Message);
                            Console.ForegroundColor = ConsoleColor.Green;
                            Console.WriteLine("Pressione qualquer tecla para continuar...");
                            Console.ResetColor();
                            Console.ReadKey();
                        }
                        break;

                    case 3:
                        imprimirPlayers(partida);
                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.WriteLine("Pressione qualquer tecla para retornar ao menu anterior...");
                        Console.ResetColor();
                        Console.ReadKey();
                        break;

                    default:
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("Opção inválida! Pressione qualquer tecla para retornar ao menu anterior!");
                        Console.ResetColor();
                        Console.ReadKey();
                        break;
                }
            }
            while(option < 1 || option > 3 || !partida.connected);
        }

        public static void imprimirCadastro(PartidaXadrez partida)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("======== MENU DE CADASTRO ========");
            Console.WriteLine();
            Console.WriteLine("Cadastre-se para poder jogar!");
            Console.WriteLine();

            Console.ForegroundColor = ConsoleColor.White;
            Console.Write("Digite o login que deseja utilizar!");
            string login = Console.ReadLine();

            Console.Write("Digite seu nome: ");
            string name = Console.ReadLine();

            Console.Write("Digite sua senha: ");
            string password = Console.ReadLine();
            Console.ResetColor();
            Console.WriteLine();
            partida.Register(login, password, name);
            Console.WriteLine("Cadastro realizado com sucesso!");
        }

        public static void imprimirLogin(PartidaXadrez partida)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("======== MENU DE LOGIN ========");
            Console.WriteLine();
            Console.WriteLine("Faça login para poder jogar!");
            Console.WriteLine();
            Console.ForegroundColor = ConsoleColor.White;
            Console.Write("Digite seu login: ");
            string login = Console.ReadLine();
            
            Console.Write("Digite sua senha: ");
            string password = Console.ReadLine();
            Console.ResetColor();
            bool connectedUser = partida.Login(login, password);

            if(!connectedUser)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                //throw new PartidaException("Login ou senha inválidos!");
            }
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Login realizado com sucesso!");
            Console.ResetColor();
            Console.ReadKey();
            Console.Clear();

            if(partida.connectedPlayer01 != null && partida.connectedPlayer02 != null)
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine($"Sejam Bem Vindos {partida.connectedPlayer01.Name} e {partida.connectedPlayer02.Name} !");
                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine("Pressione qualquer tecla para continuar...");
                Console.ResetColor();
                Console.ReadKey();
            }
        }

        public static void imprimirPlayers(PartidaXadrez partida)
        {
            Console.WriteLine("======== LISTA DE JOGADORES CADASTRADOS ========");
            foreach(Player.Player i in partida.players)
            {
                Console.WriteLine($"Login: {i.Login} - Nome: {i.Name} - Vitórias: {i.Wins}");
                Console.WriteLine();
            }
        }
    }
}
