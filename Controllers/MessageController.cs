using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SignalIRServerTest.Models;
using SignalIRServerTest.Services;

namespace SignalIRServerTest.Controllers
{
    [Route("[controller]")]
    public class MessageController : Controller
    {
         private UnitOfWork _unitOfWork;
         private readonly ILogger<MessageController> _logger;

         public MessageController(UnitOfWork unitOfWork, ILogger<MessageController> logger)
         {
             _unitOfWork = unitOfWork;
             _logger = logger;
         }

        [HttpGet("All")]
        [Authorize]
        public List<UserConversation> GetConversations()
        {
            if (!(HttpContext.User.Identity is ClaimsIdentity identity))
            {
                return null;
            }

            var id = identity.Claims.First().Value;

            var listWithUser = _unitOfWork.UserConversationFormRepository.Get(filter: u => u.IdUser.ToString() == id);

            var allConversations = _unitOfWork.UserConversationFormRepository.Get(
                includeProperties: 
                                   $"{nameof(UserConversation.IdUserNavigation)}.{nameof(UserConversation.IdUserNavigation.IdRoleNavigation)}," +
                                   $"{nameof(UserConversation.IdConversationNavigation)},"
                                   )
                .ToList()
                .Where(c => listWithUser
                    .ToList()
                    .FirstOrDefault(cv => c.IdConversation == cv.IdConversation) != null && c.IdUser.ToString() != id);

            return allConversations.ToList();
        }

        [HttpGet("Id={id}")]
        [Authorize]
        public List<MessageAttachment> GetMessageAttachmentsByConversationId([FromRoute] int id)
        {
            if (id == 0)
            {
                return null;
            }

            //var replaceMessages = _unitOfWork.MessageAttachmentRepository
            //    .Get(filter: m => m.IdMessageNavigation.MessageContent == String.Empty,
            //        includeProperties: $"{nameof(MessageAttachment.IdMessageNavigation)}," +
            //                           $"{nameof(MessageAttachment.IdAttachmentNavigation)}," +
            //                           $"IdMessageNavigation.{nameof(Message.IdUserNavigation)}");

            //replaceMessages.ToList().ForEach(m =>
            //{
            //    _unitOfWork.MessageAttachmentRepository.Delete(m);
            //    _unitOfWork.MessageRepository.Delete(m.IdMessage);
            //});
            //_unitOfWork.Save();

            var messages = _unitOfWork.MessageAttachmentRepository
                .Get(filter: m => m.IdMessageNavigation.IdConversation == id,
                    includeProperties: $"{nameof(MessageAttachment.IdMessageNavigation)}," +
                                       $"{nameof(MessageAttachment.IdAttachmentNavigation)}," +
                                       $"IdMessageNavigation.{nameof(Message.IdUserNavigation)}," +
                                       $"{nameof(MessageAttachment.IdAttachmentNavigation)}.{nameof(Attachment.IdTypeNavigation)}");

            return messages.ToList();
        }

        [HttpGet("idUser={idUser}&title={title}")]
        [Authorize]
        public List<UserConversation> GetConversationsByFilter([FromRoute] int idUser,[FromRoute] string title)
        {
            if (idUser == 0)
            {
                return null;
            }

            if (!(HttpContext.User.Identity is ClaimsIdentity identity))
            {
                return null;
            }

            var id = identity.Claims.First().Value;

            var foundConversations = _unitOfWork.UserConversationFormRepository
                .Get(includeProperties: $"{nameof(UserConversation.IdUserNavigation)}," +
                                        $"{nameof(UserConversation.IdConversationNavigation)}").ToList();

            var conversationsWithUser = foundConversations.Where(c => c.IdUser.ToString() == id).ToList();

            foundConversations = foundConversations
                .Where(c => conversationsWithUser.Any(cv => cv.IdConversation == c.IdConversation)
                && c.IdUser == idUser).ToList();

            return foundConversations;
        }

        [HttpPost("Add")]
        [Authorize]

