using Newtonsoft.Json;
using Sunny.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace EAP.Client.NonSecs.Message
{
    public class NonSecsMessageWrapper : NonSecsMessage
    {
        public NonSecsService? nonSecsService { get; set; }
        public DateTime MessageTime { get; set; } = DateTime.Now;

        public int Stream { get; set; } = 0;
        public int Function { get; set; } = 0;

        public string PrimaryMessageString { get; set; } = string.Empty;
        public string SecondaryMessageString { get; set; } = string.Empty;
        public object? PrimaryMessage
        {
            get
            {
                try
                {
                    //TODO 通过PrimaryMessage转换成子类补全代码
                    var baseMsg = JsonConvert.DeserializeObject<NonSecsMessage>(PrimaryMessageString);
                    if (baseMsg == null)
                        throw new ArgumentException("无法反序列化为基类Message");
                    string typeName = $"S{baseMsg.Stream}F{baseMsg.Function}";
                    Type targetType = FindTypeByName(typeName);
                    if (targetType == null)
                        throw new InvalidOperationException($"找不到类型: {typeName}");

                    // 第四步：再次反序列化到目标类型
                    return JsonConvert.DeserializeObject(PrimaryMessageString, targetType);
                }
                catch (Exception)
                {
                    return null;
                }
            }
        }

        public object? SecondaryMessage
        {
            get
            {
                try
                {
                    var baseMsg = JsonConvert.DeserializeObject<NonSecsMessage>(SecondaryMessageString);
                    if (baseMsg == null)
                        throw new ArgumentException("无法反序列化为基类Message");
                    string typeName = $"S{baseMsg.Stream}F{baseMsg.Function}";
                    Type targetType = FindTypeByName(typeName);
                    if (targetType == null)
                        throw new InvalidOperationException($"找不到类型: {typeName}");

                    return JsonConvert.DeserializeObject(SecondaryMessageString, targetType);
                }
                catch (Exception)
                {
                    return null;
                }
            }
        }

        private static Type FindTypeByName(string typeName)
        {
            // 从当前程序集中查找类型
            Assembly currentAssembly = Assembly.GetExecutingAssembly();
            Type type = currentAssembly.GetType(typeName);

            if (type != null && typeof(NonSecsMessage).IsAssignableFrom(type))
                return type;

            // 尝试查找嵌套类型或其他程序集
            foreach (Type t in currentAssembly.GetTypes())
            {
                if (t.Name == typeName && typeof(NonSecsMessage).IsAssignableFrom(t))
                    return t;
            }
            return null;
        }

        public async Task? TryReplyAsync(NonSecsMessage message)
        {
            _ = await nonSecsService?.SendMessage(message);
        }
    }
}
