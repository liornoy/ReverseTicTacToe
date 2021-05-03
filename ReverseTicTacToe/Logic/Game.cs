using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReverseTicTacToeLogic
{
    class ReverseTicTacToe
    {
        public enum eGameStatus
        {
            ClearBoard,
            Ongoing,
            Player1Won,
            Player2Won,
            TieGame,
        }

        public enum eLastMoveStatus
        {
            Good, FailedSlotTaken, FailedGameOver, FailedOutOfBounds
        }


        public enum eGameMode
        {
            PvP,
            PvC
        }
        public enum eTurns
        {
            Player1, Player2
        }

        private enum eBoardScanningDirections
        {
            Up, Down, Right, Left, TopRight, TopLeft, BottomRight, BottomLeft,
        }


        private const char k_AccessToSlotFailedChar = '0';
        private static int s_DefaultBoardDimension = 3;
        private static int s_I_GameBoardMinimumDimension = 3;
        private static int s_GameBoardMaximumDimension = 9;
        private static char s_DefaultPlayer1Char = 'X';
        private static char s_DefaultPlayer2Char = 'O';
        private static eGameMode s_DefaultGameMode = eGameMode.PvP;
        private static eTurns s_DefaultFirstTurn = eTurns.Player1;
        private static char s_DefaultEmptySlotChar = ' ';


        //members:
        private char m_Player1Char, m_Player2Char, m_EmptySlotChar;
        private char[,] m_GameBoard;
        private int m_BoardDimension, m_Player1Score, m_Player2Score, m_FilledSlotsCounter;
        private eGameMode m_GameMode;
        private eTurns m_CurrentTurn;
        private eGameStatus m_GameStatus;



        // Ofir - maybe we should make some other constructors with different parameters
        // to give the user more options - improve reusabillity
        public ReverseTicTacToe(int i_BoardDimension, eGameMode i_GameMode)
        {
            //Ofir - since we didnt learn exceptions yet, how should be handle out of range board size in constructor?
            m_BoardDimension = IsInRange(i_BoardDimension, s_GameBoardMaximumDimension, s_I_GameBoardMinimumDimension)
                                   ? i_BoardDimension : s_DefaultBoardDimension;
            m_GameMode = i_GameMode;
            m_Player1Char = s_DefaultPlayer1Char;
            m_Player2Char = s_DefaultPlayer2Char;
            m_CurrentTurn = s_DefaultFirstTurn;
            m_EmptySlotChar = s_DefaultEmptySlotChar;
            m_GameBoard = new char[m_BoardDimension, m_BoardDimension];
            ClearBoard();
            m_Player1Score = 0;
            m_Player2Score = 0;
            m_GameStatus = eGameStatus.ClearBoard;
            m_FilledSlotsCounter = 0;

        }

        public int BoardSize
        {
            get
            {
                return m_BoardDimension;
            }
            set
            {
                if(m_GameStatus == eGameStatus.ClearBoard && IsInRange(value, s_I_GameBoardMinimumDimension,
                       s_GameBoardMaximumDimension ))
                {
                    m_BoardDimension = value;
                    m_GameBoard = new char[m_BoardDimension, m_BoardDimension];
                    ClearBoard();
                }

            }
        }

        public static bool IsInRange(int i_Num, int i_Min, int i_Max)
        {
            return i_Num <= i_Max && i_Num >= i_Min;
        }

        public bool IsSlotIndexWithinBounds(int i_Row, int i_Col)
        {
            return (IsInRange(i_Row, 1, m_BoardDimension) && IsInRange(i_Col, 1, m_BoardDimension));
        }



        public bool AttemptMove(int i_Row, int i_Col, out eLastMoveStatus o_MoveStatus)
        {
            o_MoveStatus = eLastMoveStatus.Good;
            char charToInsert = (m_CurrentTurn == eTurns.Player1) ? m_Player1Char : m_Player2Char;
            const bool v_MoveMadeSuccessfully = true;
            bool moveMade = !v_MoveMadeSuccessfully;

            if(!IsSlotIndexWithinBounds(i_Row, i_Col))
            {
                o_MoveStatus = eLastMoveStatus.FailedOutOfBounds;
            }
            else if(IsGameOver())
            {
                o_MoveStatus = eLastMoveStatus.FailedGameOver;
            }
            else if(!IsSlotFree(i_Row, i_Col))
            {
                o_MoveStatus = eLastMoveStatus.FailedSlotTaken;
            }
            else
            {
                o_MoveStatus = eLastMoveStatus.Good;
                makeMove(i_Row, i_Col);
                moveMade = v_MoveMadeSuccessfully;
            }

            return moveMade;
        }

        //This functions assumes the move is legal and can be done.
        private void makeMove(int i_Row, int i_Col)
        {
            m_GameBoard[i_Row - 1, i_Col - 1] = (m_CurrentTurn == eTurns.Player1) ? m_Player1Char : m_Player2Char;
            if(isSlotPartOfFullSequence(i_Row, i_Col))
            {
                m_GameStatus = (m_CurrentTurn == eTurns.Player1) ? eGameStatus.Player2Won : eGameStatus.Player2Won;
                if((m_CurrentTurn == eTurns.Player1))
                {
                    m_Player2Score++;
                }
                else
                {
                    m_Player1Score++;
                }
            }
            else
            {
                switchTurns();
            }
        }

        //OFIR - we could make this a bit more efficient by after each direction checking if we've hit a sequence, but that would make the
        //code much uglier, what do you think?
        private bool isSlotPartOfFullSequence(int i_Row, int i_Col)
        {
            const bool v_SequenceFound = true;
            bool isPartOfFullSequence = !v_SequenceFound;
            int verticalSequence = 1 + SequenceSizeFromSlotToDirection(i_Row, i_Col, eBoardScanningDirections.Up) +
                                     SequenceSizeFromSlotToDirection(i_Row, i_Col, eBoardScanningDirections.Down);
            int horizontalSequence = 1 + SequenceSizeFromSlotToDirection(i_Row, i_Col, eBoardScanningDirections.Right) +
                                     SequenceSizeFromSlotToDirection(i_Row, i_Col, eBoardScanningDirections.Left);
            int declinedDiagonalSequence = 1 + SequenceSizeFromSlotToDirection(i_Row, i_Col, eBoardScanningDirections.TopLeft) +
                                     SequenceSizeFromSlotToDirection(i_Row, i_Col, eBoardScanningDirections.BottomRight);
            int inclinedDiagonalSequence = 1 + SequenceSizeFromSlotToDirection(i_Row, i_Col, eBoardScanningDirections.BottomLeft) +
                                     SequenceSizeFromSlotToDirection(i_Row, i_Col, eBoardScanningDirections.TopRight);

            if(verticalSequence == m_BoardDimension || horizontalSequence == m_BoardDimension
                                                    || declinedDiagonalSequence == m_BoardDimension
                                                    || inclinedDiagonalSequence == m_BoardDimension)
            {
                isPartOfFullSequence = v_SequenceFound;
            }

            return isPartOfFullSequence;
        }


        //This function will scan the board, STARTING from the given slot(i_Row, i_Col), and TOWARDS the given direction.
        //It will return how many EQUAL SLOTS (to the starting slot) there are in that direction - NOT including the original slot.
        private int SequenceSizeFromSlotToDirection(int i_Row,int i_Col, eBoardScanningDirections i_ScanningDirection)
        {
            const bool v_insideSequence = true;
            bool isInsideSequence = v_insideSequence;
            int sequenceSize = 0;
            int nextRow = i_Row, nextCol = i_Col;
            advanceToNextSlotInGivenDirection(ref nextRow, ref nextCol, i_ScanningDirection);

            while (IsSlotIndexWithinBounds(nextRow, nextCol) && isInsideSequence)
            {
                if (m_GameBoard[i_Row - 1, i_Col - 1] == m_GameBoard[nextRow - 1, nextCol - 1])
                {
                    sequenceSize++;
                    advanceToNextSlotInGivenDirection(ref nextRow, ref nextCol, i_ScanningDirection);
                }
                else
                {
                    isInsideSequence = !v_insideSequence;
                }
            }

            return sequenceSize;
        }

        //NOTE - This function does NOT check for out of bounds when advancing the indexes.
        private void advanceToNextSlotInGivenDirection(ref int io_Row, ref int io_Col, eBoardScanningDirections i_Direction)
        {
            switch (i_Direction)
            {
                case eBoardScanningDirections.Up:
                    io_Row--;
                    break;
                case eBoardScanningDirections.TopRight:
                    io_Row--;
                    io_Col++;
                    break;
                case eBoardScanningDirections.BottomLeft:
                    io_Row++;
                    io_Col--;
                    break;
                case eBoardScanningDirections.BottomRight:
                    io_Row++;
                    io_Col++;
                    break;
                case eBoardScanningDirections.Down:
                    io_Row++;
                    break;
                case eBoardScanningDirections.Left:
                    io_Col--;
                    break;
                case eBoardScanningDirections.Right:
                    io_Col++;
                    break;
                case eBoardScanningDirections.TopLeft:
                    io_Row--;
                    io_Col--;
                    break;
            }
        }


        private void switchTurns()
        {
            m_CurrentTurn = (m_CurrentTurn == eTurns.Player1) ? eTurns.Player2 : eTurns.Player1;
        }

        public bool IsGameOver()
        {
            return (m_GameStatus == eGameStatus.Player1Won || m_GameStatus == eGameStatus.Player2Won
                                                          || m_GameStatus == eGameStatus.TieGame);
        }

        public bool IsSlotFree(int i_Row, int i_Col)
        {
            const bool v_SlotFree = true;
            bool isSlotFree;
            if(!IsSlotIndexWithinBounds(i_Row, i_Col))
                isSlotFree = !v_SlotFree;
            else
            {
                isSlotFree = (m_GameBoard[i_Row - 1, i_Col - 1] == m_EmptySlotChar);
            }

            return isSlotFree;
        }


        public bool GetSlotContentByIndex(int i_Row, int i_Col, out char o_Char)
        {
            const bool v_Successful = true;
            bool isSuccessful = !v_Successful;
            if (!IsSlotIndexWithinBounds(i_Row, i_Col))
            {
                o_Char = k_AccessToSlotFailedChar;
            }
            else
            {
                o_Char = m_GameBoard[i_Row - 1, i_Col - 1];
                isSuccessful = v_Successful;
            }
            return isSuccessful;
        }

        public void ClearBoard()
        {
            //maybe switch to foreach here?
            for (int i = 0; i < m_BoardDimension; i++)
            {
                for (int j = 0; j < m_BoardDimension; j++)
                {
                    m_GameBoard[i, j] = m_EmptySlotChar;
                }
            }
        }
    }


}
