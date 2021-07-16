#if UNITY_EDITOR && UNITY_ANDROID
using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using System.Text;

namespace SkillzSDK.Internal.Build.Android
{
	internal sealed class BuildDotGradle : IDisposable
	{
		const string TargetSdkVersionTag = "targetSdkVersion";
		const string CompileSdkVersionTag = "compileSdkVersion";
		const string AndroidToolsPluginTag = "'com.android.tools.build:gradle:";

		private const string FileDependencyFormat = "implementation(name: '{0}', ext:'{1}')";
		private const string AAR = "aar";
		private const string JAR = "jar";

		private bool modified;

		private readonly string filePath;
		private readonly List<string> allLines;

		public BuildDotGradle(string filePath)
		{
			this.filePath = filePath;
			allLines = new List<string>();

			try
			{
				allLines = File.ReadAllLines(filePath).ToList();
			}
			catch (Exception ex)
			{
				Debug.LogWarning(ex);
			}
		}

		public void ChangeTargetSdkVersion(uint targetSdkVersion)
		{
			var index = allLines.FindIndex(line => line.Contains(TargetSdkVersionTag));
			if (index == -1)
			{
				return;
			}

			allLines[index] = TryReplaceSubstring(allLines[index], TargetSdkVersionTag, $"{TargetSdkVersionTag} {targetSdkVersion}", out var wasModified);
			modified |= wasModified;
		}

		public void ChangeCompileSdkVersion(uint compileSdkVersion)
		{
			var index = allLines.FindIndex(line => line.Contains(CompileSdkVersionTag));
			if (index == -1)
			{
				return;
			}

			allLines[index] = TryReplaceSubstring(allLines[index], CompileSdkVersionTag, $"{CompileSdkVersionTag} {compileSdkVersion}", out var wasModified);
			modified |= wasModified;
		}

		public void ChangeAndroidToolsPluginVersion(string version)
		{
			var index = allLines.FindIndex(line => line.Contains(AndroidToolsPluginTag));
			if (index == -1)
			{
				return;
			}

			// NOTE: This method will delete the end ' char, so we purposely add it back in.
			allLines[index] = TryReplaceSubstring(allLines[index], AndroidToolsPluginTag, $"{AndroidToolsPluginTag}{version}'", out var wasModified);
			modified |= wasModified;
		}

		public void ExcludeItemFromDependency(string item, string dependency, ExcludeType excludeType = ExcludeType.Group)
		{
			var index = allLines.FindIndex(line => line.Contains(dependency));
			if (index == -1)
			{
				return;
			}

			var excludeLabel = excludeType.ToString().ToLowerInvariant();

			Debug.Log(string.Format(Constants.LogFormat, $"Excluding {excludeLabel} '{item}' from dependency '{dependency}'"));

			// Wrap the fully qualified dependency in ()s - parenthesis
			var lineOriginalDependency = allLines[index];

			var iOpenQuote = lineOriginalDependency.IndexOf(dependency) - 1;
			var iClosingQuote = lineOriginalDependency.LastIndexOf("'");
			var quotedDependency = lineOriginalDependency.Substring(iOpenQuote, iClosingQuote - iOpenQuote + 1);

			allLines[index] = lineOriginalDependency.Replace(quotedDependency, $"({quotedDependency})");

			// Count the number of spaces to pad the left side with
			var numBeginSpaces = lineOriginalDependency.TakeWhile(character => char.IsWhiteSpace(character)).Count();
			var lineLeftPadding = string.Concat(Enumerable.Repeat(' ', numBeginSpaces));

			// Build out something similar to:
			// {
			//     exclude [group|module]: '<item>'
			// }
			var stringBuilder = new StringBuilder();
			stringBuilder.AppendLine($"{lineLeftPadding}{{");
			stringBuilder.AppendLine($"{lineLeftPadding}    exclude {excludeLabel}: '{item}'");
			stringBuilder.AppendLine($"{lineLeftPadding}}}");

			Debug.Log(string.Format(Constants.LogFormat, $"{allLines[index]}"));
			Debug.Log(string.Format(Constants.LogFormat, stringBuilder.ToString()));

			// Skip if the dependency already has exclusion semantics.
			if (allLines[index + 1].Trim() == "{" &&
				allLines[index + 2].Trim() == $"exclude {excludeLabel}: '{item}'" &&
				allLines[index + 3].Trim() == "}")
			{
				Debug.Log($"Dependency '{dependency}' already has exclusion semantics applied.");
				return;
			}

			allLines.InsertRange(index + 1, stringBuilder.ToString().Split('\n'));

			modified |= true;
		}

