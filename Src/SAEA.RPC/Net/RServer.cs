﻿/****************************************************************************
*Copyright (c) 2018 yswenli All Rights Reserved.
*CLR版本： 4.0.30319.42000
*机器名称：WENLI-PC
*公司名称：yswenli
*命名空间：SAEA.RPC.Net
*文件名： RServer
*版本号： v4.5.1.2
*唯一标识：5732cac7-ae47-4e6c-8533-844e350f3f81
*当前的用户域：WENLI-PC
*创建人： yswenli
*电子邮箱：wenguoli_520@qq.com
*创建时间：2018/5/16 16:16:54
*描述：
*
*=====================================================================
*修改标记
*修改时间：2018/5/16 16:16:54
*修改人： yswenli
*版本号： v4.5.1.2
*描述：
*
*****************************************************************************/
using SAEA.Sockets.Core;
using SAEA.Sockets.Core.Tcp;
using SAEA.Sockets.Interface;
using System;

namespace SAEA.RPC.Net
{
    /// <summary>
    /// provider socket处理
    /// </summary>
    internal class RServer : IocpServerSocket
    {
        /// <summary>
        /// 收到消息
        /// </summary>
        public event Action<IUserToken, RSocketMsg> OnMsg;

        public RServer(int port = 39654, int bufferSize = 100 * 1024, int count = 10000)
            : base(port: port, context: new RContext(), bufferSize: bufferSize, count: count)
        {

        }

        protected override void OnReceiveBytes(IUserToken userToken, byte[] data)
        {
            ((RCoder)userToken.Unpacker).Unpack(data, (r) =>
            {
                OnMsg.Invoke(userToken, r);
            });
        }
        /// <summary>
        /// 回复
        /// </summary>
        /// <param name="userToken"></param>
        /// <param name="msg"></param>
        internal void Reply(IUserToken userToken, RSocketMsg msg)
        {
            SendAsync(userToken, ((RCoder)userToken.Unpacker).Encode(msg));
        }
    }
}
