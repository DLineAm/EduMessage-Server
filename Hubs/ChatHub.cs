using Microsoft.AspNet.SignalR.Hubs;
using Microsoft.AspNetCore.SignalR;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SignalIRServerTest.Hubs
{
    [HubName("ChatHub")]
    public class ChatHub : Hub
    {
        public override Task OnConnectedAsync()
        {
            return base.OnConnectedAsync();
        }
        public async Task Send(string message)
        {
            await this.Clients.All.SendAsync("send", message);
        }
    }
}
