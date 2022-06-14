using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using SignalIRServerTest.Models;

using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace SignalIRServerTest
{
    [Route("[controller]")]
    public class UserController : Controller
    {
        private readonly UnitOfWork _unitOfWork;

        public UserController(UnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        [HttpGet("All")]
        [Authorize]
        public async Task<List<User>> GetUsers()
        {
            if (!(HttpContext.User.Identity is ClaimsIdentity identity))
            {
                return null;
            }

            var id =  identity.Claims.First().Value;

            var list = _unitOfWork.UserRepository.Get(
                includeProperties: $"{nameof(SignalIRServerTest.Models.User.IdSchoolNavigation)}," +
                                   $"{nameof(Models.User.IdRoleNavigation)}",
                filter: u => u.Approved).ToList();
            var user = list.FirstOrDefault(u => u.Id.ToString() == id);
            list.Remove(user);
            return list.ToList();
        }

        [HttpGet("NotApproved")]
        [Authorize]
        public async Task<List<User>> GetNotApprovedUsers()
        {
            if (!(HttpContext.User.Identity is ClaimsIdentity identity))
            {
                return null;
            }

            var id =  identity.Claims.First().Value;

            var list = _unitOfWork.UserRepository.Get(
                includeProperties: $"{nameof(SignalIRServerTest.Models.User.IdSchoolNavigation)}",
                filter: u => !u.Approved).ToList();
            //var user = list.FirstOrDefault(u => u.Id.ToString() == id);
            //list.Remove(user);
            return list.ToList();
        }

        [Authorize]
        [HttpPut("Id={id:int}&ApprovedStatus={approved:bool}")]
        public async Task<bool> ChangeUserApprovedStatus([FromRoute] int id, [FromRoute] bool approved)
        {
            var user = _unitOfWork.UserRepository
                .GetById(id);

            if (user == null)
            {
                return false;
            }

            user.Approved = approved;
            _unitOfWork.Save();
            return true;
        }


        [HttpGet("All.schoolId={schoolId}.roleId={roleId}.fullName={fullName}")]
        [Authorize]
        public async Task<List<User>> GetUsersBySchool([FromRoute] int schoolId, [FromRoute] int roleId, [FromRoute] string fullName)
        {
            if (!(HttpContext.User.Identity is ClaimsIdentity identity))
            {
                return null;
            }

            var id =  identity.Claims.First().Value;

            var list = _unitOfWork.UserRepository.Get(
                includeProperties: $"{nameof(SignalIRServerTest.Models.User.IdSchoolNavigation)}," +
                                   $"{nameof(Models.User.IdRoleNavigation)}")
                .ToList();
            if (roleId != -1)
            {
                list = list.FindAll(u => u.IdRole == roleId);
            }

            if (schoolId != -1 && schoolId != 0)
            {
                list = list.FindAll(u => u.IdSchool == schoolId);
            }

            if (!(fullName is {Length: 0}) && !(fullName.Length == 1 && fullName[0] == '"'))
            {
                var nameList = fullName.Split(' ');

                list = nameList.Aggregate(list, (current, s) =>
                    current.FindAll(u => u.FirstName.ToLower().Contains(s.ToLower()) 
                                         || u.LastName.ToLower().Contains(s.ToLower()) 
                                         || u.MiddleName.ToLower().Contains(s.ToLower())));
            }
            var user = list.FirstOrDefault(u => u.Id.ToString() == id);
            list.Remove(user);
            return list.ToList();
        }
    }
}