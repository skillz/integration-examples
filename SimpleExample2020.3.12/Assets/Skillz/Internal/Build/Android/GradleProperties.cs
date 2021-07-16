using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

namespace SkillzSDK.Internal.Build.Android
{
	/// <summary>
	/// Opens and modifies a gradle.properties file.
	/// </summary>
	internal sealed class GradleProperties : IDisposable
	{
		private const char SeparatorChar = '=';

		public string SkillzMajorVersionValue
		{
			get
			{
				var index = fileContents.FindIndex(line => line.Contains(Constants.SkillzVersionVariable));
				if (index == -1)
				{
					Debug.LogWarning(string.Format(Constants.LogFormat, $"The local copy of gradle.properties does not define {Constants.SkillzVersionVariable}"));
					return string.Empty;
				}

				var subStrings = fileContents[index].Split(new[] { SeparatorChar }, StringSplitOptions.RemoveEmptyEntries);
				if (subStrings.Length != 2)
				{
					Debug.LogWarning(string.Format(Constants.LogFormat, $"The line containing {Constants.SkillzVersionVariable} does not appear to be formatted correctly:"));
					Debug.LogWarning(string.Format(Constants.LogFormat, fileContents[index]));
					return string.Empty;
				}

				return subStrings[1];
			}
		}

		public IReadOnlyList<string> FileContents
		{
			get
			{
				return fileContents;
			}
		}

		private bool modified;

		private readonly string filePath;
		private readonly List<string> fileContents;

		public GradleProperties(string filePath)
		{
			this.filePath = filePath;
			fileContents = new List<string>();

			try
			{
				fileContents = File.ReadAllLines(filePath).ToList();
			}
			catch (Exception ex)
			{
				Debug.LogError(ex);
			}
		}

		public void Dispose()
		{
			if (!modified)
			{
				return;
			}

			Debug.Log($"Saving '{filePath}'");
			File.WriteAllLines(filePath, fileContents);
		}

		public void Append(string line)
		{
			fileContents.Add(line);
			modified |= true;
		}

		public void SetAt(int index, string value)
		{
			fileContents[index] = value;
		}
	}
}