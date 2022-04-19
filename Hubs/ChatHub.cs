using System;
using Microsoft.AspNet.SignalR.Hubs;
using Microsoft.AspNetCore.SignalR;

using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using SignalIRServerTest.Models;
using Hub = Microsoft.AspNetCore.SignalR.Hub;

namespace SignalIRServerTest.Hubs
{
    [HubName("ChatHub")]
    [Authorize(AuthenticationSchemes = "Bearer")]
    public class ChatHub : Hub
    {
        public async Task SendAll(string message)
        {
            await this.Clients.AllExcept(new List<string> { Context.ConnectionId }).SendAsync("Send", message);
        }

        public async Task SendToUser(string message, int userId)
        {
            var unitOfWork = new UnitOfWork();
            int id = Convert.ToInt32(Context.User.Claims.First().Value);
            var user = unitOfWork.UserRepository.GetById(id);
            if (id != userId)
            {
                await Clients.User(userId.ToString())
                    .SendAsync("ReceiveForMe", message, user!);
            }
        }
    }
}
