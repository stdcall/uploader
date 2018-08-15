using System;
using System.Linq;
using System.Collections.Generic;
using helloworld.Entities;
using helloworld.Helpers;
using System.Security.Cryptography;
namespace helloworld.Services 
{
    
    public interface IUploadService
    {
        Upload Create(string fname, byte[] file, int owner);
        (string, byte[]) Download(int id);
        void Delete(int id, int uid);
        IEnumerable<Upload> ForOwner(int uid);
    }

    public class UploadService : IUploadService
    {
        private fileappdbContext _context;
        private  SHA512CryptoServiceProvider _csp; 
        public UploadService(fileappdbContext context)
        {
            _context = context;
            _csp = new SHA512CryptoServiceProvider();
        }

        public IEnumerable<Upload> ForOwner(int uid) =>
          _context.Uploads.Where(x => x.Owner == uid);
      
        public Upload Create(string fname, byte[] file, int owner)
        {
            Random rnd = new Random();
            Upload upload = new Upload() {
                Name = fname,
                Date = DateTime.Now.AddDays(rnd.Next(-100,100)),
                Owner = owner,
                Sha256 = _csp.ComputeHash(file)
            };
            if (!_context.Uploads.Any(x => 
                x.Sha256.SequenceEqual(upload.Sha256))) 
            {
                upload.Blob = file;
            }
            _context.Uploads.Add(upload);
            _context.SaveChanges();
            
            return upload;
        }
        public (string, byte[]) Download(int id)
        {
            var upload = _context.Uploads.Find(id);
 
            var name = upload != null ? upload.Name : 
                throw new AppException("File doesn't exist.");;
 
            var file  = upload.Blob != null ? upload.Blob : 
                _context.Uploads.First(x => 
                    x.Blob != null && upload.Sha256.SequenceEqual(x.Sha256)).Blob;
           
            return (name, file);
        }
        public void Delete(int id, int uid)
        {
            var upload = _context.Uploads.Find(id);
            if (upload != null && upload.Owner == uid)
            {
                if (upload.Blob != null) 
                {
                    var otherupl = _context.Uploads.FirstOrDefault(x => 
                        x.Sha256.SequenceEqual(upload.Sha256) && x.Id != upload.Id);
                    if (otherupl != null) 
                    {
                        otherupl.Blob = upload.Blob;
                        _context.Uploads.Update(otherupl);
                    }
                }
                _context.Uploads.Remove(upload);
                _context.SaveChanges();
            }
        }
    }
}