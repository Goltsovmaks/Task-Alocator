using Microsoft.AspNetCore.JsonPatch;
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
        public IActionResult ManageEvent(int user_id = -1, int event_to_remove = -1)
        {
            if (user_id == -1) return RedirectToAction("Index");
            ViewBag.UserId = user_id;
            //удалим событие, если оно задано
            if (event_to_remove != -1)
            {
                Event event_record = new Event() { EventId = event_to_remove };
                db.Events.Attach(event_record);
                db.Events.Remove(event_record);
                db.SaveChanges();
            }
            return View(db.Events.ToList());
        }
        [HttpPost]
        public string ManageEvent(Event event_record, bool is_mixed, DateTime date_to_start, int duration)
        {
            if (!is_mixed) //запрет за наложение
            {
                if (duration == 0) //способ задания даты = интервал [dateBegin,dateEnd]
                {
                    //проверим, есть ли хоть одна пересекающаяся с event_record запись
                    var intersected_event = db.Events.FirstOrDefault(
                            crnt_event => !(crnt_event.DateEnd <= event_record.DateBegin ||
                                            event_record.DateEnd <= crnt_event.DateBegin));

                    if (intersected_event != null)
                    {
                        return "Пересекается с " + intersected_event.EventId;
                    }
                }
                
                // способ задание даты - продолжительность, поиск подходящей продолжительности
                else if (db.Events.Count() != 0)
                {
                    var sorted_events = db.Events.OrderBy(e => e.DateBegin);//.ThenBy(e => e.DateEnd);

                    DateTime max_date = date_to_start;

                    foreach(Event e in sorted_events)
                    {
                        if ((e.DateBegin - max_date).TotalDays >= duration) //нашли ближайший промежуток
                        {
                            break;
                        }

                        if (e.DateEnd > max_date)
                        {
                            max_date = e.DateEnd;
                        }
                    }

                    date_to_start = max_date;
                }
            }

            if (duration != 0)
            {
                event_record.DateBegin = date_to_start;
                event_record.DateEnd = date_to_start.AddDays(duration);
            }

            //нет пересечения или это нам не важно
            db.Events.Add(event_record);
            db.SaveChanges();
            return "Запись добавлена";
        }


        [HttpGet]
        public IActionResult EditEvent(int? event_id)
        {

            //Обработка ошибок
           /* if (event_id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Event event_ = db.Events.Find(event_id);
            if (event_ == null)
            {
                return HttpNotFound();
            }*/

                var edited_record = db.Events.SingleOrDefault(e => e.EventId == event_id);
            // != null
            return View(edited_record);
        }

         [HttpPost]
         public IActionResult EditEvent(Event event_record, bool is_mixed)
         {
            if (!is_mixed) //поставили запрет на нлаожение
            {
                //Дублирование кода!!!
                //проверим, есть ли хоть одна пересекающаяся с event_record запись
                var intersected_event = db.Events.FirstOrDefault(
                        crnt_event => !(crnt_event.DateEnd <= event_record.DateBegin ||
                                        event_record.DateEnd <= crnt_event.DateBegin));

                if (intersected_event != null)
                {
                    ModelState.AddModelError("EventId", "Дата пересекается с датой №" + intersected_event.EventId);
                    return View(event_record);
                }
            }


            var edited_record = db.Events.SingleOrDefault(e => e.EventId == event_record.EventId);
            edited_record.DateBegin = event_record.DateBegin;
            edited_record.DateEnd = event_record.DateEnd;

            edited_record.Description = event_record.Description;
            db.SaveChanges();
            return RedirectToAction("ManageEvent", "Home",
                                    new { user_id = event_record.UserId, event_to_remove = -1 });
         }

        //пересекаются ли интервалы
        public static bool isIntersection(DateTime lhs_begin, DateTime lhs_end,
                            DateTime rhs_begin, DateTime rhs_end)
        {
            return !(lhs_end <= rhs_begin ||
                     rhs_end <= lhs_begin);
        }
    }
}
