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
using PagedList;
using PagedList.Mvc;
using mvcBLOG.Helpers;

namespace mvcBLOG.Controllers
{
    [RequireHttps]
    public class HomeController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        public ActionResult Index(int? page, string searchstr)
        {
            ViewBag.Search = searchstr;
            var blogList = IndexSearch(searchstr);

            var pageSize = 5;
            var pageNumber = (page ?? 1);

            //var temp = db.BlogPosts.OrderByDescending(B => B.Created).Where(x => x.Published == true).ToList();
            //var listPosts = db.BlogPosts.AsQueryable();

            //return View(listPosts.OrderByDescending(p => p.Created).Where(x => x.Published == true).ToPagedList(pageNumber, pageSize));
            return View(blogList.OrderByDescending(B => B.Created).Where(x => x.Published == true).ToPagedList(pageNumber, pageSize));
        }

        public IQueryable<BlogPost> IndexSearch(string searchStr)
        {
            IQueryable<BlogPost> result = null;
            if (searchStr != null)
            {
                result = db.BlogPosts.AsQueryable();
                result = result.Where(p => p.Title.Contains(searchStr) || 
                p.Body.Contains(searchStr) || 
                p.Comments.Any(c => c.Body.Contains(searchStr) || 
                c.Author.FirstName.Contains(searchStr) || 
                c.Author.LastName.Contains(searchStr) || 
                c.Author.DisplayName.Contains(searchStr) || 
                c.Author.Email.Contains(searchStr)));
            }
            else
            {
                result = db.BlogPosts.AsQueryable();
            }
            return result.OrderByDescending(p => p.Created);
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
        //get conntact view action 1
        public ActionResult Contact()
        {
            //assigns the email model to model 2
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
                        Subject = "Blog Contact Email",
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