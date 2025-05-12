using log4net;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace EAP.Client.Sfis
{
    public class BaymaxService
    {
        public class BaymaxTrans : EventArgs
        {
            public string MachineRequest { get; set; }
            public bool Result { get; set; } = false;
            public string BaymaxResponse { get; set; }
            //public string? ErrorMessage { get; set; }
        }
        internal static ILog traLog = LogManager.GetLogger("Trace");
        public event EventHandler<BaymaxTrans> OnBaymaxTransCompleted;
        public delegate string HandleBaymaxResponse(BaymaxService sender, string machineRequest, string baymaxResponse);
        public void StartBaymaxForwardingService(string machineIp, int listenPort, string baymaxIp, int bayMaxPort, HandleBaymaxResponse? handle = null)
        {
            List<string> accectIps = new List<string> { "127.0.0.1" };
            if (!string.IsNullOrEmpty(machineIp)) accectIps.Add(machineIp);
            //中转Baymax服务
            TcpListener listener = new TcpListener(IPAddress.Any, listenPort);
            listener.Start();


            Task task = new Task(() =>
            {
                while (true)
                {
                    TcpClient machineClient = listener.AcceptTcpClient();
                    try
                    {
                        using (machineClient)
                        {
                            CancellationTokenSource cts = new CancellationTokenSource(TimeSpan.FromSeconds(30));
                            Task clientTask = Task.Run(async () =>
                            {
                                var remoteIp = ((IPEndPoint)machineClient.Client.RemoteEndPoint).Address.ToString();
                                //if (!accectIps.Contains(remoteIp))
                                //{
                                //    machineClient.Close();
                                //    return;
                                //}
                                NetworkStream stream = machineClient.GetStream();

                                byte[] buffer = new byte[102400];
                                int bytesRead = stream.Read(buffer, 0, buffer.Length);
                                string requestStr = Encoding.UTF8.GetString(buffer, 0, bytesRead);
                                requestStr = requestStr + ",GROUP_RECORD=???";//包装机额外查询最后一站信息
                                BaymaxTrans baymaxTrans = GetBaymaxTrans(baymaxIp, bayMaxPort, requestStr).Result;
                                OnBaymaxTransCompleted?.Invoke(this, baymaxTrans);

                                if (handle != null && baymaxTrans.Result)
                                    baymaxTrans.BaymaxResponse = handle(this, requestStr, baymaxTrans.BaymaxResponse);
                                byte[] response = Encoding.UTF8.GetBytes(baymaxTrans.BaymaxResponse);
                                await stream.WriteAsync(response, 0, response.Length);
                                traLog.Info($"Send to Machine: {baymaxTrans.BaymaxResponse}");
                                //Thread.Sleep(500);//SEMES时间要长点

                                //machineClient.Close();
                            }, cts.Token);

                            clientTask.Wait(cts.Token);
                        }                            
                    }
                    catch (OperationCanceledException)
                    {
                        // 处理超时的情况
                        Console.WriteLine("Client handling timed out.");
                    }
                    catch (Exception ex)
                    {
                        // 处理其他异常
                        Console.WriteLine("An error occurred: " + ex.Message);
                    }
                    finally
                    {
                        machineClient.Close();
                    }
                }
            });
            task.Start();
        }

        public async Task<BaymaxTrans> GetBaymaxTrans(string baymaxIp, int bayMaxPort, string request, CancellationToken cancellationToken = default)
        {
            var baymaxTrans = new BaymaxTrans
            {
                MachineRequest = request
            };

            using var baymaxClient = new TcpClient();
            try
            {
                // 设置连接超时
                var connectTask = baymaxClient.ConnectAsync(baymaxIp, bayMaxPort);
                if (await Task.WhenAny(connectTask, Task.Delay(5000, cancellationToken)) != connectTask)
                {
                    throw new TimeoutException("连接SFIS服务器超时");
                }

                using var baymaxStream = baymaxClient.GetStream();
                baymaxStream.ReadTimeout = 5000; // 读取超时设置为5秒
                baymaxStream.WriteTimeout = 5000; // 写入超时设置为5秒

                // 准备请求数据
                var baymaxRequest = Encoding.UTF8.GetBytes(request);

                // 保持原有逻辑：先启动读取操作
                var responseBuffer = new byte[102400];
                var readTask = baymaxStream.ReadAsync(responseBuffer, 0, responseBuffer.Length, cancellationToken);

                // 发送请求
                await baymaxStream.WriteAsync(baymaxRequest, 0, baymaxRequest.Length, cancellationToken);
                await baymaxStream.FlushAsync(cancellationToken);
                traLog.Info($"Send to SFIS: {request}");

                // 等待读取完成
                var bytesRead = await readTask;

                // 直接使用UTF8编码转换
                baymaxTrans.BaymaxResponse = Encoding.UTF8.GetString(responseBuffer, 0, bytesRead);
                baymaxTrans.Result = true;

                traLog.Info($"Receive from SFIS: {baymaxTrans.BaymaxResponse}");
            }
            catch (OperationCanceledException)
            {
                baymaxTrans.Result = false;
                baymaxTrans.BaymaxResponse = "Fail,SFIS操作被取消";
                traLog.Warn("SFIS操作被取消");
            }
            catch (SocketException ex)
            {
                baymaxTrans.Result = false;
                baymaxTrans.BaymaxResponse = $"Fail,SFIS网络错误: {ex.SocketErrorCode}";
                traLog.Error($"SFIS网络错误: {ex.SocketErrorCode}");
            }
            catch (TimeoutException ex)
            {
                baymaxTrans.Result = false;
                baymaxTrans.BaymaxResponse = "Fail,SFIS连接超时";
                traLog.Error("SFIS连接超时");
            }
            catch (Exception ex)
            {
                baymaxTrans.Result = false;
                baymaxTrans.BaymaxResponse = "Fail,SFIS未知错误";
                traLog.Error($"SFIS未知错误: {ex.Message}");
            }

            return baymaxTrans;
        }
    }
}
