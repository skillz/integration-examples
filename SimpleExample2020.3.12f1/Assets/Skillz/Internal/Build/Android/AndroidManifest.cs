#if UNITY_EDITOR && UNITY_ANDROID
using System.Xml;

namespace SkillzSDK.Internal.Build.Android
{
	/// <summary>
	/// Modifies and AndroidManifest.xml document.
	/// </summary>
	internal sealed class AndroidManifest : AndroidXmlDocument
	{
		private readonly XmlElement ManifestElement;
		private readonly XmlElement ApplicationElement;

		public AndroidManifest(string path)
			: base(path)
		{
			ManifestElement = DocumentElement;
			ApplicationElement = SelectSingleNode("/manifest/application") as XmlElement;
		}

		public XmlElement GetActivityWithLaunchIntent()
		{
			return (XmlElement)SelectSingleNode("/manifest/application/activity[intent-filter/action/@android:name='android.intent.action.MAIN' and " +
					"intent-filter/category/@android:name='android.intent.category.LAUNCHER']", NamespaceManager);
		}

		public void AddUsesFeature(string name, string value)
		{
			var preExisting = ManifestElement.UsesFeatureElements(name);
			foreach (var element in preExisting)
			{
				ManifestElement.RemoveChild(element);
			}

			var usesFeature = CreateElement(UsesFeatureElementName);
			usesFeature.Attributes.Append(CreateAndroidAttribute(name, value));

			ManifestElement.AppendChild(usesFeature);
		}

		public void AddMetadataElement(string name, string value)
		{
			var preExisting = ApplicationElement.MetadataElements(name);
			foreach (var element in preExisting)
			{
				ApplicationElement.RemoveChild(element);
			}

			var metadataElement = CreateElement(MetadataElementName);
			metadataElement.Attributes.Append(CreateAndroidAttribute("name", name));
			metadataElement.Attributes.Append(CreateAndroidAttribute("value", value));

			ApplicationElement.AppendChild(metadataElement);
		}

		public void AddUsesLibrary(string name, bool required)
		{
			var preExisting = ApplicationElement.UsesLibraryElements(name);
			foreach (var element in preExisting)
			{
				ApplicationElement.RemoveChild(element);
			}

			var usesLibrary = CreateElement(UsesLibraryElementName);
			usesLibrary.Attributes.Append(CreateAndroidAttribute("name", name));
			usesLibrary.Attributes.Append(CreateAndroidAttribute("required", required.ToString().ToLowerInvariant()));

			ApplicationElement.AppendChild(usesLibrary);
		}

		public void SetApplicationTheme(string appTheme)
		{
			var themeAttribute = ApplicationElement.Attribute("theme");
			if (themeAttribute != null)
			{
				themeAttribute.Value = appTheme;
				return;
			}

			ApplicationElement.Attributes.Append(CreateAndroidAttribute("theme", appTheme));
		}

		public void SetStartingActivityName(string activityName)
		{
			var nameAttribute = GetActivityWithLaunchIntent().Attribute("name");
			if (nameAttribute != null)
			{
				nameAttribute.Value = activityName;
				return;
			}

			GetActivityWithLaunchIntent().Attributes.Append(CreateAndroidAttribute("name", activityName));
		}

		public void SetSupportsScreens(bool small, bool normal, bool large, bool xLarge, bool anyDensity)
		{
			const string supportsScreens = "supports-screens";

			if (HasElement(supportsScreens))
			{
				ManifestElement.RemoveChild(SelectSingleNode($"manifest/{supportsScreens}"));
			}

			var element = CreateElement(supportsScreens);

			element.Attributes.Append(CreateAndroidAttribute("smallScreens", small.ToString().ToLowerInvariant()));
			element.Attributes.Append(CreateAndroidAttribute("normalScreens", normal.ToString().ToLowerInvariant()));
			element.Attributes.Append(CreateAndroidAttribute("largeScreens", large.ToString().ToLowerInvariant()));
			element.Attributes.Append(CreateAndroidAttribute("xlargeScreens", xLarge.ToString().ToLowerInvariant()));
			element.Attributes.Append(CreateAndroidAttribute("anyDensity", anyDensity.ToString().ToLowerInvariant()));

			ManifestElement.PrependChild(element);
		}

		public void SetLaunchMode(string value)
		{
			var launchModeAttribute = GetActivityWithLaunchIntent().Attribute("launchMode");
			if (launchModeAttribute != null)
			{
				launchModeAttribute.Value = value;
				return;
			}

			GetActivityWithLaunchIntent().Attributes.Append(CreateAndroidAttribute("launchMode", value));
		}

		public void SetClearTaskOnLaunch(string value)
		{
			var clearTaskOnLaunchAttribute = GetActivityWithLaunchIntent().Attribute("clearTaskOnLaunch");
			if (clearTaskOnLaunchAttribute != null)
			{
				clearTaskOnLaunchAttribute.Value = value;
				return;
			}

			GetActivityWithLaunchIntent().Attributes.Append(CreateAndroidAttribute("clearTaskOnLaunch", value));
		}

		public void SetAlwaysRetainTaskState(string value)
		{
			var alwaysRetainTaskStateAttribute = GetActivityWithLaunchIntent().Attribute("alwaysRetainTaskState");
			if (alwaysRetainTaskStateAttribute != null)
			{
				alwaysRetainTaskStateAttribute.Value = value;
				return;
			}

			GetActivityWithLaunchIntent().Attributes.Append(CreateAndroidAttribute("alwaysRetainTaskState", value));
		}


		internal void RemoveUsesSdkElement()
		{
			var preExisting = ManifestElement.UsesSdkElement();
			if (preExisting == null)
			{
				return;
			}

			ManifestElement.RemoveChild(preExisting);
		}

		private bool HasElement(string name)
		{
			return (SelectSingleNode($"manifest/{name}") as XmlElement) != null;
		}

		private XmlAttribute CreateAndroidAttribute(string key, string value)
		{
			XmlAttribute attr = CreateAttribute(AndroidAttributePrefix, key, AndroidXmlNamespace);
			attr.Value = value;
			return attr;
		}
	}
}
#endif