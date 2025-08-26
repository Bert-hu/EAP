using EAP.Client.RabbitMq;
using EAP.Client.Utils;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;

namespace EAP.Client.Services
{
    public class GetRecipeNameResponse
    {
        public bool Result { get; set; } = false;
        public string Message { get; set; }
        public string Id { get; set; }
        public string EquipmentTypeId { get; set; }
        public string RecipeName { get; set; }
    }

    public class ResponseMessage
    {
        public bool Result { get; set; } = false;
        public string Message { get; set; }

    }

    public class RmsFunction
    {

        public static GetRecipeNameResponse GetRecipeName(IConfiguration configuration, string projectName)
        {
            var rmsUrl = configuration.GetSection("Custom")["RmsApiUrl"];

            var compreNameReqUrl = rmsUrl.TrimEnd('/') + "/api/GetRecipeName";
            var compreNameReq = new { EquipmentTypeId = configuration.GetSection("Custom")["EquipmentType"], RecipeNameAlias = projectName };
            var response = HttpClientHelper.HttpPostRequestAsync<GetRecipeNameResponse>(compreNameReqUrl, compreNameReq).Result;
            if (response != null)
            {
                return response;
            }
            else
            {
                return new GetRecipeNameResponse { Result = false, Message = "Api GetRecipeName网络异常" };
            }
        }

        internal static ResponseMessage CompareRecipeBody(RabbitMqService rabbitMq, IConfiguration configuration, string recipeName)
        {
            var rep = new ResponseMessage();
            string equipmentId = configuration.GetSection("Custom")["EquipmentId"];

            var rabbitTrans = new RabbitMqTransaction()
            {
                TransactionName = "CompareRecipeBody",
                EquipmentID = equipmentId,
                Parameters = new Dictionary<string, object>() { { "EquipmentId", equipmentId }, { "RecipeName", recipeName }, }
            };
            var repTrans = rabbitMq.ProduceWaitReply("Rms.Service", rabbitTrans);
            if (repTrans != null)
            {
                var result = false;
                var message = string.Empty;
                if (repTrans.Parameters.TryGetValue("Result", out object _result)) result = (bool)_result;
                if (repTrans.Parameters.TryGetValue("Message", out object _message)) message = _message?.ToString();
                if (!result)
                {
                    rep.Message = "Compare recipe fail: " + message;
                }
                else
                {
                    rep.Message = "Compare recipe success!";
                    rep.Result = true;
                }
            }
            else
            {
                rep.Message = "Compare recipe fail: Timeout";
            }
            return rep;
        }
    }
}
