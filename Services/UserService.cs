using System;
using System.Collections.Generic;
using System.Linq;
using helloworld.Entities;
using helloworld.Helpers;
using System.Security.Cryptography;
namespace helloworld.Services
{
    public interface IUserService
    {
        User Create(User user, string password);
        User GetById(int id);
        User Authenticate(string email, string password);
    }

    public class UserService : IUserService
    {
        private fileappdbContext _context;

        public UserService(fileappdbContext context)
        {
            _context = context;
        }

        public User Authenticate(string email, string password)
        {
            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
                return null;

            var user = _context.Users.SingleOrDefault(x => x.Email == email);

            if (user == null)
                return null;

            if (!VerifyPasswordHash(password, user.Password, user.Salt))
                return null;
            
            return user;
        }

        public User GetById(int id) => _context.Users.Find(id);

        public User Create(User user, string password)
        {
            if (string.IsNullOrWhiteSpace(password))
                throw new AppException("Password is required");

            if (_context.Users.Any(x => x.Email == user.Email))
                throw new AppException($"Email {user.Email} is already registered");

            (user.Password, user.Salt) = CreatePasswordHash(password);
            _context.Users.Add(user);
            _context.SaveChanges();

            return user;
        }

       private static (byte[] pwdHash, byte[] pwdSalt) CreatePasswordHash(string pwd)
        {
            if (string.IsNullOrWhiteSpace(pwd))
                throw new ArgumentException("Value cannot be empty.", "password");

            using (var hmac = new HMACSHA512()) 
            {
                return (hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(pwd)), hmac.Key);
            }
        }

        private static bool VerifyPasswordHash(string pwd, byte[] hash, byte[] salt)
        {
            if (string.IsNullOrWhiteSpace(pwd))
                throw new ArgumentException("Value cannot be empty.", "password");

            using (var hmac = new HMACSHA512(salt))
            {
                var computedHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(pwd));
                return computedHash.SequenceEqual(hash);
            }
        }

    }
}
