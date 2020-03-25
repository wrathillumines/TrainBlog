using Microsoft.AspNet.Identity;
using Newtonsoft.Json;
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
    public class HomeController : Controller
    {
        ApplicationDbContext db = new ApplicationDbContext();

        //
        // reCAPTCHA
        /// <summary>  
        /// Validate Captcha  
        /// </summary>  
        /// <param name="response"></param>  
        /// <returns></returns> 
        public static Captcha ValidateCaptcha(string response)
        {
            string secret = System.Web.Configuration.WebConfigurationManager.AppSettings["recaptchaPrivateKey"];
            var client = new WebClient();
            var jsonResult = client.DownloadString(string.Format("https://www.google.com/recaptcha/api/siteverify?secret={0}&response={1}", secret, response));
            return JsonConvert.DeserializeObject<Captcha>(jsonResult.ToString());
        }

        //
        // Password Checking Device
        public ActionResult CheckForPassword()
        {
            var userId = User.Identity.GetUserId();
            var passHash = db.Users.Find(userId).PasswordHash;
            return Content(passHash);
        }

        //
        // GET: Index
        public ActionResult Index(int? page)
        {
            int pageSize = 5;
            int pageNumber = (page ?? 1);
            ViewBag.Carousel = db.Carousels.OrderBy(c => Guid.NewGuid()).ToList();
            ViewBag.AboutSnippet = db.AboutSnippets.OrderBy(a => a.Id).ToList();

            return View(db.BlogPosts.OrderByDescending(b => b.Created).ToPagedList(pageNumber, pageSize));
        }

        //
        // GET: About
        public ActionResult About()
        {
            return View(db.Abouts.FirstOrDefault());
        }

        //
        // GET: AboutSnippetEdit
        [Authorize(Roles = "King")]
        public ActionResult AboutSnippetEdit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            AboutSnippet aboutSnippet = db.AboutSnippets.Find(id);
            if (aboutSnippet == null)
            {
                return HttpNotFound();
            }
            return View(aboutSnippet);
        }

        //
        // POST: AboutSnippetEdit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AboutSnippetEdit([Bind(Include = "Id,ImageUrl,Text")] AboutSnippet about, HttpPostedFileBase image)
        {
            if (ModelState.IsValid)
            {
                var creator = User.Identity.GetUserId();

                about.CreatorId = creator;

                if (ImageUploadHelper.IsWebFriendlyImage(image))
                {
                    var ext = Path.GetExtension(image.FileName);
                    var fileName = Guid.NewGuid() + ext;
                    image.SaveAs(Path.Combine(Server.MapPath("~/Uploads/"), fileName));
                    about.ImageUrl = "/Uploads/" + fileName;
                }
                db.Entry(about).State = EntityState.Modified;
                db.SaveChanges();
            }

            return RedirectToAction("Index", "Home");
        }

        //
        // GET: AboutEdit
        [Authorize(Roles = "King")]
        public ActionResult AboutEdit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            About about = db.Abouts.Find(id);
            if (about == null)
            {
                return HttpNotFound();
            }
            return View(about);
        }

        //
        // POST: AboutEdit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AboutEdit([Bind(Include = "Id,ImageUrl,Text")] About about, HttpPostedFileBase image)
        {
            if (ModelState.IsValid)
            {
                var creator = User.Identity.GetUserId();

                about.CreatorId = creator;

                if (ImageUploadHelper.IsWebFriendlyImage(image))
                {
                    var ext = Path.GetExtension(image.FileName);
                    var fileName = Guid.NewGuid() + ext;
                    image.SaveAs(Path.Combine(Server.MapPath("~/Uploads/"), fileName));
                    about.ImageUrl = "/Uploads/" + fileName;
                }
                db.Entry(about).State = EntityState.Modified;
                db.SaveChanges();
            }

            return RedirectToAction("Index", "Home");
        }

        //
        // GET: CarouselPartial
        public ActionResult CarouselPartial()
        {
            return View(db.Carousels.OrderByDescending(c => c.ImageUrl).ToList());
        }

        //
        // GET: CarouselUploader
        [Authorize(Roles = "King")]
        public ActionResult CarouselUploader()
        {
            return View();
        }

        //
        // POST: CarouselUploader
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CarouselUploader(Carousel carousel, HttpPostedFileBase image)
        {
            if (ModelState.IsValid)
            {
                carousel.CreatorId = User.Identity.GetUserId();

                if (ImageUploadHelper.IsWebFriendlyImage(image))
                {
                    var ext = Path.GetExtension(image.FileName);
                    var fileName = Guid.NewGuid() + ext;
                    image.SaveAs(Path.Combine(Server.MapPath("~/Uploads/"), fileName));
                    carousel.ImageUrl = "/Uploads/" + fileName;
                }
                db.Carousels.Add(carousel);
                db.SaveChanges();

                return RedirectToAction("Index");
            }

            return RedirectToAction("UploadFailed");
        }

        //
        // GET: UploadFailed
        public ActionResult UploadFailed()
        {
            return View();
        }

        //
        // GET: ManageCarousel
        [Authorize(Roles = "King")]
        public ActionResult ManageCarousel()
        {
            return View(db.Carousels.OrderByDescending(i => i.Id));
        }

        //
        // GET: CarouselDelete
        [Authorize(Roles = "King")]
        public ActionResult CarouselDelete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Carousel carousel = db.Carousels.Find(id);
            if (carousel == null)
            {
                return HttpNotFound();
            }
            return View(carousel);
        }

        //
        // POST: CarouselDelete
        [HttpPost, ActionName("CarouselDelete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Carousel carousel = db.Carousels.Find(id);
            db.Carousels.Remove(carousel);
            db.SaveChanges();
            return RedirectToAction("Index", "Home");
        }

        //
        // GET: PrivacyPolicy
        public ActionResult PrivacyPolicy()
        {
            return View();
        }

        //
        // GET: Contact
        public ActionResult Contact()
        {
            return View();
        }

        //
        // POST: Contact
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Contact(EmailModel model)
        {
            Captcha response = ValidateCaptcha(Request["g-recaptcha-response"]);

            if (response.Success && ModelState.IsValid)
            {
                try
                {
                    var from = $"{model.FromEmail}<{WebConfigurationManager.AppSettings["emailto"]}>";

                    var email = new MailMessage(from, WebConfigurationManager.AppSettings["emailto"])
                    {
                        Subject = $"Railfan Site Message From {model.FromName} - {model.Subject}",
                        Body = model.Body,
                        IsBodyHtml = true
                    };

                    var svc = new PersonalEmail();
                    await svc.SendAsync(email);

                    return RedirectToAction("Sent");
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    await Task.FromResult(0);
                }
            }
            return RedirectToAction("Whoops");
        }

        //
        // GET: Whoops
        public ActionResult Whoops()
        {
            return View();
        }

        //
        // GET: Email Sent
        public ActionResult Sent()
        {
            return View();
        }
    }
}