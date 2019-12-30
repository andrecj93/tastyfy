using Microsoft.AspNetCore.Hosting;

[assembly: HostingStartup(typeof(Tastyfy.Areas.Identity.IdentityHostingStartup))]

namespace Tastyfy.Areas.Identity
{
    public class IdentityHostingStartup : IHostingStartup
    {
        public void Configure(IWebHostBuilder builder)
        {
            builder.ConfigureServices((context, services) =>
            {
            });
        }
    }
}