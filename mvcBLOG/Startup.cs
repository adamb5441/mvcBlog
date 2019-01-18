using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(mvcBLOG.Startup))]
namespace mvcBLOG
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
