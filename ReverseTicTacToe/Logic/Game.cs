using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReverseTicTacToe.Logic
{
    class Game
    {
        private Player m_Player1, m_Player2;
        private int m_CurrentTurn;
        public int [][] GameBoard { get; set; }
        public enum eGameTypes
        {
            PVP = 1,
            PVC = 2
        }
        public Game(int i_BoardSize, eGameTypes i_GameType)
        {
            m_CurrentTurn = 1;
            if(i_GameType == eGameTypes.PVP)
            {
                m_Player1 = new Player('X', false);
                m_Player2 = new Player('O', false);
            } else if(i_GameType == eGameTypes.PVC)
            {
                m_Player1 = new Player('X', false);
                m_Player2 = new Player('O', true);
            }
        }
    }
}
