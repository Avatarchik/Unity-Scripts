using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShipController : MonoBehaviour
{
    [Header("Identification")]
    [SerializeField] private int factionId = 0;
    [SerializeField] private string shipName = "";

    [Header("Health")]
    [SerializeField] private float hullValue = 0f;
    [SerializeField] private float hullArmor = 0f;
    [SerializeField] private float shieldValue = 0f;
    [SerializeField] private float shieldArmor = 0f;

    [Header("Movement")]
    [SerializeField] private float stopDistance = 1f;
    [SerializeField] private float targetAngleThreshold = 0.05f;
    [SerializeField] private float rotationSpeed = 1f;
    [SerializeField] private float forwardSpeed = 1f;
    [SerializeField] private float forwardTurnSpeed = 1f;

    [Header("Weapons")]
    [SerializeField] private GunTurret[] gunTurrets;
    [SerializeField] private MissileTurret[] missileTurrets;

    [Header("Sensors")]
    [SerializeField] private float detectionRange;

    [Header("Prefabs")]
    [SerializeField] private GameObject deathEffect;

    private Vector3 currentMoveTarget;
    private Transform currentFireTarget;

    private void Update()
    {
        MoveToAndTurn();
        FindNearestTarget();
    }

    public int GetFactionId()
    {
        return factionId;
    }

    public void SufferDamage(float hullDamage, float hullPierce, float shieldDamage, float shieldPierce)
    {
        if (shieldDamage > 0 && shieldValue > 0)
        {
            shieldPierce -= shieldArmor;
            if (shieldPierce > 0)
                shieldDamage += shieldPierce * 2;
            else
                shieldDamage += shieldPierce;
            shieldValue -= shieldDamage;
        }
        else if (hullDamage > 0 && hullPierce > 0)
        {
            if (shieldValue > 0)
            {
                shieldValue -= hullDamage;
            }
            else
            {
                hullPierce -= hullArmor;
                if (hullPierce > 0)
                    hullDamage += hullPierce * 2;
                else
                    hullDamage += hullPierce;
                hullValue -= hullDamage;
            }
        }

        if (hullValue <= 0)
            Die();
    }

    public void SetCurrentMoveTarget(Vector3 moveTarget)
    {
        this.currentMoveTarget = moveTarget;
    }

    public void SetCurrentFireTarget(Transform fireTarget)
    {
        this.currentFireTarget = fireTarget;
    }

    private void Die()
    {
        if (deathEffect != null)
        {
            GameObject effect = (GameObject)Instantiate(deathEffect, transform.position, Quaternion.identity);
            Destroy(effect, 2f);
        }
        Destroy(gameObject);
    }

    private void MoveToAndTurn()
    {
        if (currentMoveTarget != Vector3.zero)
        {
            float distanceFromTarget = Vector3.Distance(currentMoveTarget, transform.position);
            if (distanceFromTarget > stopDistance)
            {
                Vector3 directionToTarget = transform.position - currentMoveTarget;
                directionToTarget.Normalize();
                float angleToTarget = Vector3.Angle(transform.position, currentMoveTarget);
                if (angleToTarget < 0)
                    angleToTarget += Mathf.PI * 2;
                if (angleToTarget > targetAngleThreshold)
                {
                    if (angleToTarget < 0)
                    {
                        Quaternion rotationToTarget = Quaternion.LookRotation(directionToTarget);
                        transform.rotation = Quaternion.Slerp(transform.rotation,
                                                              rotationToTarget,
                                                              rotationSpeed * Time.deltaTime);
                    }
                    else
                    {
                        Quaternion rotationToTarget = Quaternion.LookRotation(-directionToTarget);
                        transform.rotation = Quaternion.Slerp(transform.rotation,
                                                              rotationToTarget,
                                                              rotationSpeed * Time.deltaTime);
                    }
                    transform.position += transform.forward * forwardTurnSpeed * forwardSpeed * Time.deltaTime;
                }
                else
                {
                    transform.position += transform.forward * forwardSpeed * Time.deltaTime;
                }
            }
            else
            {
                currentMoveTarget = Vector3.zero;
            }
        }
    }

    private void FindNearestTarget()
    {
        ShipController[] shipObjects = FindObjectsOfType<ShipController>();
        ShipController nearestTarget = null;
        float shortestDistance = Mathf.Infinity;
        foreach (ShipController ship in shipObjects)
        {
            if (ship.factionId != this.factionId)
            {
                float distanceToTarget = Vector3.Distance(transform.position, ship.transform.position);
                if (distanceToTarget <= detectionRange && distanceToTarget < shortestDistance)
                {
                    shortestDistance = distanceToTarget;
                    nearestTarget = ship;
                }
            }
        }
        if (nearestTarget != null)
        {
            foreach (GunTurret gunTurret in gunTurrets)
            {
                gunTurret.SetTarget(nearestTarget.transform);
            }
            foreach (MissileTurret missileTurret in missileTurrets)
            {
                missileTurret.SetTarget(nearestTarget.transform);
            }
        }
    }
}
