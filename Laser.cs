using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VolumetricLines;


public class Laser : MonoBehaviour
{
    [Header("Damage")]
    [Range(0, 500)] public float hullDamage;
    [Range(0, 50)] public float hullPierce;
    [Range(0, 500)] public float shieldDamage;
    [Range(0, 50)] public float shieldPierce;


    [Header("Physics")]
    public bool alignToVelocity;
    public float inherentDeviation;
    public float timeToLive;
    public float gravityModifier;
    public LayerMask hitMask;

    [Header("Effects")]
    public Color laserColor;
    public AudioSource[] fireSounds;

    [Header("Prefabs")]
    public GameObject laserPrefab;
    public ParticleSystem impactFxPrefab;

    private bool isActive;
    private float destructionCountdown = 0f;
    private Vector3 currentVelocity = Vector3.forward;

    private void Start()
    {
        _ = Instantiate(laserPrefab, transform.position, transform.rotation, transform);
    }

    public void Fire(Vector3 point, Quaternion initialRotation, Vector3 initialVelocity, float muzzleVelocity, float deviation)
    {
        // Calculate random deviations
        Vector3 angleDeviation = Vector3.zero;
        angleDeviation.x = Random.Range(-deviation, deviation) + Random.Range(-inherentDeviation, inherentDeviation);
        angleDeviation.y = Random.Range(-deviation, deviation) + Random.Range(-inherentDeviation, inherentDeviation);
        Quaternion rotationDeviation = Quaternion.Euler(angleDeviation);

        // Initial position + rotation
        transform.position = point;
        transform.rotation = initialRotation * rotationDeviation;

        currentVelocity = (transform.forward * muzzleVelocity) + initialVelocity;
        destructionCountdown = Time.time + timeToLive;
        isActive = true;
    }

    private void FixedUpdate()
    {
        if (!isActive) return;

        if (Time.time > destructionCountdown)
        {
            Die(transform.position, false);
        }
        else
        {
            HitResponse hit = CheckHit();
            if (hit.HasHit)
                Hit(hit.RayHit);
            else
                Move();
        }
    }

    private void Die(Vector3 position, bool isFromImpact)
    {
        if (isFromImpact && impactFxPrefab != null)
        {
            ParticleSystem impactFx = Instantiate(impactFxPrefab, position, transform.rotation);
            impactFx.Play();
            Destroy(impactFx, 5f);
        }
        Destroy(gameObject);
    }

    private HitResponse CheckHit()
    {
        Ray velocityRay = new Ray(transform.position, currentVelocity.normalized);
        bool rayHasHit = Physics.Raycast(velocityRay, out RaycastHit rayHit, currentVelocity.magnitude * Time.deltaTime, hitMask);

        HitResponse hitResponse = new HitResponse(rayHasHit, rayHit);

        return hitResponse;
    }

    private void Hit(RaycastHit hit)
    {
        GameObject recipient = hit.transform.gameObject;
        ShipController ship = recipient.GetComponent<ShipController>();
        if (ship != null)
        {
            ship.SufferDamage(hullDamage, hullPierce, shieldDamage, shieldPierce);
        }
        Die(hit.point, true);
    }

    private void Move()
    {
        transform.Translate(currentVelocity * Time.deltaTime, Space.World);

        // Bullet drop
        currentVelocity += Physics.gravity * gravityModifier * Time.deltaTime;

        // Align to velocity
        if (alignToVelocity)
            transform.rotation = Quaternion.LookRotation(currentVelocity);
    }
}
