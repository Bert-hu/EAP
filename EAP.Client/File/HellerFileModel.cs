using System;
using System.Collections.Generic;

namespace EAP.Client.File
{
    public class BoardDataEvent
    {
        public string intType { get; set; }
        public MsgEnvelope msgEnvelope { get; set; }
        public MsgBody msgBody { get; set; }
    }

    public class MsgEnvelope
    {
        public string msgName { get; set; }
        public string version { get; set; }
        public string dateTime { get; set; }
        public string lineName { get; set; }
        public string lineId { get; set; }
        public string machineName { get; set; }
        public string machineId { get; set; }
        public string machineHostName { get; set; }
        public string msgUniqueId { get; set; }
        public string msgReferenceId { get; set; }
        public string msgAckReq { get; set; }
    }

    public class MsgBody
    {
        public string boardID { get; set; }
        public int laneNumber { get; set; }
        public object userName { get; set; }
        public string recipeName { get; set; }
        public double o2PPM { get; set; }
        public double totalPower { get; set; }
        public double entrExhaustPressure { get; set; }
        public double exitExhaustPressure { get; set; }
        public double totalN2Flow { get; set; }
        public int conveyorQty { get; set; }
        public List<Conveyor> conveyor { get; set; }
        public int zoneQty { get; set; }
        public List<Zone> topZones { get; set; }
        public List<Zone> botZones { get; set; }
        public int coolingQty { get; set; }
        public List<Zone> coolingZones { get; set; }
        public int monitorQty { get; set; }
        public object monitorZones { get; set; }
        public object type { get; set; }
    }

    public class Conveyor
    {
        public double SP { get; set; }
        public double PV { get; set; }
    }

    public class Zone
    {
        public double SP { get; set; }
        public double PV { get; set; }
    }
}