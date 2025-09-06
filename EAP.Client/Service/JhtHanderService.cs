using EAP.Client.RabbitMq;
using EAP.Client.Secs;
using log4net;
using Microsoft.Extensions.Configuration;
using Microsoft.VisualBasic.Logging;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Window;

namespace EAP.Client.Service
{
    internal class JhtHanderService
    {
        internal static ILog traLog = LogManager.GetLogger("Trace");
        internal static ILog dbgLog = LogManager.GetLogger("Debug");

        private readonly RabbitMqService rabbitMqservice;   
        private readonly IConfiguration configuration;
                public JhtHanderService(RabbitMqService rabbitMqservice, IConfiguration configuration)
        {
            this.rabbitMqservice = rabbitMqservice;
            this.configuration = configuration;
        }

        public void UpdateMachineIP()
        {
            var ips = Dns.GetHostAddresses(Dns.GetHostName());
            // 过滤测试网段的Ipv4地址
            var ipv4ips = ips.Where(ip => ip.AddressFamily == AddressFamily.InterNetwork).ToList();
            var ipv4 = ipv4ips.FirstOrDefault(ip => ip.ToString().StartsWith("10.6"));
            if (ipv4 == null)
            {
                // 如果没有找到10.6开头的IP，取第一个IPv4地址
                ipv4 = ipv4ips.FirstOrDefault();
            }

            if (ipv4 != null)
            {
                var trans = new RabbitMqTransaction
                {
                    EquipmentID = configuration.GetSection("Custom")["EquipmentId"],
                    TransactionName = "UpdateMachineIP",
                    NeedReply = true,
                    ExpireSecond = 5,
                    ReplyChannel = configuration.GetSection("RabbitMQ")["QueueName"],
                    Parameters = new Dictionary<string, object>
                    {
                        { "MachineIP", ipv4.ToString() }
                    }
                };
                rabbitMqservice.Produce("HandlerAgv.Service", trans);
            }
        }
        public bool UpdateMachineInputTrayCount(int count)
        {
            try
            {
                var trans = new RabbitMqTransaction
                {
                    EquipmentID = configuration.GetSection("Custom")["EquipmentId"],
                    TransactionName = "UpdateMachineInputTrayCount",
                    NeedReply = true,
                    ExpireSecond = 5,
                    ReplyChannel = configuration.GetSection("RabbitMQ")["QueueName"],
                    Parameters = new Dictionary<string, object>
                    {
                        { "InputTrayCount", count }
                    }
                };
                var reply = rabbitMqservice.ProduceWaitReply("HandlerAgv.Service", trans);
                if (reply != null)
                {
                    var result = reply.Parameters.ContainsKey("Result") ? Convert.ToBoolean(reply.Parameters["Result"]) : false;
                    if (result)
                    {
                        return true;
                    }
                    else
                    {
                        traLog.Warn($"更新入料口盘数失败: {(reply.Parameters.ContainsKey("Message") ? reply.Parameters["Message"].ToString() : "未知错误")}");
                        return false;
                    }
                }
                else
                {
                    traLog.Warn($"更新入料口盘数超时。");
                    return false;
                }
            }
            catch (Exception ex)
            {
                traLog.Error($"更新入料口盘数失败: {ex.ToString()}");
                return false;
            }
        }
        public bool UpdateMachineOutputTrayCount(int count)
        {
            try
            {
                var trans = new RabbitMqTransaction
                {
                    EquipmentID = configuration.GetSection("Custom")["EquipmentId"],
                    TransactionName = "UpdateMachineOutputTrayCount",
                    NeedReply = true,
                    ExpireSecond = 5,
                    ReplyChannel = configuration.GetSection("RabbitMQ")["QueueName"],
                    Parameters = new Dictionary<string, object>
                    {
                        { "OutputTrayCount", count }
                    }
                };
                var reply = rabbitMqservice.ProduceWaitReply("HandlerAgv.Service", trans);
                if (reply != null)
                {
                    var result = reply.Parameters.ContainsKey("Result") ? Convert.ToBoolean(reply.Parameters["Result"]) : false;
                    if (result)
                    {
                        return true;
                    }
                    else
                    {
                        traLog.Warn($"更新出料口盘数失败: {(reply.Parameters.ContainsKey("Message") ? reply.Parameters["Message"].ToString() : "未知错误")}");
                        return false;
                    }
                }
                else
                {
                    traLog.Warn($"更新出料口盘数超时。");
                    return false;
                }
            }
            catch (Exception ex)
            {
                traLog.Error($"更新出料口盘数失败: {ex.ToString()}");
                return false;
            }

        }
        public bool UpdateAgvEnabled(bool enabled)
        {
            try
            {
                var trans = new RabbitMqTransaction
                {
                    EquipmentID = configuration.GetSection("Custom")["EquipmentId"],
                    TransactionName = "UpdateAgvEnabled",
                    NeedReply = true,
                    ExpireSecond = 3,
                    ReplyChannel = configuration.GetSection("RabbitMQ")["QueueName"],
                    Parameters = new Dictionary<string, object>
                    {
                        { "AgvEnabled", enabled }
                    }
                };
                var reply = rabbitMqservice.ProduceWaitReply("HandlerAgv.Service", trans);
                if (reply != null)
                {
                    var result = reply.Parameters.ContainsKey("Result") ? Convert.ToBoolean(reply.Parameters["Result"]) : false;
                    if (result)
                    {
                        traLog.Info($"更新AGV模式成功: {(enabled ? "开启" : "关闭")}");
                        return true;
                    }
                    else
                    {
                        traLog.Warn($"更新AGV模式失败: {(reply.Parameters.ContainsKey("Message") ? reply.Parameters["Message"].ToString() : "未知错误")}");
                        return false;
                    }
                }
                else
                {
                    traLog.Warn($"更新AGV模式超时。");
                    return false;
                }
            }
            catch (Exception)
            {

                throw;
            }
        }
        public bool UpdateLot(string lot)
        {
            try
            {
                var trans = new RabbitMqTransaction
                {
                    EquipmentID = configuration.GetSection("Custom")["EquipmentId"],
                    TransactionName = "UpdateCurrentLot",
                    NeedReply = true,
                    ExpireSecond = 5,
                    ReplyChannel = configuration.GetSection("RabbitMQ")["QueueName"],
                    Parameters = new Dictionary<string, object>
                    {
                        { "CurrentLot", lot }
                    }
                };
                var reply = rabbitMqservice.ProduceWaitReply("HandlerAgv.Service", trans);
                if (reply != null)
                {
                    var result = reply.Parameters.ContainsKey("Result") ? Convert.ToBoolean(reply.Parameters["Result"]) : false;
                    if (result)
                    {
                        return true;
                    }
                    else
                    {
                        traLog.Warn($"更新CurrentLot失败: {(reply.Parameters.ContainsKey("Message") ? reply.Parameters["Message"].ToString() : "未知错误")}");
                        return false;
                    }
                }
                else
                {
                    traLog.Warn($"更新CurrentLot超时。");
                    return false;
                }
            }
            catch (Exception ex)
            {
                traLog.Error($"更新CurrentLot失败: {ex.ToString()}");
                return false;
            }
        }

