using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;
using System.Collections;
using UnityEngine.SceneManagement;

public class MenuMaster : MonoBehaviour
{
    public Animator transitionAnim;
    public GameObject optionPanel;
    public AudioSource audioSource;
    public GameObject[] tutorialPanels;

    private int tutoIndex;

    public static MenuMaster instance;
    private void Awake()
    {
        if (instance != null)
        {
            Debug.LogWarning("Il y a plus d'une instance de MenuMaster dans la scène");
            return;
        }
        instance = this;
    }

    private void Start()
    {
        PlayMusic();
        tutoIndex = 0;
    }

    public void LoadNextLevel()
    {
        transitionAnim.SetTrigger("Close");
        StartCoroutine(WaitLoadScene(SceneManager.GetActiveScene().buildIndex + 1));
    }

    public void OpenOption()
    {
        optionPanel.SetActive(true);
    }

    public void QuitGame()
    {
        Application.Quit();
    }
    private void PlayMusic()
    {
        audioSource.time = Parameters.musicTime;
        audioSource.Play();
    }

    public void NextTutorial()
    {
        if (tutoIndex > 2)
        {
            for (int i = 0; i < tutorialPanels.Length; i++)
            {
                tutorialPanels[i].SetActive(false);
            }
            tutoIndex = 0;
            return;
        }
        else
        {
            tutorialPanels[tutoIndex].SetActive(true);
            tutoIndex++;
        }
    }

    public void BackTutorial()
    {
        tutoIndex--;
        tutorialPanels[tutoIndex].SetActive(false);
    }

    //CoRoutines
    private IEnumerator WaitLoadScene(int _sceneBuildIndex)
    {
        yield return new WaitForSeconds(1.5f);

        Parameters.musicTime = audioSource.time;
        SceneManager.LoadScene(_sceneBuildIndex);
    }
}
