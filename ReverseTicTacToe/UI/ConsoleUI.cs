using System;
using System.Reflection.Emit;
using System.Text;
using ReverseTicTacToe.Logic;
using static System.Console;

namespace ReverseTicTacToe.UI
{
    class ConsoleUI
    {
        private Game m_Game;
        private const int QUIT = -1;
        private const string QUITSYMBOL = "Q";
        private static ConsoleUI instance = null;
        private ConsoleUI()
        {
        }
        public static ConsoleUI Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new ConsoleUI();
                }
                return instance;
            }
        }
        public  void Init()
        {
        }
        public  void Run()
        {
            
            WriteLine("Welcome to Reverse Tic Tac Toe!" + Environment.NewLine);
            int BoardSize =  getBoardSizeFromUser();
            Game.eGameTypes GameType = getGameTypeFromUser();
            m_Game = new Game(BoardSize, GameType);
           // while(true)
            //{
            printGameBoard();
            playTurn();
            ReadLine();
            
            //}
        }

        private void playTurn()
        {
            int row = 0;
            int col = 0;
            //int return status: continue, place_taken, finished game
            
            if(m_Game.CurrentTurn == 1 || m_Game.GameType == Game.eGameTypes.PVP)
            {
Label: 
                getRowColFromUser(out row, out col);
                string msg ="";
                int retStatus;
                if (row == QUIT) // if press Q
                {
                    m_Game.Playerforfeit();
                     msg = getEndGameMsg(Game.StatusWin);
                }
                // if Place is taken/ win/ tie
                else if(!m_Game.PlaceSymbolAndReturnStatus(row, col, out retStatus))
                {
                    if(retStatus == Game.StatusTaken)
                    {
                        WriteLine("Place is taken!");
                        goto Label;
                        
                    }
                    else
                    {
                         msg = getEndGameMsg(retStatus);
                    }
                }
                WriteLine(msg);
            }
            else
            {

            }
        }

        private string getEndGameMsg(int i_RetStatus)
        {
            StringBuilder stringBuilder = new StringBuilder();
            if(i_RetStatus == Game.StatusWin)
            {
                //write who wins
            }
            else
            {
                stringBuilder.Append("This is a Tie!");
            }
           // stringBuilder.AppendFormat("Score is: {0} Player1: {1}", Environment.NewLine, m_Game
            return stringBuilder.ToString();
        }

        private void getRowColFromUser(out int i_Row, out int i_Col)
        {
            string turnMsg = GetTurnMessage();
            string userTurnInput;
            do
            {
                WriteLine(turnMsg);
                userTurnInput = ReadLine();
            }
            while (!turnInputIsValid(userTurnInput, out i_Row, out i_Col));
        }

        private bool turnInputIsValid(string i_UserTurnInput, out int o_Row, out int o_Col)
        {
            o_Row = QUIT;
            o_Col = QUIT;
            const bool v_IsValid = true;
            bool turnInputIsValid = !v_IsValid;
            i_UserTurnInput = removeWhitSpaces(i_UserTurnInput);
            if (i_UserTurnInput == QUITSYMBOL)
            {
                turnInputIsValid = v_IsValid;
            }
            // Expecting here to have the following string: 
            // "i,j" - where i is row and j is col.
            else if (i_UserTurnInput.Length == 3 && i_UserTurnInput[1] == ',')
            {
                char[] charArray = i_UserTurnInput.ToCharArray();
                if(char.IsDigit(charArray[0]) && char.IsDigit(charArray[2]) && charArray[1] == ',')
                {
                    int row = int.Parse(charArray[0].ToString());
                    int col = int.Parse(charArray[2].ToString());
                    //checking that the row and col are within range of board size
                    if(isInRage(row, 0,m_Game.BoardSize) &&
                        (isInRage(col, 0,m_Game.BoardSize)))
                    {
                        turnInputIsValid = v_IsValid;
                        o_Row = row;
                        o_Col = col;
                    }
                }
            }
            return turnInputIsValid;
        }

        private string removeWhitSpaces(string i_UserTurnInput)
        {
            StringBuilder stringBuilder = new StringBuilder(i_UserTurnInput);
            int index = stringBuilder.ToString().IndexOf(' ');
            while(index != -1)
            {
                stringBuilder.Remove(index, 1);
                index = stringBuilder.ToString().IndexOf(' ');
            }
            return stringBuilder.ToString();
        }

        private bool isInRage(int i_Num, int i_Min, int i_Max)
        {
            return i_Num >= i_Min && i_Num <= i_Max;
        }

        public string GetTurnMessage()
        {
            string TurnMsg;
            if((Game.eTurns)m_Game.CurrentTurn == Game.eTurns.PLAYER1)
            {
                TurnMsg = "Player 1 your turn, please enter where to place your symbol (row, col): "
                          + Environment.NewLine;
            }
            else
            {
                if(m_Game.GameType == Game.eGameTypes.PVP)
                {
                    TurnMsg = "Player 2 your turn, please enter where to place your symbol (row, col): "
                              + Environment.NewLine;
                }
                else
                {
                    TurnMsg = "The computer is now calculating what's the best next move is:"
                              + Environment.NewLine;
                }
            }
            return TurnMsg;
        }

        private void printGameBoard()
        {
            StringBuilder boardOutPut = new StringBuilder();
            int boardLength = m_Game.BoardSize;
            for (int i = 0; i <= boardLength; i++)
            {
                for(int j = 0; j <= boardLength; j++)
                {
                    if(i == 0 && j == 0)
                    {
                        boardOutPut.Append("  ");
                    }
                    else if(i == 0)
                    {
                        boardOutPut.Append(j);
                        boardOutPut.Append("   ");
                    }
                    else if(j == 0)
                    {
                        boardOutPut.Append(i);
                    }
                    else
                    {
                        boardOutPut.AppendFormat("| {0} ", m_Game.GameBoard[i - 1, j - 1]);
                    }
                }

                if(i > 0)
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

        private int getBoardSizeFromUser()
        {
            string userInputStr = "";
            StringBuilder msg = new StringBuilder();
            msg.AppendFormat("Please enter the board size (choose between: {0} - {1})",Game.MINBOARDSIZE,Game.MAXBOARDSIZE);
            msg.Append(Environment.NewLine);
            do
            {
                WriteLine(msg);
                userInputStr = ReadLine();
            }
            while(!isValidInput(userInputStr, Game.MINBOARDSIZE, Game.MAXBOARDSIZE));
            return int.Parse(userInputStr);
        }

        private  Game.eGameTypes getGameTypeFromUser()
        {
            string userInputStr;
            StringBuilder msg = new StringBuilder();
            msg.Append("Please enter the game type ('1' for PVP, '2' for PVC):");
            msg.Append(Environment.NewLine);
            do
            {
                WriteLine(msg);
                userInputStr = ReadLine();
            }
            while(!isValidInput(userInputStr,1,2));
            return (Game.eGameTypes)System.Enum.Parse(typeof(Game.eGameTypes), userInputStr);
        }

        public bool isValidInput(string i_UserInputStr, int min, int max)
        {
            const bool v_IsValid = true;
            bool isValidinput = v_IsValid;
            if (i_UserInputStr.Length != 1)
            {
                isValidinput = !v_IsValid;
            }
            else
            {
                char digitChar = i_UserInputStr[0];
                if (!char.IsDigit(digitChar))
                {
                    isValidinput = !v_IsValid;
                }
                else
                {
                    int digitNum = int.Parse(i_UserInputStr);
                    if (digitNum < min || digitNum > max)
                    {
                        isValidinput = !v_IsValid;
                    }
                }
            }
            return isValidinput;
        }
    }
}

