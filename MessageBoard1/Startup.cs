using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(MessageBoard1.Startup))]
namespace MessageBoard1
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
