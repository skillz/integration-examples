#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace SkillzSDK.UnityEditor
{
	// Pilfered from https://answers.unity.com/questions/489942/how-to-make-a-readonly-property-in-inspector.html
	[CustomPropertyDrawer(typeof(ReadOnlyAttribute))]
	public sealed class ReadOnlyPropertyDrawer : PropertyDrawer
	{
		private const int HeightKludgePadding = 100;

		public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
		{
			// Make a label word wrap its text if it's wider than the property pane's width
			var style = EditorStyles.label;
			style.wordWrap = true;
			style.clipping = TextClipping.Overflow;

			return style.CalcSize(label).y + HeightKludgePadding;
		}

		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
		{
			var style = EditorStyles.label;
			style.wordWrap = true;
			style.clipping = TextClipping.Overflow;

			EditorGUI.LabelField(position, label.text, GetPropertyStringValue(property), style);
		}

		private string GetPropertyStringValue(SerializedProperty property)
		{
			switch (property.propertyType)
			{
				case SerializedPropertyType.Integer:
					return property.intValue.ToString();

				case SerializedPropertyType.Boolean:
					return property.boolValue.ToString();

				case SerializedPropertyType.Float:
					return property.floatValue.ToString();

				case SerializedPropertyType.String:
					return property.stringValue;

				default:
					return string.Format("Property type {0} not supported", property.propertyType);
			}
		}
	}
}
#endif