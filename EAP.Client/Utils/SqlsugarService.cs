using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SqlSugar;
using System.Reflection;

namespace EAP.Client.Utils
{
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
            string? oracleConnectionString = $"DataSource={Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location)}\\data.sqlite";

            SqlSugar.ConnectionConfig connectionConfig = new SqlSugar.ConnectionConfig()
            {
                DbType = SqlSugar.DbType.Sqlite,
                ConnectionString = oracleConnectionString,
                IsAutoCloseConnection = false,
                InitKeyType = InitKeyType.Attribute,
                MoreSettings = new ConnMoreSettings()
                {
                    IsAutoToUpper = false // 是否转大写，默认是转大写的可以禁止转大写
                },
                ConfigureExternalServices = new ConfigureExternalServices//把不包含id的字段设为可空
                {
                    EntityService = (t, column) =>
                    {
                        if (!column.PropertyName.ToLower().Contains("id"))
                        {
                            column.IsNullable = true;
                        }
                    }
                }
            };

            //Action<string, SugarParameter[]> onLogExecuting = (sql, pars) =>
            //{
            //    // OnLogExecuting 的逻辑
            //};

            SqlSugarScope sqlSugar = new SqlSugarScope(connectionConfig, db =>
            {
                //db.Aop.OnLogExecuting = onLogExecuting;
            });

            //初始化namespace 为EAP.Client.Model.Database的表
            var assembly = Assembly.GetExecutingAssembly();
            var types = assembly.GetTypes();
            sqlSugar.CodeFirst.InitTables(types.Where(t => t.Namespace != null && t.IsClass == true && t.Namespace.StartsWith("EAP.Client.Model.Database")).ToArray());

            return sqlSugar;
        }
    }

}
