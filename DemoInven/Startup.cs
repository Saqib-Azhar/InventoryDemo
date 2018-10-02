using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(DemoInven.Startup))]
namespace DemoInven
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
