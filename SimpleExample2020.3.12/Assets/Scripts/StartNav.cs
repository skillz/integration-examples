using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using SkillzSDK;

public class StartNav : MonoBehaviour
{

    public Button playButton;

    // Start is called before the first frame update
    void Start()
    {
        playButton.onClick.AddListener(OnPlayButton);
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void OnPlayButton()
    {
        if (Application.platform == RuntimePlatform.IPhonePlayer || Application.platform == RuntimePlatform.Android)
        {
            Debug.Log("OnPlayButton call SkillzCrossPlatform.LaunchSkillz");
            GameDelegate gameDelegate = GameDelegate.getInstance();
            SkillzCrossPlatform.LaunchSkillz(gameDelegate);
        }
        else
        {
            SceneManager.LoadScene("GameScene");
        }
    }
}
