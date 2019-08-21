﻿/****************************************************************************
*Copyright (c) 2018 yswenli All Rights Reserved.
*CLR版本： 4.0.30319.42000
*机器名称：WENLI-PC
*公司名称：yswenli
*命名空间：SAEA.TcpP2P.Net
*文件名： QContext
*版本号： v5.0.0.1
*唯一标识：cad46d37-3703-4ffa-a721-eee312ed3eeb
*当前的用户域：WENLI-PC
*创建人： yswenli
*电子邮箱：wenguoli_520@qq.com
*创建时间：2018/3/1 21:29:36
*描述：
*
*=====================================================================
*修改标记
*修改时间：2018/3/1 21:29:36
*修改人： yswenli
*版本号： v5.0.0.1
*描述：
*
*****************************************************************************/
using SAEA.Sockets.Base;
using SAEA.Sockets.Interface;

namespace SAEA.TcpP2P.Net
{
    public class PContext : IContext
    {
        public IUserToken UserToken { get; set; }

        public IUnpacker Unpacker { get; set; }

        /// <summary>
        /// 上下文
        /// 支持IContext 扩展
        /// </summary>
        public PContext()
        {
            this.UserToken = new BaseUserToken();
            this.Unpacker = new PCoder();
            this.UserToken.Unpacker = this.Unpacker;
        }
    }
}
