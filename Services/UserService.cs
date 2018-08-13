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

            // check if username exists
            if (user == null)
                return null;

            // check if password is correct
            if (!VerifyPasswordHash(password, user.Password, user.Salt))
                return null;
            // authentication successful
            return user;
        }

        public User GetById(int id)
        {
            return _context.Users.Find(id);
        }

        public User Create(User user, string password)
        {
            // validation
            if (string.IsNullOrWhiteSpace(password))
                throw new AppException("Password is required");

            if (_context.Users.Any(x => x.Email == user.Email))
                throw new AppException("Email \"" + user.Email + "\" is already registered");

            byte[] passwordHash, passwordSalt;
            CreatePasswordHash(password, out passwordHash, out passwordSalt);

            user.Password = passwordHash;
            user.Salt = passwordSalt;

            _context.Users.Add(user);
            _context.SaveChanges();

            return user;
        }

       private static void CreatePasswordHash(string pwd, out byte[] pwdHash, out byte[] pwdSalt)
        {
            if (string.IsNullOrWhiteSpace(pwd)) 
                throw new ArgumentException("Value cannot be empty.", "password");

            var hmac = new HMACSHA512();
            pwdSalt = hmac.Key;
            pwdHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(pwd));        
        }

        private static bool VerifyPasswordHash(string pwd, byte[] Hash, byte[] Salt)
        {
            if (string.IsNullOrWhiteSpace(pwd)) 
                throw new ArgumentException("Value cannot be empty.", "password");

            var hmac = new HMACSHA512(Salt);
            var computedHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(pwd));
            return computedHash.SequenceEqual(Hash);
        }

    }
}

