using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ProjectileStats")]
public class ProjectileStats : ScriptableObject
{
    public enum DamageType
    {
        PHYSICAL,
        IONIC,
        HYBRID
    };

    [Header("Damage Type")]
    public DamageType damageType;

    [Header("Damage")]
    public float damage;
    public float pierce;

    [Header("Physics")]
    public bool alignToVelocity;
    public float rangeToLive;
    public LayerMask hitMask;

    [Header("Prefabs")]
    public GameObject projectilePrefab;
    public ParticleSystem impactFxPrefab;
    public AudioSource[] fireSounds;
}
