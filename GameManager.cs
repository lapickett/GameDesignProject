using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using System;
using System.Runtime.InteropServices;

/*
 * Author: Lucas Pickett
 * Date Created: Aug/08/2020
 * Date Modified: Aug/08/2020
 * Description: Manages basic game resources such as health, score, and outputs to UI elements in the game scene.
 */

public class GameManager : MonoBehaviour
{
    /*** Public Variables ***/
    
    #region GM Singleton
    public static GameManager gm;
    private void Awake()
    {
        if(gm != null)
        {
            Debug.LogWarning("More than one instance the game manager found!");
        }
        else 
        {
            gm = this;
            DontDestroyOnLoad(gm);
        }
    } // end Awake()
    #endregion

    public static int playerlives;
    public static int score;

    [Header("GENERAL SETTINGS")]
    public string gameTitle = "Untitled Game";
    public string gameCredits = "Made by Me";
    public string copyrightDate = "Copyright " + thisDay;

    [Space(10)]
    public TMP_Text titleDisplay;
    public TMP_Text creditsDisplay;
    public TMP_Text copyrightDisplay;

    [Header("GAME SETTINGS")]
    public GameObject player;
    public int defaultScore = 0;
    public int defaultLives = 1000;

    
    [Tooltip("Can the level be beat by a certain score")]
    public bool canBeatLevel = false;
    [Tooltip("What score must be achieved for a win")]
    public int beatLevelScore;
    public bool timedLevel = false;
    public float startTime = 5.0f;

    [Space(10)]
    public AudioSource backgroundMusicAudio;
    public AudioClip gameOverSFX;
    public AudioClip beatLevelSFX;
    public bool isBackgroundMusicOver;

    [HideInInspector]
    public enum gameStates { Playing, Death, GameOver, BeatLevel};
    public gameStates gameState = gameStates.Playing;
    public bool gameIsOver = false;
    public bool playerIsDead = false;

    [Header("MENU SETTINGS")]
    public GameObject MainMenuCanvas;
    public GameObject HUDCanvas;
    public GameObject EndScreenCanvas;
    public GameObject FooterCanvas;

    [Space(10)]
    public string scoreTitle = "Score: ";
    [HideInInspector]
    public TMP_Text scoreTitleDisplay;
    [HideInInspector]
    public TMP_Text scoreValueDisplay;

    [Space(10)]
    public string livesTitle = "Lives: ";
    [HideInInspector]
    public TMP_Text livesTitleDisplay;
    [HideInInspector]
    public TMP_Text livesValueDisplay;

    [Space(10)]
    public string timerTitle = "Timer: ";
    [HideInInspector]
    public TMP_Text timerTitleDisplay;
    [HideInInspector]
    public TMP_Text timerValueDisplay;

    [Space(10)]
    public string gameOver = "Game Over";
    [HideInInspector]
    public TMP_Text gameOverDisplay;
    public string loseMessage = "You Lose";
    public string winMessage = "You Win";
    [HideInInspector]
    public TMP_Text messageDisplay;

    public string firstLevel;
    public string nextLevel;
    public string levelToLoad;
    public string currentLevel;

    private float currentTime;
    private bool gameStarted = false;
    private static bool rePlay = false;
    private static string thisDay = System.DateTime.Now.ToString("yyyy");


    // Start is called before the first frame update
    void Start()
    {
        HideMenu();
        MainMenu();
        levelToLoad = firstLevel;
    }

    //reset the three displayed variables
    public void Reset()
    {
        if (timedLevel)
        {
            currentTime = startTime;
            timerValueDisplay.text = currentTime.ToString();
            timerTitleDisplay.text = timerTitle.ToString();
        }

        if (defaultScore != null)
        {
            score = defaultScore;
            scoreValueDisplay.text = score.ToString();
            scoreTitleDisplay.text = scoreTitle.ToString();
        }

        if (defaultLives != null)
        {
            playerlives = defaultLives;
            livesValueDisplay.text = playerlives.ToString();
            livesTitleDisplay.text = livesTitle.ToString();
        }
        PlayGame();
    }

    //makes all ui's invisible
    public void HideMenu()
    {
        if (MainMenuCanvas)
            MainMenuCanvas.SetActive(false);

        if (FooterCanvas)
            FooterCanvas.SetActive(false); 

        if(EndScreenCanvas)
            EndScreenCanvas.SetActive(false);

        if (HUDCanvas)
            HUDCanvas.SetActive(false);
    }

    //pops up main menu
    public void MainMenu()
    {
        if (defaultScore != null)
            score = defaultScore;

        if (defaultLives != null)
            playerlives = defaultLives;

        titleDisplay.text = gameTitle;
        creditsDisplay.text = gameCredits;
        copyrightDisplay.text = copyrightDate;

        if (MainMenuCanvas)
            MainMenuCanvas.SetActive(true);

        if (FooterCanvas)
            FooterCanvas.SetActive(true);
    }

