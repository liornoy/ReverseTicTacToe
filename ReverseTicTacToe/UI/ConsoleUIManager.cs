namespace ReverseTicTacToe.UI
{
    using System;
    using System.Text;

    public class ConsoleUIManager
    {
        private const int k_SeparatorBetweenLinesDuplicateFactor = 4;
        private const int k_SeparatorBetweenLinesAddition = 1;
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
        private const char k_LinesSeparatorChar = '=';
        private const char k_ColumnsSeparatorChar = '|';
        private string m_ErrorMessage;
        private bool m_FirstTurn;
        private Logic.ReverseTicTacToe m_Game;

        /// <summary>
        /// This function is the main loop of the game. It prints the board, get user input, and calls logic side to process each move.
        /// </summary>
        public void Run()
        {
            bool gameRunning = true;

            init();
            while (gameRunning)
            {
                if (m_Game.GameStatus == Logic.ReverseTicTacToe.eGameStatus.Ongoing)
                {
                    clearScreenAndPrintGameBoard();
                    printErrorMsgIfAny();
                    makeTurn();
                    m_FirstTurn = false;
                }
                else
                {
                    clearScreenAndPrintGameBoard();
                    printEndGame();
                    gameRunning = askUserForAnotherRound();
                }
            }
        }

        /// <summary>
        /// This function get a string and checks if it is a number in range of i_Min, i_Max
        /// returns true if it in range, and also if true return the number in o_Number,
        /// else if false o_Number will be -1
        /// </summary>
        private static bool checkIsStringRepresentsNumberWithinRangeAndParse(string i_UserInputStr, out int o_Number,
            int i_Min, int i_Max)
        {
            int num;
            const bool v_ValidBoardSizeInput = true;
            bool isValidBoardSizeInput = !v_ValidBoardSizeInput;

            o_Number = k_InitVal;
            if (int.TryParse(i_UserInputStr, out num))
            {
                if (ReverseTicTacToe.Logic.ReverseTicTacToe.IsInRange(num, i_Min, i_Max))
                {
                    o_Number = num;
                    isValidBoardSizeInput = v_ValidBoardSizeInput;
                }
            }

            return isValidBoardSizeInput;
        }

        /// <summary>
        /// Removes all whitespaces from given string
        /// </summary>
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

        /// <summary>
        /// This function initialize the UIManager, gets board size and game type from the user, and
        /// creates the game logic object
        /// </summary>
        private void init()
        {
            m_ErrorMessage = null;
            m_FirstTurn = true; 
            Console.WriteLine("Welcome to Reverse Tic Tac Toe!" + Environment.NewLine);
            int boardSize = getBoardSizeFromUser();
            Logic.ReverseTicTacToe.eGameMode gameMode = getGameTypeFromUser();
            m_Game = new Logic.ReverseTicTacToe(boardSize, gameMode);
        }

        private void printErrorMsgIfAny()
        {
            if (m_ErrorMessage != null)
            {
                Console.WriteLine(m_ErrorMessage);
                m_ErrorMessage = null;
            }
        }

        private bool askUserForAnotherRound()
        {
            string userAnswer;
            const bool v_AnotherRound = true;
            bool anotherRound = v_AnotherRound;
            bool validInput;
            string userMsg = string.Format("Do you want to play another round? ({0}/{1})", k_YesSymbol, k_NoSymbol);

            do
            {
                printErrorMsgIfAny();
                Console.WriteLine(userMsg);
                userAnswer = Console.ReadLine();
                validInput = userAnswer == k_YesSymbol || userAnswer == k_NoSymbol;
                if (!validInput)
                {
                    m_ErrorMessage = "invalid input";
                }
            }

            while (!validInput);

            if (userAnswer == k_YesSymbol)
            {
                m_Game.RestartGame();
                m_FirstTurn = true;
            }

            if (userAnswer == k_NoSymbol)
            {
                Console.WriteLine("Thanks for playing! bye bye!");
                anotherRound = !v_AnotherRound;
            }

            return anotherRound;
        }

        private void clearScreenAndPrintGameBoard()
        {
            StringBuilder boardOutPut = new StringBuilder();
            int boardLength = m_Game.BoardSize;

            Ex02.ConsoleUtils.Screen.Clear();
            Console.WriteLine();

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
                        boardOutPut.AppendFormat("{0} {1} ", k_ColumnsSeparatorChar, symbol);
                    }
                }

                if (i > 0)
                {
                    boardOutPut.Append(k_ColumnsSeparatorChar);
                }
                
                boardOutPut.Append(Environment.NewLine);
                boardOutPut.Append(' ');
                boardOutPut.Append(k_LinesSeparatorChar, (boardLength * k_SeparatorBetweenLinesDuplicateFactor) + k_SeparatorBetweenLinesAddition);
                boardOutPut.Append(Environment.NewLine);
            }

            Console.WriteLine(boardOutPut);
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
                msg.AppendFormat("Game Over! {0} won! Players score:{1}", m_Game.Player1Name, Environment.NewLine);
            }
            else
            {
                msg.AppendFormat("Game Over! {0} won! Players score:{1}", m_Game.Player2Name, Environment.NewLine);
            }

            msg.AppendFormat("{0}: {1}{2}{3}: {4}", m_Game.Player1Name, m_Game.Player1Score, Environment.NewLine, m_Game.Player2Name, m_Game.Player2Score);
            Console.WriteLine(msg);
        }

        private bool getMoveInputOrQuit(out int o_Row, out int o_Col)
        {
            StringBuilder msg = new StringBuilder();
            string userInputStr;
            bool isValidInput;
            string pcTurnPlayedStr = string.Empty;

            if (m_Game.GameMode == Logic.ReverseTicTacToe.eGameMode.PvC && !m_FirstTurn)
            {
                pcTurnPlayedStr = string.Format("{0} has made his move.{1}", m_Game.Player2Name, Environment.NewLine);
            }

            msg.AppendFormat("{0}{1} it's your turn! please enter row,col:", pcTurnPlayedStr, m_Game.CurrentTurn.ToString());
            do
            {
                printErrorMsgIfAny();
                Console.WriteLine(msg);
                userInputStr = Console.ReadLine();
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
        /// Checks if the string represents row,col and returns the row and col if possible
        /// </summary>
        private bool checkIsTurnInputValidAndParse(string i_UserTurnInput, out int o_Row, out int o_Col)
        {
            const bool v_IsValid = true;
            bool turnInputIsValid = !v_IsValid;

            o_Row = k_InitVal;
            o_Col = k_InitVal;
            i_UserTurnInput = removeWhiteSpaces(i_UserTurnInput);
            if (i_UserTurnInput == k_QuitSymbol)
            {
                turnInputIsValid = v_IsValid;
            }
            else
            {
                // Expecting here to have the following string: 
                // "i,j" - where i is row and j is col.
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
        
        private int getBoardSizeFromUser()
        {
            string userInputStr;
            int boardSize;
            bool isValidInput;
            string msg = string.Format("Please enter the board size (choose between: {0} - {1})",
                              k_MinBoardSize, k_MaxBoardSize);

            do
            {
                printErrorMsgIfAny();
                Console.WriteLine(msg);
                userInputStr = Console.ReadLine();
                isValidInput = checkIsStringRepresentsNumberWithinRangeAndParse(userInputStr, out boardSize,
                    k_MinBoardSize, k_MaxBoardSize);
                if (!isValidInput)
                {
                    m_ErrorMessage = "invalid input!";
                    Ex02.ConsoleUtils.Screen.Clear();
                }
            }

            while (!isValidInput);
            
            return boardSize;
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
                printErrorMsgIfAny();
                Console.WriteLine(msg);
                userInputStr = Console.ReadLine();
                isValidInput = checkIsStringRepresentsNumberWithinRangeAndParse(userInputStr,
                    out gameTypeNum, k_MinGameType, k_MaxGameType);
                if (!isValidInput)
                {
                    m_ErrorMessage = "invalid input!";
                    Ex02.ConsoleUtils.Screen.Clear();
                }
            }

            while (!isValidInput);

            return (Logic.ReverseTicTacToe.eGameMode)gameTypeNum;
        }
    }
}
