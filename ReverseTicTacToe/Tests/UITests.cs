/*
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ReverseTicTacToe.UI;
using static System.Console;

namespace ReverseTicTacToe.Tests
{
    class UITests
    {
        public static void TestUI()
        {
            ConsoleUIManager consoleUiManager = new ConsoleUIManager();
            testIsValidInput(consoleUiManager);


        }

        private static void testIsValidInput(ConsoleUIManager consoleUiManager)
        {
            int numOfTests = 7, min = 3, max = 9;
            int outRes;
            bool[] bools = new bool[] { false, false, false, false, true, true, true };
            string[] strings = new string[] { "FAILE", "", "-1", "20", "3", "9", "5" };
            for (int i = 0; i < numOfTests; i++)
            {
                Write("Test Number: " + i + ": ");
                bool testRes = ReverseTicTacToe.UI.ConsoleUIManager.checkIsStringRepresentsNumberWithinRangeAndParse(strings[i], out outRes, min, max);
                if (testRes != bools[i])
                {
                    WriteLine("FAIL. " + testRes.ToString() + " != " + bools[i].ToString());
                }
                else
                {
                    WriteLine("SUCCESS!");
                }
            }
        }
    }
*/