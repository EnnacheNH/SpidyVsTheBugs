using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;
using System.Collections;
using UnityEngine.SceneManagement;
public class GameMaster : MonoBehaviour
{
    public Text timerValueText;
    public GameObject pausePanel;
    public Animator transitionAnim;
    public Animator countdownAnim;
    public Animator ObjectiveAnim;
    public Text countdownText;
    public int enemyCount;
    public float[] timeStars;
    public GameObject winPanel;
    public GameObject losePanel;
    public GameObject optionPanel;
    public AudioSource audioSource;

    [HideInInspector]
    public float timerValue;
    private string[] timerValueStrings;
    private string timerValueString2;

    private int countdownCount;

    private bool isPaused;
    private bool isCountingDown;
    private int killCount;


    public static GameMaster instance;
    private void Awake()
    {
        if (instance != null)
        {
            Debug.LogWarning("Il y a plus d'une instance de GameMaster dans la scène");
            return;
        }
        instance = this;
    }

    void Start()
    {
        isPaused = false;
        timerValue = 0f;
        killCount = 0;

        //Countdown
        SpidyMovement.instance.isPaused = true;
        isCountingDown = true;
        countdownCount = 3;
        countdownText.text = "";
        StartCoroutine(StartScene());

        //Play Music
        PlayMusic();
    }
    private void FixedUpdate()
    {
        if (!isCountingDown)
        {
            timerValue += Time.fixedDeltaTime;
            timerValueStrings = timerValue.ToString().Split('.');
            if (timerValueStrings[1].Length >= 2)
            {
                timerValueString2 = timerValueStrings[1].Substring(0, 2);
            }

            timerValueText.text = timerValueStrings[0] + "." + timerValueString2 + "   s";
        }
    }
    private void Update()
    {
        if (!isCountingDown)
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                if (!isPaused)
                {
                    PauseGame();
                }
                else
                {
                    ResumeGame();
                }
            }
        }
        //Time SLOW
        if (Input.GetKeyDown(KeyCode.H))
        {
            //Time.timeScale = 0.2f;
        }
    }

    public void addKill()
    {
        killCount++;
        if (killCount >= enemyCount)
        {
            isCountingDown = true;
            winPanel.SetActive(true);
        }
    }

    public void Lose()
    {
        isCountingDown = true;
        SpidyMovement.instance.isPaused = true;
        losePanel.SetActive(true);
    }

    //Panels
    void PauseGame()
    {
        isPaused = true;
        Time.timeScale = 0;

        SpidyMovement.instance.isPaused = isPaused;
        pausePanel.SetActive(true);
        /*
        pausePanel.SetActive(true);
        playerMovement.isGamePaused = true;
        shooting.isGamePaused = true;
        audioManager.isStopped = true;
        audioManager.musicAudioSource.Pause();
        */
    }
    public void ResumeGame()
    {
        isPaused = false;
        Time.timeScale = 1;

        SpidyMovement.instance.isPaused = isPaused;
        pausePanel.SetActive(false);
        /*
        pausePanel.SetActive(false);
        playerMovement.isGamePaused = false;
        shooting.isGamePaused = false;
        audioManager.isStopped = false;
        audioManager.musicAudioSource.UnPause();
        */
    }

    public void OpenOption()
    {
        optionPanel.SetActive(true);
    }

    public void LoadNextLevel()
    {
        transitionAnim.SetTrigger("Close");
        StartCoroutine(WaitLoadScene(SceneManager.GetActiveScene().buildIndex + 1));
    }

    public void LoadMainMenu()
    {
        Time.timeScale = 1;
        transitionAnim.SetTrigger("Close");
        StartCoroutine(WaitLoadScene(0));
    }

    public void RetryLevel()
    {
        Time.timeScale = 1;
        transitionAnim.SetTrigger("Close");
        StartCoroutine(WaitLoadScene(SceneManager.GetActiveScene().buildIndex));

    }

    private void PlayMusic()
    {
        audioSource.time = Parameters.musicTime;
        audioSource.Play();
    }

    //CoRoutines
    private IEnumerator StartScene()
    {
        yield return new WaitForSeconds(1.5f);
        countdownText.text = countdownCount.ToString();
        countdownAnim.SetTrigger("Count");
        countdownCount--;
        StartCoroutine(WaitCountdown());
    }
    private IEnumerator WaitCountdown()
    {
        yield return new WaitForSeconds(1f);
        if (countdownCount > 0)
        {
            countdownText.text = countdownCount.ToString();
            countdownAnim.SetTrigger("Count");
            countdownCount--;
            StartCoroutine(WaitCountdown());
        }
        else
        {
            countdownText.text = "GO!";
            countdownAnim.SetTrigger("Count");

            isCountingDown = false;
            SpidyMovement.instance.isPaused = false;
            StartCoroutine(FinishCountdown());
        }
    }

    private IEnumerator FinishCountdown()
    {
        yield return new WaitForSeconds(0.5f);
        //isCountingDown = false;
        //SpidyMovement.instance.isPaused = false;
        ObjectiveAnim.SetTrigger("Finish");
    }

    private IEnumerator WaitLoadScene(int _sceneBuildIndex)
    {
        yield return new WaitForSeconds(1.5f);

        Parameters.musicTime = audioSource.time;
        SceneManager.LoadScene(_sceneBuildIndex);
    }
}
