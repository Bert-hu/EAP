using EAP.Client.Secs;
using log4net;
using log4net.Util;
using Microsoft.Extensions.Logging;
using Secs4Net;
using static Secs4Net.Item;

namespace EAP.Client.Secs
{ 
    internal static class SecsInitialization
    {
        private static ISecsGem _secsGem;
        private static CommonLibrary _commonLibrary;
        private static bool IsInitializing = false;
        private static readonly ILog traceLog = LogManager.GetLogger("Trace");
        internal static async Task Initialization(ISecsGem secsGem, CommonLibrary commonLibrary)
        {
            _secsGem = secsGem;
            _commonLibrary = commonLibrary;
            if (!IsInitializing)//判断重复进入
            {
                IsInitializing = true;
                try
                {
                    CancellationTokenSource cancellationTokenSource = new CancellationTokenSource(TimeSpan.FromSeconds(30));
                    var token = cancellationTokenSource.Token;
                    await Task.Delay(1000);
                   // var s1f13 = _commonLibrary.GetSecsMessageByName("S1F13");
                   var s1f13 = new SecsMessage(1, 13, true)
                   {
                       SecsItem = L()
                    };  
                    var rep = await _secsGem.SendAsync(s1f13, token);
                    if (rep.F == 14)
                    {
                        await Task.Delay(1000);

                        if (_commonLibrary.SecsConfigs.EnableDynamicEvent)
                        {
                            if (await DisableEvent(token))
                                if (await UnlinkEventReport(token))
                                    if (await UndefineReport(token))
                                        if (await DefineReport(token))
                                            if (await LinkEventReport(token))
                                                await EnableEvent(token);
                        }
                        if (_commonLibrary.SecsConfigs.EnableAllAlarm)
                        {
                            if (await DisableAlarm(token))
                                await EnableAlarm(token);
                        }
                    }
                    traceLog.Warn("Initialization completed!");
                }
                catch (OperationCanceledException)
                {
                    traceLog.Warn("Initialization task canceled due to timeout");
                }
                catch (Exception ex)
                {
                    traceLog.Error($"Error when initialization: {ex.Message}", ex);
                }
                finally
                {
                    IsInitializing = false;
                }
            }
            else
            {
                traceLog.Warn("Repeated initialization, skipping...");
            }
        }

        private static async Task<bool> DisableAlarm(CancellationToken cancellation)
        {
            traceLog.Info($"Sending S5F3 disable all alarms...");
            var req = new SecsMessage(5, 3)
            {
                SecsItem = L(
                    B(0x00),
                    U4()
                )
            };
            var rep = await _secsGem.SendAsync(req, cancellation);
            var ack = rep.SecsItem.FirstValue<byte>();

            switch (ack)
            {
                case 0:
                    traceLog.Info("Initial -> S5F3 disable all alarms - OK");
                    return true;
                default:
                    traceLog.Warn(String.Format("{0} - Disable Event Denied. Equipment rejects S5F4 message", ack.ToString()));
                    return false;
            }
        }

        private static async Task<bool> EnableAlarm(CancellationToken cancellation)
        {
            traceLog.Info($"Sending S5F3 enable all alarms...");
            var req = new SecsMessage(5, 3)
            {
                SecsItem = L(
                    B(0x80),
                    U4()
                )
            };
            var rep = await _secsGem.SendAsync(req, cancellation);
            var ack = rep.SecsItem.FirstValue<byte>();

            switch (ack)
            {
                case 0:
                    traceLog.Info("Initial -> S5F3 enable all alarms - OK");
                    return true;
                default:
                    traceLog.Warn(String.Format("{0} - Enable Event Denied. Equipment rejects S5F4 message", ack.ToString()));
                    return false;
            }
        }

