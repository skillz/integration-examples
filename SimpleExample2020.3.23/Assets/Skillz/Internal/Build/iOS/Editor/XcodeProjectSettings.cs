#if UNITY_EDITOR && UNITY_IOS
using System;
using ActualUnityEditor = UnityEditor;
using UnityEditor.iOS.Xcode;
using UnityEngine;

namespace SkillzSDK.Internal.Build.iOS
{
	/// <summary>
	/// Modifies the settings for an exported XCode project.
	/// </summary>
	internal abstract class XcodeProjectSettings : IDisposable
	{
		protected const string UnityIPhoneTarget = "Unity-iPhone";

		private const string LogFormat = "[SKillz] {0}";
		private const string EnableBitcodeSetting = "ENABLE_BITCODE";
		private const string No = "NO";
		private const string CoreImage = "CoreImage.framework";

		public static XcodeProjectSettings Load(string projectPath)
		{
#if UNITY_2019_3_OR_NEWER
			return new Unity2019Dot3Implementation(projectPath);
#else
			return new PreUnity2019Dot3Implementation(projectPath);
#endif
		}

		protected readonly PBXProject xcodeProject;
		private readonly string projectPath;

		protected XcodeProjectSettings(string projectPath)
		{
			this.projectPath = projectPath;

			xcodeProject = new PBXProject();
			try
			{
				xcodeProject.ReadFromFile(projectPath);
			}
			catch (System.Exception exception)
			{
				ActualUnityEditor.EditorUtility.DisplayDialog(
					"Error Opening the XCode Project",
					$"Error opening pbxproj file at '{projectPath}' for Skillz: {exception.Message}",
					"OK"
				);

				throw;
			}
		}

		public void Dispose()
		{
			Debug.Log(string.Format(LogFormat, $"Saving XCode project changes at '{projectPath}'"));
			xcodeProject.WriteToFile(projectPath);
		}

		public void DisableBitcode()
		{
			Debug.Log("Disabling Bitcode...");
			DisableBitcodeCore();
		}

		public void AddRunScript()
		{
			var shellScript = "if [ -e \"${BUILT_PRODUCTS_DIR}/${FRAMEWORKS_FOLDER_PATH}/Skillz.framework/postprocess.sh\" ]; then\n\t/bin/sh \"${BUILT_PRODUCTS_DIR}/${FRAMEWORKS_FOLDER_PATH}/Skillz.framework/postprocess.sh\"\nfi";

			AddRunScriptCore("Run Script", "/bin/sh", shellScript);
		}

		public void ModifyMiscellaneous()
		{
			Debug.Log(string.Format(LogFormat, "Modifying miscellaneous settings..."));
			ModifyMiscellaneousCore();
		}

		public void AddFrameworks()
		{
			Debug.Log(string.Format(LogFormat, "Adding any necessary frameworks..."));
			AddFrameworksCore();
		}

		protected abstract void DisableBitcodeCore();
		protected abstract void AddRunScriptCore(string buildPhaseName, string shellPath, string shellScript);
		protected abstract void ModifyMiscellaneousCore();
		protected abstract void AddFrameworksCore();

		/// <summary>
		/// Implementation for modifying XCode project settings for Unity 2019.2 and below.
		/// </summary>
		private sealed class PreUnity2019Dot3Implementation : XcodeProjectSettings
		{
			public PreUnity2019Dot3Implementation(string projectPath)
				: base(projectPath)
			{
			}

			protected override void DisableBitcodeCore()
			{
				xcodeProject.SetBuildProperty(xcodeProject.TargetGuidByName(UnityIPhoneTarget), EnableBitcodeSetting, No);
			}

			protected override void AddRunScriptCore(string buildPhaseName, string shellPath, string shellScript)
			{
				xcodeProject.AddShellScriptBuildPhase(
					xcodeProject.TargetGuidByName(UnityIPhoneTarget),
					buildPhaseName,
					shellPath,
					shellScript
				);
			}

			protected override void ModifyMiscellaneousCore()
			{
				// Do nothing
			}

			protected override void AddFrameworksCore()
			{
				// Do nothing
			}
		}

#if UNITY_2019_3_OR_NEWER
		/// <summary>
		/// Implementation for modifying XCode project settings for Unity 2019.3 and above.
		/// 2019.3 changed the XCode project structure, so this implementation is required
		/// in order to be able to get the Skillz iOS SDK to compile in an exported XCode project.
		/// See: https://docs.unity3d.com/Manual/StructureOfXcodeProject.html
		/// </summary>
		private sealed class Unity2019Dot3Implementation : XcodeProjectSettings
		{
			private const string UnityFrameworkTarget = "UnityFramework";

			public Unity2019Dot3Implementation(string projectPath)
				: base(projectPath)
			{
			}

			protected override void DisableBitcodeCore()
			{
				xcodeProject.SetBuildProperty(xcodeProject.GetUnityFrameworkTargetGuid(), EnableBitcodeSetting, No);
				xcodeProject.SetBuildProperty(xcodeProject.GetUnityMainTargetGuid(), EnableBitcodeSetting, No);
			}

			protected override void AddRunScriptCore(string buildPhaseName, string shellPath, string shellScript)
			{
				xcodeProject.AddShellScriptBuildPhase(
					xcodeProject.GetUnityMainTargetGuid(),
					buildPhaseName,
					shellPath,
					shellScript
				);
			}

			protected override void ModifyMiscellaneousCore()
			{
				Debug.Log(string.Format(LogFormat, "Adding UnityFramework.h to the header search paths..."));

				// Workaround for a 2019.3 bug where `main.mm` won't compile because
				// <UnityFramework/UnityFramework.h> was missing in the headers search path.
				xcodeProject.SetBuildProperty(xcodeProject.GetUnityMainTargetGuid(), "HEADER_SEARCH_PATHS", "$(SRCROOT)");

				Debug.Log(string.Format(LogFormat, "Setting validate workspace..."));

				// Workaround for a Xcode 12.3+ change that discourages the use of legacy fat frameworks 
				// in favor of XCFrameworks
				xcodeProject.SetBuildProperty(xcodeProject.GetUnityMainTargetGuid(), "VALIDATE_WORKSPACE", "YES");
			}

			protected override void AddFrameworksCore()
			{
				Debug.Log(string.Format(LogFormat, "Adding CoreImage framework to Unity..."));
				xcodeProject.AddFrameworkToProject(xcodeProject.GetUnityFrameworkTargetGuid(), CoreImage, true);
			}


		}
#endif
	}
}
#endif