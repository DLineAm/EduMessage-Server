using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using SignalIRServerTest.Models;

namespace SignalIRServerTest.Controllers
{
    [Route("[controller]")]
    public class MessageController : Controller
    {
        private UnitOfWork _unitOfWork;

        public MessageController(UnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
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
                includeProperties: $"{nameof(UserConversation.IdUserNavigation)}," +
                                   $"{nameof(UserConversation.IdConversationNavigation)}")
                .ToList()
                .Where(c => listWithUser
                    .ToList()
                    .FirstOrDefault(cv => c.IdConversation == cv.IdConversation) != null && c.IdUser.ToString() != id);

            return allConversations.ToList();
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
                return new KeyValuePair<int, List<int>>(-1, null);
            }
        }
    }
}