        private static async Task<bool> DisableEvent(CancellationToken cancellation)
        {
            traceLog.Info($"Sending S2F37 disable all events...");
            var req = new SecsMessage(2, 37)
            {
                SecsItem = L(
                    Boolean(false),
                    L()
                )
            };
            var rep = await _secsGem.SendAsync(req, cancellation);
            var ack = rep.SecsItem.FirstValue<byte>();

            switch (ack)
            {
                case 0:
                    traceLog.Info("Initial -> S2F37 disable all events - OK");
                    return true;
                case 1:
                    traceLog.Warn(String.Format("{0} - Enable/Disable Event Denied. At least one CEID does not exist", ack.ToString()));
                    return false;
                default:
                    traceLog.Warn(String.Format("{0} - Enable/Disable Event Denied. Equipment rejects S2F37 message", ack.ToString()));
                    return false;
            }
        }

        private static async Task<bool> UnlinkEventReport(CancellationToken cancellation)
        {
            traceLog.Info($"Sending S2F35 unlink event report...");
            var req = new SecsMessage(2, 35)
            {
                SecsItem = L(
                    U4(1),
                    L(
                        from ceid in _commonLibrary.Ceids.Values
                        select
                        L(
                            U4((uint)ceid.ID),
                            L()
                            )
                        )
                )
            };
            var rep = await _secsGem.SendAsync(req, cancellation);
            var ack = rep.SecsItem.FirstValue<byte>();

            switch (ack)
            {
                case 0:
                    traceLog.Info("Initial -> S2F35 unlink event report - OK");
                    return true;
                case 1:
                    traceLog.Warn(String.Format("{0} - Link/Unlink Report Denied. Insufficient Space", ack.ToString()));
                    return false;
                case 2:
                    traceLog.Warn(String.Format("{0} - Link/Unlink Report Denied. Invalid Format", ack.ToString()));
                    return false;
                case 3:
                    traceLog.Warn(String.Format("{0} - Link/Unlink Report Denied. At least one CEID already defined", ack.ToString()));
                    return false;
                case 4:
                    traceLog.Warn(String.Format("{0} - Link/Unlink Report Denied. At least one CEID does not exist", ack.ToString()));
                    return false;
                case 5:
                    traceLog.Warn(String.Format("{0} - Link/Unlink Report Denied. At least one RPTID does not exist", ack.ToString()));
                    return false;
                default:
                    traceLog.Warn(String.Format("{0} - Link/Unlink Report Denied. Equipment rejects S2F35 message", ack.ToString()));
                    return false;
            }
        }
        private static async Task<bool> UndefineReport(CancellationToken cancellation)
        {
            traceLog.Info($"Sending S2F33 undefine report...");
            var req = new SecsMessage(2, 33)
            {
                SecsItem =
                L(
                    U4(1),
                    L(
                        from report in _commonLibrary.Reports.Values
                        select
                        L(
                            U4((uint)report.ID),
                            L()
                            )
                        )
                )
            };
            var rep = await _secsGem.SendAsync(req, cancellation);
            var ack = rep.SecsItem.FirstValue<byte>();

            switch (ack)
            {
                case 0:
                    traceLog.Info("Initial -> S2F33 undefine report - OK");
                    return true;
                case 1:
                    traceLog.Warn(String.Format("{0} - Define/Undefine Report Denied. Insufficient space", ack.ToString()));
                    return false;
                case 2:
                    traceLog.Warn(String.Format("{0} - Define/Undefine Report Denied. Invalid Format", ack.ToString()));
                    return false;
                case 3:
                    traceLog.Warn(String.Format("{0} - Define/Undefine Report Denied. At least one RPTID already defined", ack.ToString()));
                    return false;
                case 4:
                    traceLog.Warn(String.Format("{0} - Define/Undefine Report Denied. At least one VID does not exist", ack.ToString()));
                    return false;
                default:
                    traceLog.Warn(String.Format("{0} - Define/Undefine Report Denied. Equipment rejects S2F33 message", ack.ToString()));
                    return false;
            }
        }

