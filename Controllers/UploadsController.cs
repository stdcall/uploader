// More info at
// https://docs.microsoft.com/en-us/aspnet/core/mvc/models/file-uploads?view=aspnetcore-2.1

using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;
using System.Linq;
using helloworld.Services;
using helloworld.DTOs;
using helloworld.Helpers;
namespace helloworld.controllers
{
    [Authorize]
    [Route("api/[controller]")]
    public class UploadsController : Controller {
        private IUserService _userService;
        private IUploadService _uploadService;
        public UploadsController(IUserService userService, IUploadService uploadService)
        {
            _userService = userService;
            _uploadService = uploadService;
        }
        [HttpGet]
        public IActionResult GetAll()
        {
            int uid = int.Parse(this.User.FindFirstValue(ClaimTypes.Name));
            var uplds = _uploadService.ForOwner(uid).OrderBy(x => x.Date);
            var uploadDTOs = from u in uplds
                            select new UploadDTO() 
                            {Id = u.Id, Name = u.Name, Date = u.Date}; 

            return Ok(uploadDTOs);
        }
        [HttpPost]
        public IActionResult UploadFile(List<IFormFile> files) // async Task<>
        {
            if (files.Count == 0) return BadRequest(new { message = "Zero files" });
            try
            {
                foreach (var formFile in files)
                {
                    if (formFile.Length > 0)
                    {
                        using (var memoryStream = new MemoryStream())
                        {
                            int uid = int.Parse(this.User.FindFirstValue(ClaimTypes.Name));

                            string fname = formFile.FileName;
                            formFile.CopyTo(memoryStream);
                            //await formFile.CopyToAsync(memoryStream);
                            byte[] file = memoryStream.ToArray();
                        
                            _uploadService.Create(fname, file, uid);
                        }
                    }
                }
                return Ok();
            }
            catch(Exception ex)
            {
                return BadRequest(new { message = ex.Message});
            }
        }
        [AllowAnonymous]
        [HttpGet("{id}")]
        public FileResult DownloadFile(int id)
        {
            try 
            {
                byte[] blob;
                string fname;
                _uploadService.Download(id, out fname, out blob);
                return File(blob, "application/x-msdownload", fname);
            }
            catch(AppException)
            {
                return null;
            }
        }
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            int uid = int.Parse(this.User.FindFirstValue(ClaimTypes.Name));
            _uploadService.Delete(id, uid);
            return Ok();
        }  
    }
}