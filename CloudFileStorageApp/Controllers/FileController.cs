using CloudFileStorageApp.Data;
using CloudFileStorageApp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace CloudFileStorageApp.Controllers
{
    public class FileController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _environment;
        private readonly UserManager<IdentityUser> _userManager;

        public FileController(ApplicationDbContext context, IWebHostEnvironment environment, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _environment = environment;
            _userManager = userManager;
        }

        [Authorize]
        public IActionResult Index()
        {
            var userId = _userManager.GetUserId(User);
            ViewBag.Folders = _context.StoredFiles
                .Where(f => f.UserId == userId && !f.IsDeleted && !string.IsNullOrEmpty(f.FolderName))
                .Select(f => f.FolderName)
                .Distinct()
                .ToList();

            var files = _context.StoredFiles
                .Where(f => f.UserId == userId && !f.IsDeleted)
                .OrderByDescending(f => f.UploadedOn)
                .ToList();

            return View("Index", files);
        }

        [Authorize]
        public IActionResult Folder(string name)
        {
            var userId = _userManager.GetUserId(User);
            ViewBag.Folders = _context.StoredFiles
                .Where(f => f.UserId == userId && !f.IsDeleted && !string.IsNullOrEmpty(f.FolderName))
                .Select(f => f.FolderName)
                .Distinct()
                .ToList();

            var files = _context.StoredFiles
                .Where(f => f.UserId == userId && !f.IsDeleted && f.FolderName == name)
                .OrderByDescending(f => f.UploadedOn)
                .ToList();

            ViewBag.FolderName = name;
            return View("Index", files);
        }

        public IActionResult Recent()
        {
            var userId = _userManager.GetUserId(User);
            var oneWeekAgo = DateTime.Now.AddDays(-7);

            var files = _context.StoredFiles
                .Where(f => f.UserId == userId && !f.IsDeleted && f.UploadedOn >= oneWeekAgo)
                .OrderByDescending(f => f.UploadedOn)
                .ToList();

            ViewBag.Folders = _context.StoredFiles
                .Where(f => f.UserId == userId && !f.IsDeleted && !string.IsNullOrEmpty(f.FolderName))
                .Select(f => f.FolderName)
                .Distinct()
                .ToList();

            return View("Index", files);
        }

        public IActionResult Trash()
        {
            var userId = _userManager.GetUserId(User);
            var files = _context.StoredFiles
                .Where(f => f.UserId == userId && f.IsDeleted)
                .OrderByDescending(f => f.DeletedOn)
                .ToList();

            ViewBag.Folders = new List<string>();
            return View("Index", files);
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Upload(IFormFile file, string? folder)
        {
            if (file != null && file.Length > 0)
            {
                var uploadsFolder = Path.Combine(_environment.WebRootPath, "uploads");
                if (!Directory.Exists(uploadsFolder))
                    Directory.CreateDirectory(uploadsFolder);

                var filePath = Path.Combine(uploadsFolder, file.FileName);
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }

                var userId = _userManager.GetUserId(User);
                var storedFile = new StoredFile
                {
                    FileName = file.FileName,
                    FilePath = "/uploads/" + file.FileName,
                    UploadedOn = DateTime.Now,
                    FolderName = folder,
                    UserId = userId
                };

                _context.StoredFiles.Add(storedFile);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction("Index");
        }

        public IActionResult Download(int id)
        {
            var file = _context.StoredFiles.FirstOrDefault(f => f.Id == id);
            if (file == null)
                return NotFound();

            var path = Path.Combine(_environment.WebRootPath, file.FilePath.TrimStart('/'));
            var mimeType = "application/octet-stream";
            return PhysicalFile(path, mimeType, file.FileName);
        }

        [Authorize]
        public async Task<IActionResult> Delete(int id)
        {
            var file = await _context.StoredFiles.FindAsync(id);
            if (file == null || file.UserId != _userManager.GetUserId(User))
                return NotFound();

            file.IsDeleted = true;
            file.DeletedOn = DateTime.Now;

            _context.Update(file);
            await _context.SaveChangesAsync();

            return RedirectToAction("Index");
        }

        public async Task<IActionResult> PermanentDelete(int id)
        {
            var file = await _context.StoredFiles.FindAsync(id);
            if (file == null || file.UserId != _userManager.GetUserId(User))
                return NotFound();

            var path = Path.Combine(_environment.WebRootPath, file.FilePath.TrimStart('/'));
            if (System.IO.File.Exists(path))
                System.IO.File.Delete(path);

            _context.StoredFiles.Remove(file);
            await _context.SaveChangesAsync();

            return RedirectToAction("Trash");
        }

        public async Task<IActionResult> Restore(int id)
        {
            var file = await _context.StoredFiles.FindAsync(id);
            if (file == null || file.UserId != _userManager.GetUserId(User))
                return NotFound();

            file.IsDeleted = false;
            file.DeletedOn = null;

            _context.Update(file);
            await _context.SaveChangesAsync();

            return RedirectToAction("Trash");
        }

        [HttpPost]
        public async Task<IActionResult> Rename(int id, string newName)
        {
            var file = await _context.StoredFiles.FindAsync(id);
            if (file == null || file.UserId != _userManager.GetUserId(User)) return NotFound();

            var oldPath = Path.Combine(_environment.WebRootPath, file.FilePath.TrimStart('/'));
            var newPath = Path.Combine(_environment.WebRootPath, "uploads", newName);

            System.IO.File.Move(oldPath, newPath);

            file.FileName = newName;
            file.FilePath = "/uploads/" + newName;
            _context.Update(file);
            await _context.SaveChangesAsync();

            return RedirectToAction("Index");
        }

        public async Task<IActionResult> GenerateLink(int id)
        {
            var file = await _context.StoredFiles.FindAsync(id);
            if (file == null || file.UserId != _userManager.GetUserId(User)) return NotFound();

            if (string.IsNullOrEmpty(file.ShareToken))
                file.ShareToken = GenerateShareToken();

            file.IsPublic = true;

            _context.StoredFiles.Update(file);
            await _context.SaveChangesAsync();

            TempData["PublicLink"] = Url.Action("Public", "File", new { token = file.ShareToken }, Request.Scheme);
            return RedirectToAction("Index");
        }

        [AllowAnonymous]
        public IActionResult Public(string token)
        {
            var file = _context.StoredFiles.FirstOrDefault(f => f.ShareToken == token && f.IsPublic);
            if (file == null) return NotFound();

            var path = Path.Combine(_environment.WebRootPath, file.FilePath.TrimStart('/'));
            var mimeType = "application/octet-stream";
            return PhysicalFile(path, mimeType, file.FileName);
        }

        private string GenerateShareToken()
        {
            return Convert.ToBase64String(Guid.NewGuid().ToByteArray())
                .Replace("/", "_").Replace("+", "-");
        }

        public static string GetFileIcon(string fileName)
        {
            string ext = Path.GetExtension(fileName).ToLower();
            return ext switch
            {
                ".pdf" => "fa-solid fa-file-pdf text-danger",
                ".doc" or ".docx" => "fa-solid fa-file-word text-primary",
                ".xls" or ".xlsx" => "fa-solid fa-file-excel text-success",
                ".jpg" or ".jpeg" or ".png" => "fa-solid fa-file-image text-warning",
                ".zip" or ".rar" => "fa-solid fa-file-zipper text-muted",
                ".txt" => "fa-solid fa-file-lines text-info",
                _ => "fa-solid fa-file text-secondary",
            };
        }

        public static string FormatFileSize(string filePath)
        {
            try
            {
                var fullPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", filePath.TrimStart('/'));
                var length = new FileInfo(fullPath).Length;
                return length switch
                {
                    < 1024 => $"{length} B",
                    < 1048576 => $"{length / 1024.0:F2} KB",
                    < 1073741824 => $"{length / 1048576.0:F2} MB",
                    _ => $"{length / 1073741824.0:F2} GB"
                };
            }
            catch
            {
                return "-";
            }
        }
    }
}
