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

        public string CompareTo(RecipePara other, List<RMS_PARAMETER_SCOPE> scope)
        {
            var differences = new List<string>();

            // 比较 PreheatTime
            var preheatTimeScope = scope.FirstOrDefault(it => it.PARAMETER_NAME == "PreheatTime");
            if (preheatTimeScope != null)
            {
                var difference = preheatTimeScope.CompareValue(this.PreheatTime, other.PreheatTime);
                if (!string.IsNullOrEmpty(difference)) differences.Add(difference);
            }

            // 比较 PreheatTemp
            var preheatTempScope = scope.FirstOrDefault(it => it.PARAMETER_NAME == "PreheatTemp");
            if (preheatTempScope != null)
            {
                var difference = preheatTempScope.CompareValue(this.PreheatTemp, other.PreheatTemp);
                if (!string.IsNullOrEmpty(difference)) differences.Add(difference);
            }

            // 比较 NozzleTemp
            var nozzleTemp = scope.FirstOrDefault(it => it.PARAMETER_NAME == "NozzleTemp");
            if (nozzleTemp != null)
            {
                var difference = nozzleTemp.CompareValue(this.NozzleTemp, other.NozzleTemp);
                if (!string.IsNullOrEmpty(difference)) differences.Add(difference);
            }

            // 比较 HeatTemp
            var heatTempScope = scope.FirstOrDefault(it => it.PARAMETER_NAME == "HeatTemp");
            if (heatTempScope != null)
            {
                var difference = heatTempScope.CompareValue(this.HeatTemp, other.HeatTemp);
                if (!string.IsNullOrEmpty(difference)) differences.Add(difference);
            }

            // 比较 GlueConfigs
            if (this.GlueConfigs != null && other.GlueConfigs != null)
            {
                for (int i = 0; i < Math.Min(this.GlueConfigs.Count, other.GlueConfigs.Count); i++)
                {
                    var thisGlue = this.GlueConfigs[i];
                    var otherGlue = other.GlueConfigs[i];

                    // 比较 StartX
                    var startXScope = scope.FirstOrDefault(it => it.PARAMETER_NAME == "StartX");
                    if (startXScope != null)
                    {
                        var difference = startXScope.CompareValue(thisGlue.StartX, otherGlue.StartX);
                        if (!string.IsNullOrEmpty(difference)) differences.Add($"GlueConfig[{i}] " + difference);
                    }

                    // 比较 StartY
                    var startYScope = scope.FirstOrDefault(it => it.PARAMETER_NAME == "StartY");
                    if (startYScope != null)
                    {
                        var difference = startYScope.CompareValue(thisGlue.StartY, otherGlue.StartY);
                        if (!string.IsNullOrEmpty(difference)) differences.Add($"GlueConfig[{i}] " + difference);
                    }

                    // 比较 EndX
                    var endXScope = scope.FirstOrDefault(it => it.PARAMETER_NAME == "EndX");
                    if (endXScope != null)
                    {
                        var difference = endXScope.CompareValue(thisGlue.EndX, otherGlue.EndX);
                        if (!string.IsNullOrEmpty(difference)) differences.Add($"GlueConfig[{i}] " + difference);
                    }

                    // 比较 EndY
                    var endYScope = scope.FirstOrDefault(it => it.PARAMETER_NAME == "EndY");
                    if (endYScope != null)
                    {
                        var difference = endYScope.CompareValue(thisGlue.EndY, otherGlue.EndY);
                        if (!string.IsNullOrEmpty(difference)) differences.Add($"GlueConfig[{i}] " + difference);
                    }

                    // 比较 Weight
                    var weightScope = scope.FirstOrDefault(it => it.PARAMETER_NAME == "Weight");
                    if (weightScope != null)
                    {
                        var difference = weightScope.CompareValue(thisGlue.Weight, otherGlue.Weight);
                        if (!string.IsNullOrEmpty(difference)) differences.Add($"GlueConfig[{i}] " + difference);
                    }
                }
                // 检查GlueConfigs数量是否不同
                if (this.GlueConfigs.Count != other.GlueConfigs.Count)
                {
                    differences.Add($"GlueConfigs数量不同: {this.GlueConfigs.Count} != {other.GlueConfigs.Count}");
                }
            }
            else if ((this.GlueConfigs == null) != (other.GlueConfigs == null))
            {
                differences.Add("GlueConfigs存在性不一致: 一个为null，另一个不为null");
            }

            return differences.Count == 0 ? string.Empty : string.Join(Environment.NewLine, differences);
        }
        public string CompareToOld(RecipePara other, List<RMS_PARAMETER_SCOPE> scope)
        {
            var differences = new List<string>();

            // 比较 PreheatTime
            var preheatTimeScope = scope.FirstOrDefault(it => it.PARAMETER_NAME == "PreheatTime");
            if (preheatTimeScope != null)
            {
                var diference = preheatTimeScope.CompareValue(this.PreheatTime, other.PreheatTime);
                if (!string.IsNullOrEmpty(diference)) differences.Add(diference);
            }
            //if (Math.Abs(this.PreheatTime - other.PreheatTime) > 20)
            //{
            //    differences.Add($"PreheatTime: {this.PreheatTime} != {other.PreheatTime}");
            //}

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
