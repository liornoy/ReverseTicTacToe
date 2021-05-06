using System;


namespace ReverseTicTacToe.Logic
{
    class ReverseTicTacToe
    {
        public enum eGameStatus
        {
            Ongoing,
            Player1Won,
            Player2Won,
            TieGame,
        }

        public enum eLastActionStatus
        {
            Good,
            FailedSlotTaken,
            FailedGameOver,
            FailedOutOfBounds
        }


        public enum eGameMode
        {
            PvP = 1,
            PvC = 2
        }

        public enum eTurns
        {
            Player1,
            Player2,
        }

        private enum eBoardScanningDirections
        {
            Up,
            Down,
            Right,
            Left,
            TopRight,
            TopLeft,
            BottomRight,
            BottomLeft,
        }

        private const char k_AccessToSlotFailedChar = '0';
        private const int k_StartingDefaultBoardDimension = 3;
        private const char k_StartingDefaultPlayer1Char = 'X';
        private const char k_StartingDefaultPlayer2Char = 'O';
        private const char k_StartingDefaultEmptySlotChar = ' ';
        private const int k_MinimumGameBoardDimension = 2;
        private const int k_MaximumGameBoardDimension = 100; //not sure about this
        private const int k_StartingAIProbingDepthDefault = 5;

        private static int s_DefaultBoardDimension = k_StartingDefaultBoardDimension;
        private static char s_DefaultPlayer1Char = k_StartingDefaultPlayer1Char;
        private static char s_DefaultPlayer2Char = k_StartingDefaultPlayer2Char;
        private static eGameMode s_DefaultGameMode = eGameMode.PvP;
        private static eTurns s_DefaultFirstTurn = eTurns.Player1;
        private static char s_DefaultEmptySlotChar = k_StartingDefaultEmptySlotChar;



        //members:
        private char m_Player1Char, m_Player2Char, m_EmptySlotChar;
        private char[,] m_GameBoard;
        private int m_BoardDimension, m_movesMadeCounter;
        private eGameMode m_GameMode;
        private eTurns m_CurrentTurn;
        private eGameStatus m_GameStatus;
        private Player m_Player1, m_Player2;
        private int m_AIProbingDepth;



        // Ofir - maybe we should make some other constructors with different parameters
        // to give the user more options - improve reusabillity
        public ReverseTicTacToe(int i_BoardDimension, eGameMode i_GameMode)
        {
            //Ofir - since we didnt learn exceptions yet, how should be handle out of range board size in constructor?
            m_BoardDimension = IsInRange(i_BoardDimension, k_MinimumGameBoardDimension, k_MaximumGameBoardDimension)
                                   ? i_BoardDimension
                                   : s_DefaultBoardDimension;
            m_GameMode = i_GameMode;
            m_Player1Char = s_DefaultPlayer1Char;
            m_Player2Char = s_DefaultPlayer2Char;
            m_CurrentTurn = s_DefaultFirstTurn;
            m_EmptySlotChar = s_DefaultEmptySlotChar;
            m_GameBoard = new char[m_BoardDimension, m_BoardDimension];
            clearBoard();
            m_Player1 = new Player("Player1");
            m_Player2 = m_GameMode == eGameMode.PvP ? new Player("Player2") : new Player("Computer");
            m_GameStatus = eGameStatus.Ongoing;
            m_movesMadeCounter = 0;
            m_AIProbingDepth = k_StartingAIProbingDepthDefault;

        }

        public int BoardSize
        {
            get
            {
                return m_BoardDimension;
            }
            set
            {
                if (IsBoardClear() && IsInRange(value, k_MinimumGameBoardDimension, k_MaximumGameBoardDimension))
                {
                    m_BoardDimension = value;
                    m_GameBoard = new char[m_BoardDimension, m_BoardDimension];
                    clearBoard();
                }


            }
        }

        public bool IsBoardClear()
        {
            return m_movesMadeCounter == 0;
        }

        public static bool IsInRange(int i_Num, int i_Min, int i_Max)
        {
            return i_Num <= i_Max && i_Num >= i_Min;
        }

        public bool IsSlotIndexWithinBounds(int i_Row, int i_Col)
        {
            return (IsInRange(i_Row, 1, m_BoardDimension) && IsInRange(i_Col, 1, m_BoardDimension));
        }


