using System.Collections;
using UnityEngine;

public class ButterflyAI : MonoBehaviour
{
    public float moveSpeed;
    public float hoveringPatternRadius;
    public float hoveringSpeed;
    public float randomPercentage;
    public float kitingDistance;
    public float targetHysteresis;
    public float attackCooldown;
    public float projectileSpeed;

    public GameObject projectilePrefab;

    private Vector2 spidyPosition;
    private Vector2 targetPosition;
    private Vector2 targetVector;
    private Vector2 movementVector;
    
    private Vector2 hoveringCenter;
    private Vector2 hoveringCircleCenter;

    private float currentAngle;
    private float newAngle;
    private Vector2 newCircleVector;
    private float randomRange;
    private float randomSpeed;

    private Vector2 aimingVector;
    private float aimingAngle;

    private bool isOnCircle1;
    [HideInInspector] public bool isChasing;
    private bool canAttack;

    void Start()
    {
        spidyPosition = SpidyMovement.instance.transform.position;
        hoveringCenter = transform.position;
        isOnCircle1 = true;

        currentAngle = -180f * Mathf.Deg2Rad;

        randomRange = hoveringSpeed * randomPercentage;

        isChasing = false;
        canAttack = false;
    }

    private void FixedUpdate()
    {
        //Move
        if (isChasing)
        {
            spidyPosition = SpidyMovement.instance.transform.position;
            targetPosition = spidyPosition + (hoveringCenter - spidyPosition).normalized * kitingDistance;

            targetVector = targetPosition - hoveringCenter;
            if (targetVector.magnitude > targetHysteresis || targetVector.magnitude < -targetHysteresis)
            {
                movementVector = targetVector.normalized * moveSpeed * Time.fixedDeltaTime;
                hoveringCenter += movementVector;
            }
        }
        
        if (isOnCircle1)
        {
            hoveringCircleCenter = new Vector2(hoveringCenter.x + hoveringPatternRadius, hoveringCenter.y);
            hoveringSpeed = Mathf.Abs(hoveringSpeed);

            if (currentAngle > 180f * Mathf.Deg2Rad)
            {
                isOnCircle1 = false;
                currentAngle = 0f;

                hoveringCircleCenter = new Vector2(hoveringCenter.x - hoveringPatternRadius, hoveringCenter.y);
                hoveringSpeed = -Mathf.Abs(hoveringSpeed);

            }
        }
        else
        {
            hoveringCircleCenter = new Vector2(hoveringCenter.x - hoveringPatternRadius, hoveringCenter.y);
            hoveringSpeed = -Mathf.Abs(hoveringSpeed);

            if (currentAngle < -360f * Mathf.Deg2Rad)
            {
                isOnCircle1 = true;
                currentAngle = -180f * Mathf.Deg2Rad;

                hoveringCircleCenter = new Vector2(hoveringCenter.x + hoveringPatternRadius, hoveringCenter.y);
                hoveringSpeed = Mathf.Abs(hoveringSpeed);
            }
        }

        randomSpeed = hoveringSpeed + Random.Range(-randomRange, randomRange);
        newAngle = currentAngle + (randomSpeed * Time.fixedDeltaTime);
        
        newCircleVector = new Vector2(Mathf.Cos(newAngle) * hoveringPatternRadius, Mathf.Sin(newAngle) * hoveringPatternRadius);
        transform.position = hoveringCircleCenter + newCircleVector;

        currentAngle = newAngle;

        //Attack
        if (canAttack)
        {
            canAttack = false;

            aimingVector = spidyPosition - hoveringCenter;
            aimingAngle = Mathf.Atan2(aimingVector.y, aimingVector.x) * Mathf.Rad2Deg;
            //Instantiate(projectilePrefab, transform.position, Quaternion.Euler(0f, 0f, aimingAngle));
            GameObject _projectileGO = Instantiate(projectilePrefab, transform.position, Quaternion.Euler(0f, 0f, aimingAngle));
            _projectileGO.GetComponent<Projectile>().InitiateProjectile(aimingVector, projectileSpeed);
            StartCoroutine(waitAttack(attackCooldown));
        }
    }

    private IEnumerator waitAttack(float _waitTime)
    {
        yield return new WaitForSeconds(_waitTime);
        canAttack = true;
    }

    public void begingChase()
    {
        isChasing = true;
        StartCoroutine(waitAttack(attackCooldown));
    }

    //Collider
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            GameMaster.instance.addKill();
            Destroy(this.gameObject);
        }
    }
}