        /// <summary>
        /// 更新物料名称
        /// </summary>
        /// <param name="materialName">物料名称</param>
        /// <returns>更新是否成功</returns>
        public bool UpdateMaterialName(string materialName)
        {
            try
            {
                // 构建RabbitMQ事务消息
                var trans = new RabbitMqTransaction
                {
                    EquipmentID = configuration.GetSection("Custom")["EquipmentId"], // 设备ID（复用配置）
                    TransactionName = "UpdateMaterialName", // 事务名称（区分操作类型）
                    NeedReply = true, // 需要等待回复
                    ExpireSecond = 5, // 超时时间（保持5秒）
                    ReplyChannel = configuration.GetSection("RabbitMQ")["QueueName"], // 回复队列（复用配置）
                    Parameters = new Dictionary<string, object>
            {
                { "MaterialName", materialName } // 传递物料名称参数
            }
                };

                // 发送消息并等待回复
                var reply = rabbitMqservice.ProduceWaitReply("HandlerAgv.Service", trans);

                if (reply != null)
                {
                    // 解析回复结果
                    var result = reply.Parameters.ContainsKey("Result") ? Convert.ToBoolean(reply.Parameters["Result"]) : false;
                    if (result)
                    {
                        return true; // 更新成功
                    }
                    else
                    {
                        // 记录更新失败日志（包含错误信息）
                        traLog.Warn($"更新MaterialName失败: {(reply.Parameters.ContainsKey("Message") ? reply.Parameters["Message"].ToString() : "未知错误")}");
                        return false;
                    }
                }
                else
                {
                    // 无回复（超时）
                    traLog.Warn($"更新MaterialName超时。");
                    return false;
                }
            }
            catch (Exception ex)
            {
                // 捕获异常并记录
                traLog.Error($"更新MaterialName失败: {ex.ToString()}");
                return false;
            }
        }

