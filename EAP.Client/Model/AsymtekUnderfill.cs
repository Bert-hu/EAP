using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EAP.Client.Model
{
    public class Traceability
    {
        public string ProjectName { get; set; }
        public string ModelName { get; set; }
        public string EquipmentId { get; set; }
        public string RecipeName { get; set; }
        public string PanelSn { get; set; }
        public DateTime InputTime { get; set; }
        public RecipePara RecipePara { get; set; }

       
    }
    public class RecipePara
    {
        public double PreheatTime { get; set; }
        public double PreheatTemp { get; set; }
        public double NozzleTemp { get; set; }
        public double HeatTemp { get; set; }
        public List<GlueConfig> GlueConfigs { get; set; }
        public string CompareTo(RecipePara other)
        {
            var differences = new List<string>();

            // 比较 PreheatTime
            if (Math.Abs(this.PreheatTime - other.PreheatTime) > 20)
            {
                differences.Add($"PreheatTime: {this.PreheatTime} != {other.PreheatTime}");
            }

            // 比较 PreheatTemp
            if (Math.Abs(this.PreheatTemp - other.PreheatTemp) > 20)
            {
                differences.Add($"PreheatTemp: {this.PreheatTemp} != {other.PreheatTemp}");
            }

            // 比较 NozzleTemp
            if (Math.Abs(this.NozzleTemp - other.NozzleTemp) > 5)
            {
                differences.Add($"NozzleTemp: {this.NozzleTemp} != {other.NozzleTemp}");
            }

            // 比较 HeatTemp
            if (Math.Abs(this.HeatTemp - other.HeatTemp) > 20)
            {
                differences.Add($"HeatTemp: {this.HeatTemp} != {other.HeatTemp}");
            }

            // 比较 GlueConfigs
            if (this.GlueConfigs != null && other.GlueConfigs != null)
            {
                for (int i = 0; i < Math.Min(this.GlueConfigs.Count, other.GlueConfigs.Count); i++)
                {
                    var thisGlue = this.GlueConfigs[i];
                    var otherGlue = other.GlueConfigs[i];

                    // 比较 StartX
                    if (Math.Abs(thisGlue.StartX - otherGlue.StartX) > 200)
                    {
                        differences.Add($"GlueConfig[{i}].StartX: {thisGlue.StartX} != {otherGlue.StartX}");
                    }

                    // 比较 StartY
                    if (Math.Abs(thisGlue.StartY - otherGlue.StartY) > 200)
                    {
                        differences.Add($"GlueConfig[{i}].StartY: {thisGlue.StartY} != {otherGlue.StartY}");
                    }

                    // 比较 EndX
                    if (Math.Abs(thisGlue.EndX - otherGlue.EndX) > 200)
                    {
                        differences.Add($"GlueConfig[{i}].EndX: {thisGlue.EndX} != {otherGlue.EndX}");
                    }

                    // 比较 EndY
                    if (Math.Abs(thisGlue.EndY - otherGlue.EndY) > 200)
                    {
                        differences.Add($"GlueConfig[{i}].EndY: {thisGlue.EndY} != {otherGlue.EndY}");
                    }

                    // 比较 Weight
                    double weightDiff = Math.Abs(thisGlue.Weight - otherGlue.Weight);
                    double weightThreshold = thisGlue.Weight * 0.03;
                    if (weightDiff > weightThreshold)
                    {
                        differences.Add($"GlueConfig[{i}].Weight: {thisGlue.Weight} != {otherGlue.Weight}");
                    }
                }
            }

            return differences.Count == 0 ? string.Empty : string.Join(Environment.NewLine, differences);
        }

        //public string CompareTo1(RecipePara other)
        //{
        //    var differences = new List<string>();

        //    // 比较 RecipePara 的基本属性
        //    differences.AddRange(CompareBasicProperties(other));

        //    // 比较 GlueConfigs
        //    var glueConfigDiffs = CompareGlueConfigs(other.GlueConfigs);
        //    differences.AddRange(glueConfigDiffs);

        //    return differences.Count == 0 ? string.Empty : string.Join(Environment.NewLine, differences);
        //}

        //private IEnumerable<string> CompareBasicProperties(RecipePara other)
        //{
        //    var properties = GetType().GetProperties()
        //       .Where(p => p.PropertyType != typeof(List<GlueConfig>));

        //    foreach (var property in properties)
        //    {
        //        var value1 = property.GetValue(this);
        //        var value2 = property.GetValue(other);
        //        if (!Equals(value1, value2))
        //        {
        //            yield return $"{property.Name}: {value1} != {value2}";
        //        }
        //    }
        //}

        private IEnumerable<string> CompareGlueConfigs(List<GlueConfig> otherConfigs)
        {
            var allKeys = GlueConfigs.Select(c => (c.Pass, c.Table))
               .Union(otherConfigs.Select(c => (c.Pass, c.Table)))
               .ToList();

            foreach (var key in allKeys)
            {
                var config1 = GlueConfigs.FirstOrDefault(c => c.Pass == key.Pass && c.Table == key.Table);
                var config2 = otherConfigs.FirstOrDefault(c => c.Pass == key.Pass && c.Table == key.Table);

                if (config1 == null || config2 == null)
                {
                    yield return $"GlueConfig (Pass: {key.Pass}, Table: {key.Table}) exists in one object but not the other.";
                    continue;
                }

                var glueConfigProperties = typeof(GlueConfig).GetProperties()
                   .Where(p => p.Name != "Pass" && p.Name != "Table");

                foreach (var property in glueConfigProperties)
                {
                    var value1 = property.GetValue(config1);
                    var value2 = property.GetValue(config2);
                    if (!Equals(value1, value2))
                    {
                        yield return $"GlueConfig (Pass: {key.Pass}, Table: {key.Table}) {property.Name}: {value1} != {value2}";
                    }
                }
            }
        }
    }
    public class GlueConfig
    {
        public int Pass { get; set; }
        public int Table { get; set; }
        public double StartX { get; set; }
        public double StartY { get; set; }
        public double EndX { get; set; }
        public double EndY { get; set; }
        public double Weight { get; set; }
    }
}
