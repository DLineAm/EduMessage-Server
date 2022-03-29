using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SignalIRServerTest.Services
{
    public class CodeGenerator
    {
        private static string s_pattern = "1234567890ABCDEFGHIJKLMNOPQRSTUVWZYZ";
        public static string Generate()
        {
            string result = string.Empty;

            for (int i = 0; i < 6; i++)
            {
                result += s_pattern[new Random().Next(0, s_pattern.Length)];
            }

            return result;
        }

    }
}
