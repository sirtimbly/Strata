using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(IsoComponents.Startup))]
namespace IsoComponents
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
