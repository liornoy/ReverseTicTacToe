using System;
using System.Text;
using static System.Console;
using ReverseTicTacToe.Logic;
using Ex02.ConsoleUtils;    

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
        private const int k_MinBoardIndex = 0;
        private const int k_RowIndex = 0;
        private const int k_ColIndex = 1;

        private const string k_QuitSymbol = "Q";
        private string errorMessage;

        private Logic.ReverseTicTacToe m_Game;
        public ConsoleUIManager()
        {
        }

        public  void Init()
        {
            Logic.ReverseTicTacToe.eGameMode gameMode;
            WriteLine("Welcome to Reverse Tic Tac Toe!" + Environment.NewLine);
            int boardSize = getBoardSizeFromUser();
            gameMode = getGameTypeFromUser();
            m_Game = new Logic.ReverseTicTacToe(boardSize, gameMode);
        }
        public  void Run()
        {
            errorMessage = "";
            bool gameRunning = true;
            Init();
            while (gameRunning) // is it ok to be always true?
            {
                if(m_Game.GameStatus == Logic.ReverseTicTacToe.eGameStatus.Ongoing)
                {
                    printGameBoard();
                    if(errorMessage != "")
                    {
                        WriteLine(errorMessage);
                        errorMessage = "";
                    }
                    makeTurn();
                }
                else
                {
                    printEndGame();
                    // Need to ask if want to play again, if yes Reset Logic.
                }
            }
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
                        char sybmol;
                        m_Game.GetSlotContentByIndex(i, j, out sybmol);
                        boardOutPut.AppendFormat("| {0} ", sybmol);
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
            int row, col;
            bool isMoveMadeSuccessfully;
            getMoveInput(out row, out col);
            isMoveMadeSuccessfully = m_Game.AttemptMove(row, col, out lastActionStatus);
            if (isMoveMadeSuccessfully)
            {

            }
            else
            {
               // Logic.ReverseTicTacToe.eLastActionStatus.FailedOutOfBounds - will never happen because
               // I stop the user from advanced from entering out of bounds values.
                if (lastActionStatus == Logic.ReverseTicTacToe.eLastActionStatus.FailedSlotTaken)
                {
                    errorMessage = "Slot taken";
                }
            }
        }

        private void printEndGame()
        {
            StringBuilder msg = new StringBuilder();
            if(m_Game.GameStatus == Logic.ReverseTicTacToe.eGameStatus.TieGame)
            {
                msg.Append("Game Over! It's a tie! Players score:");
            }
            if (m_Game.GameStatus == Logic.ReverseTicTacToe.eGameStatus.Player1Won)
            {
                msg.Append("Game Over! Player1 won! Players score:");
            }
            else
            {
                msg.Append("Game Over! Player2 won! Players score:");
            }
            msg.AppendFormat("Player1: {0}{1}Player2: {2}", m_Game.Player1Score, Environment.NewLine, m_Game.Player2Score);
            WriteLine(msg);
        }

        private void getMoveInput(out int o_Row, out int o_Col)
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
                    int row, col;
                    if(int.TryParse(splitedUserInputStrings[k_RowIndex], out row)
                       && int.TryParse(splitedUserInputStrings[k_ColIndex], out col))
                    {
                        if(isInRage(row, k_MinBoardIndex, m_Game.BoardSize) &&
                           isInRage(col, k_MinBoardIndex, m_Game.BoardSize))
                        {
                            turnInputIsValid = v_IsValid;
                            o_Row = row;
                            o_Col = col;
                        }
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
            msg.AppendFormat("Please enter the board size (choose between: {0} - {1})", k_MinBoardSize, k_MaxBoardSize);
            msg.Append(Environment.NewLine);
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
        public Logic.ReverseTicTacToe.eGameMode getGameTypeFromUser()
        {
            string userInputStr;
            int gameTypeNum;
            StringBuilder msg = new StringBuilder();
            msg.AppendFormat("Please enter the game type ('{0}' for {1}, '{2}' for {3}):",
                (int)Logic.ReverseTicTacToe.eGameMode.PvP, Logic.ReverseTicTacToe.eGameMode.PvP,
                (int)Logic.ReverseTicTacToe.eGameMode.PvC,Logic.ReverseTicTacToe.eGameMode.PvC);

            msg.Append(Environment.NewLine);
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

