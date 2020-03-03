/*
 * Copyright
 *
 * https://github.com/LokiHonoo/development-resources
 * Copyright (C) LH.Studio 2015. All rights reserved.
 *
 * This code page is published under the terms of the MIT license.
 */

using System;
using System.Text;
using System.Xml;

namespace LH.Xml
{
    /// <summary>
    /// Xml 辅助。
    /// </summary>
    public static class XmlHelper
    {
        /// <summary>
        /// 格式化设置。
        /// </summary>
        private static readonly XmlWriterSettings _settings = new XmlWriterSettings { Indent = true, NewLineChars = Environment.NewLine };

        /// <summary>
        /// 返回格式化后的 XmlDocument 字符串。
        /// </summary>
        /// <param name="doc">XmlDocument 文档。</param>
        /// <returns></returns>
        public static string GetFormattedString(XmlDocument doc)
        {
            return GetFormattedString(doc, _settings);
        }

        /// <summary>
        /// 指定格式化设置，返回格式化后的 XmlDocument 字符串。
        /// </summary>
        /// <param name="doc">XmlDocument 文档。</param>
        /// <param name="settings">格式化设置。</param>
        /// <returns></returns>
        public static string GetFormattedString(XmlDocument doc, XmlWriterSettings settings)
        {
            if (doc is null)
            {
                throw new ArgumentNullException(nameof(doc));
            }
            StringBuilder sb = new StringBuilder();
            using (XmlWriter writer = XmlWriter.Create(sb, settings))
            {
                doc.WriteContentTo(writer);
                writer.Close();
            }
            return sb.ToString();
        }

        /// <summary>
        /// 保存格式化后的 XmlDocument 到文件。
        /// </summary>
        /// <param name="path">保存的文件路径。</param>
        /// <param name="doc">XmlDocument 文档。</param>
        public static void SaveFormattedToFile(string path, XmlDocument doc)
        {
            SaveFormattedToFile(path, doc, _settings);
        }

        /// <summary>
        /// 指定格式化设置，保存格式化后的 XmlDocument 到文件。
        /// </summary>
        /// <param name="path">保存的文件路径。</param>
        /// <param name="doc">XmlDocument 文档。</param>
        /// <param name="settings">格式化设置。</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("样式", "IDE0063:使用简单的 \"using\" 语句", Justification = "<挂起>")]
        public static void SaveFormattedToFile(string path, XmlDocument doc, XmlWriterSettings settings)
        {
            if (doc is null)
            {
                throw new ArgumentNullException(nameof(doc));
            }
            using (XmlWriter writer = XmlWriter.Create(path, settings))
            {
                doc.WriteContentTo(writer);
                writer.Close();
            }
        }
    }
}