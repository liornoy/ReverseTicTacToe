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
        private string errorMessage;

        private Logic.ReverseTicTacToe m_Game;
        private  void init()
        {
            WriteLine("Welcome to Reverse Tic Tac Toe!" + Environment.NewLine);
            int boardSize = getBoardSizeFromUser();
            Logic.ReverseTicTacToe.eGameMode gameMode = getGameTypeFromUser();
            m_Game = new Logic.ReverseTicTacToe(boardSize, gameMode);
        }
        public  void Run()
        {
            errorMessage = "";
            bool gameRunning = true;
            init();
            while (gameRunning)
            {
                if(m_Game.GameStatus == Logic.ReverseTicTacToe.eGameStatus.Ongoing)
                {
                    printGameBoard();
                    writeErrorMsgIfAny(ref errorMessage);
                    makeTurn();
                }
                else
                {
                    printGameBoard();
                    printEndGame();
                    gameRunning = askUserForAnotherRound();
                }
            }
        }

        private void writeErrorMsgIfAny(ref string i_ErrorMessage)
        {
            if (errorMessage != "")
            {
                WriteLine(errorMessage);
                errorMessage = "";
            }
        }

        private bool askUserForAnotherRound()
        {
            string userAnswer;
            const bool v_AnotherRound = true;
            bool anotherRound = v_AnotherRound;
            WriteLine("Do you want to play another round? (y/n)");
            do
            {
                userAnswer = ReadLine();
            }
            while(userAnswer != "y" && userAnswer != "n");

            if(userAnswer == "y")
            {
                m_Game.RestartGame();
            }

            if(userAnswer == "n")
            {
                WriteLine("Thanks for playing! bye bye!");
                anotherRound = !v_AnotherRound;
            }

            return anotherRound;
        }

        private void printGameBoard()
        {
            Ex02.ConsoleUtils.Screen.Clear();
            StringBuilder boardOutPut = new StringBuilder();
            int boardLength = m_Game.BoardSize;
            for (int i = 0; i <= boardLength; i++)
            {
                for (int j = 0; j <= boardLength; j++)
                {
                    if (i == 0 && j == 0)
                    {
                        boardOutPut.Append("  ");
                    }
                    else if (i == 0)
                    {
                        boardOutPut.AppendFormat("{0}   ",j);
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
            if(playerQuit)
            {
                m_Game.Forfeit(out lastActionStatus);
            }
            else
            {
                bool isMoveMadeSuccessfully = m_Game.AttemptMove(playerRowInput, playerColInput, out lastActionStatus);
                if (!isMoveMadeSuccessfully) {
                    if (lastActionStatus == Logic.ReverseTicTacToe.eLastActionStatus.FailedSlotTaken)
                    {
                        errorMessage = "Slot taken";
                    }
                    else if(lastActionStatus == Logic.ReverseTicTacToe.eLastActionStatus.FailedOutOfBounds)
                    {
                        errorMessage = "Index out of bounds";
                    }
                }
            }
        }

        private void printEndGame()
        {
            StringBuilder msg = new StringBuilder();
            if(m_Game.GameStatus == Logic.ReverseTicTacToe.eGameStatus.TieGame)
            {
                msg.AppendFormat("Game Over! It's a tie! Players score:{0}", Environment.NewLine);
            }
            if (m_Game.GameStatus == Logic.ReverseTicTacToe.eGameStatus.Player1Won)
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
            msg.AppendFormat("{0} it's your turn! please enter row,col:", m_Game.CurrentTurn.ToString());
            do
            {
                WriteLine(msg);
                userInputStr = ReadLine();
            }
            while(!isTurnInputValid(userInputStr, out o_Row, out o_Col));

            return userInputStr == k_QuitSymbol;
        }
        /// <summary>
        /// Checks if the string represents row,col
        /// </summary>
        private bool isTurnInputValid(string i_UserTurnInput, out int o_Row, out int o_Col)
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
                if(splitedUserInputStrings.Length == k_NumOfDimensions)
                {
                    if(int.TryParse(splitedUserInputStrings[k_RowIndex], out o_Row)
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
            msg.AppendFormat("Please enter the board size (choose between: {0} - {1})",
                              k_MinBoardSize, k_MaxBoardSize);
            do
            {
                WriteLine(msg);
                userInputStr = ReadLine();
            }

            while (!isStringRepresentsNumberWithinRange(userInputStr, out boardSize, k_MinBoardSize, k_MaxBoardSize));

            return boardSize;
        }
        /// <summary>
        /// This function get a string and checks if it is a number in range of i_Min, i_Max
        /// returns true if it in range, and also if true return the number in o_Number,
        /// else if false o_Number will be -1
        /// </summary>
        public bool isStringRepresentsNumberWithinRange(string i_UserInputStr, out int o_Number, int i_Min, int i_Max)
        {
            int boardSize;
            const bool v_ValidBoardSizeInput = true;
            bool isValidBoardSizeInput = !v_ValidBoardSizeInput;
            o_Number = k_InitVal;
            if(int.TryParse(i_UserInputStr, out boardSize))
            {
                if(isInRage(boardSize, i_Min, i_Max))
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
            msg.AppendFormat("Please enter the game type ('{0}' for {1}, '{2}' for {3}):",
                (int)Logic.ReverseTicTacToe.eGameMode.PvP, Logic.ReverseTicTacToe.eGameMode.PvP,
                (int)Logic.ReverseTicTacToe.eGameMode.PvC,Logic.ReverseTicTacToe.eGameMode.PvC);
            do
            {
                WriteLine(msg);
                userInputStr = ReadLine();
            }
            while (!isStringRepresentsNumberWithinRange(userInputStr, out gameTypeNum, k_MinGameType, k_MaxGameType));

            return (Logic.ReverseTicTacToe.eGameMode)(gameTypeNum);
        }
    }
}

