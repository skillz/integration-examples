using UnityEngine;
using UnityEngine.UI;
using SkillzSDK.Internal.API.UnityEditor;
using SkillzSDK.Settings;

namespace SkillzSDK
{
	public sealed class SecondsCountdown : MonoBehaviour
	{
		[SerializeField]
		private Text secondsText;

		private const int TotalSeconds = 3;

		private float timeRemaining;

		private void Awake()
		{
			timeRemaining = TotalSeconds;
			secondsText.text = timeRemaining.ToString("0");
		}

		private void Update()
		{
			if (timeRemaining < 0f)
			{
				return;
			}

			timeRemaining -= Time.deltaTime;
			secondsText.text = timeRemaining.ToString("0");

			if (timeRemaining < 0f)
			{
				var matchInfoJson = MatchInfoJson.Build(SkillzSettings.Instance.GameID);
				SkillzCrossPlatform.InitializeSimulatedMatch(matchInfoJson);
				SkillzState.NotifyMatchWillBegin(matchInfoJson);
			}
		}
	}
}