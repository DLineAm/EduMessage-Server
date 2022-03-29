﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SignalIRServerTest.Models;

namespace SignalIRServerTest.Controllers
{
    [Route("[controller]")]
    public class UserController : Controller
    {
        private UnitOfWork _unitOfWork = new UnitOfWork();
        //private EducationContext db = new EducationContext();

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

                //var user = await db.Users.FindAsync(userId);

                //if (user == null)
                //{
                //    return false;
                //}

                //user.Image = image;

                //db.Update(user);
                //await db.SaveChangesAsync();

                return true;
            }
            catch (Exception e)
            {
                return false;
            }
        }
    }
}