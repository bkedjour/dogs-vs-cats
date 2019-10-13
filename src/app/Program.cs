using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Google.Cloud.Diagnostics.AspNetCore;

namespace DogsVsCats
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
                    webBuilder.UseGoogleDiagnostics();
                    webBuilder.UseStartup<Startup>();
                });
    }
}
