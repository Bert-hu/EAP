using HandlerAgv.Service.Models.Database;
using SqlSugar;

namespace HandlerAgv.Service.Services
{
    public class DbConfigurationService
    {

        private readonly IConfiguration configuration;
        private readonly ISqlSugarClient _sqlSugarClient;
        private static bool _isDevelopment = true;

        public DbConfigurationService(ISqlSugarClient sqlSugarClient, IConfiguration configuration, IWebHostEnvironment env)
        {
            this.configuration = configuration;
            this._sqlSugarClient = sqlSugarClient;
            _isDevelopment = env.IsDevelopment();
        }

        public string? GetConfigurations(string key)
        {

            var configurations = _sqlSugarClient.Queryable<HandlerConfig>().InSingle(key);
            if (configurations != null)
            {
                if (_isDevelopment)
                {
                    return configurations.DEBUGVALUE;
                }
                else
                {
                    return configurations.VALUE;
                }
            }
            else
            {
                //update db
                if (_isDevelopment)
                {
                    var item = new HandlerConfig() { KEY = key, DEBUGVALUE = null };
                    var x = _sqlSugarClient.Storageable<HandlerConfig>(item).ToStorage();
                    x.AsInsertable.ExecuteCommand();//不存在插入
                    x.AsUpdateable.UpdateColumns(z => z.DEBUGVALUE).ExecuteCommand();//存在更新
                }
                else
                {
                    var item = new HandlerConfig() { KEY = key, VALUE = null };
                    var x = _sqlSugarClient.Storageable<HandlerConfig>(item).ToStorage();
                    x.AsInsertable.ExecuteCommand();//不存在插入
                    x.AsUpdateable.UpdateColumns(z => z.VALUE).ExecuteCommand();//存在更新
                }
                return null;
            }
        }

        public void SetConfigurations(string key, string value)
        {
            using (var _sqlSugarClient = SqlsugarService.GetSqlSugarClient(configuration))
            {
                if (_isDevelopment)
                {
                    var item = new HandlerConfig() { KEY = key, DEBUGVALUE = value };
                    var x = _sqlSugarClient.Storageable<HandlerConfig>(item).ToStorage();
                    x.AsInsertable.ExecuteCommand();//不存在插入
                    x.AsUpdateable.UpdateColumns(z => z.DEBUGVALUE).ExecuteCommand();//存在更新
                }
                else
                {
                    var item = new HandlerConfig() { KEY = key, VALUE = value };
                    var x = _sqlSugarClient.Storageable<HandlerConfig>(item).ToStorage();
                    x.AsInsertable.ExecuteCommand();//不存在插入
                    x.AsUpdateable.UpdateColumns(z => z.VALUE).ExecuteCommand();//存在更新
                }
            }
        }

    }

    public static class DbConfigurationServiceExtensions
    {
        public static IServiceCollection AddDbConfigurationService(this IServiceCollection services)
        {
            services.AddSingleton<DbConfigurationService>();
            return services;
        }
    }
}
