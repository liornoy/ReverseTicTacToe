using System;
using System.Text;
using static System.Console;

namespace ReverseTicTacToe.UI
{
    class ConsoleUIManager
    {
        private const int k_MinBoardSize = 3;
        private const int k_MaxBoardSize = 9;
        private const int k_MinGameType = 1;
        private const int k_MaxGameType = 2;
        private const int k_NumOfDimensions = 2;
        private const int k_InitVal = -1;
        private const int k_RowIndex = 0;
        private const int k_ColIndex = 1;
        private const string k_QuitSymbol = "Q";
        private const string k_YesSymbol = "y";
        private const string k_NoSymbol = "n";
        private string m_ErrorMessage;
        private bool m_firstTurn;
        

        private Logic.ReverseTicTacToe m_Game;
        private void init()
        {
            WriteLine("Welcome to Reverse Tic Tac Toe!" + Environment.NewLine);
            int boardSize = getBoardSizeFromUser();
            Logic.ReverseTicTacToe.eGameMode gameMode = getGameTypeFromUser();
            m_Game = new Logic.ReverseTicTacToe(boardSize, gameMode);
            m_ErrorMessage = "";
            m_firstTurn = true;
        }
        public void Run()
        {
            bool gameRunning = true;
            init();
            while (gameRunning)
            {
                if (m_Game.GameStatus == Logic.ReverseTicTacToe.eGameStatus.Ongoing)
                {
                    clearScreenAndPrintGameBoard();
                    writeErrorMsgIfAny();
                    makeTurn();
                    m_firstTurn = false;
                }
                else
                {
                    clearScreenAndPrintGameBoard();
                    printEndGame();
                    gameRunning = askUserForAnotherRound();
                }
            }
        }

        private void writeErrorMsgIfAny()
        {
            if (m_ErrorMessage != "")
            {
                WriteLine(m_ErrorMessage);
                m_ErrorMessage = "";
            }
        }

        private bool askUserForAnotherRound()
        {
            string userAnswer;
            const bool v_AnotherRound = true;
            bool anotherRound = v_AnotherRound;
            WriteLine(string.Format("Do you want to play another round? ({0}/{1})",k_YesSymbol, k_NoSymbol ));
            do
            {
                userAnswer = ReadLine();
            }
            while (userAnswer != k_YesSymbol && userAnswer != k_NoSymbol);

            if (userAnswer == k_YesSymbol)
            {
                m_Game.RestartGame();
            }

            if (userAnswer == k_NoSymbol)
            {
                WriteLine("Thanks for playing! bye bye!");
                anotherRound = !v_AnotherRound;
            }

            return anotherRound;
        }

        private void clearScreenAndPrintGameBoard()
        {
            //Ex02.ConsoleUtils.Screen.Clear(); REMOVED ONLY FOR DEBBUG PURPOSE

            Console.WriteLine();

            StringBuilder boardOutPut = new StringBuilder();
            int boardLength = m_Game.BoardSize;
            for (int i = 0; i <= boardLength; i++)
            {
                for (int j = 0; j <= boardLength; j++)
                {
                    if (i == 0 && j == 0)
                    {
                        boardOutPut.Append("   ");
                    }
                    else if (i == 0)
                    {
                        boardOutPut.AppendFormat("{0}   ", j);
                    }
                    else if (j == 0)
                    {
                        boardOutPut.Append(i);
                    }
                    else
                    {
                        char symbol;
                        m_Game.GetSlotContentByIndex(i, j, out symbol);
                        boardOutPut.AppendFormat("| {0} ", symbol);
                    }
                }

                if (i > 0)
                {
                    boardOutPut.Append("|");
                }

                boardOutPut.Append(Environment.NewLine);
                boardOutPut.Append(' ');
                boardOutPut.Append('=', (boardLength * 4) + 1);
                boardOutPut.Append(Environment.NewLine);
            }
            WriteLine(boardOutPut);
        }
        private void makeTurn()
        {
            Logic.ReverseTicTacToe.eLastActionStatus lastActionStatus;
            int playerRowInput, playerColInput;

            bool playerQuit = getMoveInputOrQuit(out playerRowInput, out playerColInput);
            if (playerQuit)
            {
                m_Game.Forfeit(out lastActionStatus);
            }
            else
            {
                bool isMoveMadeSuccessfully = m_Game.AttemptMove(playerRowInput, playerColInput, out lastActionStatus);
                if (!isMoveMadeSuccessfully)
                {
                    if (lastActionStatus == Logic.ReverseTicTacToe.eLastActionStatus.FailedSlotTaken)
                    {
                        m_ErrorMessage = "Slot taken";
                    }
                    else if (lastActionStatus == Logic.ReverseTicTacToe.eLastActionStatus.FailedOutOfBounds)
                    {
                        m_ErrorMessage = "Index out of bounds";
                    }
                }
            }
        }

        private void printEndGame()
        {
            StringBuilder msg = new StringBuilder();
            if (m_Game.GameStatus == Logic.ReverseTicTacToe.eGameStatus.TieGame)
            {
                msg.AppendFormat("Game Over! It's a tie! Players score:{0}", Environment.NewLine);
            }
            else if (m_Game.GameStatus == Logic.ReverseTicTacToe.eGameStatus.Player1Won)
            {
                msg.AppendFormat("Game Over! Player1 won! Players score:{0}", Environment.NewLine);
            }
            else
            {
                msg.AppendFormat("Game Over! Player2 won! Players score:{0}", Environment.NewLine);
            }
            msg.AppendFormat("Player1: {0}{1}Player2: {2}", m_Game.Player1Score, Environment.NewLine, m_Game.Player2Score);
            WriteLine(msg);
        }

