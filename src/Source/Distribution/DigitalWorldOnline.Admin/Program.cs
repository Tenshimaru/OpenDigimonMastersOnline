using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;

namespace DigitalWorldOnline.Admin
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args).ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                    webBuilder.UseUrls("https://localhost:41001", "https://localhost:5002", "http://localhost:41002");
                });
    }
}
