using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using SkillzSDK;

public class GameController : MonoBehaviour
{
    private int retrySeconds = 2;
    private float score = 0;
    private int retryCount = 0;
    private int maxRetries = 2;

    public Button quitButton;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void OnQuitButton()
    {
        // Generate random score
        score = Random.Range(0.0f, 10000.0f);
#if UNITY_IOS || UNITY_ANDROID  
        TryToSubmitScore(score);
#else
        SceneManager.LoadScene("StartScene");
#endif        
    }

    void TryToSubmitScore(float score) 
    {
        SkillzCrossPlatform.SubmitScore(score, OnSuccess, OnFailure);
    }

    void OnSuccess() {
        Debug.Log("**** Success ****");
        ResetRetryCount();
        SkillzCrossPlatform.ReturnToSkillz();
    }

    void OnFailure(string reason) {
        Debug.LogWarning("**** Fail: " + reason);
        retryCount += 1;
        StartCoroutine(RetrySubmit());
    }

    IEnumerator RetrySubmit() {
        yield return new WaitForSeconds(retrySeconds);
        if (retryCount <= maxRetries)
        {
            TryToSubmitScore(score);
        } 
        else 
        {
            ResetRetryCount();
            SkillzCrossPlatform.DisplayTournamentResultsWithScore(score);
        }
    }

    void ResetRetryCount() {
        retryCount = 0;
    }
}
