/*
 * Copyright
 *
 * https://github.com/LokiHonoo/development-resources
 * Copyright (C) LH.Studio 2015. All rights reserved.
 *
 * This code page is published under the terms of the MIT license.
 */

using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Text;
using System.Xml;

namespace LH.Xml
{
    /// <summary>
    /// Xml 辅助。
    /// </summary>
    internal static class XmlHelper
    {
        /// <summary>
        /// 格式化设置。
        /// </summary>
        private static readonly XmlReaderSettings _readerSettings = new XmlReaderSettings { XmlResolver = null };

        /// <summary>
        /// 格式化设置。
        /// </summary>
        private static readonly XmlWriterSettings _writerSettings = new XmlWriterSettings { Indent = true, NewLineChars = Environment.NewLine };

        /// <summary>
        /// 返回格式化后的 XmlDocument 字符串。
        /// </summary>
        /// <param name="doc">XmlDocument 文档。</param>
        /// <returns></returns>
        internal static string GetFormattedString(XmlDocument doc)
        {
            return GetFormattedString(doc, _writerSettings);
        }

        /// <summary>
        /// 指定格式化设置，返回格式化后的 XmlDocument 字符串。
        /// </summary>
        /// <param name="doc">XmlDocument 文档。</param>
        /// <param name="settings">格式化设置。</param>
        /// <returns></returns>
        internal static string GetFormattedString(XmlDocument doc, XmlWriterSettings settings)
        {
            if (doc == null)
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
        /// 将 Xml 文本转换为 XmlDocument 实例。
        /// </summary>
        /// <param name="xml">Xml 文本。</param>
        /// <returns></returns>
        internal static XmlDocument Parse(string xml)
        {
            return Parse(xml, null, _readerSettings);
        }

        /// <summary>
        /// 将 Xml 文本转换为 XmlDocument 实例。
        /// </summary>
        /// <param name="xml">Xml 文本。</param>
        /// <param name="resolver">解析器。</param>
        /// <param name="settings">格式化设置。</param>
        /// <returns></returns>
        [SuppressMessage("Style", "IDE0063:使用简单的 \"using\" 语句", Justification = "<挂起>")]
        internal static XmlDocument Parse(string xml, XmlResolver resolver, XmlReaderSettings settings)
        {
            XmlDocument doc = new XmlDocument() { XmlResolver = resolver };
            using (StringReader reader = new StringReader(xml))
            {
                using (XmlReader xReader = XmlReader.Create(reader, settings))
                {
                    doc.Load(xReader);
                }
            }
            return doc;
        }

        /// <summary>
        /// 保存格式化后的 XmlDocument 到文件。
        /// </summary>
        /// <param name="path">保存的文件路径。</param>
        /// <param name="doc">XmlDocument 文档。</param>
        internal static void SaveFormattedToFile(string path, XmlDocument doc)
        {
            SaveFormattedToFile(path, doc, _writerSettings);
        }

        /// <summary>
        /// 指定格式化设置，保存格式化后的 XmlDocument 到文件。
        /// </summary>
        /// <param name="path">保存的文件路径。</param>
        /// <param name="doc">XmlDocument 文档。</param>
        /// <param name="settings">格式化设置。</param>
        [SuppressMessage("样式", "IDE0063:使用简单的 \"using\" 语句", Justification = "<挂起>")]
        internal static void SaveFormattedToFile(string path, XmlDocument doc, XmlWriterSettings settings)
        {
            if (doc == null)
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