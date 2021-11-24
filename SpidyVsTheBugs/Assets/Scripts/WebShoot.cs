using System.Collections;
using UnityEngine;

public class WebShoot : MonoBehaviour
{
    public float lifeSpan;
    public float maxLenght;

    public Rigidbody2D rb;
    public Animator animator;

    //private Vector3 startPosition;
    private Vector2 travelDistance;

    private bool isWalled;

    // Start is called before the first frame update
    void Start()
    {
        //startPosition = transform.position;
        isWalled = false;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        //travelDistance = transform.position - startPosition;
        travelDistance = transform.position - SpidyMovement.instance.transform.position;
        if (travelDistance.magnitude > maxLenght && !isWalled)
        {
            SpidyMovement.instance.ResetWebShoot();
            Destroy(this.gameObject);
        }
        
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Wall") || collision.CompareTag("Enemy"))
        {
            isWalled = true;

            rb.velocity = Vector3.zero;
            animator.SetTrigger("Walled");

            SpidyMovement.instance.WebDash();
            StartCoroutine(WaitDestroy());
        }
    }

    //Coroutine
    private IEnumerator WaitDestroy()
    {
        yield return new WaitForSeconds(lifeSpan);
        Destroy(this.gameObject);
    }


}