    //called when first playing the game, or when resetting a level
    public void PlayGame()
    {
        if (MainMenuCanvas)
            MainMenuCanvas.SetActive(false);

        if (FooterCanvas)
            FooterCanvas.SetActive(false);

        if (HUDCanvas)
            HUDCanvas.SetActive(true);

        if (timedLevel)
        {
            timerValueDisplay.text = currentTime.ToString();
            timerTitleDisplay.text = timerTitle.ToString();
        }

        if (defaultScore != null)
        {
            scoreValueDisplay.text = score.ToString();
            scoreTitleDisplay.text = scoreTitle.ToString();
        }

        if (defaultLives != null)
        {
            livesValueDisplay.text = playerlives.ToString();
            livesTitleDisplay.text = livesTitle.ToString();
        }

        gameStarted = true;
        gameState = gameStates.Playing;
        playerIsDead = false;
        SceneManager.LoadScene(levelToLoad, LoadSceneMode.Additive);
        currentLevel = levelToLoad;
    }

    //called after death
    public void RestartLevel()
    {
        playerIsDead = false;
        SceneManager.UnloadSceneAsync(currentLevel);
        PlayGame();
    }

    //called after beating the level
    public void StartNextLevel(String lvl)
    {
        isBackgroundMusicOver = false;

        if (defaultLives != null)
            playerlives = defaultLives;

        SceneManager.UnloadScene(currentLevel);
        levelToLoad = lvl;
        PlayGame();
    }

    //called to restart the game
    public void RestartGame()
    {
        if (defaultScore != null)
            score = defaultScore;

        if (defaultLives != null)
            playerlives = defaultLives;

        SceneManager.UnloadScene(currentLevel);
        levelToLoad = firstLevel;
        PlayGame();

    }

    //quits application
    public void QuitGame()
    {
        Application.Quit();
    }

    // Update is called once per frame
    void Update()
    {
        //having some trouble with this, also have had a ton of trouble with buttons and just debugging them
        int a = Mathf.FloorToInt(Time.time * 1000);
        if (a % 100 == 0)
        {
            if (Input.GetKey("escape"))
                QuitGame();

            if (Input.GetKey("end"))
                gameState = gameStates.GameOver;

            if (Input.GetKey("delete"))
                playerIsDead = true;

            if (Input.GetKey("home"))
                gameState = gameStates.BeatLevel;
        }
        

        switch (gameState)
        {
            case gameStates.Playing:
                if (playerIsDead)
                {
                    if(playerlives > 0)
                    {
                        playerlives -= 1;
                        RestartLevel();
                    }
                    else
                    {
                        gameState = gameStates.Death;
                    }
                }
                break;
            case gameStates.Death:
                if (backgroundMusicAudio)
                {
                    backgroundMusicAudio.volume -= 0.01f;
                    if (backgroundMusicAudio.volume <= 0.0f)
                    {
                        if (gameOverSFX)
                        {
                            AudioSource.PlayClipAtPoint(gameOverSFX,
                            gameObject.transform.position);
                        }
                        messageDisplay.text = loseMessage;
                        gameState = gameStates.GameOver;
                    }
                }
                else
                {
                    if (gameOverSFX)
                    {
                        AudioSource.PlayClipAtPoint(gameOverSFX,
                        gameObject.transform.position);
                    }
                    messageDisplay.text = loseMessage;
                    gameState = gameStates.GameOver;
                }
                break;
            case gameStates.GameOver:
                if (player)
                    player.SetActive(false);

                if (HUDCanvas)
                    HUDCanvas.SetActive(false);

                if (EndScreenCanvas)
                {
                    EndScreenCanvas.SetActive(true);
                    gameOverDisplay.text = gameOver;
                }

                if(FooterCanvas)
                {
                    FooterCanvas.SetActive(true);
                }

                break;
            case gameStates.BeatLevel:
                if (backgroundMusicAudio)
                {
                    backgroundMusicAudio.volume -= 0.01f;
                    if (backgroundMusicAudio.volume <= 0.0f)
                    {
                        if (beatLevelSFX)
                        {
                            AudioSource.PlayClipAtPoint(beatLevelSFX,
                            gameObject.transform.position);
                        }
                        if (levelToLoad != null)
                        {
                            StartNextLevel(nextLevel);
                        }
                        else
                        {
                            messageDisplay.text = winMessage;
                            gameState = gameStates.GameOver;
                        }
                    }
                }
                else
                {
                    if (beatLevelSFX)
                    {
                        AudioSource.PlayClipAtPoint(beatLevelSFX,
                        gameObject.transform.position);
                    }
                    messageDisplay.text = winMessage;
                    gameState = gameStates.GameOver;
                }
                break;
        }
    }
}
