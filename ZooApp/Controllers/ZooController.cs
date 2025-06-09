using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ZooApp.Data;
using System.Linq;
using ZooApp.Models;

namespace ZooApp.Controllers
{
    public class ZooController : Controller
    {
        private readonly ZooContext _context;

        public ZooController(ZooContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Sunrise()
        {
            var enclosures = _context.Enclosures
                .Include(e => e.Animals)
                .ToList();

            var result = enclosures.Select(e => new
            {
                EnclosureName = e.Name,
                SunriseStatuses = e.Animals.Select(a => new
                {
                    a.Name,
                    a.ActivityPattern,
                    Status = a.GetSunriseStatus()
                }).ToList()
            }).ToList();

            return View(result);
        }


        public IActionResult Sunset()
        {
            var enclosures = _context.Enclosures
                .Include(e => e.Animals)
                .ToList();

            var result = enclosures.Select(e => new
            {
                EnclosureName = e.Name,
                SunsetStatuses = e.Animals.Select(a => new
                {
                    a.Name,
                    a.ActivityPattern,
                    Status = a.GetSunsetStatus()
                }).ToList()
            }).ToList();

            return View(result);
        }



        public IActionResult FeedingTime()
        {
            var enclosures = _context.Enclosures
                .Include(e => e.Animals)
                .ThenInclude(a => a.Prey)
                .ToList();

            var result = enclosures.Select(e => new
            {
                EnclosureName = e.Name,
                Warning = e.Animals.Any(pred =>
                    pred.DietaryClass == DietaryClass.Carnivore &&
                    pred.Prey != null &&
                    pred.Prey.Any(p => e.Animals.Contains(p)))
                    ? " Let op: prooidieren aanwezig bij roofdieren!"
                    : null,
                DietDetails = e.Animals.Select(a => new
                {
                    a.Name,
                    a.DietaryClass,
                    Description = a.GetFeedingDescription()
                })
            });

            return View(result);
        }


        public IActionResult CheckConstraints()
        {
            var enclosures = _context.Enclosures
                .Include(e => e.Animals)
                .ToList();

            var result = enclosures.Select(e => new
            {
                EnclosureName = e.Name,
                Size = e.Size,
                AnimalCount = e.Animals.Count,
                RequiredSpace = e.Animals.Sum(a => a.SpaceRequirement),
                Status = e.Size >= e.Animals.Sum(a => a.SpaceRequirement)
                    ? "✅ Verblijf voldoet aan alle eisen."
                    : "⚠️ Verblijf heeft onvoldoende ruimte."
            });

            return View(result);
        }

    }
}
