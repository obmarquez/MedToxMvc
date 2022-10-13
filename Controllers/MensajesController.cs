using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MedToxMVC.Controllers
{
    public class MensajesController : Controller
    {
        public IActionResult IndexNutricion()
        {
            return View();
        }

        public IActionResult IndexOdontologia()
        {
            return View();
        }
    }
}
