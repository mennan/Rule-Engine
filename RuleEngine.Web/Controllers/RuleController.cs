using System;
using Microsoft.AspNetCore.Mvc;

namespace RuleEngine.Web.Controllers
{
    public class RuleController : Controller
    {
        public IActionResult Index()
        {

            return View();
        }

        public IActionResult List()
        {
            return View();
        }

        public IActionResult Run(Guid id)
        {
            if (id.Equals(Guid.Empty))
                return NotFound();

            ViewBag.RuleId = id;

            return View();
        }
    }
}
