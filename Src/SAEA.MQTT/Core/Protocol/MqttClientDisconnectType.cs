﻿/****************************************************************************
*项目名称：SAEA.MQTT.Core.Protocol
*CLR 版本：4.0.30319.42000
*机器名称：WENLI-PC
*命名空间：SAEA.MQTT.Core.Protocol
*类 名 称：MqttClientDisconnectType
*版 本 号： v4.5.1.2
*创建人： yswenli
*电子邮箱：wenguoli_520@qq.com
*创建时间：2019/1/15 15:41:31
*描述：
*=====================================================================
*修改时间：2019/1/15 15:41:31
*修 改 人： yswenli
*版 本 号： v4.5.1.2
*描    述：
*****************************************************************************/
using System;
using System.Collections.Generic;
using System.Text;

namespace SAEA.MQTT.Core.Protocol
{
    public enum MqttClientDisconnectType
    {
        Clean,
        NotClean
    }
}
