﻿/*
 * Copyright
 *
 * https://github.com/LokiHonoo/development-resources
 * Copyright (C) Loki Honoo 2015. All rights reserved.
 *
 * This code page is published under the terms of the MIT license.
 */

using System;
using System.Text;
using System.Xml;

namespace Honoo.Xml
{
    /// <summary>
    /// Xml 辅助。
    /// </summary>
    public static class XmlHelper
    {
        /// <summary>
        /// 写设置参数。
        /// </summary>
        private static readonly XmlWriterSettings _writerSettings = new XmlWriterSettings { Indent = true, NewLineChars = Environment.NewLine, Encoding = new UTF8Encoding(false) };

        /// <summary>
        /// 返回格式化后的 XmlDocument 字符串。
        /// </summary>
        /// <param name="doc">XmlDocument 文档。</param>
        /// <returns></returns>
        public static string GetFormattedString(XmlDocument doc)
        {
            if (doc is null)
            {
                throw new ArgumentNullException(nameof(doc));
            }
            StringBuilder sb = new StringBuilder();
            using (XmlWriter writer = XmlWriter.Create(sb, _writerSettings))
            {
                doc.Save(writer);
                writer.Close();
            }
            return sb.ToString();
        }

        /// <summary>
        /// 保存格式化后的 XmlDocument 到文件。
        /// </summary>
        /// <param name="doc">XmlDocument 文档。</param>
        /// <param name="path">保存的文件路径。</param>
        public static void SaveFormattedToFile(XmlDocument doc, string path)
        {
            if (doc is null)
            {
                throw new ArgumentNullException(nameof(doc));
            }
            using (XmlWriter writer = XmlWriter.Create(path, _writerSettings))
            {
                doc.Save(writer);
                writer.Close();
            }
        }
    }
}