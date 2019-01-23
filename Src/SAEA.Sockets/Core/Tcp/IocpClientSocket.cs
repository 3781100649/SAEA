﻿/****************************************************************************
 * 
  ____    _    _____    _      ____             _        _   
 / ___|  / \  | ____|  / \    / ___|  ___   ___| | _____| |_ 
 \___ \ / _ \ |  _|   / _ \   \___ \ / _ \ / __| |/ / _ \ __|
  ___) / ___ \| |___ / ___ \   ___) | (_) | (__|   <  __/ |_ 
 |____/_/   \_\_____/_/   \_\ |____/ \___/ \___|_|\_\___|\__|
                                                             

*Copyright (c) 2018 yswenli All Rights Reserved.
*CLR版本： 2.1.4
*机器名称：WENLI-PC
*公司名称：wenli
*命名空间：SAEA.Sockets.Core.Tcp
*文件名： IocpClientSocket
*版本号： V4.0.0.1
*唯一标识：ef84e44b-6fa2-432e-90a2-003ebd059303
*当前的用户域：WENLI-PC
*创建人： yswenli
*电子邮箱：wenguoli_520@qq.com
*创建时间：2018/3/1 15:54:21
*描述：
*
*=====================================================================
*修改标记
*修改时间：2018/3/1 15:54:21
*修改人： yswenli
*版本号： V4.0.0.1
*描述：
*
*****************************************************************************/

