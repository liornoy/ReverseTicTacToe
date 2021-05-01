using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ReverseTicTacToe.UI;
using static System.Console;

namespace ReverseTicTacToe.Tests
{
    class isValidInputParams
    {
        public string m_Input;
        public int m_Max, m_Min;
        public bool m_ExpectedRes;

        public isValidInputParams(string i_Input, int i_Min, int i_Max, bool i_ExpectedRes)
        {
            m_Input = i_Input;
            m_Max = i_Max;
            m_Min = i_Min;
            m_ExpectedRes = i_ExpectedRes;
        }
    }
    class UITests
    {
        static isValidInputParams[] m_Params;

        public static void TestUI()
        {
            ConsoleUI consoleUi = ConsoleUI.Instance;
            int numOfTests = 8;
            m_Params = new isValidInputParams[numOfTests];
            m_Params[0] = new isValidInputParams("gibrish", 1, 5, false);
            m_Params[1] = new isValidInputParams("-1", 1, 5, false);
            m_Params[2] = new isValidInputParams("0", 1, 5, false);
            m_Params[3] = new isValidInputParams("13", 1, 5, false);
            m_Params[4] = new isValidInputParams("", 1, 5, false);
            m_Params[5] = new isValidInputParams("1", 1, 5, true);
            m_Params[6] = new isValidInputParams("5", 1, 5, true);
            m_Params[7] = new isValidInputParams("3", 1, 5, true);
            for(int i = 0; i < numOfTests; i++)
            {
                bool res = consoleUi.isValidInput(m_Params[i].m_Input, m_Params[i].m_Min, m_Params[i].m_Max);
                if (res == m_Params[i].m_ExpectedRes)
                {
                    StringBuilder msg = new StringBuilder();
                    msg.AppendFormat("Test number {0}: PASSED",i);
                    WriteLine(msg);
                }
                else
                {
                    StringBuilder msg = new StringBuilder();
                    msg.AppendFormat("Test number {0}: Failed",i);
                    msg.Append(Environment.NewLine);
                    msg.AppendFormat("Test Params: string: {0}, Min: {1}, Max: {2}, Expected Result: {3}",
                        m_Params[i].m_Input, m_Params[i].m_Min, m_Params[i].m_Max,m_Params[i].m_ExpectedRes);
                    WriteLine(msg);
                }
            }
        }
    }

    
}
