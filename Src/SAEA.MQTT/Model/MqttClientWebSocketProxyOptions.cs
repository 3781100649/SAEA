﻿/****************************************************************************
*项目名称：SAEA.MQTT.Model
*CLR 版本：4.0.30319.42000
*机器名称：WENLI-PC
*命名空间：SAEA.MQTT.Model
*类 名 称：MqttClientWebSocketProxyOptions
*版 本 号： v4.5.1.2
*创建人： yswenli
*电子邮箱：wenguoli_520@qq.com
*创建时间：2019/1/14 19:30:10
*描述：
*=====================================================================
*修改时间：2019/1/14 19:30:10
*修 改 人： yswenli
*版 本 号： v4.5.1.2
*描    述：
*****************************************************************************/
using System;
using System.Collections.Generic;
using System.Text;

namespace SAEA.MQTT.Model
{
    public class MqttClientWebSocketProxyOptions
    {
        public string Address { get; set; }

        public string Username { get; set; }

        public string Password { get; set; }

        public string Domain { get; set; }

        public bool BypassOnLocal { get; set; }

        public string[] BypassList { get; set; }
    }
}
