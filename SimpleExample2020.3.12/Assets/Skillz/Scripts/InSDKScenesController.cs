using UnityEngine;
using SkillzSDK.Internal;

namespace SkillzSDK
{
	public sealed class InSDKScenesController : MonoBehaviour
	{
		public void LoadTournamentSelectionScene()
		{
			SDKScenesLoader.Load(SDKScenesLoader.TournamentSelectionScene);
		}

		public void LoadEnteringMatchScene()
		{
			SDKScenesLoader.Load(SDKScenesLoader.EnteringMatchScene);
		}
	}
}