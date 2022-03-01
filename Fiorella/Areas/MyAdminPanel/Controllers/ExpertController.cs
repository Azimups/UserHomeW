using System;
using System.IO;
using System.Threading.Tasks;
using Fiorella.Areas.MyAdminPanel.Data;
using Fiorella.DataLayerAccess;
using Fiorella.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Fiorella.Areas.MyAdminPanel.Controllers
{
    [Area("MyAdminPanel")]
    [Authorize]
    public class ExpertController : Controller
    {
        private readonly AppDbContext _dbContext;
        private readonly IWebHostEnvironment _environment;

        public ExpertController(AppDbContext dbContext, IWebHostEnvironment environment)
        {
            _dbContext = dbContext;
            _environment = environment;
        }
        [AllowAnonymous]
        public async Task<IActionResult> Index()
        {
            var experts = await _dbContext.Experts.ToListAsync();
            return View(experts);
        }

        public async Task<IActionResult> Detail(int? id)
        {
            if (id==null)
            {
                return NotFound();
            }

            var expert = await _dbContext.Experts.FindAsync(id);
            if (expert==null)
            {
                return NotFound();
            }

            return View(expert);
        }

        public async Task<IActionResult> Create()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create( Expert expertImg)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }

                if (!expertImg.Photo.ContentType.Contains("image"))
                {
                    ModelState.AddModelError("Photos","Yuklediyiniz photo olmalidir");
                    return View();
                }
                if (expertImg.Photo.Length > 1024 * 1000)
                {
                    ModelState.AddModelError("Photos","Yuklediyiniz photo 1 mbdan az olmalidir");
                    return View();
                }

                var filename = await expertImg.Photo.GenerateFile(Constants.ImageFolderPath);
                expertImg.Image = filename;
                await _dbContext.Experts.AddAsync(expertImg); 
                await _dbContext.SaveChangesAsync();
            
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id==null)
            {
                return NotFound();
            }

            var expert = await _dbContext.Experts.FindAsync(id);
            if (expert==null)
            {
                return NotFound();
            }

            return View(expert);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName("Delete")]
        public async Task<IActionResult> DeleteExpert(int? id)
        {
            if (id==null)
            {
                return NotFound();
            }

            var expert = await _dbContext.Experts.FindAsync(id);
            if (expert==null)
            {
                return NotFound();
            }

            var path = Path.Combine(Constants.ImageFolderPath, expert.Image);
            if (System.IO.File.Exists(path))
            {
                System.IO.File.Delete(path);
            }
            
            _dbContext.Experts.Remove(expert);
            await _dbContext.SaveChangesAsync();
            
            return RedirectToAction(nameof(Index));
        }
        public async Task<IActionResult> Update(int? id)
        {
            if (id==null)
            {
                return NotFound();
            }

            var expert = await _dbContext.Experts.FindAsync(id);
            if (expert==null)
            {
                return NotFound();
            }

            return View(expert);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(int? id, Expert expert)
        {
            if (id==null)
            {
                return NotFound();
            }

            if (id!=expert.Id)
            {
                return BadRequest();
            }
            var existExpert = await _dbContext.Experts.FindAsync(id);
            
            if (!ModelState.IsValid)
            {
                ModelState.AddModelError("Error","Error var");
                return View(existExpert);
            }
            if (existExpert==null)
            {
                return NotFound();
            }

            if (!expert.Photo.IsImage())
            {
                ModelState.AddModelError("Photo", "Yuklediyiniz shekil olmalidir.");
                return View(existExpert);
            }

            if (!expert.Photo.IsAllowedSize(1))
            {
                ModelState.AddModelError("Photo", "1 mb-dan az olmalidir.");
                return View(existExpert);
            }

            var path = Path.Combine(Constants.ImageFolderPath, existExpert.Image);
            if (System.IO.File.Exists(path))
            {
                System.IO.File.Delete(path);
            }

            var filename = await expert.Photo.GenerateFile(Constants.ImageFolderPath);
            existExpert.Image = filename;
            
            
            existExpert.Name = expert.Name;
            existExpert.Job = expert.Job;
            
            await _dbContext.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
    }
}