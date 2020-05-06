using System;
using UnityEngine;

namespace SkillzSDK.Settings
{
	[Serializable]
	public sealed class StringKeyValue
	{
		[SerializeField]
		public string Key;

		[SerializeField]
		public string Value;
	}
}