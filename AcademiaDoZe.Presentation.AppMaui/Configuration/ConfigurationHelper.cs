using AcademiaDoZe.Application.DependencyInjection;
using AcademiaDoZe.Application.Enums;
namespace AcademiaDoZe.Presentation.AppMaui.Configuration
{
    public static class ConfigurationHelper
    {
        public static void ConfigureServices(IServiceCollection services)
        {
            // dados conexão

            const string dbServer = "localhost";
            const string dbDatabase = "db_academia_do_ze";
            const string dbUser = "root";
            const string dbPassword = "0811";
            //const string dbComplemento = "TrustServerCertificate=True;Encrypt=True;";
            // se for necessário indicar a porta, incluir junto em dbComplemento

            // Configurações de conexão
            const string connectionString =
            "Server=mysql-academiaze;Port=3307;Database=db_academia_do_ze;Uid=root;Pwd=0811;";


            const EAppDatabaseType databaseType = EAppDatabaseType.MySql;
            // Configura a fábrica de repositórios com a string de conexão e tipo de banco
            services.AddSingleton(new RepositoryConfig
            {
                ConnectionString = connectionString,
                DatabaseType = databaseType
            });
            // configura os serviços da camada de aplicação
            services.AddApplicationServices();
        }
    }
}