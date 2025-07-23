
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

                #region SwaggerGen�汾����


                //option.SwaggerDoc("v1", new OpenApiInfo { Title = "EAP Api", Version = "v1" });

                typeof(ApiVersion).GetEnumNames().ToList().ForEach(version =>
                {
                    option.SwaggerDoc(version, new OpenApiInfo { Title = "HandlerAgv.Service Api", Version = version });
                });

                #endregion

                #region ���SwaggerGenע��

                //// ʹ�÷����ȡxml�ļ�����������ļ���·��

                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";

                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);

                //// ����xmlע�ͣ��÷����ڶ����������ÿ�������ע�ͣ�Ĭ��Ϊfalse��

                option.IncludeXmlComments(xmlPath, true);
                option.DocumentFilter<HiddenApiFilter>();//������Թ�����

                ////��action�����ƽ�����������ж�����Ϳ��Կ���Ч���ˡ�

                //option.OrderActionsBy(o => o.RelativePath);

                #endregion

            });
            builder.Services.AddSqlSugarService();

            builder.Services.AddSingleton<RabbitMqService>();
            builder.Services.AddHostedService<RabbitMqWorker>();

            builder.Services.AddHostedService<CommonWorker>();


            var app = builder.Build();
            app.UseStaticFiles();//����wwwroot�����ļ�
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
