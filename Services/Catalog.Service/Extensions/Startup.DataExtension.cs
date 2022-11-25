using System;
using Catalog.API.Infrastructure.DataStore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Catalog.API.Extensions
{
    public static class DataExtension
    {
        public static IServiceCollection RegisterRelationalDatabase(this IServiceCollection services,
            IConfiguration configuration)
        {
            var useInMemoryStore = configuration["Data:UseInMemoryStore"] != null &&
                                 configuration["Data:UseInMemoryStore"]
                                     .Equals("true", StringComparison.OrdinalIgnoreCase);

            //var useInMemoryStore = configInMemory;
            // 1/16/2020 - lw- connection string now pulled from Azure Key Vault
            //var connectionString = configuration["catalogdbsecret"] ??
            //                       throw new ArgumentNullException("Connection string for Catalog database is Null");

            if (useInMemoryStore)
            { 
                services.AddEntityFrameworkInMemoryDatabase()
                    .AddDbContext<DataContext>(options => { options.UseInMemoryDatabase(Guid.NewGuid().ToString()); });
                return services;
            }

            var connectionString = configuration["catalogdbsecret"] ??
                                       throw new ArgumentNullException("catalogdbsecret",
                                           "Data store connection string for catalog missing");

            services.AddEntityFrameworkSqlServer()
                .AddDbContext<DataContext>(options =>
                {
                    options.UseSqlServer(connectionString, sqlOptions =>
                    {
                        sqlOptions.CommandTimeout(45)
                            .EnableRetryOnFailure(
                                3,
                                TimeSpan.FromSeconds(15),
                                null);
                    });
                });


            //if (configuration == null)
            //    throw new Exception("configuration is null");

            //var configInMemory = configuration["Data:UseInMemoryStore"] != null &&
            //                     configuration["Data:UseInMemoryStore"]
            //                         .Equals("true", StringComparison.OrdinalIgnoreCase);

            ////var configInMemory2 = configuration["Data:UseInMemoryStore"]?
            ////                         .Equals("true", StringComparison.OrdinalIgnoreCase);

            //var useInMemoryStore = configInMemory;

            //var connectionStrging = configuration["CatalogConnectionString"];
            //if (useInMemoryStore || string.IsNullOrEmpty(connectionStrging))
            //    services.AddEntityFrameworkInMemoryDatabase()
            //        .AddDbContext<DataContext>(options => { options.UseInMemoryDatabase(); });
            //else
            //    services.AddEntityFrameworkSqlServer()
            //        .AddDbContext<DataContext>(options => { options.UseSqlServer(connectionStrging); });

            return services;
        }
    }
}