using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(BlockChainExplorer.Startup))]
namespace BlockChainExplorer
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
