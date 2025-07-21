using Newtonsoft.Json;

namespace LaserMonitor.Service.Services
{
    public class ConfigManager<T> where T : new()
    {
        private readonly string _filePath;

        public ConfigManager()
        {
            // 获取类名并生成文件名
            var className = typeof(T).Name;
            _filePath = $"{className}.json";
        }

        public T LoadConfig()
        {
            if (!File.Exists(_filePath))
            {
                // 如果文件不存在，生成默认配置文件
                var defaultConfig = new T();
                GenerateDefaultConfig(defaultConfig);
                Console.WriteLine($"Configuration file created: {_filePath}");
                return defaultConfig; // 返回默认配置
            }

            var json = File.ReadAllText(_filePath);
            return JsonConvert.DeserializeObject<T>(json);
        }

        public void SaveConfig(T config)
        {
            var json = JsonConvert.SerializeObject(config, Formatting.Indented);
            File.WriteAllText(_filePath, json);
        }

        private void GenerateDefaultConfig(T defaultConfig)
        {
            var json = JsonConvert.SerializeObject(defaultConfig, Formatting.Indented);
            File.WriteAllText(_filePath, json);
        }
    }
}