		public void RemoveMoatRepository()
		{
			var index = allLines.FindIndex(line => line.Contains("https://s3.amazonaws.com/moat-sdk-builds"));
			if (index == -1)
			{
				Debug.Log("The project does not reference the moat repository");
				return;
			}

			if (!allLines[index - 1].Trim().StartsWith("maven {"))
			{
				Debug.Log("The project does not reference the moat repository");
				return;
			}

			if (!allLines[index + 1].Trim().StartsWith("}"))
			{
				Debug.Log("The project does not reference the moat repository");
				return;
			}

			Debug.Log("Removing the moat repository");
			allLines.RemoveRange(index - 1, 3);

			modified |= true;
		}

		public void ExcludeAARDependency(string fullName)
		{
			ExcludeFileDependency(fullName, AAR);
		}

		public void FixFirebaseLocalRepoPath()
		{
			Debug.Log("Checking if the gradle is referencing a local Firebase repo...");

			const string wrongLocalRepo = "Assets/Firebase/m2repository";

			var index = allLines.FindIndex(0, line => line.Contains(wrongLocalRepo));
			if (index == -1)
			{
				Debug.Log("The project is not referencing a local Firebase repo.");
				return;
			}

			// The default folder in the Play Resolver is `Assets/GeneratedLocalRepo`.
			// Unfortunately, there doesn't seem to be a way to access this setting
			// programmatically, so assume the dev has kept it at the default value.
			const string defaultLocalRepo = "GeneratedLocalRepo";

			var theLine = allLines[index];
			allLines[index] = theLine.Replace(wrongLocalRepo, $"Assets/{defaultLocalRepo}/Firebase/m2repository");

			Debug.Log($"Patched the local Firebase repo to '{allLines[index]}'");

			modified |= true;
		}

		public void Dispose()
		{
			if (!modified)
			{
				return;
			}

			Debug.Log(string.Format(Constants.LogFormat, $"Saving '{filePath}'"));
			File.WriteAllLines(filePath, allLines);
		}

		private string TryReplaceSubstring(string gradleLine, string oldValue, string newValue, out bool modified)
		{
			modified = false;

			if (string.IsNullOrEmpty(gradleLine))
			{
				return gradleLine;
			}

			if (!gradleLine.Contains(oldValue))
			{
				return gradleLine;
			}

			var iStart = gradleLine.IndexOf(oldValue, StringComparison.InvariantCulture);

			var iTrimStart = iStart + oldValue.Length;
			iTrimStart = iTrimStart < gradleLine.Length ? iTrimStart : gradleLine.Length - 1;

			gradleLine = gradleLine.Remove(iTrimStart);

			Debug.Log(string.Format(Constants.LogFormat, $"'{oldValue}' -> '{newValue}'"));

			modified = true;

			return gradleLine.Replace(oldValue, newValue);
		}

		private void ExcludeFileDependency(string fullName, string extension)
		{
			var index = allLines.FindIndex(line => IsFileDependency(line, fullName, extension));
			if (index == -1)
			{
				return;
			}

			Debug.Log(string.Format(Constants.LogFormat, $"Removing the line '{string.Format(FileDependencyFormat, fullName, extension)}'"));
			allLines.RemoveAt(index);

			modified = true;
		}

		private bool IsFileDependency(string line, string fullName, string extension)
		{
			return string.Compare(line.Trim(), string.Format(FileDependencyFormat, fullName, extension), StringComparison.InvariantCulture) == 0;
		}
	}
}
#endif