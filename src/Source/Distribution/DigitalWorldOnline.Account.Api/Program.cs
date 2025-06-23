namespace DigitalWorldOnline.Api
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
            .ConfigureWebHostDefaults(webBuilder =>
            {
                webBuilder.UseStartup<Startup>();
                webBuilder.UseEnvironment("Adm");
            })
            .ConfigureHostConfiguration(hostConfig =>
            {
                hostConfig.SetBasePath(Directory.GetCurrentDirectory());
                hostConfig.AddEnvironmentVariables("DSO");
            });
    }
}