        /// <summary>
        /// 更新组名称
        /// </summary>
        /// <param name="groupName">组名称</param>
        /// <returns>更新是否成功</returns>
        public bool UpdateGroupName(string groupName)
        {
            try
            {
                // 构建RabbitMQ事务消息
                var trans = new RabbitMqTransaction
                {
                    EquipmentID = configuration.GetSection("Custom")["EquipmentId"], // 设备ID（复用配置）
                    TransactionName = "UpdateGroupName", // 事务名称（区分操作类型）
                    NeedReply = true, // 需要等待回复
                    ExpireSecond = 5, // 超时时间（保持5秒）
                    ReplyChannel = configuration.GetSection("RabbitMQ")["QueueName"], // 回复队列（复用配置）
                    Parameters = new Dictionary<string, object>
            {
                { "GroupName", groupName } // 传递组名称参数
            }
                };

                // 发送消息并等待回复
                var reply = rabbitMqservice.ProduceWaitReply("HandlerAgv.Service", trans);

                if (reply != null)
                {
                    // 解析回复结果
                    var result = reply.Parameters.ContainsKey("Result") ? Convert.ToBoolean(reply.Parameters["Result"]) : false;
                    if (result)
                    {
                        return true; // 更新成功
                    }
                    else
                    {
                        // 记录更新失败日志（包含错误信息）
                        traLog.Warn($"更新GroupName失败: {(reply.Parameters.ContainsKey("Message") ? reply.Parameters["Message"].ToString() : "未知错误")}");
                        return false;
                    }
                }
                else
                {
                    // 无回复（超时）
                    traLog.Warn($"更新GroupName超时。");
                    return false;
                }
            }
            catch (Exception ex)
            {
                // 捕获异常并记录
                traLog.Error($"更新GroupName失败: {ex.ToString()}");
                return false;
            }
        }

        public (bool, string) SemdAgvTask(string taskType)
        {
            try
            {
                var trans = new RabbitMqTransaction
                {
                    EquipmentID = configuration.GetSection("Custom")["EquipmentId"],
                    TransactionName = $"Send{taskType}Task",
                    NeedReply = true,
                    ExpireSecond = 6,
                    ReplyChannel = configuration.GetSection("RabbitMQ")["QueueName"]
                };
                var reply = rabbitMqservice.ProduceWaitReply("HandlerAgv.Service", trans);
                if (reply != null)
                {
                    if (reply.Parameters.ContainsKey("Result") && Convert.ToBoolean(reply.Parameters["Result"]))
                    {
                        traLog.Info($"{taskType}任务发送成功");
                        return (true, $"{taskType}任务发送成功");
                    }
                    else
                    {
                        string message = reply.Parameters.ContainsKey("Message") ? reply.Parameters["Message"].ToString() : "未知错误";
                        traLog.Warn($"{taskType}任务发送失败: {message}");
                        return (false, $"{taskType}任务发送失败: {message}");
                    }
                }
                else
                {
                    traLog.Warn($"{taskType}任务发送超时。");
                    return (false, $"{taskType}任务发送超时");
                }
            }
            catch (Exception ex)
            {
                traLog.Error($"发送AGV任务失败: {ex.ToString()}");
                return (false, "发送AGV任务失败");
            }
        }

        public void LoaderEmpty()
        {
            try
            {
                // 构建RabbitMQ事务消息
                var trans = new RabbitMqTransaction
                {
                    EquipmentID = configuration.GetSection("Custom")["EquipmentId"], // 设备ID（复用配置）
                    TransactionName = "LoaderEmpty", // 事务名称（区分操作类型）
                    NeedReply = false, // 需要等待回复
                    ExpireSecond = 5, // 超时时间（保持5秒）
                    ReplyChannel = configuration.GetSection("RabbitMQ")["QueueName"], // 回复队列（复用配置）
                };

                // 发送消息并等待回复
                rabbitMqservice.Produce("HandlerAgv.Service", trans);


            }
            catch (Exception ex)
            {
                // 捕获异常并记录
                traLog.Error($"发送LoaderEmpty失败: {ex.ToString()}");
            }

        }

    }
}
