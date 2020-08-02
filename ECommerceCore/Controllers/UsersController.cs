using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using ECommerceCore.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ECommerceCore.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly ShopoContext _shopoContext;

        public UsersController(ShopoContext shopoContext)
        {
            _shopoContext = shopoContext;
        }

        // GET: api/Users/5
        [HttpGet(Name = "Get")]
        public ActionResult<Users> Get([FromQuery] string email, [FromQuery] string password)
        {
            Users user;

            if (new EmailAddressAttribute().IsValid(email) && password != "")
            {
                var passwordBytes = System.Text.Encoding.UTF8.GetBytes(password);
                user = _shopoContext.Users
               .Select(u => new Users { UserId = u.UserId, Email = u.Email, Name = u.Name, DateOfBirth = u.DateOfBirth, Phone = u.Phone, DateJoined = u.DateJoined, UpdatedDate = u.UpdatedDate, Activated = u.Activated, Password = u.Password })
               .FirstOrDefault(u => u.Email == email && u.Password == passwordBytes);
                if (user == null)
                {
                    user = new Users();
                    return NotFound();
                }

            }
            else
            {
                return BadRequest();
            }
            return user;
        }

        public class PostResponse
        {
            public int Id { get; set; }
        }
        public class UserBody
        {
            public string Email { get; set; }
            public string Name { get; set; }
            public DateTime DateOfBirth { get; set; }
            public string Phone { get; set; }
            public bool Activated { get; set; }
            public string Password { get; set; }
        }
        // POST: api/Users
        [HttpPost]
        public async Task<PostResponse> Post([FromBody] UserBody user)
        {
            var response = new PostResponse();
            if (new EmailAddressAttribute().IsValid(user.Email))
            {

                if (!_shopoContext.Users.Any(u => u.Email == user.Email))
                {
                    var encodedPassword = System.Text.Encoding.UTF8.GetBytes(user.Password);

                    Users newUser = new Users();
                    newUser.Email = user.Email;
                    newUser.Name = user.Name;
                    newUser.DateOfBirth = user.DateOfBirth;
                    newUser.Phone = user.Phone;
                    newUser.Activated = user.Activated;
                    newUser.DateJoined = DateTime.Now;
                    newUser.UpdatedDate = DateTime.Now;
                    newUser.Password = encodedPassword;

                    _shopoContext.Users.Add(newUser);
                    await _shopoContext.SaveChangesAsync();

                    var fetchUser = (from u in _shopoContext.Users
                                     where u.Email == newUser.Email
                                     select new Users { UserId = u.UserId }).ToList();

                    response.Id = fetchUser[0].UserId;

                }
            }
            else
            {
                BadRequest();
            }
            return response;
        }


        public class PatchResponse
        {
            public bool PasswordChanged { get; set; }
            public bool UserActivatedDeactivated { get; set; }
            public Users User { get; set; }
        }

        public class PatchRequest
        {
            public string Email { get; set; }
            public string Password { get; set; }
            public bool? Activate { get; set; }
            public string ChangedPassword { get; set; }
        }

        // PATCH: api/Users/5
        [HttpPatch]
        public async Task<PatchResponse> Patch([FromBody] PatchRequest request)
        {
            var response = new PatchResponse();
            if (new EmailAddressAttribute().IsValid(request.Email))
            {
                var passwordBytes = System.Text.Encoding.UTF8.GetBytes(request.Password);
                var user = await _shopoContext.Users.FirstOrDefaultAsync(u => u.Email == request.Email && u.Password == passwordBytes);
                if (user != null)
                {
                    if (request.ChangedPassword != "")
                    {
                        var encryptedPassword = System.Text.Encoding.UTF8.GetBytes(request.ChangedPassword);
                        user.Password = encryptedPassword;
                        response.PasswordChanged = true;
                    }
                    else
                    {
                        user.Activated = (bool)request.Activate;
                        response.UserActivatedDeactivated = true;
                    }
                    await _shopoContext.SaveChangesAsync();
                    response.User = user;
                }
            }
            else
            {
                BadRequest();
            }
            return response;
        }


    }
}
