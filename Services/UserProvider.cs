
using System.Linq;
using Microsoft.AspNetCore.SignalR;
using SignalIRServerTest.Models;

namespace SignalIRServerTest.Services
{
    public class UserProvider : IUserIdProvider
    {
        public string GetUserId(HubConnectionContext connection)
        {
            var userId = connection.User?.Claims.First().Value;
            return userId;
        }
    }
}