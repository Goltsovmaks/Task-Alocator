using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Task_Alocator.Models;

namespace Task_Alocator.Controllers
{
    public class HomeController : Controller
    {
        EventContext db;

        public HomeController(EventContext context)
        {
            db = context;
        }

        [HttpGet]
        public IActionResult Index()
        {
              return View(db.Users.ToList());
        }

        [HttpPost]
        public string Index(User user)
        {
            db.Users.Add(user);
            db.SaveChanges();
            return "Пользователь, " + user.UserId + ", добавлен.";
        }

        [HttpGet]
        public IActionResult AddEvent(int? id)
        {
            if (id == null) return RedirectToAction("Index");
            ViewBag.UserId = id;
            return View(db.Events.ToList());
        }
        [HttpPost]
        public string AddEvent(Event event_record)
        {
            db.Events.Add(event_record);
            // сохраняем в бд все изменения
            db.SaveChanges();
            return "Запись для"  + event_record.UserId+ ", добавлена";
        }

    }
}
