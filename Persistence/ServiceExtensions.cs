using System;
using Dapper;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Persistence.Repositories;

namespace Persistence
{
    public static class ServiceExtensions
    {
        public static IServiceCollection AddPersistence(this IServiceCollection services, IConfiguration configuration)
        {
            SqlMapper.AddTypeHandler(new MySqlGuidTypeHandler());
            SqlMapper.RemoveTypeMap(typeof(Guid));
            SqlMapper.RemoveTypeMap(typeof(Guid?));

            return services
                .AddSqlClient(configuration)
                .AddRepositories();
        }
        
        private static IServiceCollection AddRepositories(this IServiceCollection services)
        {
            return services
                .AddSingleton<ITodosRepository, TodosRepository>()
                .AddSingleton<IUserRepository, UserRepository>()
                .AddSingleton<IApiKeyRepository, ApiKeyRepository>();
        }

        private static IServiceCollection AddSqlClient(this IServiceCollection services, IConfiguration configuration)
        {
           //var connectionString = configuration.GetSection("ConnectionStrings")["SqlConnectionString"];
           // kitas metodas
           var connectionString = configuration
               .GetSection("ConnectionStrings")
               .GetSection("SqlConnectionString").Value;
           // kitas metodas
           /*var connectionStringBuilder = new MySqlConnectionStringBuilder();
           connectionStringBuilder.Server = "Localhost";
           connectionStringBuilder.Port = 3306;
           connectionStringBuilder.UserID = "testas";
           connectionStringBuilder.Password = "Testas;2020";
           connectionStringBuilder.Database = "todos";
           var connectionString = connectionStringBuilder.GetConnectionString(true);
           var sqlclient = new SqlClient(connectionString);*/
            return services.AddTransient<ISqlClient>(_ => new SqlClient(connectionString));
        }
    }
}