using System;
using Microsoft.AspNet.SignalR.Hubs;
using Microsoft.AspNetCore.SignalR;

using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Newtonsoft.Json;
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

        public async Task ChangeMessage(int messageId)
        {
            try
            {
                var unitOfWork = new UnitOfWork();
                var message = unitOfWork.MessageRepository
                    .GetById(messageId);
                if (message == null)
                {
                    return;
                }

                var messageAttachments = unitOfWork.MessageAttachmentRepository
                    .Get(includeProperties:$"{nameof(MessageAttachment.IdAttachmentNavigation)}," +
                                           $"{nameof(MessageAttachment.IdMessageNavigation)}",
                        filter: a => a.IdMessage == messageId);

                var conversationId = message.IdConversation;
                

                await SendToUsersInConversation(conversationId, "MessageChanged", messageAttachments);
            }
            catch (Exception e)
            {

            }
        }

        public async Task DeleteMessage(int messageId)
        {
            try
            {
                var unitOfWork = new UnitOfWork();

                var attachments = unitOfWork.MessageAttachmentRepository
                    .Get(filter: m => m.IdMessage == messageId,
                        includeProperties:$"{nameof(MessageAttachment.IdMessageNavigation)}")
                    .ToList();


                var dbMessage = unitOfWork.MessageRepository
                    .GetById(messageId);
                if (dbMessage == null)
                {
                    return;
                }
                int? conversationId = dbMessage.IdConversation;

                unitOfWork.MessageRepository.Delete(dbMessage);
                attachments.ForEach(unitOfWork.MessageAttachmentRepository.Delete);
                unitOfWork.Save();

                attachments.ForEach( a => a.IdMessageNavigation = dbMessage);

                await SendToUsersInConversation(conversationId, "ReceiveDeletedMessage", messageId);
            }
            catch (Exception e)
            {

            }
        }

        private async Task SendToUsersInConversation(int? conversationId, string methodName, object arg1)
        {
            int id = Convert.ToInt32(Context.User.Claims.First().Value);

            using var unitOfWork = new UnitOfWork();

            var users = unitOfWork.UserConversationFormRepository
                .Get(filter: m => m.IdConversation == conversationId && m.IdUser != id)
                .Select(c => c.IdUser);

            foreach (var userInConversationId in users)
            {
                if (userInConversationId == null)
                {
                    continue;
                }

                await Clients.User(userInConversationId.ToString())
                    .SendAsync(methodName, arg1);
            }
        }

        public async Task<bool> SendToUser(int messageId, int userId)
        {
            //var messageList = JsonConvert.DeserializeObject<List<MessageAttachment>>(messageString);
            try
            {
                var unitOfWork = new UnitOfWork();
                var messageList = unitOfWork.MessageAttachmentRepository
                    .Get(filter: m => m.IdMessage == messageId,
                        includeProperties: $"{nameof(MessageAttachment.IdMessageNavigation)}," +
                                           $"{nameof(MessageAttachment.IdAttachmentNavigation)}");
                var dbMessage = messageList.FirstOrDefault().IdMessageNavigation;

                //Message message = messageList.FirstOrDefault().IdMessageNavigation;

                //var dbMessage = unitOfWork.MessageRepository.Insert(message).Entity;
                //unitOfWork.Save();

                //messageList.ForEach(m =>
                //{
                //    m.IdMessage = dbMessage.Id;
                //    m.IdMessageNavigation = null;
                //    unitOfWork.MessageAttachmentRepository.Insert(m);
                //    unitOfWork.Save();
                //});
                var idString = Context.User.Claims.First().Value;
                int id = Convert.ToInt32(idString);
                User user = unitOfWork.UserRepository.GetById(id);

                var conversation = dbMessage.IdConversation;
                var users = unitOfWork.UserConversationFormRepository
                    .Get(filter: m => m.IdConversation == conversation && m.IdUser != id)
                    .Select(c => c.IdUser);

                foreach (var userInConversationId in users)
                {
                    if (userInConversationId == null)
                    {
                        continue;
                    }

                    await Clients.User(userInConversationId.ToString())
                        .SendAsync("ReceiveForMe", messageList, user!);
                }

                await Clients.User(idString)
                    .SendAsync("ReceiveAddedMessage", dbMessage.Id);

                //await Clients.User(userId.ToString())
                //    .SendAsync("ReceiveForMe", messageList, user!);

                return true;
            }
            catch (Exception e)
            {
                return false;
            }
        }
    }
}
