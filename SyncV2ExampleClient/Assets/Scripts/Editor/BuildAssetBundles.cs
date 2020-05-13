#if UNITY_EDITOR
using System.IO;
using UnityEditor;
using UnityEngine;

/// <summary>
/// Command to build the AssetBundles for the in-Unity SDK scenes.
/// </summary>
public static class BuildAssetBundle
{
	private const string LogFormat = "[Skillz] {0}";
	private const string BundlesCreatedTitle = "AssetBundles Created";
	private const string OK = "OK";

	private static readonly string BundlesCreatedMessageFormat = "AssetBundles were created at '{0}'";
	private static readonly string AssetBundleFolder = Path.Combine("Assets", "Skillz", "Internal", "AssetBundles");

	[MenuItem("Assets/Build AssetBundles")]
#pragma warning disable IDE0051 // Remove unused private members
	private static void BuildAllAssetBundles()
#pragma warning restore IDE0051 // Remove unused private members
	{
		Debug.Log(string.Format(LogFormat, "Building AssetBundles for the in-Unity SDK scenes..."));

		Directory.CreateDirectory(AssetBundleFolder);

		BuildTargetAssetBundle(AssetBundleFolder, BuildTarget.StandaloneOSX | BuildTarget.StandaloneWindows64);

		EditorUtility.DisplayDialog(BundlesCreatedTitle, string.Format(BundlesCreatedMessageFormat, AssetBundleFolder), OK);
	}

	private static void BuildTargetAssetBundle(string outputFolder, BuildTarget targetPlatform)
	{
		Debug.Log(string.Format(LogFormat, $"Building AssetBundles for '{targetPlatform}' -> '{outputFolder}'"));
		BuildPipeline.BuildAssetBundles(outputFolder, BuildAssetBundleOptions.None, targetPlatform);
	}
}
#endif