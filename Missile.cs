using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Missile : MonoBehaviour
{
    [Header("Damage")]
    public float hullDamage;
    public float hullPierce;
    public float shieldDamage;
    public float shieldPierce;

    [Header("Physics")]
    public float moveSpeed;
    public float turnSpeed;
    public float launchSpeed;
    public float timeToLive;
    public float secondsBeforeHoming;
    public LayerMask hitMask;

    [Header("Prefabs")]
    public TrailRenderer trailPrefab;
    public ParticleSystem impactFxPrefab;

    private bool shouldFollow = false;
    private float destructionCountdown = 0f;
    private Vector3 currentVelocity;
    private TrailRenderer missileTrail;
    private Transform currentTarget;
    private Vector3 lastKnownTargetPosition;
    private Rigidbody body;

    public void Fire(Vector3 firePoint, Transform target)
    {
        currentTarget = target;
        lastKnownTargetPosition = currentTarget.position;
        transform.position = firePoint;
        currentVelocity = transform.forward;
        destructionCountdown = Time.time + timeToLive;
    }

    private void Start()
    {
        body = GetComponent<Rigidbody>();
        missileTrail = Instantiate(trailPrefab, transform.position, transform.rotation, transform);
        missileTrail.enabled = false;
        StartCoroutine(WaitBeforeHoming());
    }

    private void FixedUpdate()
    {
        if (currentTarget != null)
        {
            lastKnownTargetPosition = currentTarget.position;
        }
        if (Time.time > destructionCountdown)
        {
            Die(transform.position, false);
        }
        else
        {
            //HitResponse hit = CheckHit();
            //if (hit.HasHit)
            //    Hit(hit.RayHit);
            //else
            Move();
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (shouldFollow)
        {
            GameObject recipient = collision.transform.gameObject;
            ShipController ship = recipient.GetComponent<ShipController>();
            if (ship != null)
            {
                ship.SufferDamage(hullDamage, hullPierce, shieldDamage, shieldPierce);
            }
            Die(transform.position, true);
        }
    }

    //private void Hit(RaycastHit hit)
    //{
    //    GameObject recipient = hit.transform.gameObject;
    //    Debug.Log("Hit " + recipient.name);
    //    Die(hit.point, true);
    //}

    private void Die(Vector3 position, bool isFromImpact)
    {
        if (isFromImpact && impactFxPrefab != null)
        {
            ParticleSystem impactFx = Instantiate(impactFxPrefab, position, transform.rotation);
            impactFx.Play();
        }
        Destroy(gameObject);
    }

    private void Move()
    {
        if (!shouldFollow)
        {
            body.velocity = transform.forward * launchSpeed;
        }
        else
        {
            body.velocity = transform.forward * moveSpeed;
            Quaternion rotationToTarget = Quaternion.LookRotation(lastKnownTargetPosition - transform.position);
            body.MoveRotation(Quaternion.RotateTowards(transform.rotation, rotationToTarget, turnSpeed));
        }
    }

    private IEnumerator WaitBeforeHoming()
    {
        yield return new WaitForSeconds(secondsBeforeHoming);
        shouldFollow = true;
        missileTrail.enabled = true;
        missileTrail.startWidth *= 10;
    }
}
