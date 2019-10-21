using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(TrainBlog.Startup))]
namespace TrainBlog
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
