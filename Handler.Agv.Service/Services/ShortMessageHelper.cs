using System;
using System.Collections.Generic;
using System.Linq;

using SqlSugar;

namespace HandlerAgv.Service.Services
{
    [SugarTable("USI_PUBLIC.MSG_SEND")]

    public class MSG_SEND
    {
        public int? MSG_ID { get; set; }
        public string PROGRAM { get; set; }
        public string MESSAGE { get; set; }
        public string MOBILES { get; set; }
        public string SEND_FLAG { get; set; }
        public DateTime? UPDATE_DATE { get; set; }
    }
    public class ShortMessageHelper
    {
        public static void SendMsgToPhoneList(string proram, string message, List<string> phonelist)
        {
            string connstr = "User ID=USI_PUBLIC;Password=public123;pooling=true;Data Source=(DESCRIPTION = (ADDRESS_LIST= (ADDRESS = (PROTOCOL = TCP)(HOST = 192.168.40.128)(PORT = 1521))) (CONNECT_DATA = (SID = SPC)))";
            var db = SqlsugarService.GetSqlSugarClient(connstr);
            if (message.Length > 70) message = message.Substring(0, 70);
            foreach (var phone in phonelist)
            {
                var exist = db.Queryable<MSG_SEND>().First(it => it.PROGRAM == proram && it.MESSAGE == message && it.MOBILES == phone && it.UPDATE_DATE > DateTime.Now.AddMinutes(-5));

                if (exist == null)
                {
                    db.Insertable<MSG_SEND>(new MSG_SEND
                    {
                        PROGRAM = proram,
                        MESSAGE = message,
                        MOBILES = phone,
                        SEND_FLAG = "N",
                        UPDATE_DATE = DateTime.Now
                    }).ExecuteCommand();
                }
            }
        }
    }
}
