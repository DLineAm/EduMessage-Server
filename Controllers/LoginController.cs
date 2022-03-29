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

namespace SignalIRServerTest.Controllers
{
    [Route("[controller]")]
    public class LoginController : Controller
    {
        private UnitOfWork _unitOfWork = new UnitOfWork();
        //private static EducationContext db = new EducationContext();


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
            var list = _unitOfWork.CityReposytory.Get();
            //var list = await db.Cities.ToListAsync();
            return list.ToList();
        }

        [HttpGet("Users")]
        public async Task<List<User>> GetUsers()
        {
            var list = _unitOfWork.UserRepository.Get();
            //var list = await db.Users.ToListAsync();
            return list.ToList();
        }

        [HttpGet("Users.login={loginemail}.password={password}")]
        public async Task<KeyValuePair<User, string>> GetUserForLogin([FromRoute] string loginemail, [FromRoute] string password)
        {
            var list = _unitOfWork.UserRepository.Get(
                includeProperties: $"{nameof(SignalIRServerTest.User.IdRoleNavigation)}," +
                $"{nameof(SignalIRServerTest.User.IdCityNavigation)}," +
                $"{nameof(SignalIRServerTest.User.IdSchoolNavigation)}," +
                $"{nameof(SignalIRServerTest.User.IdGroupNavigation)}," +
                $"{nameof(SignalIRServerTest.User.IdEducationFormNavigation)}," +
                $"IdGroupNavigation.IdSpecialityNavigation");
            //var list = await db.Users
            //    .Include(u => u.IdRoleNavigation)
            //    .Include(u => u.IdCityNavigation)
            //    .Include(u => u.IdSchoolNavigation)
            //    .Include(u => u.IdGroupNavigation)
            //    .Include(u => u.IdEducationFormNavigation)
            //    .Include(u => u.IdGroupNavigation.IdSpecialityNavigation)
            //    .ToListAsync();
            try
            {
                User findedUser = list.FirstOrDefault(p => p.Login == loginemail || p.Email == loginemail && p.Password == password);
                if (findedUser == null)
                {
                    return new KeyValuePair<User, string>(null, null);
                }
                var jwt = CreateJwtToken(findedUser);
                return new KeyValuePair<User, string>(findedUser, jwt);
            }
            catch (System.Exception e)
            {
                return new KeyValuePair<User, string>(null, null);
            }
        }

        [HttpPost("Schools")]
        public async Task<List<School>> GetSchools([FromBody] City city)
        {
            if (city == null)
            {
                return null;
            }

            var schools = _unitOfWork.SchoolReposytory.Get(filter: s => s.IdCity == city.Id);

            //var schools = (await db.Schools.ToListAsync()).Where(p => p.IdCity == city.Id).ToList();
            return schools.ToList();
        }

        [HttpGet("Specialities")]
        public async Task<List<Speciality>> GetSpecialities()
        {
            var list = _unitOfWork.SpecialityRepository.Get();
            //var list = await db.Specialities.ToListAsync();
            return list.ToList();
        }

        [HttpGet("Groups")]
        public async Task<List<Group>> GetGroups()
        {
            var list = _unitOfWork.GroupRepository.Get();
            //var list = await db.Groups.ToListAsync();
            return list.ToList();
        }

        [HttpGet("Groups.idSpeciality={idSpeciality}")]
        public async Task<List<Group>> GetGroupsFromSpeciality([FromRoute] int idSpeciality)
        {
            var list = _unitOfWork.GroupRepository.Get(filter: g => g.IdSpeciality == idSpeciality);
            //var list = (await db.Groups.ToListAsync()).Where(f => f.IdSpeciality == idSpeciality).ToList();
            return list.ToList();
        }

        [HttpGet("Specialities.idGroup={idGroup}")]
        public async Task<Speciality> GetSpecialityFromGroup([FromRoute] int idGroup)
        {
            var group = _unitOfWork.GroupRepository.GetById(idGroup);
            //var group = await db.Groups.FirstOrDefaultAsync(g => g.Id == idGroup);
            var speciality = _unitOfWork.SpecialityRepository.GetById(group.Id);
            //var speciality = await db.Specialities.FirstOrDefaultAsync(s => s.Id == group.Id);
            return speciality;
        }

