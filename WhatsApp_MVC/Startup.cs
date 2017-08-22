using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(WhatsApp_MVC.Startup))]
namespace WhatsApp_MVC
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