        private bool getMoveInputOrQuit(out int o_Row, out int o_Col)
        {
            StringBuilder msg = new StringBuilder();
            string userInputStr;
            bool isValidInput;
            string pcTurnPlayedStr = "";
           if(m_Game.GameMode == Logic.ReverseTicTacToe.eGameMode.PvC && !m_firstTurn) {
                pcTurnPlayedStr = String.Format("Computer made his move.{0}", Environment.NewLine);
           }
            msg.AppendFormat("{0}{1} it's your turn! please enter row,col:", pcTurnPlayedStr, m_Game.CurrentTurn.ToString());
            do
            {
                writeErrorMsgIfAny();
                WriteLine(msg);
                userInputStr = ReadLine();
                isValidInput = checkIsTurnInputValidAndParse(userInputStr, out o_Row, out o_Col);
                if (!isValidInput)
                {
                    m_ErrorMessage = "invalid input!";
                    clearScreenAndPrintGameBoard();
                }
            }
            while (!isValidInput);

            return userInputStr == k_QuitSymbol;
        }
        /// <summary>
        /// Checks if the string represents row,col
        /// </summary>
        private bool checkIsTurnInputValidAndParse(string i_UserTurnInput, out int o_Row, out int o_Col)
        {
            o_Row = k_InitVal;
            o_Col = k_InitVal;
            const bool v_IsValid = true;
            bool turnInputIsValid = !v_IsValid;
            i_UserTurnInput = removeWhiteSpaces(i_UserTurnInput);
            if (i_UserTurnInput == k_QuitSymbol)
            {
                turnInputIsValid = v_IsValid;
            }
            // Expecting here to have the following string: 
            // "i,j" - where i is row and j is col.
            else
            {
                string[] splitedUserInputStrings = i_UserTurnInput.Split(',');
                if (splitedUserInputStrings.Length == k_NumOfDimensions)
                {
                    if (int.TryParse(splitedUserInputStrings[k_RowIndex], out o_Row)
                       && int.TryParse(splitedUserInputStrings[k_ColIndex], out o_Col))
                    {
                        turnInputIsValid = v_IsValid;
                    }
                }
            }

            return turnInputIsValid;
        }
        private static string removeWhiteSpaces(string i_UserTurnInput)
        {
            StringBuilder stringBuilder = new StringBuilder(i_UserTurnInput);
            int index = stringBuilder.ToString().IndexOf(' ');
            while (index != -1)
            {
                stringBuilder.Remove(index, 1);
                index = stringBuilder.ToString().IndexOf(' ');
            }
            return stringBuilder.ToString();
        }
        private int getBoardSizeFromUser()
        {
            string userInputStr;
            int boardSize;
            StringBuilder msg = new StringBuilder();
            bool isValidInput;
            msg.AppendFormat("Please enter the board size (choose between: {0} - {1})",
                              k_MinBoardSize, k_MaxBoardSize);
            do
            {
                writeErrorMsgIfAny();
                WriteLine(msg);
                userInputStr = ReadLine();
                isValidInput = checkIsStringRepresentsNumberWithinRangeAndParse(
                    userInputStr,
                    out boardSize,
                    k_MinBoardSize,
                    k_MaxBoardSize);
                if (!isValidInput)
                {
                    m_ErrorMessage = "invalid input!";
                    Ex02.ConsoleUtils.Screen.Clear();
                }
            }

            while (!isValidInput);

            return boardSize;
        }
        /// <summary>
        /// This function get a string and checks if it is a number in range of i_Min, i_Max
        /// returns true if it in range, and also if true return the number in o_Number,
        /// else if false o_Number will be -1
        /// </summary>
        public bool checkIsStringRepresentsNumberWithinRangeAndParse(string i_UserInputStr, out int o_Number, int i_Min, int i_Max)
        {
            int boardSize;
            const bool v_ValidBoardSizeInput = true;
            bool isValidBoardSizeInput = !v_ValidBoardSizeInput;
            o_Number = k_InitVal;
            if (int.TryParse(i_UserInputStr, out boardSize))
            {
                if (isInRage(boardSize, i_Min, i_Max))
                {
                    o_Number = boardSize;
                    isValidBoardSizeInput = v_ValidBoardSizeInput;
                }
            }

            return isValidBoardSizeInput;
        }
        private static bool isInRage(int i_Num, int i_Min, int i_Max)
        {
            return i_Num >= i_Min && i_Num <= i_Max;
        }
        private Logic.ReverseTicTacToe.eGameMode getGameTypeFromUser()
        {
            string userInputStr;
            int gameTypeNum;
            StringBuilder msg = new StringBuilder();
            bool isValidInput;
            msg.AppendFormat("Please enter the game type ('{0}' for {1}, '{2}' for {3}):",
                (int)Logic.ReverseTicTacToe.eGameMode.PvP, Logic.ReverseTicTacToe.eGameMode.PvP,
                (int)Logic.ReverseTicTacToe.eGameMode.PvC, Logic.ReverseTicTacToe.eGameMode.PvC);
            do
            {
                writeErrorMsgIfAny();
                WriteLine(msg);
                userInputStr = ReadLine();
                isValidInput = checkIsStringRepresentsNumberWithinRangeAndParse(
                    userInputStr,
                    out gameTypeNum,
                    k_MinGameType,
                    k_MaxGameType);
                if (!isValidInput)
                {
                    m_ErrorMessage = "invalid input!";
                    Ex02.ConsoleUtils.Screen.Clear();
                }
            }
            while (!isValidInput);

            return (Logic.ReverseTicTacToe.eGameMode)(gameTypeNum);
        }
    }
}
