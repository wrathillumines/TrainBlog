﻿using Microsoft.AspNet.Identity;
using PagedList;
using System;
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

            return View(db.BlogPosts.OrderByDescending(b => b.Created).ToPagedList(pageNumber, pageSize));
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
        // GET: About
        public ActionResult About()
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
            if (ModelState.IsValid)
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
            return View(model);
        }

        //
        // GET: Email Sent
        public ActionResult Sent()
        {
            return View();
        }
    }
}