
using HandlerAgv.Service.Services;
using log4net.Config;

namespace HandlerAgv.Service
{
    public class Program
    {
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
            builder.Services.AddSwaggerGen();

            builder.Services.AddHostedService<CommonWorker>();

            var app = builder.Build();

            app.UseStaticFiles();//加载wwwroot里面文件

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
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
