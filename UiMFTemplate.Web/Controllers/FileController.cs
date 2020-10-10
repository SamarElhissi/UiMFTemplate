namespace UiMFTemplate.Web.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Threading.Tasks;
    using Filer.Core;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Options;
    using Microsoft.Extensions.Primitives;
    using Microsoft.Net.Http.Headers;
    using UiMFTemplate.Infrastructure;
    using UiMFTemplate.Infrastructure.Configuration;
    using UiMFTemplate.Infrastructure.User;
    using UiMFTemplate.Users;
    using SixLabors.ImageSharp;
    using SixLabors.ImageSharp.Processing;

    public class FileController : Controller
    {
        private readonly IWebHostEnvironment env;
        private readonly IFileManager fileManager;
        private readonly UserContext userContext;
        private readonly UserManager<ApplicationUser> userManager;
        private readonly IOptions<AppConfig> appConfig;

        public FileController(IFileManager fileManager,
            UserContext userContext,
            UserManager<ApplicationUser> userManager,
            IWebHostEnvironment env,
            IOptions<AppConfig> appConfig)
        {
            this.fileManager = fileManager;
            this.userContext = userContext;
            this.userManager = userManager;
            this.env = env;
            this.appConfig = appConfig;
        }

        public static void ResizeAndSaveImage(Stream stream, string filename)
        {
            using (var image = Image.Load(stream))
            {
                // now we can get the new height and width
                var newWidth = image.Width > 800 ? 800 : image.Width;
                var newHeight = (newWidth / image.Width) * image.Height;

                image.Mutate(x => x
                    .Resize(newWidth, newHeight)
                );
                image.Save(filename); // Automatic encoder selected based on extension.
            }
        }

        [HttpGet]
        public virtual async Task<FileResult> Download(int id)
        {
            var file = await this.fileManager.Files
                .Include(t => t.Data)
                .SingleOrDefaultAsync(t => t.Id == id);

            var data = file.DecompressFile();

            // 1. All files are read-only (due to the design of Filer), hence
            //	we can cache files on client's machine. However just in case for some reason
            //	files ever do change, we want cache to be invalidated, that is why cache time is
            //	set to 24 hours only. 
            // 2. We also need to indicate that files shouldn't be cached on a proxy/intermediary,
            //	because access to those files might require special security permissions. This
            //	caching on a proxy might pose a security concern.
            var lastModified = new DateTimeOffset(file.CreatedOn);
            var entityTag = new EntityTagHeaderValue("\"file-" + file.Id + "\"");
            this.Response.Headers.Add(new KeyValuePair<string, StringValues>("Cache-Control", "private, max-age=86400"));

            // Images and PDFs should be displayed in the browser directly, i.e. -
            // content-disposition header shouldn't be set.
            if (file.Name.IsImageFilename() ||
                file.Name.EndsWith("pdf", StringComparison.OrdinalIgnoreCase))
            {
                return this.File(
                    data,
                    file.MimeType,
                    lastModified,
                    entityTag);
            }

            return this.File(
                data,
                file.MimeType,
                file.Name,
                lastModified,
                entityTag);
        }

        [HttpGet]
        public List<string> GetWhiteListMimeType()
        {
            return MimeTypeUtils.WhiteListMimeType.Select(o => o.Key).ToList();
        }

        [HttpPost]
        public async Task<IActionResult> Upload()
        {
            var user = this.userContext.IsAuthenticated
                ? await this.userManager.Users.SingleOrDefaultAsync(t => t.UserName == this.userContext.User.UserName)
                : null;

            if (user == null)
            {
                return this.Unauthorized();
            }

            var files = this.Request.Form.Files;
            var filesResult = new List<int>();

            foreach (var file in files)
            {
                if (file.Length > 0)
                {
                    using (var fileStream = file.OpenReadStream())
                    {
                        var fileId = await this.fileManager.SaveFile(
                            file.FileName,
                            file.ContentType,
                            ReadFully(fileStream),
                            CompressionFormat.None,
                            user.Id);

                        var entity = await this.fileManager.GetById(fileId);

                        filesResult.Add(entity.Id);
                    }
                }
            }

            return new JsonResult(new
            {
                FileIds = filesResult.ToArray()
            });
        }

        [HttpPost]
        public async Task<IActionResult> Upload2()
        {
            var user = this.userContext.IsAuthenticated
                ? await this.userManager.Users.SingleOrDefaultAsync(t => t.UserName == this.userContext.User.UserName)
                : null;

            if (user == null)
            {
                return this.Unauthorized();
            }

            var files = this.Request.Form.Files;
            var newFileName = string.Empty;

            foreach (var file in files)
            {
                if (file.Length > 0)
                {
                    var fileName = file.FileName;
                    //Assigning Unique Filename (Guid)
                    var myUniqueFileName = Convert.ToString(Guid.NewGuid());

                    //Getting file Extension
                    var fileExtension = Path.GetExtension(fileName);

                    // concating FileName + FileExtension
                    newFileName = myUniqueFileName + fileExtension;

                    // Combines two strings into a path.
                    fileName = Path.Combine(this.env.WebRootPath, "images", newFileName);

                    ResizeAndSaveImage(file.OpenReadStream(), fileName);
                }
            }

            return new JsonResult(new
            {
                url = this.appConfig.Value.SiteRoot + "/images/" + newFileName
            });
        }

        /// <summary>
        /// Reads stream into a byte array.
        /// </summary>
        /// <param name="input">Stream instance.</param>
        /// <returns>Byte array.</returns>
        private static byte[] ReadFully(Stream input)
        {
            using (var ms = new MemoryStream())
            {
                input.CopyTo(ms);
                return ms.ToArray();
            }
        }
    }
}
