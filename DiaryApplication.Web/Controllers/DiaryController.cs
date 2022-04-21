using System;
using System.Web;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using DiaryApplication.Models;
using DiaryApplication.Web.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using DiaryApplication.Web.Services;
using Microsoft.AspNetCore.Http;
using System.IO;
using System.IO.Compression;
using static System.Net.Mime.MediaTypeNames;
using Microsoft.AspNetCore.Hosting;
using DiaryApplication.Models.ViewModels;

namespace DiaryApplication.Web.Controllers
{
    [Authorize]
    public class DiaryController : Controller
    {
        private readonly UserManager<DiaryApplicationUser> _userManager;
        private readonly IDiaryService _diaryService;
        private readonly IHttpContextAccessor _httpContextAccessor;


        public DiaryController(UserManager<DiaryApplicationUser> userManager, IDiaryService diaryService, IHttpContextAccessor httpContextAccessor)
        {
            _userManager = userManager;
            _diaryService = diaryService;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task <IActionResult> Home( )
        {
            var baseUrl = $"{_httpContextAccessor.HttpContext.Request.Scheme}://{_httpContextAccessor.HttpContext.Request.Host}{_httpContextAccessor.HttpContext.Request.PathBase}/uploads/";
            var user = await _userManager.GetUserAsync(User);
            var posts = await _diaryService.GetAllPostsAsync(user.Id);
            var list = new List<DiaryPostEntity>();
            foreach (var item in posts)
            {
                item.ImageUrl = baseUrl + item.ImageUrl;
                list.Add(item);
            }
            return View(list);

        }

        //GET: Diaryontroller
        public async Task<IActionResult> Index()
        {
            var baseUrl = $"{_httpContextAccessor.HttpContext.Request.Scheme}://{_httpContextAccessor.HttpContext.Request.Host}{_httpContextAccessor.HttpContext.Request.PathBase}/uploads/";
            var user = await _userManager.GetUserAsync(User);
            var posts = await _diaryService.GetAllPostsAsync(user.Id);
            var list = new List<DiaryPostEntity>();
            foreach (var item in posts)
            {
                item.ImageUrl = baseUrl + item.ImageUrl;
                list.Add(item);
            }
            return View(list);

        }

        // GET: Diaryontroller/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var baseUrl = $"{_httpContextAccessor.HttpContext.Request.Scheme}://{_httpContextAccessor.HttpContext.Request.Host}{_httpContextAccessor.HttpContext.Request.PathBase}/uploads/";
            var user = await _userManager.GetUserAsync(User);
            var posts = await _diaryService.GetAllPostsAsync(user.Id);
            var list = new List<DiaryPostEntity>();
            foreach(var item in posts)
            {
                item.ImageUrl = baseUrl + item.ImageUrl;
                list.Add(item);
            }
            var diaryPostEntity = await _diaryService.GetPostById(Convert.ToInt32(id), user.Id);
            if (diaryPostEntity == null)
            {
                return NotFound();
            }

            return View(diaryPostEntity);
        }

        // GET: Diaryontroller/Create
        public async Task<IActionResult> Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(DiaryInput diaryInput)
        {
            var diaryPostEntity = new DiaryPostEntity();

            if (ModelState.IsValid)
            {
                if (diaryInput.ImageUrl == null || diaryInput.ImageUrl.Length == 0)
                    return Content("file not selected");

                var path = Path.Combine(
                            Directory.GetCurrentDirectory(), "wwwroot/uploads",
                            diaryInput.ImageUrl.FileName);


                using (var stream = new FileStream(path, FileMode.Create))
                {
                    await diaryInput.ImageUrl.CopyToAsync(stream);
                }
                var user = await _userManager.GetUserAsync(User);
                diaryPostEntity.UserId = user.Id;
                diaryPostEntity.CreatedDate = DateTime.UtcNow;
                diaryPostEntity.Title = diaryInput.Title;
                diaryPostEntity.Content = diaryInput.Content;
                diaryPostEntity.ImageUrl = diaryInput.ImageUrl.FileName;
                await _diaryService.CreatePostAsync(diaryPostEntity);
                return RedirectToAction(nameof(Index));
            }
            return View(diaryPostEntity);
        }


        // GET: Diaryontroller/Edit/5
        public async Task<IActionResult> Edit(int? id )
        {
            var DiaryPostEntity = new DiaryPostEntity();
            if (id == null)
            {
                return NotFound();
            }
            else
            {
                var user = await _userManager.GetUserAsync(User);
                var diaryInput = await _diaryService.GetPostById(Convert.ToInt32(id), user.Id);
                return View(diaryInput);
            }
        }



        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit( int id,DiaryInput diaryInput )
        {   
            if (diaryInput.Id!= null)
            {
                var diaryPostEntity = new DiaryPostEntity();

                if (ModelState.IsValid)
                {
                    if (diaryInput.ImageUrl == null || diaryInput.ImageUrl.Length == 0)
                        return Content("file not selected");

                    var path = Path.Combine(
                                Directory.GetCurrentDirectory(), "wwwroot/uploads",
                                diaryInput.ImageUrl.FileName);


                    using (var stream = new FileStream(path, FileMode.Create))
                    {
                        await diaryInput.ImageUrl.CopyToAsync(stream);
                    }
                    var user = await _userManager.GetUserAsync(User);

                    diaryPostEntity.UserId = user.Id;
                    diaryPostEntity.Id = diaryInput.Id;
                    diaryPostEntity.CreatedDate = DateTime.UtcNow;
                    diaryPostEntity.Title = diaryInput.Title;
                    diaryPostEntity.Content = diaryInput.Content;
                    diaryPostEntity.ImageUrl = diaryInput.ImageUrl.FileName;
                    await _diaryService.UpdatePostAsync(diaryPostEntity);
                    return RedirectToAction(nameof(Index));
                }
                return View(diaryPostEntity);
            }
            else
            {
                return NotFound();
            }
        }

        // GET: Diaryontroller/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var user = await _userManager.GetUserAsync(User);
            var diaryPostEntity = await _diaryService.GetPostById(Convert.ToInt32(id), user.Id);
            if (diaryPostEntity == null)
            {
                return NotFound();
            }

            return View(diaryPostEntity);
        }

        // POST: Diaryontroller/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var user = await _userManager.GetUserAsync(User);
            var diaryPostEntity = await _diaryService.GetPostById(Convert.ToInt32(id), user.Id);
            await _diaryService.DeletePostAsync(diaryPostEntity);
            return RedirectToAction(nameof(Index));
        }
    }
}
