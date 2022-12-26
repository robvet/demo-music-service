using System;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

namespace Catalog.API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

       
        public static IHostBuilder CreateHostBuilder(string[] args)
        {
            return Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    // https://learn.microsoft.com/en-us/aspnet/core/fundamentals/host/web-host?view=aspnetcore-6.0#capture-startup-errors
                    webBuilder.CaptureStartupErrors(true);

                    //https://learn.microsoft.com/en-us/aspnet/core/fundamentals/host/web-host?view=aspnetcore-7.0#detailed-errors
                    webBuilder.UseSetting(WebHostDefaults.DetailedErrorsKey, "true");

                    webBuilder.UseStartup<Startup>();

                    webBuilder.ConfigureAppConfiguration((builderContext, config) =>
                    {
                        config.AddEnvironmentVariables();                      
                    });
                });
        } 
    }
}