        public bool AttemptMove(int i_Row, int i_Col, out eLastActionStatus o_MoveStatus)
        {
            o_MoveStatus = eLastActionStatus.Good;
            char charToInsert = GetCurrentPlayerChar();
            const bool v_MoveMadeSuccessfully = true;
            bool isMoveMadeSuccessfully = !v_MoveMadeSuccessfully;

            if (!IsSlotIndexWithinBounds(i_Row, i_Col))
            {
                o_MoveStatus = eLastActionStatus.FailedOutOfBounds;
            }
            else if (IsGameOver())
            {
                o_MoveStatus = eLastActionStatus.FailedGameOver;
            }
            else if (!IsSlotFree(i_Row, i_Col))
            {
                o_MoveStatus = eLastActionStatus.FailedSlotTaken;
            }
            else
            {
                o_MoveStatus = eLastActionStatus.Good;
                makeMove(i_Row, i_Col);
                isMoveMadeSuccessfully = v_MoveMadeSuccessfully;
                if (m_GameMode == eGameMode.PvC && !IsGameOver())
                {
                    makeComputerMove/*Experimental*/();
                }
            }

            return isMoveMadeSuccessfully;
        }

        //This functions assumes the move is legal and can be done.
        private void makeMove(int i_Row, int i_Col)
        {
            m_GameBoard[i_Row - 1, i_Col - 1] = GetCurrentPlayerChar();
            m_movesMadeCounter++;
            if (isSlotPartOfFullSequence(i_Row, i_Col)) // maybe should  have a "switch" statement here?
            {
                if ((m_CurrentTurn == eTurns.Player1))
                {
                    m_GameStatus = eGameStatus.Player2Won;
                    m_Player2.Score++;
                }
                else
                {
                    m_GameStatus = eGameStatus.Player1Won;
                    m_Player1.Score++;
                }
            }

            else if (IsBoardFull())
            {
                m_GameStatus = eGameStatus.TieGame;
            }

            switchTurns();

        }


        private void makeComputerMove()
        {
            Random randomSlotGenerator = new Random();
            //We choose a random slot from the empty ones. for example : id there are currently 20 empty slot,
            //we choose a number between 1 and 20
            //Console.WriteLine(("Number of empty slots:  " + ((m_BoardDimension * m_BoardDimension) - m_movesMadeCounter)));
            int chosenEmptySlotNumber = randomSlotGenerator.Next(1, (m_BoardDimension * m_BoardDimension) - m_movesMadeCounter);
            int i = 0, j = 0;
            int chosenRow = 0, chosenCol = 0;
            int emptySlotCounter = 0;
            bool found = false;

            //Console.WriteLine(("Empty Slot Number Choice  " + chosenEmptySlotNumber));

            while (i < m_BoardDimension && !found)
            {
                j = 0;
                while (j < m_BoardDimension && !found)
                {
                    if (m_GameBoard[i, j] == m_EmptySlotChar)
                    {
                        emptySlotCounter++;
                        if (emptySlotCounter == chosenEmptySlotNumber)
                        {
                            found = true;
                            chosenRow = i + 1;
                            chosenCol = j + 1;
                        }
                    }

                    j++;
                }

                i++;
            }

            //Console.WriteLine("Computer choice : " + chosenRow + ", " + chosenCol);
            makeMove(chosenRow, chosenCol);
        }


        private void makeComputerMoveExperimental()
        {
            int chosenRow, chosenCol;
            chooseSlotAI(out chosenRow, out chosenCol, m_AIProbingDepth, m_BoardDimension * m_BoardDimension - m_movesMadeCounter);
            makeMove(chosenRow, chosenCol);
        }

