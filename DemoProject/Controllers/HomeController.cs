using DemoProject.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace DemoProject.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult About()
        {
            ViewData["Message"] = "Your application description page.";

            return View();
        }

        public IActionResult Contact()
        {
            ViewData["Message"] = "Your contact page.";

            return View();
        }

        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        public string GetEmployeeName(int employeeId)
        {
            string name;
            if (employeeId == 1)
            {
                name = "Bob";
            }
            else if (employeeId == 2)
            {
                name = "Sue";
            }
            else
            {
                name = "Not Found";
            }

            return name;
        }
    }
}