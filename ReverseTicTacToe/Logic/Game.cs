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
        public const int MINBOARDSIZE = 3;
        public const int MAXBOARDSIZE = 9;

        public const int StatusTaken = 1;
        public const int StatusWin = 2;
        public const int StatusTie = 3;

        public int BoardSize { get; set; }
        public int CurrentTurn { get; set; }

        public eGameTypes GameType { get; set; }
        public char [,] GameBoard { get; set; }

        public int player1Score { get; set; }
        public int player2Score { get; set; }

        public bool PlaceSymbolAndReturnStatus(int row, int col, out int status)
        {
            status = 0; //ignore this

            // if place empty, fill it, and return true;
            // if place taken, set status to StatusTaken and return false
            // if  win, set status to StatusWin and return false and increase the score to the wining player
            // if tie set status to StatusTie and return false

            return true;
        }

        public enum eGameTypes
        {
            PVP = 1,
            PVC = 2
        }
        public enum eTurns
        {
            PLAYER1 = 1,
            PLAYER2 = -1
        }

        public void initGame()
        {
            CurrentTurn = 1;
            //resetBoard();
        }
        public Game(int i_BoardSize, eGameTypes i_GameType)
        {
            BoardSize = i_BoardSize;
            CurrentTurn = 1;
            m_Player1 = new Player('X', false);
            GameType = i_GameType;
            bool isPc = (i_GameType == eGameTypes.PVC);
            m_Player2 = new Player('O', isPc);
            player1Score = 0;
            player2Score = 0;
            GameBoard = new char[i_BoardSize,i_BoardSize];
            for(int i = 0; i < i_BoardSize; i++)
            {
                for(int j = 0; j < i_BoardSize; j++)
                {
                    GameBoard[i, j] = ' ';
                }
            }
        }

        public void Playerforfeit()
        {
            // look who is the current player, and give point to the other player
            throw new NotImplementedException();
        }
    }
}
