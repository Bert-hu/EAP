using EAP.Client.NonSecs;
using EAP.Client.NonSecs.Message;
using EAP.Client.RabbitMq;
using log4net;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Channels;



public class NonSecsService
{
    internal static ILog nonSecsLog = LogManager.GetLogger("NonSecs");
    internal static ILog TraceLog = LogManager.GetLogger("Trace");

    private readonly IConfiguration configuration;
    private readonly NonSecsConfig config;
    private readonly Channel<NonSecsMessageWrapper> PrimaryInQueue = Channel.CreateUnbounded<NonSecsMessageWrapper>();
    private TcpClient? client;
    private TcpListener? server;
    private CancellationTokenSource? cts;

    #region 连接状态
    private ConnectionState _connectionState = ConnectionState.NotConnnected;
    public event EventHandler<ConnectionState> ConnectionChanged;
    public ConnectionState connectionState
    {
        get => _connectionState;
        set
        {
            _connectionState = value;
            ConnectionChanged?.Invoke(this, _connectionState);
        }
    }

    public enum ConnectionState
    {
        NotConnnected,
        Connected
    }
    #endregion

    public NonSecsService(IConfiguration configuration)
    {
        this.configuration = configuration;
        config = configuration.GetSection("NonSecs").Get<NonSecsConfig>();
    }

    public async Task Start(CancellationToken stoppingToken)
    {
        try
        {
            cts = CancellationTokenSource.CreateLinkedTokenSource(stoppingToken);
            IPAddress iPAddress = IPAddress.Parse(config.IpAddress);
            bool isActive = config.IsActive;

            if (isActive)
                await StartClientAsync(iPAddress, config.Port, cts.Token);
            else
                await StartServerAsync(iPAddress, config.Port, cts.Token);
        }
        catch (Exception ex)
        {
            TraceLog.Error("启动服务失败", ex);
            nonSecsLog.Error("启动服务失败", ex);
            connectionState = ConnectionState.NotConnnected;
        }
    }

    public async Task StopAsync()
    {
        try
        {
            cts?.Cancel();

            if (client != null)
            {
                client.Dispose();
            }

            if (server != null)
            {
                server.Stop();
                server = null;
            }

            connectionState = ConnectionState.NotConnnected;
        }
        catch (Exception ex)
        {
            nonSecsLog.Error("停止服务失败", ex);
        }
    }

    private async Task StartClientAsync(IPAddress ipAddress, int port, CancellationToken cancellationToken)
    {
        while (!cancellationToken.IsCancellationRequested)
        {
            try
            {
                connectionState = ConnectionState.NotConnnected;
                TcpClient _client = new TcpClient();
                await _client.ConnectAsync(ipAddress, port, cancellationToken);
                connectionState = ConnectionState.Connected;
                nonSecsLog.Info($"已连接到服务器: {ipAddress}:{port}");
                TraceLog.Info($"已连接到服务器: {ipAddress}:{port}");
                await ReadMessagesAsync(_client, cancellationToken);
                //break; // 如果读取完成正常退出循环
            }
            catch (OperationCanceledException)
            {
                nonSecsLog.Info("客户端连接操作被取消");
                TraceLog.Info("客户端连接操作被取消");
                connectionState = ConnectionState.NotConnnected;
                throw;
            }
            catch (Exception ex)
            {
                nonSecsLog.Error($"客户端连接失败: {ipAddress}:{port}，{ex.Message}");
                TraceLog.Error($"客户端连接失败: {ipAddress}:{port}，{ex.Message}");
                connectionState = ConnectionState.NotConnnected;

                // 清理可能存在的无效连接
                client?.Dispose();
                client = null;

                // 等待5秒后重试，除非取消令牌被触发
                await Task.Delay(5000, cancellationToken);
            }
        }
    }

    private async Task StartServerAsync(IPAddress ipAddress, int port, CancellationToken cancellationToken)
    {
        connectionState = ConnectionState.NotConnnected;

        try
        {
            server = new TcpListener(ipAddress, port);
            server.Start();
            nonSecsLog.Info($"服务器已启动: {ipAddress}:{port}");
            TraceLog.Info($"服务器已启动: {ipAddress}:{port}");
            while (!cancellationToken.IsCancellationRequested)
            {
                TcpClient _client = await server.AcceptTcpClientAsync(cancellationToken);
                connectionState = ConnectionState.Connected;
                var newIpAddress = (_client.Client.RemoteEndPoint as IPEndPoint)?.Address.ToString();
                var oldIpAddress = (this.client?.Client?.LocalEndPoint as IPEndPoint)?.Address.ToString();

                nonSecsLog.Info($"新客户端已连接: {newIpAddress}");
                TraceLog.Info($"新客户端已连接: {newIpAddress}");

                if (string.IsNullOrEmpty(oldIpAddress) || newIpAddress == oldIpAddress)//未建立连接或连接地址相同
                {
                    client?.Close();
                    client?.Dispose();//如果存在旧连接，关闭
                    _ = Task.Run(async () =>
                    {
                        try
                        {
                            await ReadMessagesAsync(_client, cancellationToken);
                        }
                        catch (Exception ex)
                        {
                            nonSecsLog.Error("客户端通信异常", ex);
                            TraceLog.Info("客户端通信异常", ex);
                        }
                        finally
                        {
                            //不能关闭client，否则会出错
                            _client.Dispose();
                        }
                    }, cancellationToken);
                }
                else 
                {
                    nonSecsLog.Info($"新连接IP与老连接不同，忽略");
                    TraceLog.Info($"新连接IP与老连接不同，忽略");
                    _client.Close();
                    _client.Dispose();
                }
            }
        }
        catch (Exception ex)
        {
            nonSecsLog.Error($"服务器启动失败: {ipAddress}:{port}", ex);
            TraceLog.Error($"服务器启动失败: {ipAddress}:{port}", ex);
            connectionState = ConnectionState.NotConnnected;
            throw;
        }
    }


