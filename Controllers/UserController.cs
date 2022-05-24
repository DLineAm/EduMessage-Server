using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SignalIRServerTest.Models;
using SignalIRServerTest.Services;

namespace SignalIRServerTest.Controllers
{
    [Route("[controller]")]
    public class UserController : Controller
    {
        private UnitOfWork _unitOfWork;
        private Hash _hash;

        public UserController(UnitOfWork unitOfWork, Hash hash)
        {
            _unitOfWork = unitOfWork;
            _hash = hash;
        }


        [Authorize]
        [HttpPut("UploadImage")]
        public bool UploadImage([FromBody] KeyValuePair<int, byte[]> userImagePair) 
        {
            try
            {
                var userId = userImagePair.Key;
                var image = userImagePair.Value;

                var findedUser = _unitOfWork.UserRepository.GetById(userId);
                findedUser.Image = image;

                if (ModelState.IsValid)
                {
                    _unitOfWork.UserRepository.Update(findedUser);
                    _unitOfWork.Save();
                }

                return true;
            }
            catch (Exception e)
            {
                return false;
            }
        }

        [HttpDelete("id={id:int}.password={password}")]
        [Authorize]
        public async Task<string> DeleteUser([FromRoute] int id, [FromRoute] string password)
        {
            try
            {
                var user = _unitOfWork.UserRepository.GetById(id);
                if (user == null)
                {
                    return "Not found by id";
                }

                byte[] salt = user.Salt;
                bool isPassportValid = _hash.CompareString(password, salt, user.PasswordHash);

                if (!isPassportValid)
                {
                    return "Not found by password";
                }

                user.Deleted = true;

                _unitOfWork.UserRepository.Update(user);
                _unitOfWork.Save();

                return "";
            }
            catch (Exception e)
            {
                return "Exception";
            }
        }
    }
}
