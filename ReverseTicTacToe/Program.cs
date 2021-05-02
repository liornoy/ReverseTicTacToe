using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ReverseTicTacToe.Tests;
using ReverseTicTacToe.UI;

namespace ReverseTicTacToe
{
    class Program
    {
        static void Main(string[] args)
        {
            ConsoleUI consoleUi = ConsoleUI.Instance;
           consoleUi.Init();
           consoleUi.Run();
          // UITests.TestUI();
        }
    }
}
