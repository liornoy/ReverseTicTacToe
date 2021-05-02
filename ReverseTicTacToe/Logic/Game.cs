using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReverseTicTacToe.Logic
{
    class RevereseTicTacToe
    {
        public static enum eGameMode
        {
            PvP, PvAI,
        }
        public enum eTurns
        {
            Player1 = 1, Player2 = -1, 
        }

        //Ofir - Not so sure if these should be readonly, const or static?
        private const int k_defaultBoardDimension = 3;
        private const int k_GameBoardMinimumDimension = 3;
        private const int k_GameBoardMaximumDimension = 9;
        private const char k_defaultPlayer1Char = 'X';
        private const char k_defaultPlayer2Char = 'O';
        private const eGameMode k_defaultGameMode = eGameMode.PvP;
        private const eTurns k_DefaultFirstTurn = eTurns.Player1;
        private const defaultEmplySlotChar = ' ';

        //members:
        private char m_Player1Char;
        private char m_Player2Char;
        private char[,] m_GameBoard;
        private int m_BoardDimension;
        private eTurns m_CurrentTurn;
        private eGameMode m_GameMode;
        private eTurns m_CurrentTurn;
        private char m_EmptySlotChar;

        
        // Ofir - maybe we should make some other constructors with different parameters
        // to give the user more options - improve reusabillity
        public ReverseTicTacToe(int i_BoardDimension, eGameMode i_GameMode)
        {
            //Ofir - since we didnt learn exceptions yet, how should be handle out of range board size in constructor?
            if(i_BoardDimension < k_GameBoardMinimumDimension || i_BoardDimension > k_GameBoardMaximumDimension)
                m_BoardDimension = k_defaultBoardDimension;
            m_GameMode = i_GameMode;
            m_Player1Char = k_defaultPlayer1Char;
            m_Player2Char = k_defaultPlayer2Char;
            currentTurn = k_DefaultFirstTurn;
            m_GameBoard = new char[m_BoardDimension, m_BoardDimension];
            m_EmptySlotChar = k_DefaultFirstTurn;

            foreach(char currLocation in m_GameBoard)
            {
                currLocation = ' ';
            }
        }
    }
}





































