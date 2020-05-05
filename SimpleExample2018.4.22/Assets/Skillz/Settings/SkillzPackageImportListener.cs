#if UNITY_EDITOR
using System;
using UnityEditor;
using UnityEngine;

namespace SkillzSDK.Settings
{
	public static class SkillzPackageImportListener
	{
		private const string LogFormat = "[Skillz] {0}";
		private const string SkillzPackagePrefix = "skillz";

		[InitializeOnLoadMethod]
#pragma warning disable IDE0051 // Remove unused private members
		private static void OnProjectLoad()
#pragma warning restore IDE0051 // Remove unused private members
		{
			Debug.Log(string.Format(LogFormat, "Project loaded. Listening for import of Skillz."));
			AssetDatabase.importPackageCompleted += OnImportPackageCompleted;
		}

		private static void OnImportPackageCompleted(string packageName)
		{
			if (!IsPackageSkillz(packageName))
			{
				return;
			}

			AssetDatabase.importPackageCompleted -= OnImportPackageCompleted;

			Debug.Log(string.Format(LogFormat, "Skillz package was imported. Generating files required for Android..."));
			AndroidFilesGenerator.GenerateFiles();
		}

		private static bool IsPackageSkillz(string packageName)
		{
			return packageName.StartsWith(SkillzPackagePrefix, StringComparison.InvariantCultureIgnoreCase);
		}
	}
}
#endif