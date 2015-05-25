// <copyright file="XmlExtensions.cs" company="Edge Extensions Project">
// Copyright (c) 2009 All Rights Reserved
// </copyright>
// <author>Kevin Nessland</author>
// <email>kevinnessland@gmail.com</email>
// <date>2009-07-08</date>
// <summary>Contains Xml-related extension methods.</summary>

using Creek.Dynamics;
using System.Linq;
using System.Text;
using System.Xml;

namespace Creek.Extensions
{
    /// <summary>
    /// Extension methods for the XmlNode / XmlDocument classes and its sub classes
    /// </summary>
    public static class XmlExtensions
    {
        /// <summary>
        /// Appends a child to a XML node.
        /// </summary>
        /// <param name="parentNode">The parent node.</param>
        /// <param name="name">The name of the child node.</param>
        /// <returns>The newly created XML node.</returns>
        public static XmlNode CreateChildNode(this XmlNode parentNode, string name)
        {
            XmlDocument document = parentNode is XmlDocument ? (XmlDocument)parentNode : parentNode.OwnerDocument;
            XmlNode node = document.CreateElement(name);
            parentNode.AppendChild(node);
            return node;
        }

        /// <summary>
        /// Appends a child to a XML node.
        /// </summary>
        /// <param name="parentNode">The parent node.</param>
        /// <param name="name">The name of the child node.</param>
        /// <param name="namespaceUri">The node namespace.</param>
        /// <returns>The newly cerated XML node.</returns>
        public static XmlNode CreateChildNode(this XmlNode parentNode, string name, string namespaceUri)
        {
            XmlDocument document = parentNode is XmlDocument ? (XmlDocument)parentNode : parentNode.OwnerDocument;
            XmlNode node = document.CreateElement(name, namespaceUri);
            parentNode.AppendChild(node);
            return node;
        }

        /// <summary>
        /// Appends a CData section to a XML node and prefills the provided data.
        /// </summary>
        /// <param name="parentNode">The parent node.</param>
        /// <param name="data">The CData section value.</param>
        /// <returns>The created CData Section.</returns>
        public static XmlCDataSection CreateCDataSection(this XmlNode parentNode, string data)
        {
            XmlDocument document = parentNode is XmlDocument ? (XmlDocument)parentNode : parentNode.OwnerDocument;
            XmlCDataSection node = document.CreateCDataSection(data);
            parentNode.AppendChild(node);
            return node;
        }

        /// <summary>
        /// Returns the value of a nested CData section.
        /// </summary>
        /// <param name="parentNode">The parent node.</param>
        /// <returns>The CData section content.</returns>
        public static string GetCDataSection(this XmlNode parentNode)
        {
            foreach (var node in parentNode.ChildNodes)
            {
                if (node is XmlCDataSection)
                {
                    return ((XmlCDataSection)node).Value;
                }
            }

            return null;
        }

        /// <summary>
        /// Gets an attribute value.
        /// </summary>
        /// <param name="node">The node to retreive the value from.</param>
        /// <param name="attributeName">The Name of the attribute.</param>
        /// <returns>The attribute value.</returns>
        public static string GetAttribute(this XmlNode node, string attributeName)
        {
            return GetAttribute(node, attributeName, null);
        }

        /// <summary>
        /// Gets an attribute value.
        /// If the value is empty, uses the specified default value.
        /// </summary>
        /// <param name="node">The node to retreive the value from.</param>
        /// <param name="attributeName">The Name of the attribute.</param>
        /// <param name="defaultValue">The default value to be returned if no matching attribute exists.</param>
        /// <returns>The attribute value.</returns>
        public static string GetAttribute(this XmlNode node, string attributeName, string defaultValue)
        {
            XmlAttribute attribute = node.Attributes[attributeName];
            return attribute != null ? attribute.InnerText : defaultValue;
        }

        /// <summary>
        /// Gets an attribute value converted to the specified data type
        /// </summary>
        /// <typeparam name="T">The desired return data type</typeparam>
        /// <param name="node">The node to evaluate.</param>
        /// <param name="attributeName">The Name of the attribute.</param>
        /// <returns>The attribute value.</returns>
        public static T GetAttribute<T>(this XmlNode node, string attributeName)
        {
            return GetAttribute<T>(node, attributeName, default(T));
        }

        /// <summary>
        /// Gets an attribute value converted to the specified data type.
        /// If the value is empty, uses the specified default value.
        /// </summary>
        /// <typeparam name="T">The desired return data type</typeparam>
        /// <param name="node">The node to evaluate.</param>
        /// <param name="attributeName">The Name of the attribute.</param>
        /// <param name="defaultValue">The default value to be returned if no matching attribute exists.</param>
        /// <returns>The attribute value.</returns>
        public static T GetAttribute<T>(this XmlNode node, string attributeName, T defaultValue)
        {
            var value = GetAttribute(node, attributeName);

            return !value.IsEmpty() ? value.ConvertTo<T>(defaultValue) : defaultValue;
        }

        /// <summary>
        /// Creates or updates an attribute with the passed object.
        /// </summary>
        /// <param name="node">The node to evaluate.</param>
        /// <param name="name">The attribute name.</param>
        /// <param name="value">The attribute value.</param>
        public static void SetAttribute(this XmlNode node, string name, object value)
        {
            SetAttribute(node, name, value != null ? value.ToString() : null);
        }

        /// <summary>
        /// Creates or updates an attribute with the passed value.
        /// </summary>
        /// <param name="node">The node to evaluate.</param>
        /// <param name="name">The attribute name.</param>
        /// <param name="value">The attribute value.</param>
        public static void SetAttribute(this XmlNode node, string name, string value)
        {
            if (node != null)
            {
                var attribute = node.Attributes[name, node.NamespaceURI];

                if (attribute == null)
                {
                    attribute = node.OwnerDocument.CreateAttribute(name, node.OwnerDocument.NamespaceURI);
                    node.Attributes.Append(attribute);
                }

                attribute.InnerText = value;
            }
        }

        public static dynamic ToObject(this XmlDocument x)
        {
            return Instance.FromXml(x.ToXml(Encoding.ASCII));
        }

        public static bool HasAttribute(this XmlElement e, string attrName)
        {
            return e.Attributes.Cast<XmlAttribute>().Any(v => v.Name == attrName);
        }

        public static XmlAttribute GetAttribute(this XmlElement e, string attrName)
        {
            return e.Attributes.Cast<XmlAttribute>().FirstOrDefault(a => a.Name == attrName);
        }
    }
}
