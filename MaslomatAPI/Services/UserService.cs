using MaslomatAPI.Context;
using MaslomatAPI.Entities;
using MaslomatAPI.Helpers;
using MaslomatAPI.Models;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;

namespace MaslomatAPI.Services
{
    public interface IUserService
    {
        List<User> GetAllUsers();

        UserResponse Authenticate(UserRequest request);

        List<PasswordEntity> GetPasswords(UserRequest request);
        List<PasswordEntity> GetAllPasswords();
        UserResponse Register(UserRequest request);
        PasswordsUpdateRequest SavePassword(PasswordsUpdateRequest request);
        PasswordsUpdateRequest DeletePassword(PasswordsUpdateRequest request);
        User GetById(int id);
    }

    public class UserService : IUserService
    {
        UsersContext _context;
        readonly AppSettings _appSettings;

        public UserService(UsersContext context, IOptions<AppSettings> appSettings)
        {
            _context = context;
            _appSettings = appSettings.Value;
            //ReseedDb();
        }

        void ReseedDb()
        {
            ClearDatabase();
            AddDummyUser();
            AddDummyPasswords();
        }
        void ClearDatabase()
        {
            foreach (var user in _context.Users)
                _context.Users.Remove(user);
            foreach (var password in _context.Passwords)
                _context.Passwords.Remove(password);
            _context.SaveChanges();
        }

        void AddDummyUser()
        {
            _context.Add(new User()
            {
                Name = "Ktos",
                Password = "pass",

            });
            _context.Add(new User()
            {
                Name = "Ktos2",
                Password = "pass2",
            });

            _context.SaveChanges();
        }
        void AddDummyPasswords()
        {
            foreach (var user in _context.Users)
            {
                _context.Passwords.Add(new PasswordEntity() { Designation = "Test1", Password = "pa1", UserID = user.UserID });
                _context.Passwords.Add(new PasswordEntity() { Designation = "Test2", Password = "pa2", UserID = user.UserID });
                _context.Passwords.Add(new PasswordEntity() { Designation = "Test3", Password = "pa3", UserID = user.UserID });
            }
            _context.SaveChanges();
        }

        public List<User> GetAllUsers()
        {
            return _context.Users.ToList();
        }

        public List<PasswordEntity> GetAllPasswords()
        {
            return _context.Passwords.ToList();
        }


        public UserResponse Authenticate(UserRequest request)
        {
            var user = _context.Users.SingleOrDefault(x => x.Name == request.Name && x.Password == request.Password);

            if (user == null)
                return null;

            string token = GenerateToken(user);

            return new UserResponse(user, token);
        }

        public UserResponse Register(UserRequest request)
        {
            var user = _context.Users.SingleOrDefault(x => x.Name == request.Name);

            //username already taken
            if (user != null) return null;

            //create user
            user = new User() { Name = request.Name, Password = request.Password };
            _context.Users.Add(user);
            _context.SaveChanges();

            string token = GenerateToken(user);

            return new UserResponse(user, token);
        }

        public List<PasswordEntity> GetPasswords(UserRequest request)
        {
            var user = _context.Users.SingleOrDefault(x => x.Name == request.Name && x.Password == request.Password);

            if (user == null)
                return null;

            var passwords = _context.Passwords.Where(x => x.UserID == user.UserID).ToList();

            return passwords;
        }

        public PasswordsUpdateRequest SavePassword(PasswordsUpdateRequest request)
        {
            var user = _context.Users.SingleOrDefault(x => x.Name == request.Username);

            if (user == null)
                return null;

            foreach (var password in request.Passwords)
            {
                var existingPassword = _context.Passwords.FirstOrDefault(x => x.UserID == user.UserID && x.Designation == password.Designation);
                if (existingPassword != null)
                {
                    existingPassword.Password = password.Password;
                }
                else
                {
                    var p = new PasswordEntity { Designation = password.Designation, Password = password.Password, UserID = user.UserID };
                    _context.Add(p);
                }
            }

            _context.SaveChanges();

            return request;
        }

        public PasswordsUpdateRequest DeletePassword(PasswordsUpdateRequest request)
        {
            var user = _context.Users.SingleOrDefault(x => x.Name == request.Username);

            if (user == null)
                return null;

            foreach (var password in request.Passwords)
            {
                var existingPassword = _context.Passwords.FirstOrDefault(x => x.UserID == user.UserID && x.Designation == password.Designation);
                if (existingPassword != null)
                {
                    _context.Passwords.Remove(existingPassword);
                }
            }

            _context.SaveChanges();

            return request;
        }

        public User GetById(int id)
        {
            return _context.Users.FirstOrDefault(x => x.UserID == id);
        }

        private string GenerateToken(User user)
        {
            // generate token that is valid for 7 days
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_appSettings.Secret);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[] { new Claim("id", user.UserID.ToString()) }),
                Expires = DateTime.UtcNow.AddDays(7),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
    }
}
