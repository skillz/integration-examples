#if UNITY_EDITOR && UNITY_ANDROID
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;

namespace SkillzSDK.Internal.Build.Android
{
	internal static class AndroidXmlDocumentExtensions
	{
		public static XmlAttribute Attribute(this XmlElement node, string localName)
		{
			return node.Attributes
				.OfType<XmlAttribute>()
				.FirstOrDefault(attribute =>
					string.Compare(attribute.LocalName, localName, StringComparison.InvariantCulture) == 0 &&
					string.Compare(attribute.Prefix, AndroidXmlDocument.AndroidAttributePrefix, StringComparison.InvariantCulture) == 0 &&
					string.Compare(attribute.NamespaceURI, AndroidXmlDocument.AndroidXmlNamespace, StringComparison.InvariantCulture) == 0
				);
		}

		public static IEnumerable<XmlElement> MetadataElements(this XmlElement node, string name)
		{
			return node.ChildNodes
				.OfType<XmlElement>()
				.Where(element => string.Compare(element.LocalName, AndroidXmlDocument.MetadataElementName) == 0)
				.Where(metadataElement => metadataElement.Attribute("name", name) != null);
		}

		public static IEnumerable<XmlElement> UsesFeatureElements(this XmlElement node, string name)
		{
			return node.ChildNodes
				.OfType<XmlElement>()
				.Where(element => string.Compare(element.LocalName, AndroidXmlDocument.UsesFeatureElementName) == 0)
				.Where(usesFeatureElement => usesFeatureElement.Attribute("name", name) != null);
		}

		public static IEnumerable<XmlElement> UsesLibraryElements(this XmlElement node, string name)
		{
			return node.ChildNodes
				.OfType<XmlElement>()
				.Where(element => string.Compare(element.LocalName, AndroidXmlDocument.UsesLibraryElementName) == 0)
				.Where(usesLibraryElement => usesLibraryElement.Attribute("name", name) != null);
		}

		public static XmlElement UsesSdkElement(this XmlElement node)
		{
			return node.ChildNodes
				.OfType<XmlElement>()
				.FirstOrDefault(element => string.Compare(element.LocalName, AndroidXmlDocument.UsesSdkElementName) == 0);
		}

		private static XmlAttribute Attribute(this XmlElement node, string localName, string value)
		{
			var xmlAttribute = Attribute(node, localName);
			if (xmlAttribute == null)
			{
				return null;
			}

			return string.Compare(xmlAttribute.Value, value, StringComparison.InvariantCulture) == 0
				? xmlAttribute
				: null;
		}
	}
}
#endif