using Microsoft.IdentityModel.Tokens;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SignalIRServerTest.Models
{
    public class Authentification
    {
        public const string Issuer = "EduMessageComp";
        public const string Audience = "EduMessage";
        private const string Key = "yapozhilayaaboba229";
        public const int Lifetime = 30;
        public static SymmetricSecurityKey GetSecurityKey() => new SymmetricSecurityKey(Encoding.ASCII.GetBytes(Key));
    }
}
