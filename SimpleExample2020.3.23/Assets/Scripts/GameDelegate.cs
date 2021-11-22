using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using SkillzSDK;

public class GameDelegate : ScriptableObject, SkillzMatchDelegate
{

    private static GameDelegate delegateInstance;
    private static readonly object Lock = new object();

    public static GameDelegate getInstance()
    {
        lock (Lock)
        {
            if (delegateInstance != null)
            {
                return delegateInstance;
            }
            else
            {
                delegateInstance = (GameDelegate)ScriptableObject.CreateInstance("GameDelegate");
                return delegateInstance;
            }
        }
    }


    void SkillzMatchDelegate.OnMatchWillBegin(Match matchInfo)
    {
        //Debug.Log("OnMatchWillBegin");
        SceneManager.LoadScene("GameScene");


        foreach (KeyValuePair<string, string> entry in matchInfo.GameParams)
        {
            // do something with entry.Value or entry.Key
            Debug.Log("Key: " + entry.Key + ", Value:  " + entry.Value);
        }


    }

    void SkillzMatchDelegate.OnSkillzWillExit()
    {
        //Debug.Log("OnSkillzWillExit");
        SceneManager.LoadScene("StartScene");
    }

    void SkillzMatchDelegate.OnProgressionRoomEnter() 
    {
        
    }

}