        /*
        * General concept:
        *
        * "ProbeDepth" - will hold how far ahead are we in the future moves calculations (3 moves ahead, 5 moves ahead...)
        *
        * for each empty slot on the board:
        * check if choosing that slot will cause immediate loss, if yes, return 0, if not return ProbeDepth +
        *   ("winning" scenarioes count * ProbeDepth) +  recursion  ;
        *
        *
        * return the slot with the highest count - high "no-losing" scenarioes, and it's count
        *
        */
        private int chooseSlotAI(out int o_Row, out int o_Col, int i_ProbeDepth, int i_CurrentEmptySlotsToScan)
        {
            o_Row = 0;
            o_Col = 0;
            int overallRating = 0;
            if (i_CurrentEmptySlotsToScan == 0 || i_ProbeDepth == 0)
            {
                return overallRating;
            }

            if(i_CurrentEmptySlotsToScan == 1)
            {
                getNextEmptySlotOnBoard(ref o_Row, ref o_Col);
                return overallRating;
            }


            int currentHighestMoveRating = 0, currentRow = 0, currentCol = 0;

            int currentBestMoveRow = 0, currentBestMoveCol = 0;

            int individualScenarioRating = 0;

            int currentMoveRating = 0;
            int rowWinScan = 0, colWinScan = 0;

            int rowTemp, colTemp;


            for(int i = 1; i <= i_CurrentEmptySlotsToScan; i++)
            {
                getNextEmptySlotOnBoard(ref currentRow, ref currentCol);
                m_GameBoard[currentRow, currentCol] = m_Player2Char; //filling an empty slot

                if(!isSlotPartOfFullSequence(currentRow, currentCol)) //if it's not leading to immediate loss
                {
                    currentMoveRating += i_ProbeDepth; // we increase it's rating
                    for (int j = 1; j < i_CurrentEmptySlotsToScan - 1; j++) //now checking possible moves by the (human) player,
                                                                            //in response to our move
                    {
                        getNextEmptySlotOnBoard(ref rowWinScan, ref colWinScan);
                        m_GameBoard[rowWinScan - 1, colWinScan - 1] = m_Player1Char;
                        if(!isSlotPartOfFullSequence(rowWinScan, colWinScan))
                        {
                            currentMoveRating += i_ProbeDepth; //for each possible winning scenario - increasing that slot's rating
                            overallRating += i_ProbeDepth;
                        }
                        else
                        {
                            //now, for each computer possible move, that doesn't lead to immediate loss, we simulate a move for the (human)
                            //player, and if that doesn't lead to our immediate win - we start probing recursivly.
                            individualScenarioRating = chooseSlotAI(out rowTemp, out colTemp, i_ProbeDepth - 1,
                                i_CurrentEmptySlotsToScan - 1);
                            currentMoveRating += individualScenarioRating;
                            overallRating += individualScenarioRating;
                        }
                        m_GameBoard[rowWinScan - 1, colWinScan - 1] = m_EmptySlotChar;
                    }

                    if(currentMoveRating >= currentHighestMoveRating)
                    {
                        currentHighestMoveRating = currentMoveRating;
                        currentBestMoveRow = currentRow;
                        currentBestMoveCol = currentCol;
                    }
                    m_GameBoard[currentRow, currentCol] = m_EmptySlotChar; //filling an empty slot
                }
            }

            return overallRating;

        }


        private bool getNextEmptySlotOnBoard(ref int io_Row, ref int io_Col)
        {
            int slotScannedCounter = 0;
            const bool v_EmptySlotFound = true;
            bool isEmptySlotFound = !v_EmptySlotFound;

            if (!IsBoardFull())
            {
                while(slotScannedCounter <= m_BoardDimension * m_BoardDimension && !isEmptySlotFound)
                {
                    io_Row = (io_Row == m_BoardDimension) ? 1 : io_Row + 1;
                    while(slotScannedCounter <= m_BoardDimension * m_BoardDimension && !isEmptySlotFound)
                    {
                        io_Col = (io_Col == m_BoardDimension) ? 1 : io_Col + 1;
                        if(m_GameBoard[io_Row - 1, io_Col - 1] == m_EmptySlotChar)
                        {
                            isEmptySlotFound = v_EmptySlotFound;
                        }

                        slotScannedCounter++;
                    }
                }
            }

            return isEmptySlotFound;
        }


        private char[,] cloneGameBoard()
        {
            char[,] gameBoardCopy = new char[m_BoardDimension, m_BoardDimension];
            int chosenRow, chosenCol;
            for (int i = 0; i < m_BoardDimension; i++)
            {
                for (int j = 0; j < m_BoardDimension; j++)
                {
                    gameBoardCopy[i, j] = m_GameBoard[i, j];
                }
            }

            return gameBoardCopy;
        }


        public void RestartGame()
        {
            clearBoard();
            m_CurrentTurn = s_DefaultFirstTurn;
            m_GameStatus = eGameStatus.Ongoing;
        }

        public void ClearScores()
        {
            m_Player1.Score = 0;
            m_Player2.Score = 0;
        }

        public void RestartGameAndClearScores()
        {
            RestartGame();
            ClearScores();
        }


        public bool Forfeit(out eLastActionStatus o_ActionStatus)
        {
            const bool v_ForfeitSuccessful = true;
            bool isForfeitSuccessful = !v_ForfeitSuccessful;
            if (IsGameOver())
            {
                o_ActionStatus = eLastActionStatus.FailedGameOver;
            }
            else
            {
                switch (m_CurrentTurn)
                {
                    case eTurns.Player1:
                        m_Player2.Score++;
                        m_GameStatus = eGameStatus.Player2Won;
                        break;
                    case eTurns.Player2:
                        m_Player1.Score++;
                        m_GameStatus = eGameStatus.Player1Won;
                        break;
                }

                o_ActionStatus = eLastActionStatus.Good;
                isForfeitSuccessful = v_ForfeitSuccessful;
            }

            return isForfeitSuccessful;
        }



