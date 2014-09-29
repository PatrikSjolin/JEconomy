using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(JEconomy.Startup))]
namespace JEconomy
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
