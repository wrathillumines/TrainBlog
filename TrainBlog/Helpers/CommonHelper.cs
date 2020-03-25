using TrainBlog.Models;
using Microsoft.AspNet.Identity;
using System;
using System.Linq;
using System.Web;
using TrainBlog.Enumerations;

namespace TrainBlog.Helpers
{
    public abstract class CommonHelper
    {
        protected static ApplicationDbContext Db = new ApplicationDbContext();
        protected ApplicationUser CurrentUser = new ApplicationUser();
        protected UserRolesHelper RoleHelper = new UserRolesHelper();
        protected SystemRole CurrentRole = SystemRole.None;

        protected CommonHelper()
        {
            var userId = HttpContext.Current.User.Identity.GetUserId();
            if (userId != null)
                CurrentUser = Db.Users.Find(userId);

            var stringRole = RoleHelper.ListUserRoles(userId).FirstOrDefault();

            if (!string.IsNullOrEmpty(stringRole))
                CurrentRole = (SystemRole)Enum.Parse(typeof(SystemRole), stringRole);
        }
    }
}