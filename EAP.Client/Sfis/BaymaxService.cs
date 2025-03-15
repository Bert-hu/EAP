using log4net;
using System.Net;
using System.Net.Sockets;
using System.Text;

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
                        CancellationTokenSource cts = new CancellationTokenSource(TimeSpan.FromSeconds(30));
                        Task clientTask = Task.Run(() =>
                        {

                            var remoteIp = ((IPEndPoint)machineClient.Client.RemoteEndPoint).Address.ToString();
                            if (!accectIps.Contains(remoteIp))
                            {
                                machineClient.Close();
                                return;
                            }
                            NetworkStream stream = machineClient.GetStream();

                            byte[] buffer = new byte[102400];
                            int bytesRead = stream.Read(buffer, 0, buffer.Length);
                            string requestStr = Encoding.UTF8.GetString(buffer, 0, bytesRead);
                            BaymaxTrans baymaxTrans = GetBaymaxTrans(baymaxIp, bayMaxPort, requestStr);
                            OnBaymaxTransCompleted?.Invoke(this, baymaxTrans);

                            if (handle != null && baymaxTrans.Result)
                                baymaxTrans.BaymaxResponse = handle(this, requestStr, baymaxTrans.BaymaxResponse);
                            byte[] response = Encoding.UTF8.GetBytes(baymaxTrans.BaymaxResponse);
                            stream.Write(response, 0, response.Length);
                            traLog.Info($"Send to Machine: {baymaxTrans.BaymaxResponse}");
                            Thread.Sleep(500);//SEMES时间要长点

                            machineClient.Close();
                        }, cts.Token);

                        clientTask.Wait(cts.Token);
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

        public BaymaxTrans GetBaymaxTrans(string baymaxIp, int bayMaxPort, string request)
        {
            BaymaxTrans baymaxTrans = new BaymaxTrans();
            baymaxTrans.MachineRequest = request;
            try
            {
                TcpClient baymaxClient = new TcpClient(baymaxIp, bayMaxPort);
                NetworkStream baymaxStream = baymaxClient.GetStream();
                byte[] baymaxRequest = Encoding.UTF8.GetBytes(request);
                baymaxStream.Write(baymaxRequest, 0, baymaxRequest.Length);
                baymaxStream.Flush();
                traLog.Info($"Send to SFIS: {request}");
                byte[] baymaxBuffer = new byte[102400];//SEMES emappingbufer要大些
                int baymaxBytesRead = baymaxStream.Read(baymaxBuffer, 0, baymaxBuffer.Length);
                baymaxTrans.BaymaxResponse = Encoding.UTF8.GetString(baymaxBuffer, 0, baymaxBytesRead);
                Thread.Sleep(10);
                baymaxStream.Flush();
                baymaxClient.Close();
                baymaxTrans.Result = true;
                traLog.Info($"Receive from SFIS: {baymaxTrans.BaymaxResponse}");
            }
            catch (Exception)
            {
                baymaxTrans.Result = false;
                baymaxTrans.BaymaxResponse = "Fail,SFIS Baymax connection fail";
            }
            return baymaxTrans;
        }
    }
}
