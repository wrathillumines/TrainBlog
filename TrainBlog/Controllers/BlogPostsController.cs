using HunterW_Blog.Utilities;
using Microsoft.AspNet.Identity.Owin;
using PagedList;
using System;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using System.Web;
using System.Web.Configuration;
using System.Web.Mvc;
using TrainBlog.Helpers;
using TrainBlog.Models;

namespace TrainBlog.Controllers
{
    public class BlogPostsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        private ImageUploadHelper uploadHelper = new ImageUploadHelper();

        // GET: BlogPosts
        public ActionResult Index(int? page, string searchStr)
        {
            ViewBag.Search = searchStr;
            var blogList = IndexSearch(searchStr);

            int pageSize = 5;
            int pageNumber = (page ?? 1);

            return View(blogList.ToPagedList(pageNumber, pageSize));
        }

        //LIST AND SEARCH POSTS
        public IQueryable<BlogPost> IndexSearch(string searchStr)
        {
            IQueryable<BlogPost> result = null;
            if (searchStr != null)
            {
                result = db.BlogPosts.AsQueryable();
                result = result.Where(p => p.Title.Contains(searchStr) ||
                    p.Abstract.Contains(searchStr) ||
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

        // GET: Photo Gallery
        public ActionResult Gallery(int? page)
        {
            int pageSize = 9;
            int pageNumber = (page ?? 1);

            //return View(db.BlogPosts.Where(b => b.MediaUrl != null).OrderBy(b => Guid.NewGuid()).ToPagedList(pageNumber, pageSize));

            return View(db.BlogPosts.Where(b => b.MediaUrl != null).OrderByDescending(b => b.Created).ToPagedList(pageNumber, pageSize));
        }

        // GET: BlogPosts/Details/5
        public ActionResult Details(string slug)
        {
            if (slug == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            BlogPost blogPost = db.BlogPosts.FirstOrDefault(b => b.Slug == slug);
            if (blogPost == null)
            {
                return HttpNotFound();
            }
            return View(blogPost);
        }

        // GET: BlogPosts/Create
        [Authorize(Roles = "King")]
        public ActionResult Create()
        {
            return View();
        }

        // POST: BlogPosts/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "Title,Abstract,Body")] BlogPost blogPost, HttpPostedFileBase image)
        {
            if (ModelState.IsValid)
            {
                //Create slug
                var Slug = StringUtilities.MakeSlug(blogPost.Title);
                if (string.IsNullOrWhiteSpace(Slug))
                {
                    ModelState.AddModelError("Title", "Invalid Title");
                    return View(blogPost);
                }

                //Check for duplicate slug
                if (db.BlogPosts.Any(p => p.Slug == Slug))
                {
                    ModelState.AddModelError("Title", "Title must be unique.");
                    return View(blogPost);
                }

                //Image upload
                if (ImageUploadHelper.IsWebFriendlyImage(image))
                {
                    var ext = Path.GetExtension(image.FileName);
                    var fileName = Guid.NewGuid() + ext;
                    image.SaveAs(Path.Combine(Server.MapPath("~/Uploads/"), fileName));
                    blogPost.MediaUrl = "/Uploads/" + fileName;
                }

                blogPost.Slug = Slug;
                blogPost.Created = DateTimeOffset.Now;
                db.BlogPosts.Add(blogPost);
                db.SaveChanges();

                //Subscription email
                if (ModelState.IsValid)
                {
                    var subbed = db.Users.Where(u => u.Subscribed == true);
                    var emailFrom = WebConfigurationManager.AppSettings["emailfrom"];
                    var emailTo = string.Join(",", subbed.Select(s => s.Email));
                    MailMessage mailMessage = new MailMessage(emailFrom, emailTo)
                    {
                        Subject = "New Blog Post",
                        IsBodyHtml = true,
                        Body = "<h4>New Blog Post on HW Railfanning!</h4><br /><p>To view this post, <a href='hwrailfanning.net'>click here</a> or go to hwrailfanning.net in your browser.</p><br /><p>Thanks,</p><p>Hunter</p><br /><p><small>You are currently subscribed to email updates. To unsubscribe, please visit the Edit Profile section of the blog site.</small></p>"
                    };

                    var service = new PersonalEmail();
                    await service.SendAsync(mailMessage);

                    return RedirectToAction("Posted", "BlogPosts");
                }
                return RedirectToAction("Index", "Home");
            }
            return RedirectToAction("Index", "Home");
        }

        // GET: BlogPosts/Edit/5
        [Authorize(Roles = "King")]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            BlogPost blogPost = db.BlogPosts.Find(id);
            if (blogPost == null)
            {
                return HttpNotFound();
            }
            return View(blogPost);
        }

        // POST: BlogPosts/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Title,Abstract,Slug,Body,MediaUrl,Created,Updated")] BlogPost blogPost, HttpPostedFileBase image)
        {
            if (ModelState.IsValid)
            {
                if (ImageUploadHelper.IsWebFriendlyImage(image))
                {
                    var ext = Path.GetExtension(image.FileName);
                    var fileName = Guid.NewGuid() + ext;
                    image.SaveAs(Path.Combine(Server.MapPath("~/Uploads/"), fileName));
                    blogPost.MediaUrl = "/Uploads/" + fileName;
                }
                db.Entry(blogPost).State = EntityState.Modified;
                blogPost.Updated = DateTimeOffset.Now;
                db.SaveChanges();
                return RedirectToAction("Index", "Home");
            }
            return View("Index", "Home");
        }

        // GET: BlogPosts/Delete/5
        [Authorize(Roles = "King")]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            BlogPost blogPost = db.BlogPosts.Find(id);
            if (blogPost == null)
            {
                return HttpNotFound();
            }
            return View(blogPost);
        }

        // POST: BlogPosts/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            BlogPost blogPost = db.BlogPosts.Find(id);
            db.BlogPosts.Remove(blogPost);
            db.SaveChanges();
            return RedirectToAction("Index", "Home");
        }

        //GET: Posted
        public ActionResult Posted()
        {
            return View();
        }

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
