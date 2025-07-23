using SqlSugar;
using System.Reflection;

namespace HandlerAgv.Service.Services
{
    public class SqlsugarService
    {
        public static SqlSugarClient GetSqlSugarClient(IConfiguration configuration)
        {
            string? oracleConnectionString = configuration.GetConnectionString("OracleDb");
            var db = new SqlSugarClient(new ConnectionConfig()
            {
                DbType = DbType.Oracle,
                ConnectionString = oracleConnectionString,
                IsAutoCloseConnection = true,
                InitKeyType = InitKeyType.Attribute,
                MoreSettings = new ConnMoreSettings()
                {
                    IsAutoToUpper = false // 是否转大写，默认是转大写的可以禁止转大写
                }

            }); //默认SystemTable
            return db;
        }

        public static SqlSugarClient GetSqlSugarClient(string connString)
        {
            var db = new SqlSugarClient(new ConnectionConfig()
            {
                DbType = DbType.Oracle,
                ConnectionString = connString,
                IsAutoCloseConnection = true,
                InitKeyType = InitKeyType.Attribute,
                MoreSettings = new ConnMoreSettings()
                {
                    IsAutoToUpper = false // 是否转大写，默认是转大写的可以禁止转大写
                }

            }); //默认SystemTable
            return db;
        }
    }

    public static class SqlSugarServiceExtensions
    {
        public static IServiceCollection AddSqlSugarService(this IServiceCollection services)
        {
            services.AddSingleton<ISqlSugarClient>(ConfigureSqlSugar);


            return services;
        }

        static ISqlSugarClient ConfigureSqlSugar(IServiceProvider serviceProvider)
        {
            IConfiguration configuration = serviceProvider.GetRequiredService<IConfiguration>();
            string? oracleConnectionString = configuration.GetConnectionString("OracleDb");

            ConnectionConfig connectionConfig = new ConnectionConfig()
            {
                DbType = DbType.Oracle,
                ConnectionString = oracleConnectionString,
                IsAutoCloseConnection = true,
                InitKeyType = InitKeyType.Attribute,
                MoreSettings = new ConnMoreSettings()
                {
                    IsAutoToUpper = false // 是否转大写，默认是转大写的可以禁止转大写
                }
            };

            Action<string, SugarParameter[]> onLogExecuting = (sql, pars) =>
            {
                // OnLogExecuting 的逻辑
            };

            SqlSugarScope sqlSugar = new SqlSugarScope(connectionConfig, db =>
            {
                db.Aop.OnLogExecuting = onLogExecuting;
            });


            //Code First 生成表
            var assembly = Assembly.GetExecutingAssembly();           
            //Assembly wrsmodels = Assembly.Load("Rms.Models");
            var typesInNamespace = assembly.GetTypes()
               .Where(t => t.Namespace != null && t.IsClass && t.Namespace.StartsWith("HandlerAgv.Service.Models.Database"))
               .ToList();

            foreach (var type in typesInNamespace)
            {
                try
                {
                    if (type.Name == "RecipeBody")
                    {
                        continue;
                    }

                    sqlSugar.CodeFirst.InitTables(type);
                }
                catch (Exception)
                {
                    continue;
                }
            }

            return sqlSugar;
        }


    }
}
