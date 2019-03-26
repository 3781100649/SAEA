﻿/****************************************************************************
*Copyright (c) 2018 yswenli All Rights Reserved.
*CLR版本： 2.1.4
*机器名称：WENLI-PC
*公司名称：wenli
*命名空间：SAEA.Commom
*文件名： Class1
*版本号： v4.3.2.5
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
*版本号： v4.3.2.5
*描述：
*
*****************************************************************************/

using System;
using System.Collections.Generic;
using System.Net;

namespace SAEA.Common
{
    public static class StringHelper
    {
        static int k = 1024;

        static int m = k * k;

        static long g = m * k;

        static long t = g * k;

        public static string Convert(long len)
        {
            string result = string.Empty;

            if (len < k)
            {
                result = string.Format("{0:F} B", len);
            }
            else if (len < m)
            {
                result = string.Format("{0} KB", ((len / 1.00 / k)).ToString("f2"));
            }
            else if (len < g)
            {
                result = string.Format("{0} MB", ((len / 1.00 / m)).ToString("f2"));
            }
            else
            {
                result = string.Format("{0} GB", ((len / 1.00 / g)).ToString("f2"));
            }
            return result;
        }

        public static string ToSpeedString(this long l)
        {
            return Convert(l);
        }

        public static ValueTuple<string, int> ToIPPort(this string remote)
        {
            ValueTuple<string, int> result;

            var arr = remote.Split(new string[] { ConstHelper.COLON, ConstHelper.SPACE, ConstHelper.COMMA }, StringSplitOptions.None);

            var ip = arr[0];

            var port = int.Parse(arr[1]);

            result = new ValueTuple<string, int>(ip, port);

            return result;
        }

        public static IPEndPoint ToIPEndPoint(this string remote)
        {
            var tuple = remote.ToIPPort();
            return new IPEndPoint(IPAddress.Parse(tuple.Item1), tuple.Item2);
        }


        public static string[] ToArray(this string str, bool none = false, params string[] splits)
        {
            return str.Split(splits, none ? StringSplitOptions.None : StringSplitOptions.RemoveEmptyEntries);
        }

        [Obsolete("测试用")]
        public static string[] Split2(this string str, string splitStr, StringSplitOptions option)
        {
            return str.Split(new string[] { splitStr }, option);
        }

        /// <summary>
        /// 自定义分隔
        /// </summary>
        /// <param name="str"></param>
        /// <param name="splitStr"></param>
        /// <param name="option"></param>
        /// <returns></returns>
        public static string[] Split(this string str, string splitStr, StringSplitOptions option)
        {
            var strSpan = str.AsSpan();

            var splitSapn = splitStr.AsSpan();

            int m = 0, n = 0;

            List<string> arr = new List<string>();

            while (true)
            {
                m = n;
                n = strSpan.IndexOf(splitSapn);
                if (n > -1)
                {
                    arr.Add(strSpan.Slice(0, n).ToString());
                    strSpan = strSpan.Slice(n + splitSapn.Length);
                }
                else
                {
                    arr.Add(strSpan.Slice(0).ToString());
                    break;
                }
            }
            if (option == StringSplitOptions.RemoveEmptyEntries)
            {
                arr.RemoveAll(b => string.IsNullOrEmpty(b));
            }
            return arr.ToArray();
        }


        public static string[] Split(this string str, string splitStr)
        {
            return str.Split(splitStr, StringSplitOptions.None);
        }


        public static string Substring(this string str, int start, int length)
        {
            return str.AsSpan(start, length).ToString();
        }

        public static string Substring(this string str, int start)
        {
            return str.AsSpan(start, str.Length - start).ToString();
        }

        public static int ParseToInt(this string str,int start, int count)
        {
            return int.Parse(Substring(str, start, count));
        }
    }
}
