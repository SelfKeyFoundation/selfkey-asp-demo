using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using SelfKey.Login.Service;

namespace SelfKey.LoginService
{
    public class Program
    {
        public static void Main(string[] args) => CreateWebHostBuilder(args).Build().Run();

        private static IWebHostBuilder CreateWebHostBuilder(string[] args) => WebHost.CreateDefaultBuilder(args).UseStartup<Startup>();
    }
}
