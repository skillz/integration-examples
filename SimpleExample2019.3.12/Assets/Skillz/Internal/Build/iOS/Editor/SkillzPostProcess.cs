#if UNITY_EDITOR && UNITY_IOS
using UnityEngine;
using ActualUnityEditor = UnityEditor;
using System.IO;

namespace SkillzSDK.Internal.Build.iOS
{
	using ActualUnityEditor.SKZXCodeEditor;
	//Disable warnings about code blocks that will never be reached.
	#pragma warning disable 162, 429

	public static class SkillzPostProcessBuild
	{
		//The following public static fields can be modified for developers that want to automate their Unity builds.
		//Otherwise, some dialogs will appear at the end of every new/replace build.

		/// <summary>
		/// If this is true, then this class's static fields will be used instead of prompting the developer at build time.
		/// <summary>
		private const bool AutoBuild_On = false;

		/// <summary>
		/// Whether this is a portrait game. Only used if "AutoBuild_Use" is true.
		/// </summary>
		private const bool AutoBuild_IsPortrait = false;
		/// <summary>
		/// The full path of the "SkillzSDK-iOS.embeddedframework" folder that came with the downloaded Skillz SDK.
		/// Only used if "AutoBuild_Use" is true.
		/// </summary>
		private const string AutoBuild_SkillzPath = "/Users/myUsername/Downloads/sdk_ios_10.1.19/Skillz.framework";


		/// <summary>
		/// A file with this name is used to track whether a build is appending or replacing.
		/// </summary>
		private const string checkAppendFileName = ".skillzTouch";


		[ActualUnityEditor.Callbacks.PostProcessBuild(9090)]
		public static void OnPostProcessBuild(ActualUnityEditor.BuildTarget build, string path)
		{
			//Make sure this build is for iOS.
			//Unity 4 uses 'iPhone' for the enum value; Unity 5 changes it to 'iOS'.
			if (build.ToString() != "iPhone" && build.ToString() != "iOS")
			{
				UnityEngine.Debug.LogWarning("Skillz cannot be set up for a platform other than iOS.");
				return;
			}
			if (Application.platform != RuntimePlatform.OSXEditor)
			{
				UnityEngine.Debug.LogError("Skillz cannot be set up for XCode automatically on a platform other than OSX.");
				return;
			}

			//Get whether this is an append build by checking whether a custom file has already been created.
			//If it is, then nothing needs to be modified.
			string checkAppendFilePath = Path.Combine(path, checkAppendFileName);
			FileInfo checkAppend = new FileInfo(checkAppendFilePath);
			if (checkAppend.Exists)
			{
				return;
			}

			checkAppend.Create().Close();

			bool trySetUp = SetUpSDKFiles (path);
			if (!trySetUp)
			{
				//These failures aren't fatal; the developer can do them manually.
				UnityEngine.Debug.LogWarning("Skillz XCode export is missing Skillz Framework.");
			}

			//Set up XCode project settings.
			var xcodeProjectPath = Path.Combine(path, "Unity-iPhone.xcodeproj/project.pbxproj");
			Debug.Log($"Loading the XCode project at '{xcodeProjectPath}'");

			try
			{
				using (var xcodeProjectSettings = XcodeProjectSettings.Load(xcodeProjectPath))
				{
					xcodeProjectSettings.DisableBitcode();
					xcodeProjectSettings.AddRunScript();
					xcodeProjectSettings.ModifyMiscellaneous();
				}
			}
			catch (System.Exception)
			{
				UnityEngine.Debug.LogError("Skillz automated XCode editing failed!");
			}

			XCProject project = new XCProject (path);
			if (project != null) {
				project.AddFile(
					filePath: path + "/Skillz.framework",
					parent: project.GetGroup ("Embed Frameworks"),
					tree: "SOURCE_ROOT",
					createBuildFiles: true,
					weak: false,
					embed: true
				);

				//AddFile should also add FrameworkSearchPaths if required but doesn't
				project.AddFrameworkSearchPaths ("$(PROJECT_DIR)");
				project.AddOtherLDFlags ("-ObjC -lc++ -lz -lsqlite3 -lxml2 -weak_framework PassKit -framework Skillz");

				//Unity_4 doesn't exist so we check for Unity 5 defines.  Unity 6 is used for futureproofing.
#if !UNITY_5 && !UNITY_6
				project.AddFile(Path.Combine(Application.dataPath, "Skillz", "Internal", "Build", "iOS", "IncludeInXcode", "Skillz+Unity.mm"));
#endif
				project.Save ();
			} else {
				UnityEngine.Debug.LogError("Skillz automated XCode export failed!");
				return;
			}
		}

