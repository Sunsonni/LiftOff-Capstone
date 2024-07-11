using System;
using System.IO;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Trips.Data;
using Trips.Models;
using Trips.ViewModels;

using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Authorization;

namespace Trips.Controllers

{
    public class ReviewController : Controller
    {
        private readonly TripDbContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public ReviewController(TripDbContext context, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
        }

        public IActionResult Index()
        {
            var reviews = _context.Reviews.ToList();
            var reviewViewModels = reviews
                .Select(r => new ReviewViewModel
                {
                    Author = r.Author,
                    Title = r.Title,
                    Content = r.Content,
                    PostedDate = r.PostedDate,
                    ImagePath = r.ImagePath 
                })
                .ToList();

            return View(reviewViewModels);
        }

        [HttpGet]
        public IActionResult Create()
        {
            var reviewViewModel = new ReviewViewModel();
            return View(reviewViewModel);
        }
        [Authorize]
        [HttpPost]
        public IActionResult Create(ReviewViewModel reviewViewModel)
        {
            if (ModelState.IsValid)
            {
                var review = new Review
                {
                    Author = reviewViewModel.Author,
                    Title = reviewViewModel.Title,
                    Content = reviewViewModel.Content,
                    PostedDate = DateTime.Now
                };

                
                _context.Reviews.Add(review);
                _context.SaveChanges();

               
                if (reviewViewModel.ImageFile != null && reviewViewModel.ImageFile.Length > 0)
                {
                    var uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "images");
                    var fileName = Guid.NewGuid().ToString() + "_" + Path.GetFileName(reviewViewModel.ImageFile.FileName);
                    var filePath = Path.Combine(uploadsFolder, fileName);

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        reviewViewModel.ImageFile.CopyTo(stream);
                    }

                    
                    review.ImagePath = "/images/" + fileName;
                    _context.SaveChanges(); 
                }

                return RedirectToAction("Details", new { id = review.Id });
            }

            
            return View("Create", reviewViewModel);
        }

        public IActionResult Details(int id)
        {
            var review = _context.Reviews.FirstOrDefault(r => r.Id == id);

            if (review == null)
            {
                return NotFound(); 
            }

            var reviewViewModel = new ReviewViewModel
            {
                Author = review.Author,
                Title = review.Title,
                Content = review.Content,
                PostedDate = review.PostedDate,
                ImagePath = review.ImagePath 
            };

            return View(reviewViewModel);
        }

        public IActionResult Edit()
        {
            return View();
        }

        public IActionResult Delete()
        {
            return View();
        }
    }
}