        [HttpGet("Roles")]
        public async Task<List<Role>> GetRoles()
        {
            var list = _unitOfWork.RoleRepository.Get();
            return list.ToList();
            //return await db.Roles.ToListAsync();
        }

        [HttpGet("EducationForms")]
        public async Task<List<EducationForm>> GetEducationForms()
        {
            var list = _unitOfWork.EducationFormRepository.Get();
            return list.ToList();
            //return await db.EducationForms.ToListAsync();
        }

        [HttpGet("Validate.phone={phone}")]
        public async Task<bool> ValidatePhone([FromRoute] string phone)
        {
            var result = _unitOfWork.UserRepository.GetByExpression(u=> u.Phone.ToString() == phone) == null;
            //var result = await db.Users.FirstOrDefaultAsync(u => u.Phone.ToString() == phone) == null;
            return true;
        }

        [HttpGet("Validate.email={email}")]
        public async Task<bool> ValidateEmail([FromRoute] string email)
        {
            var result = _unitOfWork.UserRepository.GetByExpression(u => u.Email == email) == null;

            //var result = await db.Users.FirstOrDefaultAsync(u => u.Email == email) == null;
            return result;
        }

        [HttpGet("Validate.login={login}")]
        public async Task<bool> ValidateLogin([FromRoute] string login)
        {
            var result = _unitOfWork.UserRepository.GetByExpression(u => u.Login == login) == null;

            //var result = await db.Users.FirstOrDefaultAsync(u => u.Login == login) == null;
            return result;
        }

        [HttpPost("Register")]
        public async Task<KeyValuePair<int, string>> RegisterUser([FromBody] User user)
        {
            try
            {
                var group = _unitOfWork.GroupRepository.GetById(user.IdGroup);
                var city = _unitOfWork.CityReposytory.GetById(user.IdCity);
                var role = _unitOfWork.RoleRepository.GetById(user.IdRole);
                var school = _unitOfWork.SchoolReposytory.GetById(user.IdSchool);

                user.IdCityNavigation = city;
                user.IdGroupNavigation = group;
                user.IdRoleNavigation = role;
                user.IdSchoolNavigation = school;

                var savedUser = _unitOfWork.UserRepository.Insert(user).Entity;
                _unitOfWork.Save();
                //db.Entry(user).State = EntityState.Added;
                //if (user.IdRole == 1)
                //{
                //    db.Entry(user.IdGroupNavigation).State = EntityState.Detached;
                //}
                //db.Entry(user.IdCityNavigation).State = EntityState.Detached;

                //var result = await db.Users.AddAsync(user);
                //await db.SaveChangesAsync();
                //var savedUser = result.Entity;
                var jwt = CreateJwtToken(savedUser);
                return new KeyValuePair<int, string>(savedUser.Id, jwt);
            }
            catch (System.Exception e)
            {

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

            var user = _unitOfWork.UserRepository.Get(
                includeProperties: $"{nameof(SignalIRServerTest.User.IdRoleNavigation)}," +
                $"{nameof(SignalIRServerTest.User.IdCityNavigation)}," +
                $"{nameof(SignalIRServerTest.User.IdSchoolNavigation)}," +
                $"{nameof(SignalIRServerTest.User.IdGroupNavigation)}," +
                $"{nameof(SignalIRServerTest.User.IdEducationFormNavigation)}," +
                $"IdGroupNavigation.IdSpecialityNavigation",
                filter: u => u.Id.ToString() == claims.First().Value)
                .FirstOrDefault();
            //var user = await db.Users
            //    .Include(u => u.IdRoleNavigation)
            //    .Include(u => u.IdCityNavigation)
            //    .Include(u => u.IdSchoolNavigation)
            //    .Include(u => u.IdGroupNavigation)
            //    .Include(u => u.IdEducationFormNavigation)
            //    .Include(u => u.IdGroupNavigation.IdSpecialityNavigation)
            //    .FirstOrDefaultAsync(u => u.Id.ToString() == claims.First().Value);

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
