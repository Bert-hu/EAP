using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EAP.Client.Models
{
    public class SinictecSpiRecipeParameter
    {
        public int HeightUSL { get; set; }
        public int HeightLSL { get; set; }
        public int AreaUSL { get; set; }
        public int AreaLSL { get; set; }
        public int VolumeUSL { get; set; }
        public int VolumeLSL { get; set; }
        public int ShiftXUSL { get; set; }
        public int ShiftYUSL { get; set; }
        public int ShapeUSL { get; set; }
        public double StencilHeight { get; set; }
        public double BridgeHeight { get; set; }
        public double BridgeWidth { get; set; }
        public double BridgeDistance { get; set; }
        public string GroupName { get; set; }
        public string BridgeType { get; set; }
        public string LightType { get; set; }
        public string VacuumChuck { get; set; }
        public int RU { get; set; }
        public int RL { get; set; }
        public int GU { get; set; }
        public int GL { get; set; }
        public int BU { get; set; }
        public int BL { get; set; }
    }

}
