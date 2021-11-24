using System.Collections;
using UnityEngine;

public class BuggedDisplay : MonoBehaviour
{
    public SpriteRenderer spriteRendererNormal;
    public SpriteRenderer spriteRendererBugged;


    public float normalPeriod;
    public float buggedPeriod;
    public float randomPercentage;

    private float normalRandomRange;
    private float buggedRandomRange;
    private float randomPeriod;

    private bool isWaiting;
    private bool isBugged;

    void Start()
    {
        normalRandomRange = normalPeriod * randomPercentage;
        buggedRandomRange = buggedPeriod * randomPercentage;

        isWaiting = false;
        isBugged = false;
        
        spriteRendererNormal.enabled = !isBugged;
        spriteRendererBugged.enabled = isBugged;
    }

    void Update()
    {
        if (!isWaiting)
        {
            isWaiting = true;
            if (isBugged)
            {
                StartCoroutine(waitSwitch(normalPeriod, normalRandomRange));
            }
            else
            {
                StartCoroutine(waitSwitch(buggedPeriod, buggedRandomRange));
            }
        }
    }

    private IEnumerator waitSwitch(float _period, float _randomRange)
    {
        isBugged = !isBugged;
        spriteRendererNormal.enabled = !isBugged;
        spriteRendererBugged.enabled = isBugged;

        randomPeriod = Random.Range(-_randomRange, _randomRange);
        yield return new WaitForSeconds(_period + randomPeriod);
        
        isWaiting = false;
    }
}
