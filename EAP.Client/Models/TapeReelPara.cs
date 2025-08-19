using EAP.Client.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EAP.Client.Models
{
    internal class TapeReelPara
    {
        public string PackageName { get; set; }
        public uint ReelQuantity { get; set; }

        public string CompareTo(TapeReelPara other, List<RMS_PARAMETER_SCOPE> scope)
        {
            var differences = new List<string>();

            //比较ReelQuantity
            var reelQuantityScope = scope.FirstOrDefault(it => it.PARAMETER_NAME == "ReelQuantity");
            if (reelQuantityScope != null)
            {
                var difference = reelQuantityScope.CompareValue(this.ReelQuantity, other.ReelQuantity);
                if (!string.IsNullOrEmpty(difference)) differences.Add(difference);
            }
            return differences.Count == 0 ? string.Empty : string.Join(Environment.NewLine, differences);
        }
    }
}