    public async IAsyncEnumerable<NonSecsMessageWrapper> GetPrimaryMessageAsync([EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        // 使用 ReadAllAsync 替代单次读取
        await foreach (var message in PrimaryInQueue.Reader.ReadAllAsync(cancellationToken))
        {
            yield return message;
        }
    }

    private async Task ReadMessagesAsync(TcpClient client, CancellationToken cancellationToken)
    {
        NetworkStream stream = client.GetStream();
        var buffer = new byte[config.SocketReceiveBufferSize];
        this.client = client;
        while (!cancellationToken.IsCancellationRequested)
        {
            try
            {
                int bytesRead = await stream.ReadAsync(buffer, cancellationToken);
                if (bytesRead == 0)
                {
                    TraceLog.Info("客户端已断开");
                    nonSecsLog.Info("客户端已断开");
                    connectionState = ConnectionState.NotConnnected;
                    break;
                }

                string rawmessage = Encoding.UTF8.GetString(buffer, 0, bytesRead);
                var messages = SplitJson(rawmessage);
                foreach (var strMessage in messages)
                {
                    nonSecsLog.Info($"In: {strMessage}");
                    NonSecsMessage message;
                    try
                    {
                        message = JsonConvert.DeserializeObject<NonSecsMessage>(strMessage);
                        if (message.Function % 2 == 1)//Primary In
                        {
                            var primaryMessage = new NonSecsMessageWrapper
                            {
                                nonSecsService = this,
                                Stream = message.Stream,
                                Function = message.Function,
                                MessageTime = DateTime.Now,
                                PrimaryMessageString = strMessage
                            };

                            await PrimaryInQueue.Writer.WriteAsync(primaryMessage);
                        }
                        else//Secondary In
                        {
                            var primaryMessage = PrimaryOutMessageQueue.OrderBy(it => it.Key.MessageTime).FirstOrDefault(it => it.Key.Stream == message.Stream && it.Key.Function == message.Function - 1);
                            if (!primaryMessage.Equals(default(KeyValuePair<NonSecsMessageWrapper, TaskCompletionSource<NonSecsMessageWrapper>>)))
                            {
                                if (PrimaryOutMessageQueue.TryGetValue(primaryMessage.Key, out TaskCompletionSource<NonSecsMessageWrapper> tcs))
                                {
                                    var result = new NonSecsMessageWrapper
                                    {
                                        nonSecsService = this,
                                        Stream = message.Stream,
                                        Function = message.Function,
                                        MessageTime = primaryMessage.Key.MessageTime,
                                        PrimaryMessageString = primaryMessage.Key.PrimaryMessageString,
                                        SecondaryMessageString = strMessage
                                    };
                                    tcs.SetResult(result);
                                }
                            }
                            else
                            {
                                // 不存在匹配项
                                nonSecsLog.Warn("无法匹配的SecondaryMessage");
                            }
                        }
                    }
                    catch (Exception)
                    {
                        nonSecsLog.Error("处理消息异常");
                    }


                }
            }
            catch (OperationCanceledException)
            {
                break;
            }
            catch (Exception ex)
            {
                nonSecsLog.Error("读取消息失败", ex);
                break;
                //throw;
            }
        }
    }

    #region 处理粘包消息

    private string[] SplitJson(string combinedJson)
    {
        if (string.IsNullOrEmpty(combinedJson))
            return new string[0];

        // 处理最常见的情况：简单的}{分隔
        if (combinedJson.Contains("}{"))
        {
            // 使用正则表达式来分割，确保我们不会错误地分割嵌套的对象
            List<string> jsonObjects = new List<string>();
            int depth = 0;
            int startIndex = 0;

            for (int i = 0; i < combinedJson.Length; i++)
            {
                if (combinedJson[i] == '{')
                    depth++;
                else if (combinedJson[i] == '}')
                    depth--;

                // 当找到一个}{并且深度为0时，说明我们找到了一个完整的JSON对象
                if (i < combinedJson.Length - 1 && combinedJson[i] == '}' && combinedJson[i + 1] == '{' && depth == 0)
                {
                    // 提取从startIndex到i的子字符串（包括i位置的}）
                    jsonObjects.Add(combinedJson.Substring(startIndex, i - startIndex + 1));
                    startIndex = i + 1; // 下一个JSON对象的起始位置
                }
            }

            // 添加最后一个JSON对象
            if (startIndex < combinedJson.Length)
            {
                jsonObjects.Add(combinedJson.Substring(startIndex));
            }

            return jsonObjects.ToArray();
        }
        else
        {
            // 如果没有找到}{，则认为整个字符串是一个JSON对象
            return new string[] { combinedJson };
        }
    }
    #endregion

    ConcurrentDictionary<NonSecsMessageWrapper, TaskCompletionSource<NonSecsMessageWrapper>> PrimaryOutMessageQueue = new ConcurrentDictionary<NonSecsMessageWrapper, TaskCompletionSource<NonSecsMessageWrapper>>();

    public async Task<NonSecsMessageWrapper?> SendMessage(string message, int timeoutSecond = 5, CancellationToken cancellationToken = default)
    {
        if (connectionState != ConnectionState.Connected)
        {
            var errorMsg = "无法发送消息：连接未建立";
            nonSecsLog.Error(errorMsg);
            throw new InvalidOperationException(errorMsg);
        }

        try
        {
            var messageObj = JsonConvert.DeserializeObject<NonSecsMessage>(message);

            // 序列化消息
            //var messageData = JsonConvert.SerializeObject(message);
            var messageBytes = Encoding.UTF8.GetBytes(message);

            // 根据isActive状态选择不同的发送方式
            if (config.IsActive) // 判断isActive状态
            {
                // 主动模式：使用客户端连接发送消息
                if (client == null || !client.Connected)
                {
                    throw new InvalidOperationException("客户端未连接");
                }

                var stream = client.GetStream();
                // 发送消息内容
                await stream.WriteAsync(messageBytes, 0, messageBytes.Length, cancellationToken);
                await stream.FlushAsync(cancellationToken);

                nonSecsLog.Info($"Out: {message}");
            }
            else
            {
                // 被动模式：使用服务器连接发送消息
                if (server == null)
                {
                    throw new InvalidOperationException("服务器未启动");
                }

                // 注意：被动模式下需要维护客户端连接集合，这里简化处理
                if (client == null || !client.Connected)
                {
                    throw new InvalidOperationException("无活跃客户端连接");
                }

                var stream = client.GetStream();

                // 发送消息内容
                await stream.WriteAsync(messageBytes, 0, messageBytes.Length, cancellationToken);
                await stream.FlushAsync(cancellationToken);

                nonSecsLog.Info($"Out: {message}");
            }

            if (messageObj.Function % 2 == 1)//Primary out
            {
                // 将消息添加到队列，使用当前时间作为键
                var timestamp = DateTime.Now;
                var tcs = new TaskCompletionSource<NonSecsMessageWrapper>();
                var primaryMessage = new NonSecsMessageWrapper
                {
                    nonSecsService = this,
                    Stream = messageObj.Stream,
                    Function = messageObj.Function,
                    MessageTime = timestamp,
                    PrimaryMessageString = message
                };

                if (!PrimaryOutMessageQueue.TryAdd(primaryMessage, tcs))
                {
                    var errorMsg = "发送消息失败：无法将消息添加到输出队列";
                    nonSecsLog.Error(errorMsg);
                    throw new InvalidOperationException(errorMsg);
                }
                // 等待响应或超时
                var completedTask = await Task.WhenAny(tcs.Task, Task.Delay(timeoutSecond * 1000, cancellationToken));

                if (completedTask == tcs.Task)
                {
                    PrimaryOutMessageQueue.TryRemove(primaryMessage, out _);
                    return await tcs.Task;
                }
                else
                {
                    PrimaryOutMessageQueue.TryRemove(primaryMessage, out _);
                    var errMessage = $"S{messageObj.Stream}F{messageObj.Function} timeout, {timeoutSecond} s";
                    nonSecsLog.Error(errMessage);
                    throw new TimeoutException(errMessage);
                }
            }
            else//Secondary out
            {
                return null;
            }
        }
        catch (OperationCanceledException)
        {
            // 从队列中移除已取消的消息
            nonSecsLog.Warn("发送消息操作被取消");
            throw;
        }
        catch (Exception ex)
        {
            // 发生错误时从队列中移除消息
            nonSecsLog.Error("发送消息时发生错误", ex);
            throw;
        }

    }

    public async Task<NonSecsMessageWrapper?> SendMessage(NonSecsMessage message, int timeoutSecond = 5, CancellationToken cancellationToken = default)
    {
        return await SendMessage(JsonConvert.SerializeObject(message), timeoutSecond, cancellationToken);
    }
}