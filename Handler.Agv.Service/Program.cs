
using HandlerAgv.Service.RabbitMq;
using HandlerAgv.Service.Services;
using log4net.Config;
using Microsoft.OpenApi.Models;
using System.Reflection;

namespace HandlerAgv.Service
{
    public class Program
    {
        enum ApiVersion
        {
            v1
        }

        public static void Main(string[] args)
        {
            XmlConfigurator.Configure(new FileInfo("log4net.config"));

            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddSqlSugarService();

            // Add services to the container.
            builder.Services.AddControllersWithViews().AddRazorRuntimeCompilation(); ;
            //builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            //builder.Services.AddSwaggerGen();

            builder.Services.AddSwaggerGen(option =>

            {

                #region SwaggerGen版本控制


                //option.SwaggerDoc("v1", new OpenApiInfo { Title = "EAP Api", Version = "v1" });

                typeof(ApiVersion).GetEnumNames().ToList().ForEach(version =>
                {
                    option.SwaggerDoc(version, new OpenApiInfo { Title = "HandlerAgv.Service Api", Version = version });
                });

                #endregion

                #region 添加SwaggerGen注释

                //// 使用反射获取xml文件，并构造出文件的路径

                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";

                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);

                //// 启用xml注释，该方法第二个参数启用控制器的注释，默认为false。

                option.IncludeXmlComments(xmlPath, true);
                option.DocumentFilter<HiddenApiFilter>();//添加属性过滤器

                ////对action的名称进行排序，如果有多个，就可以看见效果了。

                //option.OrderActionsBy(o => o.RelativePath);

                #endregion

            });
            builder.Services.AddSqlSugarService();

            builder.Services.AddSingleton<RabbitMqService>();
            builder.Services.AddHostedService<RabbitMqWorker>();

            builder.Services.AddHostedService<CommonWorker>();


            var app = builder.Build();
            app.UseStaticFiles();//加载wwwroot里面文件
            app.UseSwagger();
            app.UseSwaggerUI(c => {
                typeof(ApiVersion).GetEnumNames().ToList().ForEach(version =>
                {
                    c.SwaggerEndpoint($"/swagger/{version}/swagger.json", $"{version}");
                });
            });


            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {

            }

            app.MapControllers();
            app.UseRouting();
            app.UseAuthorization();
#pragma warning disable ASP0014
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller}/{action}/{id?}"
                    //defaults: new { controller = "dashboard", action = "Index" }
                    );
            });
#pragma warning restore ASP0014

            app.MapControllers();

            app.Run();
        }
    }
}
