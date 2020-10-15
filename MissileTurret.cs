using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MissileTurret : MonoBehaviour
{
    [Header("Debug")]
    [SerializeField] private bool enableDebugArcs = true;
    [SerializeField] private bool enableDebugLines = true;

    [Header("Yaw Segment")]
    [SerializeField] [Range(1, 180)] private float yawLimit = 180f;
    [SerializeField] private Transform yawSegment;
    [SerializeField] private Quaternion yawSegmentStartRotation;

    [Header("Pitch Segment")]
    [SerializeField] [Range(1, 180)] private float pitchLimit = 180f;
    [SerializeField] private Transform pitchSegment;
    [SerializeField] private Transform pitchDebugDrawPoint;
    [SerializeField] private Quaternion pitchSegmentStartRotation;

    [Header("Firing")]
    [SerializeField] private bool isSequentialFiring;
    [SerializeField] private float rangeThreshold;
    [SerializeField] private float delayDeviation;
    [SerializeField] private float fireDelay;
    [SerializeField] private Transform[] barrels = { };

    [Header("Aiming")]
    [SerializeField] private float turnSpeed;
    [SerializeField] private float aimedThreshold;
    [SerializeField] private float aimDeviation;

    [Header("Prefabs")]
    [SerializeField] private ParticleSystem muzzleFlashPrefab;
    [SerializeField] private Missile missilePrefab;
    [SerializeField] private AudioSource[] fireSounds = { };


    public bool IsReadyToFire { get { return fireCountdown <= 0f; } }

    private bool isAimed = false;
    private bool isInRange = false;
    private float fireCountdown = 0f;
    private Transform currentTarget;
    private Queue<Transform> barrelQueue = null;
    private Dictionary<Transform, ParticleSystem> barrelToMuzzleFlash = new Dictionary<Transform, ParticleSystem>();

    private void Awake()
    {
        fireCountdown = fireDelay + Random.Range(0, delayDeviation);

        barrelQueue = new Queue<Transform>(barrels);
        if (barrelQueue.Count == 0)
            barrelQueue.Enqueue(pitchSegment);

        if (muzzleFlashPrefab != null)
        {
            foreach (Transform barrel in barrels)
            {
                ParticleSystem muzzleFlash = Instantiate(muzzleFlashPrefab, barrel, false).GetComponent<ParticleSystem>();
                barrelToMuzzleFlash.Add(barrel, muzzleFlash);
            }
        }

        yawSegmentStartRotation = yawSegment.localRotation;
        pitchSegmentStartRotation = pitchSegment.localRotation;
    }

    private void FixedUpdate()
    {
        fireCountdown = Mathf.MoveTowards(fireCountdown, 0f, Time.deltaTime);

        if (currentTarget != null)
        {
            LookAtTarget();
            Vector3 targetPosition = currentTarget.position;
            float angleToTarget = Vector3.Angle(targetPosition - pitchSegment.position, pitchSegment.forward);
            float targetRange = (yawSegment.position - targetPosition).magnitude;
            isAimed = angleToTarget <= aimedThreshold;
            isInRange = targetRange <= rangeThreshold;
            if (isAimed && isInRange)
            {
                Prime();
            }
        }
        else
        {
            isAimed = false;
        }
        if (enableDebugLines)
        {
            DrawDebugLines();
        }
    }

    private void Prime()
    {
        if (!IsReadyToFire)
            return;

        if (isSequentialFiring)
        {
            // Fire each barrel one at a time
            Transform barrel = barrelQueue.Dequeue();
            FireFromPoint(barrel, currentTarget);
            barrelQueue.Enqueue(barrel);
        }
        else
        {
            // Fire all barrels at same time
            foreach (Transform barrel in barrelQueue)
            {
                FireFromPoint(barrel, currentTarget);
            }
        }

        fireCountdown = fireDelay + Random.Range(0, delayDeviation);
    }

    private void FireFromPoint(Transform point, Transform target)
    {
        if (barrelToMuzzleFlash.ContainsKey(point))
        {
            barrelToMuzzleFlash[point].Play();
        }
        Missile missileProjectile = Instantiate(missilePrefab, point.position, point.rotation);
        missileProjectile.Fire(point.position, target);
    }

    private void LookAtTarget()
    {
        float angle;
        Vector3 targetRelative;
        Quaternion targetRotation;
        Vector3 targetPosition = currentTarget.position;
        if (yawSegment && (yawLimit != 0f))
        {
            targetRelative = yawSegment.InverseTransformPoint(targetPosition);
            angle = Mathf.Atan2(targetRelative.x, targetRelative.z) * Mathf.Rad2Deg;
            if (angle >= 180f) angle = 180f - angle;
            if (angle <= -180f) angle = -180f + angle;
            targetRotation = yawSegment.rotation * Quaternion.Euler(0f, Mathf.Clamp(angle, -turnSpeed * Time.deltaTime, turnSpeed * Time.deltaTime), 0f);
            if ((yawLimit < 360f) && (yawLimit > 0f)) yawSegment.rotation = Quaternion.RotateTowards(yawSegment.parent.rotation * yawSegmentStartRotation, targetRotation, yawLimit);
            else yawSegment.rotation = targetRotation;
        }

        if (pitchSegment && (pitchLimit != 0f))
        {
            targetRelative = pitchSegment.InverseTransformPoint(targetPosition);
            angle = -Mathf.Atan2(targetRelative.y, targetRelative.z) * Mathf.Rad2Deg;
            if (angle >= 180f) angle = 180f - angle;
            if (angle <= -180f) angle = -180f + angle;
            targetRotation = pitchSegment.rotation * Quaternion.Euler(Mathf.Clamp(angle, -turnSpeed * Time.deltaTime, turnSpeed * Time.deltaTime), 0f, 0f);
            if ((pitchLimit < 360f) && (pitchLimit > 0f)) pitchSegment.rotation = Quaternion.RotateTowards(pitchSegment.parent.rotation * pitchSegmentStartRotation, targetRotation, pitchLimit);
            else pitchSegment.rotation = targetRotation;
        }
    }

    public void SetTarget(Transform newTarget)
    {
        if (newTarget == null)
            return;

        currentTarget = newTarget;
    }

    private void DrawDebugLines()
    {
        if (currentTarget != null)
        {
            Vector3 targetPosition = currentTarget.position;
            foreach (Transform barrel in barrelQueue)
            {
                if (isAimed && isInRange)
                    Debug.DrawRay(barrel.position, targetPosition - barrel.position, Color.green);
                else if (isAimed)
                    Debug.DrawRay(barrel.position, targetPosition - barrel.position, Color.yellow);
                else
                    Debug.DrawRay(barrel.position, targetPosition - barrel.position, Color.red);
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (!enableDebugArcs)
            return;

        const float arcSize = 5f;
        Color yawColor = new Color(.5f, 1f, .5f, .1f);
        Color pitchColor = new Color(.5f, .5f, 1f, .1f);
        UnityEditor.Handles.color = yawColor;
        UnityEditor.Handles.DrawSolidArc(yawSegment.position, transform.up, transform.forward, yawLimit, arcSize);
        UnityEditor.Handles.DrawSolidArc(yawSegment.position, transform.up, transform.forward, -yawLimit, arcSize);
        UnityEditor.Handles.color = pitchColor;
        UnityEditor.Handles.DrawSolidArc(pitchDebugDrawPoint.position, pitchDebugDrawPoint.right, pitchDebugDrawPoint.forward, -pitchLimit, arcSize);
        UnityEditor.Handles.DrawSolidArc(pitchDebugDrawPoint.position, pitchDebugDrawPoint.right, pitchDebugDrawPoint.forward, pitchLimit, arcSize);
    }
}