		private static bool SetUpSDKFiles(string projPath)
		{
			//Ask the user for the embeddedframework path if necessary.
			bool askForPath = true;
			string sdkPath = ".dummy";
			if (AutoBuild_On)
			{
				if (new DirectoryInfo(AutoBuild_SkillzPath).Exists)
				{
					askForPath = false;
					sdkPath = AutoBuild_SkillzPath;
				}
				else
				{
					ActualUnityEditor.EditorUtility.DisplayDialog("Skillz auto-build failed!",
					                            "Couldn't find the directory '" + AutoBuild_SkillzPath +
					                            	"'; please locate it manually in the following dialog.",
					                            "OK");
				}
			}
			while (askForPath && Path.GetFileName(sdkPath) != "Skillz.framework")
			{
				//If the user hit "cancel" on the dialog, quit out.
				if (sdkPath == "")
				{
					UnityEngine.Debug.Log("You canceled the auto-copying of the 'Skillz.framework'. " +
										  "You must copy it yourself into '" + projPath + "' before building the XCode project.");
					return true;
				}

				sdkPath = ActualUnityEditor.EditorUtility.OpenFolderPanel("Select the Skillz.framework file",
				                                        "", "");
			}

			//Copy the SDK files into the XCode project.
			try
			{
				DirectoryInfo newDir = new DirectoryInfo(Path.Combine(projPath, "Skillz.framework"));
				if (newDir.Exists)
				{
					newDir.Delete();
					newDir.Create();
				}
				if (!CopyFolder(new DirectoryInfo(sdkPath), newDir))
				{
					newDir.Delete();
					throw new IOException("Couldn't copy the .framework contents");
				}
			}
			catch (System.Exception e)
			{
				PrintSDKFileError(e, sdkPath, projPath);
				return false;
			}

			return true;
		}
		private static bool CopyFolder(DirectoryInfo oldPath, DirectoryInfo newPath)
		{
			if (!oldPath.Exists)
			{
				return false;
			}
			if (!newPath.Exists)
			{
				newPath.Create();
			}

			//Copy each file.
			foreach (FileInfo oldFile in oldPath.GetFiles())
			{
				oldFile.CopyTo(Path.Combine(newPath.FullName, oldFile.Name), true);
			}
			//Copy each subdirectory.
			foreach (DirectoryInfo oldSubDir in oldPath.GetDirectories())
			{
				DirectoryInfo newSubDir = newPath.CreateSubdirectory(oldSubDir.Name);
				if (!CopyFolder(oldSubDir, newSubDir))
				{
					return false;
				}
			}

			return true;
		}
		private static void PrintSDKFileError(System.Exception e, string sdkPath, string projPath)
		{
			string manualInstructions = "Failed to copy the Skillz SDK files. Please manually copy '" + sdkPath +
										"' to '" + projPath + "/'. If this error persists, please contact " +
										"integrations@skillz.com.\n\nError: " + e.Message;
			ActualUnityEditor.EditorUtility.DisplayDialog("Skillz SDK setup failed!", manualInstructions, "OK");
		}
	}

	//Restore the warnings that were disabled.
	#pragma warning restore 162, 429
}
#endif