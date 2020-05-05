using System;
using SkillzSDK;

/// <summary>
/// You should implement this interface as part of one of your Game Objects. 
/// The instance of this object should persist for the lifetime of your game, once you've launched Skillz.
/// </summary>
public interface SkillzMatchDelegate
{
    /// <summary>
    /// This method is called when a user starts a match from Skillz
    /// This method is required to impelement.
    /// </summary>
    void OnMatchWillBegin(Match matchInfo);

    /// <summary>
    /// This method is called when a user exits the Skillz experience (via Menu -> Exit)
    /// This method is optional to impelement. This method is usually used only if your game has its own Main Menu.
    /// </summary>
    void OnSkillzWillExit();
}

