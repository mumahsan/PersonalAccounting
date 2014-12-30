using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(PersonelAccounting.Web.Startup))]
namespace PersonelAccounting.Web
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
