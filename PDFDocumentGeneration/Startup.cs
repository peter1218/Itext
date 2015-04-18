using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(PDFDocumentGeneration.Startup))]
namespace PDFDocumentGeneration
{
    public partial class Startup {
        public void Configuration(IAppBuilder app) {
            ConfigureAuth(app);
        }
    }
}
