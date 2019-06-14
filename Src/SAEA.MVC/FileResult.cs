﻿/****************************************************************************
*Copyright (c) 2018 yswenli All Rights Reserved.
*CLR版本： 4.0.30319.42000
*机器名称：WENLI-PC
*公司名称：yswenli
*命名空间：SAEA.MVC
*文件名： FileResult
*版本号： v4.5.6.7
*唯一标识：10362e3c-aeb1-4282-a3b2-69774773af9e
*当前的用户域：WENLI-PC
*创建人： yswenli
*电子邮箱：wenguoli_520@qq.com
*创建时间：2018/4/16 13:16:32
*描述：
*
*=====================================================================
*修改标记
*修改时间：2018/4/16 13:16:32
*修改人： yswenli
*版本号： v4.5.6.7
*描述：
*
*****************************************************************************/
using SAEA.Common;
using SAEA.Http.Base;
using SAEA.Http.Common;
using SAEA.Http.Model;
using System.Net;
using System.Text;

namespace SAEA.MVC
{
    /// <summary>
    /// 文件类型
    /// </summary>
    public class FileResult : ActionResult, IFileResult
    {
        public new byte[] Content { get; set; }

        public FileResult(string filePath, bool isStaticsCached) : this(isStaticsCached, filePath, HttpMIME.GetType(filePath))
        {

        }

        /// <summary>
        /// 文件内容
        /// </summary>
        /// <param name="isStaticsCached"></param>
        /// <param name="filePath"></param>
        /// <param name="contentType"></param>
        /// <param name="status"></param>
        public FileResult(bool isStaticsCached, string filePath, string contentType = "", HttpStatusCode status = HttpStatusCode.OK)
        {
            if (isStaticsCached)
            {
                this.Content = StaticResourcesCache.GetOrAdd(filePath, filePath);
            }
            else
            {
                this.Content = FileHelper.Read(filePath);
            }
            this.ContentEncoding = Encoding.UTF8;
            this.ContentType = contentType;
            this.Status = status;
        }
    }
}
