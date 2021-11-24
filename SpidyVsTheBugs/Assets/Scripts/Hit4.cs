using System.Collections;
using UnityEngine;

public class Hit4 : MonoBehaviour
{
    public float triggerPeriod;
    public float randomPercentage;
    public Animator animator;

    private bool isWaiting;
    private float randomRange;
    private float randomPeriod;

    void Start()
    {
        randomRange = triggerPeriod * randomPercentage;
        
        isWaiting = true;

        randomPeriod = Random.Range(0f, randomRange);
        //Debug.Log(randomPeriod);
        StartCoroutine(waitTrigger(randomPeriod));
    }

    void Update()
    {
        if (!isWaiting)
        {
            isWaiting = true;

            randomPeriod = Random.Range(-randomRange, randomRange);
            randomPeriod += triggerPeriod;
            //Debug.Log(randomPeriod);
            StartCoroutine(waitTrigger(randomPeriod));
        }
        
    }

    //Coroutine
    private IEnumerator waitTrigger(float _waitTime)
    {
        yield return new WaitForSeconds(_waitTime);
        isWaiting = false;

        animator.SetTrigger("Hit");
    }

}
