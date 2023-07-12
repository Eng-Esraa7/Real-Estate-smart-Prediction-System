using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(RealState.Startup))]
namespace RealState
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
