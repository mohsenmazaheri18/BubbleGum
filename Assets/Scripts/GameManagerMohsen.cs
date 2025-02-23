using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManagerMohsen : MonoBehaviour
{
    #region Private Function
    
    public static GameManagerMohsen instance;
    private List<GameObject> activeButtons = new List<GameObject>(); // دکمه‌های فعال در صحنه
    
    #endregion
    
    #region public function

    [Header("Dahan ha")]
    public GameObject dahanCharkhonObject;
    public GameObject dahanMamoliObject;

    [Header("Animation")]
    public Animator playerAnimation;
    public float speedAnimation;

    [Header("Songs")]
    public AudioSource music;
    public AudioClip[] songClip;
    public int currentSongIndex;
    
    [Header("Win And Lose")]
    public GameObject win;
    public GameObject lose;
    public GameObject finalWin;

    [Header("Buttons")]
    public GameObject buttonPrefab;
    public Transform buttonParents;
    public float beatSpeed;
    public int beatCounter;
    public int beatMax;
    public bool isCounterFinished;

    [Header("Scene Random Range")]
    public Vector2 minAndMaxRangeX;
    public Vector2 minAndMaxRangeY;

    [Header("Score")]
    public TMP_Text scoreGamePanel;
    public TMP_Text scoreLosePanel;
    public TMP_Text highScoreWin;
    public TMP_Text highScoreMain;

    public int highScore;
    public int score;

    [Header("BubbleGum")] 
    public GameObject[] bubbleGum;
    public int bubbleGumLeft;

    #endregion

    #region StartGame

    void Start()
    {

        Time.timeScale = 1f;
        if (PlayerPrefs.HasKey("HighScore"))
        {

            highScore = PlayerPrefs.GetInt("HighScore");
            highScoreWin.text = "HighScore: " + highScore;
            highScoreMain.text = "HighScore: " + highScore;
        }
        else
        {
            highScoreWin.text = "HighScore: " + 0;
            highScoreMain.text = "HighScore: " + 0;
        }

        //current index of song
        currentSongIndex = 0;
        
        //beat counter of button
        beatCounter = 0;
        
        //instance to other codes
        instance = this;
        
        //counter 
        isCounterFinished = false;
    }
    
        
    //start button clicked
    public void StartButton()
    {
        dahanCharkhonObject.SetActive(true);
        dahanMamoliObject.SetActive(false);

        playerAnimation.speed = speedAnimation;
        isCounterFinished = false;
        
        StartCoroutine(SpawnButtons());
        PlaySong();
    }


    #endregion

    #region Button Manager
    
    //spawn the button in random 
    IEnumerator SpawnButtons()
    {
        if (!isCounterFinished)
        {
            //wait and create button in beat index time
            yield return new WaitForSeconds(beatSpeed);
            Debug.Log("Create Button");

            //spawn the button in random range
            Vector2 randomPosition = new Vector2(Random.Range(minAndMaxRangeX.x, minAndMaxRangeX.y),
                Random.Range(minAndMaxRangeY.x, minAndMaxRangeY.y));
            GameObject newButton = Instantiate(buttonPrefab, randomPosition, Quaternion.identity, buttonParents);
            activeButtons.Add(newButton);
            newButton.GetComponent<ButtonController>().Initialize();

            //counter ++ for buttons
            beatCounter++;

            if (beatCounter >= beatMax)
            {
                isCounterFinished = true;
            }
            else
            {
                //reload the spawn
                StartCoroutine(SpawnButtons());
            }
        }
        else
        {
            
        }
    }
    
    //button when clicked in button controller scripts
    public void OnButtonClicked(GameObject button)
    {
        activeButtons.Remove(button);
        
        score += 10;
        scoreGamePanel.text = "Score: " + score;
        scoreLosePanel.text = "Score: " + score;

        Destroy(button);
    }
    
    //when button not clicked show game over in button controller scripts
    public void CheckMissedButtons()
    {
        if (activeButtons.Count > 0) // اگر دکمه‌ای در صحنه مانده باشد و کلیک نشده باشد، باخت رخ می‌دهد
        {
            if (bubbleGumLeft == 0)
            {
                StartCoroutine(GameOver());
            }
            else
            {
                bubbleGumLeft--;
                switch (bubbleGumLeft)
                {
                    case 0:
                        bubbleGum[2].SetActive(false);
                        break;
                    case 1:
                        bubbleGum[1].SetActive(false);
                        break;
                    case 2:
                        bubbleGum[0].SetActive(false);
                        break;
                }
            }
        }
    }
    
    #endregion
    
    #region Songs

    //play song if current song index exist in song clips
    void PlaySong()
    {
        if (currentSongIndex < songClip.Length)
        {
            music.clip = songClip[currentSongIndex];
            music.Play();

            playerAnimation.SetBool("PlayAnimation", true);
            
            //score +20 every music play
            score += 20;
            
            //texts = score
            scoreGamePanel.text = "Score: " + score;
            scoreLosePanel.text = "Score: " + score;

            StartCoroutine(CheckIfMusicEnded());
        }
        
        //when player all song wins show final win menu
        else
        {
            finalWin.SetActive(true);
            
            //save high Score and set in win and menu text 
            highScore = score;
            PlayerPrefs.SetInt("HighScore", highScore);
            PlayerPrefs.Save();
            
            //load high score to menu
            highScore = PlayerPrefs.GetInt("HighScore");
            highScoreWin.text = "HighScore: " + highScore;
            highScoreMain.text = "HighScore: " + highScore;
            
            
            currentSongIndex = 0;
            enabled = false;
        }
    }

    //if music ended go next music
    IEnumerator CheckIfMusicEnded()
    {
        yield return new WaitForSeconds(music.clip.length);
        Debug.Log("Music finished!");
        
        // next order when music ended
        playerAnimation.SetBool("PlayAnimation", false);
        speedAnimation += 0.1f;
        beatSpeed -= 0.02f;
        currentSongIndex++;
        StartCoroutine(WinPanel());
    }


    #endregion

    #region WinAndLose
    //win panel true
    IEnumerator WinPanel()
    {
        yield return new WaitForSeconds(0.7f);
        win.SetActive(true);
        StartCoroutine(winPanelfalse());
    }

    //win panel false and go next music
    IEnumerator winPanelfalse()
    {
        yield return new WaitForSeconds(1f);
        win.SetActive(false);
        beatCounter = 0;
        isCounterFinished = false;
        StartCoroutine(SpawnButtons());
        PlaySong();
    }
    
    //Game over panel
    IEnumerator GameOver()
    {
        yield return new WaitForSeconds(0.1f);
        music.Stop();
        lose.SetActive(true);
        
        //save high score when lose and set in menu
        highScore = score;
        PlayerPrefs.SetInt("HighScore", highScore);
        highScore = PlayerPrefs.GetInt("HighScore");
        highScoreMain.text = "HighScore: " + highScore;

        Time.timeScale = 0.1f;
        currentSongIndex = 0;
        beatSpeed = 0;
        isCounterFinished = true;
    }
    

    #endregion

    public void Restart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void exitGame()
    {
        Application.Quit();
    }

}
