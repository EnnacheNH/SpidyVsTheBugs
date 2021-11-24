using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class Credit : MonoBehaviour
{
    public Animator transitionAnim;
    private void Start()
    {
        Parameters.musicTime = 0f;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            transitionAnim.SetTrigger("Close");
            StartCoroutine(WaitLoadScene(0));
        }
    }

    //CoRoutines
    private IEnumerator WaitLoadScene(int _sceneBuildIndex)
    {
        yield return new WaitForSeconds(1.5f);
        SceneManager.LoadScene(_sceneBuildIndex);
    }
}