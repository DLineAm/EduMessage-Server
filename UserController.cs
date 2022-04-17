using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SignalIRServerTest.Models;

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
                includeProperties: $"{nameof(SignalIRServerTest.User.IdSchoolNavigation)}").ToList();
            var user = list.FirstOrDefault(u => u.Id.ToString() == id);
            list.Remove(user);
            return list.ToList();
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
                includeProperties: $"{nameof(SignalIRServerTest.User.IdSchoolNavigation)}")
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