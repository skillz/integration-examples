#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using SkillzSDK;
using SkillzSDK.Settings;

public sealed class SkillzSettingsWindow : EditorWindow
{
	private const int MaxKeyLength = 255;
	private const int MaxValueLength = 1000;

	private GUIStyle HorizontalLine
	{
		get
		{
			if (horizontalLine != null)
			{
				return horizontalLine;
			}

			horizontalLine = new GUIStyle();
			horizontalLine.normal.background = Texture2D.whiteTexture;
			horizontalLine.margin = new RectOffset(0, 0, 4, 4);
			horizontalLine.fixedHeight = 1;

			return horizontalLine;
		}
	}

	private bool showGameSettings;
	private bool showMockMatchResults;
	private bool showMockedMatchParameters;

	private GUIStyle horizontalLine;
	private GUIStyle wrappedLabelStyle;

	[MenuItem("Skillz/Settings")]
#pragma warning disable IDE0051 // Remove unused private members
	private static void Open()
#pragma warning restore IDE0051 // Remove unused private members
	{
		var window = GetWindow<SkillzSettingsWindow>();
		window.titleContent.text = "Skillz Settings";
		window.minSize = new Vector2(300, 500);
		window.showGameSettings = true;
		window.showMockMatchResults = true;
		window.showMockedMatchParameters = true;

		window.Show();
	}

	private void OnGUI()
	{
		InitializeStyles();
		DrawGameSettingsPane();
		DrawHorizontalLine();
		DrawMockMatchResultsPane();
		DrawHorizontalLine();
		DrawSimulatedMatchParamtersPane();

		SaveChanges();
	}

	private void InitializeStyles()
	{
		if (wrappedLabelStyle != null)
		{
			return;
		}

		wrappedLabelStyle = new GUIStyle(EditorStyles.label)
		{
			wordWrap = true,
			clipping = TextClipping.Overflow
		};
	}

	private void DrawGameSettingsPane()
	{
		showGameSettings = EditorGUILayout.Foldout(showGameSettings, "Game Settings");
		if (!showGameSettings)
		{
			return;
		}

		EditorGUI.indentLevel++;

		EditorGUILayout.LabelField("Settings for configuring your Skillz-based game.");
		EditorGUILayout.Space();

		SkillzSettings.Instance.GameID = EditorGUILayout.IntField("Game ID", SkillzSettings.Instance.GameID);
		SkillzSettings.Instance.Environment = (Environment)EditorGUILayout.EnumPopup("Skillz Environment", SkillzSettings.Instance.Environment);
		SkillzSettings.Instance.Orientation = (Orientation)EditorGUILayout.EnumPopup("Skillz Orientation", SkillzSettings.Instance.Orientation);
		SkillzSettings.Instance.AllowSkillzExit = EditorGUILayout.Toggle(
			new GUIContent(
				"Allow Skillz to Exit",
				"Allows the user to exit the Skillz UI via the sidebar menu."
			),
			SkillzSettings.Instance.AllowSkillzExit
		);
		SkillzSettings.Instance.HasSyncBot = EditorGUILayout.Toggle(
			new GUIContent(
				"Has Synchronous Bot",
				"Determines if the game has support for synchronous gameplay on-boarding bots."
			),
			SkillzSettings.Instance.HasSyncBot
		);

		EditorGUI.indentLevel--;
	}

	private void DrawHorizontalLine()
	{
		var prevColor = GUI.color;
		GUI.color = Color.grey;

		GUILayout.Box(GUIContent.none, HorizontalLine);

		GUI.color = prevColor;
	}

	private void DrawMockMatchResultsPane()
	{
		showMockMatchResults = EditorGUILayout.Foldout(showMockMatchResults, "Simulated Match Results");
		if (!showMockMatchResults)
		{
			return;
		}

		EditorGUI.indentLevel++;

		EditorGUILayout.LabelField("Simulate the results of a match when running your game within the Unity editor.", wrappedLabelStyle);
		EditorGUILayout.Space();

		EditorGUILayout.BeginHorizontal();

		EditorGUIUtility.labelWidth = 100;
		SkillzSettings.Instance.SimulateMatchWins = EditorGUILayout.Toggle("Win the match", SkillzSettings.Instance.SimulateMatchWins);

		EditorGUILayout.EndHorizontal();

		EditorGUI.indentLevel--;
	}

	private void DrawSimulatedMatchParamtersPane()
	{
		showMockedMatchParameters = EditorGUILayout.Foldout(showMockedMatchParameters, "Simulated Match Parameters");
		if (!showMockedMatchParameters)
		{
			return;
		}

		EditorGUI.indentLevel++;

		EditorGUILayout.LabelField("Simulate match parameters when running your game within the Unity editor.", wrappedLabelStyle);
		EditorGUILayout.Space();

		for (var i = 0; i < SkillzSettings.MaxMatchParameters; i++)
		{
			DrawKeyValueField(i);
		}

		EditorGUI.indentLevel--;
	}

	private void SaveChanges()
	{
		if (!GUI.changed)
		{
			return;
		}

		Debug.Log("Saving Skillz settings...");
		EditorUtility.SetDirty(SkillzSettings.Instance);
		AssetDatabase.SaveAssets();
	}

	private void DrawKeyValueField(int index)
	{
		var textStyle = new GUIStyle(EditorStyles.textField);
		textStyle.alignment = TextAnchor.MiddleLeft;
		textStyle.stretchWidth = true;

		EditorGUILayout.BeginHorizontal();

		EditorGUIUtility.labelWidth = 65;

		SkillzSettings.Instance.MatchParameters[index].Key = CoerceNumChars(EditorGUILayout.TextField($"Key {index + 1}", SkillzSettings.Instance.MatchParameters[index].Key ?? string.Empty, textStyle), MaxKeyLength);
		SkillzSettings.Instance.MatchParameters[index].Value = CoerceNumChars(EditorGUILayout.TextField($"Value {index + 1}", SkillzSettings.Instance.MatchParameters[index].Value ?? string.Empty, textStyle), MaxValueLength);

		EditorGUILayout.EndHorizontal();
	}

	private static string CoerceNumChars(string value, uint maxChars)
	{
		return value.Length > maxChars ? value.Substring(0, (int)maxChars) : value;
	}
}
#endif