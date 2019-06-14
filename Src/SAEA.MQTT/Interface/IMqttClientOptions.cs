﻿/****************************************************************************
*项目名称：SAEA.MQTT.Interface
*CLR 版本：4.0.30319.42000
*机器名称：WENLI-PC
*命名空间：SAEA.MQTT.Interface
*类 名 称：IMqttClientOptions
*版 本 号： v4.5.6.7
*创建人： yswenli
*电子邮箱：wenguoli_520@qq.com
*创建时间：2019/1/14 19:27:42
*描述：
*=====================================================================
*修改时间：2019/1/14 19:27:42
*修 改 人： yswenli
*版 本 号： v4.5.6.7
*描    述：
*****************************************************************************/
using SAEA.MQTT.Core.Protocol;
using SAEA.MQTT.Model;
using System;

namespace SAEA.MQTT.Interface
{
    public interface IMqttClientOptions
    {
        string ClientId { get; }
        bool CleanSession { get; }
        IMqttClientCredentials Credentials { get; }
        MqttProtocolVersion ProtocolVersion { get; }
        IMqttClientChannelOptions ChannelOptions { get; }

        TimeSpan CommunicationTimeout { get; }
        TimeSpan KeepAlivePeriod { get; }
        TimeSpan? KeepAliveSendInterval { get; }

        MqttMessage WillMessage { get; }
    }
}
