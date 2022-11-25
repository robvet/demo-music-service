using System;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Azure.KeyVault;
using Microsoft.Azure.Services.AppAuthentication;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.AzureKeyVault;
using Microsoft.Extensions.Hosting;

namespace Catalog.API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        /// <summary>
        ///     1/16/2020 - lw - added ConfigureAppConfiguration to allow Azure Key Vault
        ///     to be included in the configuration builder. This allows us to pull the
        ///     key vault secrets directly from key vault without having to put the secret
        ///     URL in to User Secrets
        /// </summary>
        public static IHostBuilder CreateHostBuilder(string[] args)
        {
            return Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    // https://learn.microsoft.com/en-us/aspnet/core/fundamentals/host/web-host?view=aspnetcore-6.0#capture-startup-errors
                    webBuilder.CaptureStartupErrors(true);

                https://learn.microsoft.com/en-us/aspnet/core/fundamentals/host/web-host?view=aspnetcore-7.0#detailed-errors
                    webBuilder.UseSetting(WebHostDefaults.DetailedErrorsKey, "true");
                    
                    webBuilder.UseStartup<Startup>();
           
                    //webBuilder.UseUrls("http://localhost:8082");
                    webBuilder.UseKestrel();


                    webBuilder.ConfigureAppConfiguration((builderContext, config) =>
                    {
                        config.AddEnvironmentVariables();
                        //var keyVaultEndpoint = GetKeyVaultEndpoint();
                        //if (!string.IsNullOrEmpty(keyVaultEndpoint))
                        //{
                        //    var azureServiceTokenProvider = new AzureServiceTokenProvider();
                        //    var keyVaultClient = new KeyVaultClient(
                        //        new KeyVaultClient.AuthenticationCallback(
                        //            azureServiceTokenProvider.KeyVaultTokenCallback));
                        //    config.AddAzureKeyVault(
                        //        keyVaultEndpoint, keyVaultClient, new DefaultKeyVaultSecretManager());
                        //}
                    });
                });
        }

        // Gets the root Azure Key Vault endpoint from a machine level environment variable
        // that is populated in the DeployToAzure.ps1 script
        private static string GetKeyVaultEndpoint()
        {
            return Environment.GetEnvironmentVariable("KEYVAULT_ENDPOINT_MEMORY_LANE", EnvironmentVariableTarget.Machine);
        }
    }
}