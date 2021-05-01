using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReverseTicTacToe.Logic
{
    class Player
    {
        private bool m_isComputer;
        public int Score { get; set; }

        private char m_Symbol;
        public Player(char i_Symbol, bool i_IsComputer)
        {
            m_Symbol = i_Symbol;
            m_isComputer = i_IsComputer;
            Score = 0;
        }
    }
}