        public char GetCurrentPlayerChar()
        {
            char currentPlayerChar = k_StartingDefaultEmptySlotChar;
            if (m_CurrentTurn == eTurns.Player1)
            {
                currentPlayerChar = m_Player1Char;
            }
            else
            {
                currentPlayerChar = m_Player2Char;
            }

            return currentPlayerChar;
        }

        public bool IsBoardFull()
        {
            return m_movesMadeCounter == m_BoardDimension * m_BoardDimension;
        }

        //OFIR - we could make this a bit more efficient by after each direction checking if we've hit a sequence, but that would make the
        //code much uglier, what do you think?
        private bool isSlotPartOfFullSequence(int i_Row, int i_Col)
        {
            const bool v_SequenceFound = true;
            bool isPartOfFullSequence = !v_SequenceFound;
            int verticalSequence = 1 + SequenceSizeFromSlotToDirection(i_Row, i_Col, eBoardScanningDirections.Up)
                                       + SequenceSizeFromSlotToDirection(i_Row, i_Col, eBoardScanningDirections.Down);
            int horizontalSequence = 1 + SequenceSizeFromSlotToDirection(i_Row, i_Col, eBoardScanningDirections.Right)
                                       + SequenceSizeFromSlotToDirection(i_Row, i_Col, eBoardScanningDirections.Left);
            int declinedDiagonalSequence = 1 + SequenceSizeFromSlotToDirection(i_Row, i_Col, eBoardScanningDirections.TopLeft)
                                       + SequenceSizeFromSlotToDirection(i_Row, i_Col, eBoardScanningDirections.BottomRight);
            int inclinedDiagonalSequence = 1 + SequenceSizeFromSlotToDirection(i_Row, i_Col, eBoardScanningDirections.BottomLeft)
                                       + SequenceSizeFromSlotToDirection(i_Row, i_Col, eBoardScanningDirections.TopRight);

            if (verticalSequence == m_BoardDimension || horizontalSequence == m_BoardDimension
                                                    || declinedDiagonalSequence == m_BoardDimension
                                                    || inclinedDiagonalSequence == m_BoardDimension)
            {
                isPartOfFullSequence = v_SequenceFound;
            }

            return isPartOfFullSequence;
        }


        //This function will scan the board, STARTING from the given slot(i_Row, i_Col), and TOWARDS the given direction.
        //It will return how many EQUAL SLOTS (to the starting slot) there are in that direction - NOT including the original slot.
        private int SequenceSizeFromSlotToDirection(int i_Row, int i_Col, eBoardScanningDirections i_ScanningDirection)
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
        private void advanceToNextSlotInGivenDirection(
            ref int io_Row,
            ref int io_Col,
            eBoardScanningDirections i_Direction)
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
            if (!IsSlotIndexWithinBounds(i_Row, i_Col))
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

        private void clearBoard()
        {
            for (int i = 0; i < m_BoardDimension; i++)
            {
                for (int j = 0; j < m_BoardDimension; j++)
                {
                    m_GameBoard[i, j] = m_EmptySlotChar;
                }
            }

            m_movesMadeCounter = 0;
        }

        public int Player1Score
        {
            get
            {
                return m_Player1.Score;
            }
            set
            {
                if (value > 0)
                {
                    m_Player1.Score = value;
                }
            }
        }
        public int Player2Score
        {
            get
            {
                return m_Player2.Score;
            }
            set
            {
                if (value >= 0)
                {
                    m_Player2.Score = value;
                }
            }
        }

        public eGameMode GameMode
        {
            get
            {
                return m_GameMode;
            }
            set
            {
                //INCOMPLETE
            }
        }

        public eTurns CurrentTurn
        {
            get
            {
                return m_CurrentTurn;
            }
            set
            {
                m_CurrentTurn = value;
            }
        }

        public eGameStatus GameStatus
        {
            get
            {
                return m_GameStatus;
            }
        }



        public class Player
        {
            private string m_Name;
            private int m_Score;

            public Player(string i_Name)
            {
                m_Name = new string(i_Name.ToCharArray());
                m_Score = 0;
            }

            public int Score
            {
                get
                {
                    return m_Score;
                }
                set
                {
                    if (value >= 0)
                        m_Score = value;
                }
            }

        }

    }
}