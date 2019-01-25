using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using mvcBLOG.Models;
using mvcBLOG.Helpers;
using System.IO;


namespace mvcBLOG.Controllers
{
    [RequireHttps]
    public class BlogPostsController : Controller
    {
        //declare a db context to interact with
        private ApplicationDbContext db = new ApplicationDbContext();

        public ActionResult Details(string Slug, bool allcomments = false)
        {
            if (String.IsNullOrWhiteSpace(Slug))
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ViewBag.AllComments = allcomments;
            BlogPost blogPost = db.BlogPosts.FirstOrDefault(p => p.Slug == Slug);
            if (blogPost == null)
            {
                return HttpNotFound();
            }
            return View(blogPost);
        }
        // GET: BlogPosts
        [Authorize(Roles = "Admin")]
        public ActionResult Index()
        {
            //insert a list of blogpostes into the view then returns it
            return View(db.BlogPosts.OrderByDescending(B => B.Created).ToList());
        }

        // GET: BlogPosts/Details/5
        //public ActionResult Details(int? id)
        //{
        //    //check for id
        //    if (id == null)
        //    {
        //        //returns a warning of a bad request
        //        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        //    }
        //    //searchs and returns a blogpost with the given id
        //    BlogPost blogPost = db.BlogPosts.Find(id);
        //    //check if the blogpost is found
        //    if (blogPost == null)
        //    {
        //        //returns not found after search
        //        return HttpNotFound();
        //    }
        //    // returns view of the blogpost with the given id
        //    return View(blogPost);
        //}

        // GET: BlogPosts/Create
        [Authorize(Roles = "Admin")] 
        public ActionResult Create()
        {
            //returns the create view
            return View();
        }

        // POST: BlogPosts/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        //post action
        [Authorize(Roles = "Admin")]
        [HttpPost]
        //checks the information is coming from the right view
        [ValidateAntiForgeryToken]
        //create post exepts info create view
        public ActionResult Create([Bind(Include = "Title,Body,Abstract,MediaUrl,Published")] BlogPost blogPost, HttpPostedFileBase image)
        {


            var slug = StringUtilities.URLFriendly(blogPost.Title);
            if (String.IsNullOrWhiteSpace(slug))
            {
                ModelState.AddModelError("Title", "Invalid Title");
                return View(blogPost);
            }
            if(db.BlogPosts.Any(p => p.Slug == slug))
            {
                ModelState.AddModelError("Title", "The title must be unique");
                return View(blogPost);
            }
            //checks against the model for correct data
            if (ModelState.IsValid)
            {

                if (ImageUploadValidator.IsWebFriendlyImage(image))
                {
                    var filename = Path.GetFileName(image.FileName);
                    image.SaveAs(Path.Combine(Server.MapPath("~/Uploads/"), filename));
                    blogPost.MediaUrl = "/Uploads/" + filename;
                }

                blogPost.Slug = slug;
                //add current time to blog post
                blogPost.Created = DateTimeOffset.Now;
                //add to database
                db.BlogPosts.Add(blogPost);
                //save to database
                db.SaveChanges();
                //redirect to index action
                return RedirectToAction("Index");
            }
            //return view blogpost??? seems redudant
            return View(blogPost);
        }
        [Authorize(Roles = "Admin")]
        // GET: BlogPosts/Edit/5
        public ActionResult Edit(int? id)
        {
            // checks for id
            if (id == null)
            {
                //return if no id bad request
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            //search blogpost with given id
            BlogPost blogPost = db.BlogPosts.Find(id);
            //check if found
            if (blogPost == null)
            {
                //return not found 
                return HttpNotFound();
            }
            // return view with given blogpost
            return View(blogPost);
        }

        // POST: BlogPosts/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        //httppost action
        [Authorize(Roles = "Admin")]
        [HttpPost]
        //checks info is coming from the right place
        [ValidateAntiForgeryToken]
        //edit action with body request
        public ActionResult Edit([Bind(Include = "Id,Slug,Created,Updated,Title,Body,Abstract,MediaUrl,Published")] BlogPost blogPost, HttpPostedFileBase image)
        {
            if (ImageUploadValidator.IsWebFriendlyImage(image))
            {
                var filename = Path.GetFileName(image.FileName);
                image.SaveAs(Path.Combine(Server.MapPath("~/Uploads/"), filename));
                blogPost.MediaUrl = "/Uploads/"+ filename;
            }
            var slug = StringUtilities.URLFriendly(blogPost.Title);
            if (String.IsNullOrWhiteSpace(slug))
            {
                ModelState.AddModelError("Title", "Invalid Title");
                return View(blogPost);
            }
            if (blogPost.Slug != slug && db.BlogPosts.Any(p => p.Slug == slug))
            {
                ModelState.AddModelError("Title", "The title must be unique");
                return View(blogPost);
            }
            
            //check against model
            if (ModelState.IsValid)
            {
                blogPost.Slug = slug;
                blogPost.Updated = DateTimeOffset.Now;
                //modifys state
                db.Entry(blogPost).State = EntityState.Modified;
                //save chnges in the model
                db.SaveChanges();
                //return index
                return RedirectToAction("Index");
            }
            //give blogpost
            return View(blogPost);
        }
        [Authorize(Roles = "Admin")]
        // GET: BlogPosts/Delete/5
        public ActionResult Delete(int? id)
        {
            //check for id
            if (id == null)
            {
                // returns bad request if no id
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            //finds blogpost with id
            BlogPost blogPost = db.BlogPosts.Find(id);
            //checks that if was found
            if (blogPost == null)
            {
                //returns not found
                return HttpNotFound();
            }
            //returns view with given blog
            return View(blogPost);
        }

        // POST: BlogPosts/Delete/5
        [Authorize(Roles = "Admin")]
        [HttpPost, ActionName("Delete")]
        //see if request comes from valid place
        [ValidateAntiForgeryToken]
        //delete method
        public ActionResult DeleteConfirmed(int id)
        {
            //finds blogpost with id
            BlogPost blogPost = db.BlogPosts.Find(id);
            ////removes blogpost
            db.BlogPosts.Remove(blogPost);
            //saves changes
            db.SaveChanges();
            //redurects to index action
            return RedirectToAction("Index");
        }
        //reclaim memory
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
