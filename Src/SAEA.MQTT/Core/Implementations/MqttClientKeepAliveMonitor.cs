﻿/****************************************************************************
*项目名称：SAEA.MQTT.Core.Implementations
*CLR 版本：4.0.30319.42000
*机器名称：WENLI-PC
*命名空间：SAEA.MQTT.Core.Implementations
*类 名 称：MqttClientKeepAliveMonitor
*版 本 号： v4.5.6.7
*创建人： yswenli
*电子邮箱：wenguoli_520@qq.com
*创建时间：2019/1/15 15:48:51
*描述：
*=====================================================================
*修改时间：2019/1/15 15:48:51
*修 改 人： yswenli
*版 本 号： v4.5.6.7
*描    述：
*****************************************************************************/
using SAEA.MQTT.Common.Log;
using SAEA.MQTT.Core.Packets;
using SAEA.MQTT.Core.Protocol;
using SAEA.MQTT.Interface;
using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace SAEA.MQTT.Core.Implementations
{
    public class MqttClientKeepAliveMonitor
    {
        private readonly Stopwatch _lastPacketReceivedTracker = new Stopwatch();
        private readonly Stopwatch _lastNonKeepAlivePacketReceivedTracker = new Stopwatch();

        private readonly IMqttClientSession _clientSession;
        private readonly IMqttNetChildLogger _logger;

        private bool _isPaused;

        public MqttClientKeepAliveMonitor(IMqttClientSession clientSession, IMqttNetChildLogger logger)
        {
            if (logger == null) throw new ArgumentNullException(nameof(logger));

            _clientSession = clientSession ?? throw new ArgumentNullException(nameof(clientSession));

            _logger = logger.CreateChildLogger(nameof(MqttClientKeepAliveMonitor));
        }

        public TimeSpan LastPacketReceived => _lastPacketReceivedTracker.Elapsed;

        public TimeSpan LastNonKeepAlivePacketReceived => _lastNonKeepAlivePacketReceivedTracker.Elapsed;

        public void Start(int keepAlivePeriod, CancellationToken cancellationToken)
        {
            if (keepAlivePeriod == 0)
            {
                return;
            }

            Task.Run(() => RunAsync(keepAlivePeriod, cancellationToken), cancellationToken).ConfigureAwait(false);
        }

        public void Pause()
        {
            _isPaused = true;
        }

        public void Resume()
        {
            _isPaused = false;
        }

        public void Reset()
        {
            _lastPacketReceivedTracker.Restart();
            _lastNonKeepAlivePacketReceivedTracker.Restart();
        }

        public void PacketReceived(MqttBasePacket packet)
        {
            _lastPacketReceivedTracker.Restart();

            if (!(packet is MqttPingReqPacket))
            {
                _lastNonKeepAlivePacketReceivedTracker.Restart();
            }
        }

        private async Task RunAsync(int keepAlivePeriod, CancellationToken cancellationToken)
        {
            try
            {
                _lastPacketReceivedTracker.Restart();
                _lastNonKeepAlivePacketReceivedTracker.Restart();

                while (!cancellationToken.IsCancellationRequested)
                {
                    // Values described here: [MQTT-3.1.2-24].
                    // If the client sends 5 sec. the server will allow up to 7.5 seconds.
                    // If the client sends 1 sec. the server will allow up to 1.5 seconds.
                    if (!_isPaused && _lastPacketReceivedTracker.Elapsed.TotalSeconds >= keepAlivePeriod * 1.5D)
                    {
                        _logger.Warning(null, "Client '{0}': Did not receive any packet or keep alive signal.", _clientSession.ClientId);
                        _clientSession.Stop(MqttClientDisconnectType.NotClean);

                        return;
                    }

                    // The server checks the keep alive timeout every 50 % of the overall keep alive timeout
                    // because the server allows 1.5 times the keep alive value. This means that a value of 5 allows
                    // up to 7.5 seconds. With an interval of 2.5 (5 / 2) the 7.5 is also affected. Waiting the whole
                    // keep alive time will hit at 10 instead of 7.5 (but only one time instead of two times).
                    await Task.Delay(TimeSpan.FromSeconds(keepAlivePeriod * 0.5D), cancellationToken).ConfigureAwait(false);
                }
            }
            catch (OperationCanceledException)
            {
            }
            catch (Exception exception)
            {
                _logger.Error(exception, "Client '{0}': Unhandled exception while checking keep alive timeouts.", _clientSession.ClientId);
            }
            finally
            {
                _logger.Verbose("Client '{0}': Stopped checking keep alive timeout.", _clientSession.ClientId);
            }
        }
    }
}
