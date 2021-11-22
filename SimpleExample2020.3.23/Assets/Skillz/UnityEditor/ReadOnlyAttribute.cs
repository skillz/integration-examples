using UnityEngine;

namespace SkillzSDK.UnityEditor
{
	/// <summary>
	/// Attach to fields that should not be modifiable from the Inspector pane.
	/// </summary>
	public sealed class ReadOnlyAttribute : PropertyAttribute
	{
	}
}