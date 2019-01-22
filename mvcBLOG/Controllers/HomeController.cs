using mvcBLOG.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace mvcBLOG.Controllers
{
    public class HomeController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        public ActionResult Index()
        {
            var temp = db.BlogPosts.OrderByDescending(B => B.Created).Where(x => x.Published == true).ToList();

            return View(temp);
        }

        public ActionResult About()
        {

            var temp = db.BlogPosts.OrderByDescending(B => B.Created).Where(x => x.Published == true).ToList();
            if (temp.Count <= 5)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            else
            {
                temp.RemoveRange(0, 5);
                return View(temp);
            }
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}