        private static async Task<bool> DefineReport(CancellationToken cancellation)
        {
            traceLog.Info($"Sending S2F33 define report...");
            var req = new SecsMessage(2, 33)
            {
                SecsItem =
                L(
                    U4(1),
                    L(
                        from report in _commonLibrary.Reports.Values
                        select
                        L(
                            U4((uint)report.ID),
                            L(
                                from vid in report.Svids
                                select
                                U4((uint)vid.ID)
                                )
                            )
                        )
                )
            };
            var rep = await _secsGem.SendAsync(req, cancellation);
            var ack = rep.SecsItem.FirstValue<byte>();

            switch (ack)
            {
                case 0:
                    traceLog.Info("Initial -> S2F33 define report - OK");
                    return true;
                case 1:
                    traceLog.Warn(String.Format("{0} - Define/Undefine Report Denied. Insufficient space", ack.ToString()));
                    return false;
                case 2:
                    traceLog.Warn(String.Format("{0} - Define/Undefine Report Denied. Invalid Format", ack.ToString()));
                    return false;
                case 3:
                    traceLog.Warn(String.Format("{0} - Define/Undefine Report Denied. At least one RPTID already defined", ack.ToString()));
                    return false;
                case 4:
                    traceLog.Warn(String.Format("{0} - Define/Undefine Report Denied. At least one VID does not exist", ack.ToString()));
                    return false;
                default:
                    traceLog.Warn(String.Format("{0} - Define/Undefine Report Denied. Equipment rejects S2F33 message", ack.ToString()));
                    return false;
            }
        }

        private static async Task<bool> LinkEventReport(CancellationToken cancellation)
        {
            traceLog.Info($"Sending S2F35 link event report...");
            var req = new SecsMessage(2, 35)
            {
                SecsItem = L(
                    U4(1),
                    L(
                        from ceid in _commonLibrary.Ceids.Values
                        select
                        L(
                            U4((uint)ceid.ID),
                            L(
                                from report in ceid.Reports
                                select
                                U4((uint)report.ID)
                                )
                            )
                        )
                )
            };
            var rep = await _secsGem.SendAsync(req, cancellation);
            var ack = rep.SecsItem.FirstValue<byte>();

            switch (ack)
            {
                case 0:
                    traceLog.Info("Initial -> S2F35 link event report - OK");
                    return true;
                case 1:
                    traceLog.Warn(String.Format("{0} - Link/Unlink Report Denied. Insufficient Space", ack.ToString()));
                    return false;
                case 2:
                    traceLog.Warn(String.Format("{0} - Link/Unlink Report Denied. Invalid Format", ack.ToString()));
                    return false;
                case 3:
                    traceLog.Warn(String.Format("{0} - Link/Unlink Report Denied. At least one CEID already defined", ack.ToString()));
                    return false;
                case 4:
                    traceLog.Warn(String.Format("{0} - Link/Unlink Report Denied. At least one CEID does not exist", ack.ToString()));
                    return false;
                case 5:
                    traceLog.Warn(String.Format("{0} - Link/Unlink Report Denied. At least one RPTID does not exist", ack.ToString()));
                    return false;
                default:
                    traceLog.Warn(String.Format("{0} - Link/Unlink Report Denied. Equipment rejects S2F35 message", ack.ToString()));
                    return false;
            }
        }

        private static async Task<bool> EnableEvent(CancellationToken cancellation)
        {
            SecsMessage req;
            if (_commonLibrary.SecsConfigs.EnableAllEvent)
            {
                req = new SecsMessage(2, 37)
                {
                    SecsItem = L(
                    Boolean(true),
                    L()
                    )
                };
            }
            else
            {
                req = new SecsMessage(2, 37)
                {
                    SecsItem = L(
                  Boolean(true),
                  L(
                        from ceid in _commonLibrary.Ceids.Values
                        select
                        U4((uint)ceid.ID)
                      )
                  )
                };
            }
            var rep = await _secsGem.SendAsync(req, cancellation);
            var ack = rep.SecsItem.FirstValue<byte>();

            switch (ack)
            {
                case 0:
                    traceLog.Info("Initial -> S2F37 enable events - OK");
                    return true;
                case 1:
                    traceLog.Warn(String.Format("{0} - Enable/Disable Event Denied. At least one CEID does not exist", ack.ToString()));
                    return false;
                default:
                    traceLog.Warn(String.Format("{0} - Enable/Disable Event Denied. Equipment rejects S2F37 message", ack.ToString()));
                    return false;
            }
        }
    }
}
