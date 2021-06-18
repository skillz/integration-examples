using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using SkillzSDK;

public class GameController : MonoBehaviour
{
    public Button quitButton;

    // Start is called before the first frame update
    void Start()
    {
        quitButton.onClick.AddListener(OnQuitButton);

        
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void OnQuitButton()
    {
        if (Application.platform == RuntimePlatform.IPhonePlayer || Application.platform == RuntimePlatform.Android)
        {
            //
            SkillzCrossPlatform.ReportFinalScore(Random.Range(1, 53021));
        }
        else
        {
            SceneManager.LoadScene("StartScene");
        }
    }
}
