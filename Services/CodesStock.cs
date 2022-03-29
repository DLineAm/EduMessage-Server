using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SignalIRServerTest.Services
{
    public static class CodesStock
    {
        private static List<EmailCode> EmailCodes = new List<EmailCode>();

        public static void AddRecord(EmailCode emailCode)
        {
            var checkEmail = EmailCodes.FirstOrDefault(p => p.Email == emailCode.Email);
            if (checkEmail != null)
            {
                EmailCodes.Remove(checkEmail);
            }

            EmailCodes.Add(emailCode);
        }

        public static bool CheckCode(EmailCode emailCode)
        {
            var checkEmail = EmailCodes.FirstOrDefault(p => p.Email == emailCode.Email);
            if (checkEmail == null)
                return false;

            return checkEmail.Code == emailCode.Code;
        }
    }

    public class EmailCode
    {
        public string Email { get; set; }
        public string Code { get; set; }
    }
}
