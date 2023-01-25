using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace XadrezGame.Player
{
    class Player
    {
        public string Login { get; set; }
        public string Password { get; set; }
        public string Name { get; private set; }
        public int Wins { get; private set; }

        public Player(string login, string password, string name)
        {
            Login = login;
            Password = password;
            Name = name;
            Wins = 0;
        }

        public void AddWin()
        {
            Wins++;
        }

        public override string ToString()
        {
            return Name + " - " + Wins + " wins";
        }
    }
}