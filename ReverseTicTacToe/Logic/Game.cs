using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReverseTicTacToe.Logic
{
    class ReverseTicTacToe
    {
        public enum eGameMode
        {
            PvP, PvAI,
        }
        public enum eTurns
        {
            Player1 = 1, Player2 = -1,
        }

        //Ofir - Not so sure if these should be readonly, const or static?
        private const int k_DefaultBoardDimension = 3;
        private const int k_GameBoardMinimumDimension = 3;
        private const int k_GameBoardMaximumDimension = 9;
        private const char k_DefaultPlayer1Char = 'X';
        private const char k_DefaultPlayer2Char = 'O';
        private const eGameMode k_DefaultGameMode = eGameMode.PvP;
        private const eTurns k_DefaultFirstTurn = eTurns.Player1;
        private const char k_DefaultEmptySlotChar = ' ';

        //members:
        private char m_Player1Char;
        private char m_Player2Char;
        private char[,] m_GameBoard;
        private int m_BoardDimension;
        private eGameMode m_GameMode;
        private eTurns m_CurrentTurn;
        private char m_EmptySlotChar;


        // Ofir - maybe we should make some other constructors with different parameters
        // to give the user more options - improve reusabillity
        public ReverseTicTacToe(int i_BoardDimension, eGameMode i_GameMode)
        {
            //Ofir - since we didnt learn exceptions yet, how should be handle out of range board size in constructor?
            if (i_BoardDimension < k_GameBoardMinimumDimension || i_BoardDimension > k_GameBoardMaximumDimension)
                m_BoardDimension = k_DefaultBoardDimension;
            m_GameMode = i_GameMode;
            m_Player1Char = k_DefaultPlayer1Char;
            m_Player2Char = k_DefaultPlayer2Char;
            m_CurrentTurn = k_DefaultFirstTurn;
            m_GameBoard = new char[m_BoardDimension, m_BoardDimension];
            m_EmptySlotChar = k_DefaultEmptySlotChar;

            for(int i = 0; i < m_BoardDimension; i++)
            {
                for(int j = 0; j < m_BoardDimension; j++)
                {
                    m_GameBoard[i, j] = m_EmptySlotChar;
                }
            }
        }
    }
}
