using PagedList;
using System;
using System.Linq;
using System.Net.Mail;
using System.Threading.Tasks;
using System.Web.Configuration;
using System.Web.Mvc;
using TrainBlog.Models;

namespace TrainBlog.Controllers
{
    public class HomeController : Controller
    {
        ApplicationDbContext db = new ApplicationDbContext();

        //
        // GET: Index
        public ActionResult Index(int? page)
        {
            int pageSize = 3;
            int pageNumber = (page ?? 1);
            return View(db.BlogPosts.OrderByDescending(b => b.Created).ToPagedList(pageNumber, pageSize));
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
                        Subject = $"Portfolio Site Message From {model.FromName} - {model.Subject}",
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
    }
}