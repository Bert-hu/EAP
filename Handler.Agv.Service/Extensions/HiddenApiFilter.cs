using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.OpenApi.Models;

using Swashbuckle.AspNetCore.SwaggerGen;
using System.Reflection;
using System.Reflection.Metadata;


/// <summary>
/// 隐藏接口，不生成到swagger文档展示（Swashbuckle.AspNetCore 5.0.0）
/// </summary>
[AttributeUsage(AttributeTargets.Method | AttributeTargets.Class)]
public partial class HiddenApiAttribute : Attribute { }
public class HiddenApiFilter : IDocumentFilter
{
    /// <summary>
    /// 重写Apply方法，移除隐藏接口的生成
    /// </summary>
    /// <param name="swaggerDoc">swagger文档文件</param>
    /// <param name="context"></param>
    public void Apply(OpenApiDocument swaggerDoc, DocumentFilterContext context)
    {
        foreach (ApiDescription apiDescription in context.ApiDescriptions)
        {
            var api = apiDescription.ActionDescriptor as Microsoft.AspNetCore.Mvc.Controllers.ControllerActionDescriptor; //这里强转来获取到控制器的名称

            //判断Controller 或者 Action是否有HiddenApiAttribute
            if (api.ControllerTypeInfo.GetCustomAttributes(typeof(HiddenApiAttribute), true).Any() || api.MethodInfo.GetCustomAttributes(typeof(HiddenApiAttribute), true).Any())
            {
                string key = "/" + apiDescription.RelativePath;
                if (key.Contains("?"))
                {
                    int idx = key.IndexOf("?", StringComparison.Ordinal);
                    key = key.Substring(0, idx);
                }
                swaggerDoc.Paths.Remove(key);
            }
        }
    }

}