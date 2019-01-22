using mvcBLOG.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
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
            EmailModel model = new EmailModel();
            return View(model);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Contact(EmailModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var body = "<p>EmailModel From: <bold>{0}</bold>({1})</p><p>Message: </p><p>{2}</p>";
                    var from = "MyBlog<example@gmail.com>";
                    model.Body = "This is a message from your blog site. The name and the conacting person is above." + model.Body;
                    var email = new MailMessage(from, ConfigurationManager.AppSettings["emailto"])
                    {
                        Subject = "Portfolio Contact Email",
                        Body = string.Format(body, model.FromName, model.FromEmail, model.Body),
                        IsBodyHtml = true
                    };
                    var svc = new PersonalEmail();
                    await svc.SendAsync(email);
                    return RedirectToAction("Index");

                }
                catch(Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    await Task.FromResult(0);
                };
            }
            return View(model);
        }

    }
}