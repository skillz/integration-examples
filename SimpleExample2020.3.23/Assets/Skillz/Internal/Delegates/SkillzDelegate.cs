using SkillzSDK;
using UnityEngine;

public sealed class SkillzDelegate : MonoBehaviour
{
	/// <summary>
	/// This method is called when a user starts a match from Skillz
	/// This method is required to impelement.
	/// </summary>
	private void OnMatchWillBegin(string matchInfoJsonString)
	{
		SkillzState.NotifyMatchWillBegin(matchInfoJsonString);
	}

	/// <summary>
	/// This method is called when a user exits the Skillz experience (via Menu -> Exit)
	/// This method is optional to impelement. This method is usually used only if your game has its own Main Menu.
	/// </summary>
	private void OnSkillzWillExit()
	{
		SkillzState.NotifySkillzWillExit();
	}

	/// <summary>
	/// This method is called when a user enters the Progression Room (via Menu -> Progression Room)
	/// This method is optional to implement. This method should only be implemented if your game has
	/// own player progression experience.
	/// </summary>
	private void OnProgressionRoomEnter()
	{
		SkillzState.NotifyProgressionRoomEnter();
	}
}