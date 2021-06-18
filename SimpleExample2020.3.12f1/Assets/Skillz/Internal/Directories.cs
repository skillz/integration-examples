#if UNITY_EDITOR && UNITY_ANDROID

using System.IO;
using UnityEditor;
using UnityEngine;

internal static class Directories
{
	private static readonly string UnityDirectory = Path.GetDirectoryName(EditorApplication.applicationPath);

	private static readonly string UnityAssetsDirectory = Application.dataPath;

	public static readonly string UnityRootGradleDirectory = Path.Combine(UnityDirectory, Path.Combine("PlaybackEngines", Path.Combine("AndroidPlayer", Path.Combine("Tools", "gradle"))));

	public static readonly string UnityGradleLibDirectory = Path.Combine(UnityRootGradleDirectory, "lib");

	public static readonly string UnityGradlePluginsDirectory = Path.Combine(UnityGradleLibDirectory, "plugins");

	public static readonly string TempGradleDirectory = Path.Combine(UnityAssetsDirectory, Path.Combine("Skillz", Path.Combine("Build", Path.Combine("Editor", "OldGradleVersion"))));

	public static readonly string TempGradleLibDirectory = Path.Combine(TempGradleDirectory, "lib");

	public static readonly string TempGradlePluginsDirectory = Path.Combine(TempGradleLibDirectory, "plugins");

	public static readonly string SkillzLibDirectory = Path.Combine(UnityAssetsDirectory, Path.Combine("Skillz", Path.Combine("Build", Path.Combine("Editor", "lib"))));
}

#endif