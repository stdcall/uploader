using System;
using System.Collections.Generic;

namespace helloworld.Entities
{
    public partial class Upload
    {
        public Upload()
        {
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public byte[] Sha256 { get; set; }
        public byte[] Blob { get; set; }
        public DateTime Date { get; set; }
        public int Owner { get; set; }

        public User OwnerNavigation { get; set; }
    }
}


namespace helloworld.DTOs 
{
    public class UploadDTO
    {
        public int Id {get; set;}

        public string Name {get; set;}

        public DateTime Date {get; set;}
    }
}