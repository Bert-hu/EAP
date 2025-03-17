using EAP.Client.RabbitMq;
using log4net;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Secs4Net;
using System.Reflection;


namespace EAP.Client.Secs
{
    internal partial class SecsWorker : BackgroundService
    {
        private readonly ILog dbgLog = LogManager.GetLogger("Debug");
        private readonly ILog eqpLog = LogManager.GetLogger("Secs");
        private readonly ILog traLog = LogManager.GetLogger("Trace");

        private readonly ISecsGem _secsGem;
        private readonly ISecsConnection _hsmsConnection;
        private CommonLibrary _commonLibrary;
        private RabbitMq.RabbitMqService _rabbitMqService;
        private readonly IServiceProvider _serviceProvider;


        private readonly System.Threading.Timer _heartBeatTimer;
        public SecsWorker(IConfiguration configuration, ISecsConnection hsmsConnection, ISecsGem secsGem, CommonLibrary commonLibrary, RabbitMq.RabbitMqService rabbitMqService, IServiceProvider serviceProvider)
        {
            _secsGem = secsGem;
            _hsmsConnection = hsmsConnection;
            _commonLibrary = commonLibrary;
            _rabbitMqService = rabbitMqService;
            _serviceProvider = serviceProvider;

            _heartBeatTimer = new System.Threading.Timer(delegate
            {
                _secsGem.SendAsync(new SecsMessage(1, 1));
            }, null, Timeout.Infinite, Timeout.Infinite);



            _hsmsConnection.ConnectionChanged += delegate
            {
                _heartBeatTimer.Change(Timeout.Infinite, Timeout.Infinite);
                eqpLog.Info($"{DateTime.Now} Connection state = {_hsmsConnection.State}");
                traLog.Info($"{DateTime.Now} Connection state = {_hsmsConnection.State}");

                switch (_hsmsConnection.State)
                {
                    case ConnectionState.Retry:

                        break;
                    case ConnectionState.Connecting:

                        break;
                    case ConnectionState.Connected:

                        break;
                    case ConnectionState.Selected:
                        var interval = _commonLibrary.SecsConfigs.HeartBeatInterval <= 0 ? Timeout.Infinite : commonLibrary.SecsConfigs.HeartBeatInterval * 1000;
                        _heartBeatTimer.Change(100, interval);
                        Task.Run(() =>
                        {
                            SecsInitialization.Initialization(secsGem, commonLibrary);
                            using (var scope = _serviceProvider.CreateAsyncScope())
                            {
                                var handler = (ITransactionHandler)scope.ServiceProvider.GetRequiredService(typeof(GetEquipmentStatus));
                                handler.HandleTransaction(null);
                            }
                        });
                        break;
                    default:
                        break;
                }
            };

        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            try
            {
                _hsmsConnection.Start(stoppingToken);
                _hsmsConnection.LinkTestEnabled = true;
                //TODO: 改为进入类处理
                await foreach (var primaryMessageWrapper in _secsGem.GetPrimaryMessageAsync(stoppingToken))
                {
                    try
                    {
                        await Task.Run(async () => await HandlePrimaryMessage(primaryMessageWrapper));
                    }
                    catch (Exception ex)
                    {
                        dbgLog.Error("Exception occurred when processing primary message", ex);
                    }
                }
            }
            catch (OperationCanceledException)
            {
            }
        }

        private async Task HandlePrimaryMessage(PrimaryMessageWrapper wrapper)
        {
            var streamfunction = $"S{wrapper.PrimaryMessage.S}F{wrapper.PrimaryMessage.F}";
            var interfaceType = typeof(IPrimaryMessageHandler);
            var type = Assembly.GetExecutingAssembly().GetTypes().Where(t => interfaceType.IsAssignableFrom(t) && t.Name == streamfunction).FirstOrDefault();
            if (type != null)
            {
                //IPrimaryMessageHandler obj = (IPrimaryMessageHandler)Activator.CreateInstance(type);
                //await obj.HandlePrimaryMessage(wrapper, _rabbitMqService, _secsGem, commonLibrary);
                using (var scope = _serviceProvider.CreateAsyncScope())
                {
                    var handler = (IPrimaryMessageHandler)scope.ServiceProvider.GetRequiredService(type);
                    await handler.HandlePrimaryMessage(wrapper);
                }
            }
            else
            {
                if (wrapper.PrimaryMessage.ReplyExpected)
                {
                    var secondaryMessage = _commonLibrary.GetSecsMessageByName($"S{wrapper.PrimaryMessage.S}F{wrapper.PrimaryMessage.F + 1}");
                    if (secondaryMessage == null)
                    {

                        secondaryMessage = new SecsMessage(wrapper.PrimaryMessage.S, (byte)(wrapper.PrimaryMessage.F + 1))
                        {
                            SecsItem = wrapper.PrimaryMessage.SecsItem,
                        };
                    }
                    await wrapper.TryReplyAsync(secondaryMessage);
                }
            }
        }



    }
}
