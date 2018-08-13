using System;
using System.Collections.Generic;

namespace helloworld.Entities
{
    public partial class User
    {
        public User()
        {
            Uploads = new HashSet<Upload>();
        }

        public int Id { get; set; }
        public string Email { get; set; }
        public byte[] Password { get; set; }
        public byte[] Salt { get; set; }

        public ICollection<Upload> Uploads { get; set; }
    }
}

namespace helloworld.DTOs 
{
    public class UserDTO
    {
        public int Id {get; set;}

        public string Email {get; set;}

        public string Password {get; set;}
    }
}