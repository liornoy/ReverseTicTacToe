using System;
using System.Text;
using ReverseTicTacToe.Logic;
using static System.Console;
using static ReverseTicTacToe.Logic.Game.eGameTypes;

namespace ReverseTicTacToe.UI
{
    class ConsoleUI
    {
        private Game m_Game;
        private int m_BoardSize;
        

        private Game.eGameTypes m_GameType;
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
            getBoardSizeFromUser();
            getGameTypeFromUser();
            m_Game = new Game(m_BoardSize, m_GameType);
            while(true)
            {

            }
        }

        private void getBoardSizeFromUser()
        {
            string userInputStr;
            StringBuilder msg = new StringBuilder();
            msg.Append("Please enter the board size (choose between: 3/4/5/6/7/8/9)");
            msg.Append(Environment.NewLine);
            do
            {
                WriteLine(msg);
                userInputStr = ReadLine();
            }
            while(!isValidInput(userInputStr,1,9));
            m_BoardSize = int.Parse(userInputStr);
        }

        private  void getGameTypeFromUser()
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
            m_GameType = (Game.eGameTypes)System.Enum.Parse(typeof(Game.eGameTypes), userInputStr);
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

