using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.FileProviders;
using System.IO;
using ImageSharp;

namespace Cats.Controllers
{
    public class CatsController : Controller
    {
        protected IFileProvider FileProvider {get;}
        public CatsController(IFileProvider fileProvider) 
        {
            this.FileProvider = fileProvider;
        }

        [RouteAttribute("[controller]")]
        public string Index()
        {
            var cats = this.FileProvider.GetDirectoryContents(Path.Combine("wwwroot", "images", "cats"));

            return $"{cats.Count()} cats";
        }

        [RouteAttribute("[controller]/{width}/{height}")]
        public IActionResult Get(int width, int height)
        {

            var cat = this.FileProvider.GetDirectoryContents(Path.Combine("wwwroot", "images", "cats")).OrderBy(_ => Guid.NewGuid()).FirstOrDefault();
            //var image = (Bitmap)Bitmap.FromFile(cat.PhysicalPath);
            // Get the image's original width and height
            
            using (var image = Image.Load(cat.PhysicalPath))
            {
                using (var output = new MemoryStream())
                {
                    image.Resize(new ImageSharp.Processing.ResizeOptions
                    {
                        Size = new ImageSharp.Size(width, height),
                        Mode = ImageSharp.Processing.ResizeMode.Min
                    }).Resize(new ImageSharp.Processing.ResizeOptions
                    {
                        Size = new ImageSharp.Size(width, height),
                        Mode = ImageSharp.Processing.ResizeMode.Crop
                    });
                    image.Save(output);
                    return File(output.ToArray(), "image/jpeg");
                }
            }
        }
    }
}
