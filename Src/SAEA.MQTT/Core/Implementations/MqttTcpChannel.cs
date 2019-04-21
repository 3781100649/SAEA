/****************************************************************************
*��Ŀ���ƣ�SAEA.MQTT
*CLR �汾��4.0.30319.42000
*�������ƣ�WENLI-PC
*�����ռ䣺SAEA.MQTT.Core.Implementations
*�� �� �ƣ�MqttTcpChannel
*�� �� �ţ� v4.5.1.2
*�����ˣ� yswenli
*�������䣺wenguoli_520@qq.com
*����ʱ�䣺2019/1/14 19:07:44
*������
*=====================================================================
*�޸�ʱ�䣺2019/1/14 19:07:44
*�� �� �ˣ� yswenli
*�� �� �ţ� v4.5.1.2
*��    ����
*****************************************************************************/

using SAEA.MQTT.Interface;
using SAEA.MQTT.Model;
using SAEA.Sockets;
using SAEA.Sockets.Core;
using SAEA.Sockets.Core.Tcp;
using SAEA.Sockets.Interface;
using System;
using System.IO;
using System.Net.Sockets;
using System.Security.Cryptography.X509Certificates;
using System.Threading;
using System.Threading.Tasks;

namespace SAEA.MQTT.Core.Implementations
{
    public class MqttTcpChannel : IMqttChannel
    {
        private readonly IMqttClientOptions _clientOptions;

        private readonly MqttClientTcpOptions _options;


        IClientSocket _clientSocket;


        public MqttTcpChannel(IMqttClientOptions clientOptions)
        {
            _clientOptions = clientOptions ?? throw new ArgumentNullException(nameof(clientOptions));
            _options = (MqttClientTcpOptions)clientOptions.ChannelOptions;

            var builder = new SocketOptionBuilder()
                .UseStream()
                .SetIP(_options.Server)
                .SetPort(_options.Port ?? 1883);

            if (_options.TlsOptions.UseTls)
            {
                builder = builder.WithSsl(_options.TlsOptions.SslProtocol);
            }

            var option = builder.Build();

            _clientSocket = SocketFactory.CreateClientSocket(option);
        }



        public MqttTcpChannel(Socket socket, Stream sslStream)
        {
            _clientSocket = new StreamClientSocket(socket, sslStream);
        }

        public string Endpoint
        {
            get
            {
                return _clientSocket.Endpoint; ;
            }
        }

        public async Task ConnectAsync()
        {
            await _clientSocket.ConnectAsync();
        }

        public Task DisconnectAsync()
        {
            Dispose();
            return Task.FromResult(0);
        }

        public Task<int> ReadAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken)
        {
            ChannelManager.Current.Refresh(_clientSocket.Endpoint);
            return _clientSocket.ReceiveAsync(buffer, offset, count, cancellationToken);
        }

        public Task WriteAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken)
        {
            ChannelManager.Current.Refresh(_clientSocket.Endpoint);
            return _clientSocket.SendAsync(buffer, offset, count, cancellationToken);
        }

        public void Dispose()
        {
            ChannelManager.Current.Remove(_clientSocket.Endpoint);
            _clientSocket.Dispose();
        }


        private X509CertificateCollection LoadCertificates()
        {
            var certificates = new X509CertificateCollection();
            if (_options.TlsOptions.Certificates == null)
            {
                return certificates;
            }

            foreach (var certificate in _options.TlsOptions.Certificates)
            {
                certificates.Add(new X509Certificate2(certificate));
            }

            return certificates;
        }
    }
}
