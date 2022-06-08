using EduMessage.Services;

using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

using SignalIRServerTest.Models;
using SignalIRServerTest.Services;

using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SignalIRServerTest.Controllers.Interceptors;

namespace SignalIRServerTest.Controllers
{
    [Route("[controller]")]
    public class LoginController : Controller
    {
        private readonly UnitOfWork _unitOfWork;

        private readonly Hash _hash;

        private readonly ILogger<LoginController> _logger;

        //private static EducationContext db = new EducationContext();

        public LoginController(UnitOfWork unitOfWork, Hash hash, ILogger<LoginController> logger)
        {
            _unitOfWork = unitOfWork;
            _hash = hash;
            _logger = logger;

        }

        //[HttpGet("Testt")]
        //public bool GetTest(Aboba aboba)
        //{
        //    return aboba.GetResult();
        //}

        [HttpGet("Send.email={email}")]
        public bool SendEmailCode([FromRoute] string email)
        {
            var code = CodeGenerator.Generate();
            try
            {
                new Email().Send(email, $"Здравствуйте! На почту был отправлен код подтверждения, используйте его в приложении EduMessage, чтобы подтвердить указанную почту.<br><br>Код подтверждения: <h1>{code}</h1>");
                CodesStock.AddRecord(new EmailCode { Email = email, Code = code });
                return true;
            }
            catch (System.Exception e)
            {
                _logger.LogError(e, StringDecorator.GetDecoratedLogString(e.GetType(), nameof(SendEmailCode)));
                return false;
            }
        }

        [HttpGet("ValidateCode.email={email}.emailCode={emailCode}")]
        public bool ValidateEmailCode([FromRoute] string email, [FromRoute] string emailCode)
        {
            var result = CodesStock.CheckCode(new EmailCode { Email = email, Code = emailCode });
            return result;
        }

        [HttpGet("Cities")]
        public async Task<List<City>> GetCities()
        {
            var list = _unitOfWork.CityRepository.Get();
            //var list = await db.Cities.ToListAsync();
            return list.ToList();
        }



        [HttpGet("Users.login={loginemail}.password={password}")]
        public async Task<KeyValuePair<User, string>> GetUserForLogin([FromRoute] string loginemail, [FromRoute] string password)
        {
            try
            {
                var list = _unitOfWork.UserRepository.Get(
                        includeProperties: $"{nameof(SignalIRServerTest.Models.User.IdRoleNavigation)}," +
                        $"{nameof(SignalIRServerTest.Models.User.IdCityNavigation)}," +
                        $"{nameof(SignalIRServerTest.Models.User.IdSchoolNavigation)}," +
                        $"{nameof(SignalIRServerTest.Models.User.IdGroupNavigation)}," +
                        $"{nameof(SignalIRServerTest.Models.User.IdEducationFormNavigation)}," +
                        $"IdGroupNavigation.IdSpecialityNavigation");

                var userFromLogin = list
                        .FirstOrDefault(u => u.Login == loginemail || u.Email == loginemail);

                if (userFromLogin == null || userFromLogin.Deleted)
                {
                    return new KeyValuePair<User, string>(null, null);
                }

                var hashedPassword = _hash.CompareString(password, userFromLogin.Salt, userFromLogin.PasswordHash);

                if (!hashedPassword)
                {
                    return new KeyValuePair<User, string>(null, null);
                }

                if (!userFromLogin.Approved)
                {
                    return new KeyValuePair<User, string>(userFromLogin, null);
                }

                var jwt = CreateJwtToken(userFromLogin);
                return new KeyValuePair<User, string>(userFromLogin, jwt);
            }
            catch (Exception e)
            {
                _logger.LogError(e, StringDecorator.GetDecoratedLogString(e.GetType(), nameof(GetUserForLogin)));
                return new KeyValuePair<User, string>(null, null);
            }

            //try
            //{
            //    User findedUser = list.FirstOrDefault(p => p.Login == loginemail || p.Email == loginemail && p.Password == password);
            //    if (findedUser == null)
            //    {
            //        return new KeyValuePair<User, string>(null, null);
            //    }
            //    var jwt = CreateJwtToken(findedUser);
            //    return new KeyValuePair<User, string>(findedUser, jwt);
            //}
            //catch (System.Exception e)
            //{
            //    return new KeyValuePair<User, string>(null, null);
            //}
        }

        [HttpPost("Schools")]
        public async Task<List<School>> GetSchools([FromBody] City city)
        {
            if (city == null)
            {
                return null;
            }

            var schools = _unitOfWork.SchoolRepository.Get(filter: s => s.IdCity == city.Id);

            return schools.ToList();
        }

        [HttpGet("Schools")]
        public async Task<List<School>> GetSchools()
        {
            var schools = _unitOfWork.SchoolRepository.Get();

            return schools.ToList();
        }

        [HttpGet("Schools.searchText={searchText}")]
        public async Task<List<School>> GetSchoolsFromSearch([FromRoute] string searchText)
        {
            var schools = _unitOfWork.SchoolRepository.Get(filter: s => s.Name.ToLower().Contains(searchText.ToLower()));

            return schools.ToList();
        }

        [HttpGet("Specialities")]
        public async Task<List<Speciality>> GetSpecialities()
        {
            var list = _unitOfWork.SpecialityRepository.Get();
            return list.ToList();
        }

        [HttpGet("Groups")]
        public async Task<List<Group>> GetGroups()
        {
            var list = _unitOfWork.GroupRepository.Get();
            return list.ToList();
        }

        [HttpGet("Groups.idSpeciality={idSpeciality}")]
        public async Task<List<Group>> GetGroupsFromSpeciality([FromRoute] int idSpeciality)
        {
            var list = _unitOfWork.GroupRepository.Get(filter: g => g.IdSpeciality == idSpeciality);
            return list.ToList();
        }