        public KeyValuePair<int, List<int>> AddConversation([FromBody] List<UserConversation> conversations)
        {
            try
            {
                var firstConversation = conversations.FirstOrDefault();

                if (firstConversation == null)
                {
                    return new KeyValuePair<int, List<int>>(-1, null);
                }

                var savedConversation = _unitOfWork.ConversationFormRepository.Insert(firstConversation.IdConversationNavigation).Entity;
                _unitOfWork.Save();
                var conversationsList = new List<int>();
                foreach (var conversation in conversations)
                {
                    conversation.IdUser = conversation.IdUserNavigation.Id;
                    conversation.IdUserNavigation = null;
                    conversation.IdConversationNavigation = savedConversation;
                    conversation.IdConversation = savedConversation.Id;
                    var savedUserConversation = _unitOfWork.UserConversationFormRepository.Insert(conversation).Entity;
                    conversationsList.Add(savedUserConversation.Id);
                }

                _unitOfWork.Save();

                return new KeyValuePair<int, List<int>>(savedConversation.Id, conversationsList);
            }
            catch (Exception e)
            {
                _logger.LogError(e, StringDecorator.GetDecoratedLogString(e.GetType(), nameof(AddConversation)));
                return new KeyValuePair<int, List<int>>(-1, null);
            }
        }

        [HttpPost("AddMessage")]
        [Authorize]
        public async Task<int> AddMessage([FromBody] List<MessageAttachment> messageList)
        {
            if (messageList == null || messageList.Count == 0)
            {
                return -1;
            }

            try
            {

                var unitOfWork = new UnitOfWork();
                Message message = messageList.FirstOrDefault().IdMessageNavigation;

                var dbMessage = unitOfWork.MessageRepository
                    .Insert(message)
                    .Entity;
                unitOfWork.Save();

                messageList.ForEach(m =>
                {
                    m.IdMessage = dbMessage.Id;
                    m.IdMessageNavigation = null;
                    unitOfWork.MessageAttachmentRepository.Insert(m);
                    unitOfWork.Save();
                });
                //int id = Convert.ToInt32(idString);
                //User user = unitOfWork.UserRepository.GetById(id);

                //var conversation = dbMessage.IdConversation;
                //var users = unitOfWork.UserConversationFormRepository
                //    .Get(filter: m => m.IdConversation == conversation && m.IdUser != id)
                //    .Select(c => c.IdUser);

                return dbMessage.Id;
            }
            catch (Exception e)
            {
                _logger.LogError(e, StringDecorator.GetDecoratedLogString(e.GetType(), nameof(AddMessage)));
                return -1;
            }
        }

        [HttpPut("Change")]
        [Authorize]
        public async Task<bool> ChangeMessage([FromBody] List<MessageAttachment> messageAttachments)
        {
            try
            {
                var message = messageAttachments?.FirstOrDefault()?.IdMessageNavigation;

                var conversationId = message?.IdConversation;

                if (conversationId == null)
                {
                    return false;
                }

                _unitOfWork.MessageRepository.Update(message);

                _unitOfWork.MessageAttachmentRepository
                    .Get(filter: m => m.IdMessage == message.Id)
                    .Where(m => messageAttachments.All(ma => ma.Id != m.Id))
                    .ToList()
                    .ForEach(_unitOfWork.MessageAttachmentRepository.Delete);

                messageAttachments.ForEach(ma =>
                {
                    if (ma.IdAttachment != 0 && ma.IdAttachment != null)
                    {
                        ma.IdAttachmentNavigation = null;
                    }
                });

                foreach (var messageAttachment in messageAttachments)
                {
                    messageAttachment.IdMessageNavigation = null;
                    if (messageAttachment.Id == 0)
                    {
                        _unitOfWork.MessageAttachmentRepository.Insert(messageAttachment);
                        continue;
                    }
                    _unitOfWork.MessageAttachmentRepository.Update(messageAttachment);
                }

                _unitOfWork.Save();

                return true;
            }
            catch (Exception e)
            {
                _logger.LogError(e, StringDecorator.GetDecoratedLogString(e.GetType(), nameof(ChangeMessage)));
                return false;
            }
        }
    }
}