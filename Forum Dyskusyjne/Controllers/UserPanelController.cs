using Forum_Dyskusyjne.Data;
using Forum_Dyskusyjne.Models;
using Forum_Dyskusyjne.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Forum_Dyskusyjne.Controllers
{
    public class UserPanelController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IFileService _fileService;

        public UserPanelController(ApplicationDbContext context, IFileService fileService)
        {
            _context = context;
            _fileService = fileService;
        }

        [Authorize(Roles = "Administrator,NormalUser")]
        public IActionResult Index()
        {
            string LoggedUserEmail = User.Identity.Name;
            User user = _context.Users
                 .Where(x => x.Email == LoggedUserEmail)
                 .FirstOrDefault();

            return View(user);
        }

        [HttpPost]
        [Authorize(Roles = "Administrator,NormalUser")]
        public IActionResult Index(IFormFile image)
        {
            string LoggedUserEmail = User.Identity.Name;
            User user = _context.Users
                 .Where(x => x.Email == LoggedUserEmail)
                 .FirstOrDefault();

            if (image != null)
            {
                if (image.ContentType == "image/png" || image.ContentType == "image/jpeg")
                {
                    if (image.Length > 0 && image.Length < 1000000)     // Ograniczenie wielkości pliku do 1 MB
                    {
                        if (_fileService.IsImageValid(image))           // Ograniczenie rozmiaru do 1000 x 1000
                        {
                            var fileName = _fileService.UploadAvatar(image);
                            user.AvatarPath = fileName;

                            _context.Users.Update(user);
                            _context.SaveChanges();

                            ViewBag.Message = "Avatar image uploaded succesfully :)";
                        }
                        else
                        {
                            ViewBag.Message = "Max file width and height is 1000px x 1000px";
                        }
                    }
                    else
                    {
                        ViewBag.Message = "Max file size is 1 MB!";
                    }
                }
                else
                {
                    ViewBag.Message = "Only .png and .jpg files!";
                }
            }

            return View(user);
        }
        [HttpPost]
        [Authorize(Roles = "Administrator,NormalUser")]
        public IActionResult ChangeMessagePaging(int newMessagesPaging)
        {
            string LoggedUserEmail = User.Identity.Name;
            User user = _context.Users
                 .Where(x => x.Email == LoggedUserEmail)
                 .FirstOrDefault();

            if(ModelState.IsValid)
            {
                if(newMessagesPaging > 0 && newMessagesPaging <= 30)
                {
                    user.MessagePaging = newMessagesPaging;
                    _context.Update(user);
                    _context.SaveChanges();
                }
            }

            return RedirectToAction("Index");
        }
    }
}
