using System;
using System.IO;
using System.Linq;
using InfoExam1.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using File = InfoExam1.Core.File;

namespace InfoExam1.Web.Controllers
{
	public class UserController : Controller
	{
		private readonly ApplicationDbContext _context;

		public UserController(ApplicationDbContext context)
		{
			_context = context;
		}

		public IActionResult Find(string sdesc)
		{
			var file = _context.Files.FirstOrDefault(x => x.ShortDescription.Contains(sdesc));
			return View("ShowFile", file);
		}

		[HttpPost]
		public IActionResult AddFile()
		{
			var formCollection = HttpContext.Request.Form;
			var file = formCollection.Files[0];
			using (var fileStream = System.IO.File.Create($"{file.FileName}"))
			{
				file.CopyTo(fileStream);
			}

			var shortDesc = formCollection["ShortDesc"];
			var description = formCollection["Description"];
			var fileEntity = new File
			{
				ShortDescription = shortDesc,
				Description = description,
				FileName = file.FileName,
				TimesDownloaded = 0,
				DownloadCountLimit = int.Parse(formCollection["limit"]),
				UploadDateTime = DateTime.Now,
				StoreDeadline = DateTime.Now.AddDays(1),
				OptionalPassword = !string.IsNullOrEmpty(formCollection["password"].ToString()) ? formCollection["password"].ToString() : null
			};
			_context.Files.Add(fileEntity);
			_context.SaveChanges();
			return View("/Views/FileUp.cshtml");
		}

		public IActionResult ShowFiles()
		{
			var array = _context.Files.ToArray();
			return View("/Views/Files.cshtml", array);
		}

		public IActionResult FilesByName()
		{
			var array = _context.Files.OrderBy( x=> x.FileName).ToArray();
			return View("/Views/Files.cshtml", array);
		}
		
		public IActionResult GetFile(string filename)
		{
			var file = _context.Files.FirstOrDefault(x => x.FileName.StartsWith(filename));
			if (file.StoreDeadline < DateTime.Now)
			{
				_context.Files.Remove(file);
				return View("Error", new InvalidOperationException("File storage time has expired"));
			}
			return View("ShowFile", file);
		}
		
		public IActionResult FilesByDescription()
		{
			var array = _context.Files.OrderBy( x=> x.ShortDescription).ToArray();
			return View("/Views/Files.cshtml", array);
		}

		public IActionResult DownloadFile(string filename)
		{
			var firstOrDefault = _context.Files.FirstOrDefault(x => x.FileName== filename ||   x.FileName.StartsWith(filename) );
			if (firstOrDefault == null)
				return NotFound();
			firstOrDefault.TimesDownloaded += 1;
			_context.SaveChanges();

			var readAllBytes = System.IO.File.ReadAllBytes($"{filename}");
			var reg = Microsoft.Win32.Registry.ClassesRoot.OpenSubKey(Path.GetExtension($"{filename}").ToLower());
			var contentType = "application/unknown";

			if (reg != null)
			{
				var registryContentType = reg.GetValue("Content Type") as string;
				if (!string.IsNullOrWhiteSpace(registryContentType))
					contentType = registryContentType;
			}

			return File(readAllBytes, contentType);
		}
	}
}