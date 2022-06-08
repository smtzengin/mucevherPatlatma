using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    public static UIManager instance;

    [SerializeField] TMP_Text timeRemainingTxt;

    public int timeRemaining = 5;

    [SerializeField] GameObject tourResoultPanel;

    public bool isTourFinished;

    [SerializeField] TMP_Text scoreTxt;

    [HideInInspector] public int validScore = 0;

    Board board;

    [SerializeField] GameObject pausePanel;

    public string mainMenu;

    [SerializeField] GameObject nextLevelPanel;

    [SerializeField] GameObject tryAgainPanel;

    public string nextLevelName;

    public string tryAgainLevelName;

    public void Start()
    {
        isTourFinished = false;
        StartCoroutine(CountDown());
    }
    public void Awake()
    {
        instance = this;
        board = Object.FindObjectOfType<Board>();
    }

    IEnumerator CountDown()
    {
        while(timeRemaining >= 0)
        {
            yield return new WaitForSeconds(1f);

            timeRemainingTxt.text = timeRemaining.ToString()+ " sn";
            timeRemaining--;
        }


        if(timeRemaining <= 0)
        {
            SoundManager.instance.GameOverSound();
            isTourFinished = true;  
            tourResoultPanel.SetActive(true);
            Scene scene1 = SceneManager.GetSceneByName("Level1");
            Scene scene2 = SceneManager.GetSceneByName("Level2");
            Scene scene3 = SceneManager.GetSceneByName("Level3");


            if (scene1 == SceneManager.GetSceneByName("Level1"))
            {
                if (validScore > 300)
                {
                    nextLevelPanel.SetActive(true);
                }
                else
                {
                    tryAgainPanel.SetActive(true);
                }
            }
            if (scene1 == SceneManager.GetSceneByName("Level2"))
            {
                if (validScore > 700)
                {
                    nextLevelPanel.SetActive(true);
                }
                else
                {
                    tryAgainPanel.SetActive(true);
                }
            }
            if (scene1 == SceneManager.GetSceneByName("Level3"))
            {
                if (validScore > 1000)
                {
                    nextLevelPanel.SetActive(true);
                }
                else
                {
                    tryAgainPanel.SetActive(true);
                }
            }

        }
    }

    public void IncreaseScore(int inComingScore)
    {
        validScore += inComingScore;

        scoreTxt.text = validScore.ToString() + " puan";

    }
    
    public void MixBoard()
    {
        board.MixTheBoard();
    }

    public void StartOrStopGame()
    {
        if (!pausePanel.activeInHierarchy)
        {
            pausePanel.SetActive(true);
            Time.timeScale = 0f;
        }
        else
        {
            Time.timeScale = 1f;
            pausePanel.SetActive(false);
        }
    }

    public void MainMenu()
    {
        SceneManager.LoadScene(mainMenu);
    }

    public void NextLevel()
    {
        SceneManager.LoadScene(nextLevelName);
    }

    public void TryAgain()
    {
        SceneManager.LoadScene(tryAgainLevelName);
    }

}
