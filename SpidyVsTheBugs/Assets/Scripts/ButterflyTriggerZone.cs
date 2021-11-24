using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButterflyTriggerZone : MonoBehaviour
{
    public ButterflyAI butterflyAI;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            butterflyAI.begingChase();
            Destroy(this.gameObject);
        }
    }
}
