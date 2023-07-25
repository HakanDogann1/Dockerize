using AspNetCoreMVC.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace AspNetCoreMVC.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IFileProvider _fileProvider;
        private readonly IConfiguration _configuration;
        public HomeController(ILogger<HomeController> logger, IFileProvider fileProvider, IConfiguration configuration)
        {
            _logger = logger;
            _fileProvider = fileProvider;
            _configuration = configuration;
        }

        public IActionResult Index()
        {
            ViewBag.MysqlCon = _configuration["MysqlCon"];
            return View();
        }
        public IActionResult ImageShow()
        {
            var images = _fileProvider.GetDirectoryContents("wwwroot/images").ToList().Select(x => x.Name);
            return View(images);
        }
        [HttpPost]
        public IActionResult ImageShow(string imageName)
        {
            var image = _fileProvider.GetDirectoryContents("wwwroot/images").ToList().First(x=>x.Name==imageName);
            System.IO.File.Delete(image.PhysicalPath);
            return RedirectToAction("ImageShow");
        }
        [HttpGet]
        public IActionResult ImageSave()
        {
            return View();
        }
       
        [HttpPost]
        public async Task<IActionResult> ImageSave(IFormFile formFile)
        {
            if(formFile!=null && formFile.Length>0)
            {
                var fileName = Guid.NewGuid().ToString()+Path.GetExtension(formFile.FileName);
                var path =Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images",fileName);
                using var stream = new FileStream(path, FileMode.Create);
                await formFile.CopyToAsync(stream); 
            }
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
