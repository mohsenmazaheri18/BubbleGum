using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
        public static GameManager Instance;

    public AudioSource audioSource;
    public AudioClip[] songs;
    public Animator animator;
    public GameObject buttonPrefab;
    public Transform buttonParent;

    public GameObject winObject;
    public GameObject gameMenuObject; // منوی ادامه یا خروج
    public TMP_Text scoreText; // نمایش امتیاز

    public float BPM = 120;
    public float beatInterval;
    public int currentSongIndex = 0;
    public int correctClicks = 0;
    public int totalButtons = 0;
    public bool gameOver = false;
    
    public int playerLives = 3; // تعداد جان‌ها
    public int score = 0; // امتیاز بازیکن
    public float elapsedTime = 0f; // زمان سپری‌شده

    void Awake()
    {
        if (Instance == null) Instance = this;
    }

    void Start()
    {
        beatInterval = 60f / BPM;
        PlaySong();
        StartCoroutine(ScoreTimer());
    }

    void Update()
    {
        if (!gameOver)
        {
            elapsedTime += Time.deltaTime;
        }
    }

    void PlaySong()
    {
        if (currentSongIndex < songs.Length)
        {
            audioSource.clip = songs[currentSongIndex];
            audioSource.Play();
            animator.SetBool("PlayAnimation", true);

            // اطمینان از شروع دوباره دکمه‌ها
            StartCoroutine(SpawnButtons());
            StartCoroutine(CheckSongCompletion());
        }
        else
        {
            gameOver = true;
        }
    }

    IEnumerator SpawnButtons()
    {
        while (!gameOver)
        {
            yield return new WaitForSeconds(beatInterval);
            SpawnButton();
        }
    }

    void SpawnButton()
    {
        Vector2 randomPosition = new Vector2(Random.Range(-4f, 4f), Random.Range(-3f, 3f));
        GameObject newButton = Instantiate(buttonPrefab, randomPosition, Quaternion.identity, buttonParent);
        totalButtons++;
        newButton.GetComponent<ButtonController>().Initialize();
    }

    IEnumerator CheckSongCompletion()
    {
        yield return new WaitForSeconds(audioSource.clip.length);
        if (!gameOver)
        {
            if (correctClicks == totalButtons)
            {
                WinGame();
            }
            else
            {
                LoseLife();
            }
        }
    }

    public void OnButtonClicked(GameObject button)
    {
        correctClicks++;
        score += 10; // افزایش امتیاز برای هر کلیک موفق
        UpdateScoreUI();
        Destroy(button);
    }

    public void CheckMissedButtons()
    {

            LoseLife();
        
    }

    void WinGame()
    {
        gameOver = true;
        audioSource.Stop();
        winObject.SetActive(true);
        animator.SetBool("PlayAnimation", false);
        // بعد از 2 ثانیه، به آهنگ بعدی برود
        Invoke("NextSong", 2f);
        
    }
    

    void LoseLife()
    {
        playerLives--;
        if (playerLives <= 0)
        {
            GameOver();
        }
        else
        {
            Invoke("NextSong", 2f);
        }
    }

    void GameOver()
    {
        gameOver = true;
        audioSource.Stop();
        gameMenuObject.SetActive(true);
    }

    void NextSong()
    {
        gameOver = false;
        if (currentSongIndex + 1 < songs.Length)
        {
            currentSongIndex++;
            correctClicks = 0;
            totalButtons = 0;
            winObject.SetActive(false);
            gameMenuObject.SetActive(false);
        
            // افزایش BPM
            BPM += 10;
            beatInterval = 60f / BPM;

            // حذف دکمه‌های قبلی
            foreach (Transform child in buttonParent)
            {
                Destroy(child.gameObject);
            }

            // اجرای آهنگ جدید
            PlaySong();
        }
    }

    IEnumerator ScoreTimer()
    {
        while (!gameOver)
        {
            yield return new WaitForSeconds(1f);
            score += 5; // افزایش امتیاز هر ثانیه
            UpdateScoreUI();
        }
    }

    void UpdateScoreUI()
    {
        if (scoreText != null)
        {
            scoreText.text = "Score: " + score;
        }
    }

    public void RestartGame()
    {
        playerLives = 3;
        currentSongIndex = 0;
        score = 0;
        elapsedTime = 0;
        gameMenuObject.SetActive(false);
        PlaySong();
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
