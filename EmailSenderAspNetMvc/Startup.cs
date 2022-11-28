using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(EmailSenderAspNetMvc.Startup))]
namespace EmailSenderAspNetMvc
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