using SAEA.Common;
using SAEA.Sockets.Handler;
using SAEA.Sockets.Interface;
using SAEA.Sockets.Model;
using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace SAEA.Sockets.Core.Tcp
{
    /// <summary>
    /// iocp 客户端 socket
    /// </summary>
    public class IocpClientSocket : ISocket, IDisposable
    {
        Socket _socket;

        string _ip = string.Empty;

        int _port = 39654;

        IUserToken _userToken;

        SocketAsyncEventArgs _connectArgs;

        bool _connected = false;

        Action<SocketError> _connectCallBack;

        AutoResetEvent _connectEvent = new AutoResetEvent(true);

        SocketOption _SocketOption;

        public bool Connected { get => _connected; set => _connected = value; }
        public IUserToken UserToken { get => _userToken; private set => _userToken = value; }

        public bool IsDisposed { get; private set; } = false;

        public event OnErrorHandler OnError;

        public event OnDisconnectedHandler OnDisconnected;

        protected OnClientReceiveBytesHandler OnClientReceive;

        protected void RaiseOnError(string id, Exception ex)
        {
            OnError?.Invoke(id, ex);
        }

        public IocpClientSocket(SocketOption socketOption) : this(socketOption.Context, socketOption.IP, socketOption.Port, socketOption.BufferSize, socketOption.TimeOut)
        {
            _SocketOption = socketOption;
        }

        /// <summary>
        /// iocp 客户端 socket
        /// </summary>
        /// <param name="context"></param>
        /// <param name="ip"></param>
        /// <param name="port"></param>
        /// <param name="bufferSize"></param>
        /// <param name="timeOut"></param>
        public IocpClientSocket(IContext context, string ip = "127.0.0.1", int port = 39654, int bufferSize = 100 * 1024, int timeOut = 60 * 1000)
        {
            _socket = new Socket(AddressFamily.InterNetwork, System.Net.Sockets.SocketType.Stream, ProtocolType.Tcp);
            _socket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
            _socket.NoDelay = true;
            _socket.SendTimeout = _socket.ReceiveTimeout = 120 * 1000;

            _ip = ip;
            _port = port;

            OnClientReceive = new OnClientReceiveBytesHandler(OnReceived);

            _connectArgs = new SocketAsyncEventArgs
            {
                RemoteEndPoint = new IPEndPoint(IPAddress.Parse(_ip), _port)
            };
            _connectArgs.Completed += ConnectArgs_Completed;

            _userToken = UserTokenFactory.Create(context, bufferSize, IO_Completed);
        }

        /// <summary>
        /// 连接到服务器
        /// </summary>
        /// <param name="callBack"></param>
        public async void ConnectAsync(Action<SocketError> callBack = null)
        {
            _connectEvent.WaitOne();
            if (!_connected)
            {
                _connectCallBack = callBack;
                if (!_socket.ConnectAsync(_connectArgs))
                {
                    ProcessConnected(_connectArgs);
                }
            }
        }


        public void Connect()
        {
            var wait = new AutoResetEvent(false);
            ConnectAsync((s) =>
            {
                wait.Set();
            });
            if (!wait.WaitOne(10 * 1000))
            {
                _socket.Disconnect(true);
                throw new Exception("连接超时!");
            }
        }

        void ConnectArgs_Completed(object sender, SocketAsyncEventArgs e)
        {
            ProcessConnected(e);
        }

        void ProcessConnected(SocketAsyncEventArgs e)
        {
            _connectEvent.Set();
            _connected = (e.SocketError == SocketError.Success);
            if (_connected)
            {
                _userToken.ID = e.ConnectSocket.LocalEndPoint.ToString();
                _userToken.Socket = _socket;
                _userToken.Linked = _userToken.Actived = DateTime.Now;

                var readArgs = _userToken.ReadArgs;
                if (!_userToken.Socket.ReceiveAsync(readArgs))
                    ProcessReceive(readArgs);
                _connectCallBack?.Invoke(e.SocketError);
            }
        }


        void IO_Completed(object sender, SocketAsyncEventArgs e)
        {
            switch (e.LastOperation)
            {
                case SocketAsyncOperation.Receive:
                    ProcessReceive(e);
                    break;
                case SocketAsyncOperation.Send:
                    ProcessSended(e);
                    break;
                default:
                    Disconnect();
                    break;
            }
        }

        protected virtual void OnReceived(byte[] data) { }



        void ProcessReceive(SocketAsyncEventArgs e)
        {
            try
            {
                if (e.BytesTransferred > 0 && e.SocketError == SocketError.Success)
                {

                    _userToken.Actived = DateTimeHelper.Now;

                    var data = new byte[e.BytesTransferred];

                    Buffer.BlockCopy(e.Buffer, e.Offset, data, 0, e.BytesTransferred);

                    OnClientReceive?.Invoke(data);

                    if (_userToken.Socket != null && !_userToken.Socket.ReceiveAsync(e))
                        ProcessReceive(e);
                }
                else
                {
                    ProcessDisconnected(new Exception("SocketError:" + e.SocketError.ToString()));
                }
            }
            catch (Exception ex)
            {
                OnError?.Invoke(_userToken.ID, ex);
                Disconnect();
            }
        }

        void ProcessSended(SocketAsyncEventArgs e)
        {
            _userToken.Set();
            _userToken.Actived = DateTimeHelper.Now;
        }

        void ProcessDisconnected(Exception ex)
        {
            _connected = false;
            _connectEvent.Set();
            try
            {
                _userToken.Clear();
            }
            catch { }
            OnDisconnected?.Invoke(_userToken.ID, ex);
        }

        /// <summary>
        /// 异步发送
        /// </summary>
        /// <param name="userToken"></param>
        /// <param name="data"></param>
        public void SendAsync(IUserToken userToken, byte[] data)
        {
            try
            {
                userToken.WaitOne();

                var writeArgs = userToken.WriteArgs;

                writeArgs.SetBuffer(data, 0, data.Length);

                if (!userToken.Socket.SendAsync(writeArgs))
                {
                    ProcessSended(writeArgs);
                }
            }
            catch (Exception ex)
            {
                OnError?.Invoke(userToken.ID, ex);
                userToken.Set();
                Disconnect();
            }
        }

        /// <summary>
        /// 异步发送
        /// </summary>
        /// <param name="data"></param>
        public void SendAsync(byte[] data)
        {
            SendAsync(UserToken, data);
        }

        /// <summary>
        /// 同步发送
        /// </summary>
        /// <param name="data"></param>
        protected void Send(byte[] data)
        {
            if (data == null) return;

            if (_connected)
            {
                var sendNum = 0;

                int offset = 0;

                try
                {

                    while (_connected)
                    {
                        sendNum += _socket.Send(data, offset, data.Length - offset, SocketFlags.None);

                        offset += sendNum;

                        if (sendNum == data.Length)
                        {
                            break;
                        }
                    }
                    _userToken.Actived = DateTimeHelper.Now;
                }
                catch (Exception ex)
                {
                    ProcessDisconnected(ex);
                }
            }
            else
                OnError?.Invoke("", new Exception("发送失败：当前连接已断开"));
        }

        protected void SendTo(IPEndPoint remoteEP, byte[] data)
        {
            if (data == null) return;

            if (_connected)
            {
                var sendNum = 0;

                int offset = 0;

                try
                {
                    while (_connected)
                    {
                        sendNum += _socket.SendTo(data, offset, data.Length - offset, SocketFlags.None, remoteEP);

                        offset += sendNum;

                        if (sendNum == data.Length)
                        {
                            break;
                        }
                    }
                    if (!_connected)
                        ConnectArgs_Completed(this, _connectArgs);
                    _userToken.Actived = DateTimeHelper.Now;
                }
                catch (Exception ex)
                {
                    ProcessDisconnected(ex);
                }
            }
            else
                OnError?.Invoke("", new Exception("发送失败：当前连接已断开"));
        }


        public void BeginSend(byte[] data)
        {
            if (Connected)
            {
                _userToken.Socket.BeginSend(data, 0, data.Length, SocketFlags.None, null, null);
            }
        }


        public void Disconnect(Exception ex = null)
        {
            var mex = ex;

            if (this.Connected)
            {
                try
                {
                    if (_userToken != null && _userToken.Socket != null)
                        _userToken.Socket.Close();
                }
                catch (Exception sex)
                {
                    if (mex != null) mex = sex;
                }
                this.Connected = false;
                if (mex == null)
                {
                    mex = new Exception("当前用户已主动断开连接！");
                }
                if (_userToken != null)
                    OnDisconnected?.Invoke(_userToken.ID, mex);

                _userToken.Clear();
            }
        }

        private void _sessionManager_OnTimeOut(IUserToken obj)
        {
            Disconnect();
        }

        public void Dispose()
        {
            this.Disconnect();
            IsDisposed = true;
        }
    }
}