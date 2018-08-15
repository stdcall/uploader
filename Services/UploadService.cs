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
        void Download(int id, out string fname, out byte[] file);
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
        public void Download(int id, out string fname, out byte[] file)
        {
            var upload = _context.Uploads.Find(id);
            if (upload == null) throw new AppException("File doesn't exist.");
            fname = upload.Name;
            if (upload.Blob != null)
            {
                file = upload.Blob; 
            }
            else
            {
                file = _context.Uploads.First(x => 
                x.Blob != null && upload.Sha256.SequenceEqual(x.Sha256)).Blob; 
            }  
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