        [HttpGet("Specialities.idGroup={idGroup}")]
        public async Task<Speciality> GetSpecialityFromGroup([FromRoute] int idGroup)
        {
            var group = _unitOfWork.GroupRepository.GetById(idGroup);
            var speciality = _unitOfWork.SpecialityRepository.GetById(group.Id);
            return speciality;
        }

        [HttpGet("Roles")]
        public async Task<List<Role>> GetRoles()
        {
            var list = _unitOfWork.RoleRepository.Get(filter: role => role.Id != 3);
            return list.ToList();
        }

        [HttpGet("EducationForms")]
        public async Task<List<EducationForm>> GetEducationForms()
        {
            var list = _unitOfWork.EducationFormRepository.Get();
            return list.ToList();
        }

        [HttpGet("Validate.phone={phone}")]
        public async Task<bool> ValidatePhone([FromRoute] string phone)
        {
            var result = _unitOfWork.UserRepository.GetByExpression(u=> u.Phone.ToString() == phone) == null;
            return true;
        }

        [HttpGet("Validate.email={email}")]
        public async Task<bool> ValidateEmail([FromRoute] string email)
        {
            var result = _unitOfWork.UserRepository.GetByExpression(u => u.Email == email) == null;
            return result;
        }

        [HttpGet("Validate.login={login}")]
        public async Task<bool> ValidateLogin([FromRoute] string login)
        {
            var result = _unitOfWork.UserRepository.GetByExpression(u => u.Login == login) == null;
            return result;
        }

        [HttpPost("Register")]
        public async Task<KeyValuePair<int, string>> RegisterUser(
            [FromBody] KeyValuePair<User, string> pair)
        {
            try
            {
                var user = pair.Key;
                var password = pair.Value;
                var saltedPassword = _hash.GetHashedString(password, out var salt);

                var group = _unitOfWork.GroupRepository.GetById(user.IdGroup);
                var city = _unitOfWork.CityRepository.GetById(user.IdCity);
                var role = _unitOfWork.RoleRepository.GetById(user.IdRole);
                var school = _unitOfWork.SchoolRepository.GetById(user.IdSchool);

                user.IdCityNavigation = city;
                user.IdGroupNavigation = group;
                user.IdRoleNavigation = role;
                user.IdSchoolNavigation = school;
                user.PasswordHash = saltedPassword;
                user.Salt = salt;

                var savedUser = _unitOfWork.UserRepository.Insert(user).Entity;
                _unitOfWork.Save();
                var jwt = CreateJwtToken(savedUser);
                return new KeyValuePair<int, string>(savedUser.Id, jwt);
            }
            catch (System.Exception e)
            {
                _logger.LogError(e, StringDecorator.GetDecoratedLogString(e.GetType(), nameof(RegisterUser)));
                return new KeyValuePair<int, string>(-1, null);

            }
        }

        [HttpGet("GetUser.ByToken")]
        [Authorize]
        public async Task<User> GetUserByToken()
        {
            var identity = HttpContext.User.Identity as ClaimsIdentity;

            if (identity == null)
            {
                return null;
            }

            var claims = identity.Claims;

            //var users = _unitOfWork.UserRepository
            //    .Get(filter: u => u.PasswordHash == null);

            //foreach (var user1 in users)
            //{
            //    var hashedPassword = _hash.GetHashedString(user1.Password, out var salt);
            //    user1.PasswordHash = hashedPassword;
            //    user1.Salt = salt;
            //    _unitOfWork.UserRepository.Update(user1);
            //}

            //_unitOfWork.Save();

            var user = _unitOfWork.UserRepository.Get(
                includeProperties: $"{nameof(SignalIRServerTest.Models.User.IdRoleNavigation)}," +
                $"{nameof(SignalIRServerTest.Models.User.IdCityNavigation)}," +
                $"{nameof(SignalIRServerTest.Models.User.IdSchoolNavigation)}," +
                $"{nameof(SignalIRServerTest.Models.User.IdGroupNavigation)}," +
                $"{nameof(SignalIRServerTest.Models.User.IdEducationFormNavigation)}," +
                $"IdGroupNavigation.IdSpecialityNavigation",
                filter: u => u.Id.ToString() == claims.First().Value)
                .FirstOrDefault();

            return user;
        }

        private string CreateJwtToken(User user)
        {
            var now = DateTime.Now;
            var identity = GetIdentity(user);
            var jwt = new JwtSecurityToken(
                Authentification.Issuer,
                Authentification.Audience,
                notBefore: now,
                claims: identity.Claims,
                expires: now.Add(TimeSpan.FromDays(Authentification.Lifetime)),
                signingCredentials: new Microsoft.IdentityModel.Tokens.SigningCredentials(Authentification.GetSecurityKey(), SecurityAlgorithms.HmacSha256));
            var encodedJwt = new JwtSecurityTokenHandler().WriteToken(jwt);
            return encodedJwt;
        }

        private ClaimsIdentity GetIdentity(User user)
        {

            var claims = new List<Claim>
            {
                new Claim(ClaimsIdentity.DefaultNameClaimType, user.Id.ToString()),
                new Claim(ClaimsIdentity.DefaultNameClaimType, user.Login),
                new Claim(ClaimsIdentity.DefaultRoleClaimType, user.IdRoleNavigation.Title),
            };

            ClaimsIdentity identity =
                new ClaimsIdentity(claims, "Token", ClaimsIdentity.DefaultNameClaimType,
                ClaimsIdentity.DefaultRoleClaimType);
            var identityc = HttpContext.User.Identity as ClaimsIdentity;
            identityc.AddClaims(claims);
            return identity;
        }
    }
}
