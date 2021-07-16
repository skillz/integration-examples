#if UNITY_EDITOR && UNITY_ANDROID
using System.Text;
using System.Xml;

namespace SkillzSDK.Internal.Build.Android
{
	/// <summary>
	/// Base class to open and save an Android XML document.
	/// </summary>
	internal class AndroidXmlDocument : XmlDocument
	{
		public const string AndroidXmlNamespace = "http://schemas.android.com/apk/res/android";
		public const string AndroidAttributePrefix = "android";

		public const string MetadataElementName = "meta-data";
		public const string UsesFeatureElementName = "uses-feature";
		public const string UsesLibraryElementName = "uses-library";
		public const string UsesSdkElementName = "uses-sdk";

		protected XmlNamespaceManager NamespaceManager
		{
			get;
			private set;
		}

		private readonly string documentPath;

		public AndroidXmlDocument(string path)
		{
			documentPath = path;
			using (var reader = new XmlTextReader(documentPath))
			{
				reader.Read();
				Load(reader);
			}

			NamespaceManager = new XmlNamespaceManager(NameTable);
			NamespaceManager.AddNamespace(AndroidAttributePrefix, AndroidXmlNamespace);
		}

		public string Save()
		{
			return SaveAs(documentPath);
		}

		public string SaveAs(string path)
		{
			using (var writer = new XmlTextWriter(path, new UTF8Encoding(false)))
			{
				writer.Formatting = Formatting.Indented;
				Save(writer);
			}

			return path;
		}
	}
}
#endif