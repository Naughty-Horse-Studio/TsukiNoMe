/// ---------------------------------------------
/// Ultimate Character Controller
/// Copyright (c) Opsive. All Rights Reserved.

/// ---------------------------------------------
using System.Collections;
using DOFprojFPS;
using UnityEngine;
    /// <summary>
    /// A simple turret which will fire a projectile towards the character. This turret is setup for the demo scene and will likely require modifications if used in other areas.
    /// </summary>
    public class Turret : MonoBehaviour
    {
    public float health = 100f;
    public int maxDamage = 10;

    private Transform target;
    private PlayerStats targetEnemy;

    [Header("General")]

    public float range = 15f;

    [Header("Use Bullets (default)")]
    public GameObject bulletPrefab;
    public float fireRate = 1f;
    private float fireCountdown = 0f;

    [Header("Use Laser")]
    public bool useLaser = false;

    public int damageOverTime = 30;
    public float slowAmount = .5f;

    public LineRenderer lineRenderer;
    public ParticleSystem impactEffect;
    public Light impactLight;

    [Header("Unity Setup Fields")]

    public string enemyTag = "Player";

    public Transform partToRotate;
    public float turnSpeed = 10f;

    public Transform firePoint;

    [Tooltip("Optionally specify a muzzle flash that should appear when the turret is fired.")]
        [SerializeField] protected GameObject m_MuzzleFlash;
        [Tooltip("The location that the muzzle flash should spawn.")]
        [SerializeField] protected Transform m_MuzzleFlashLocation;

        [Tooltip("Optionally specify an audio clip that should play when the turret is fired.")]
        [SerializeField] protected AudioClip m_FireAudioClip;

        private GameObject m_GameObject;
        private Transform m_Transform;
        private AudioSource m_AudioSource;

        private Transform m_Target;
//        private Health m_Health;
        private float m_LastFireTime;

  

    // Use this for initialization
    void Start()
    {
        InvokeRepeating("UpdateTarget", 0f, 3f);
    }

    void UpdateTarget()
    {
     
            GameObject[] enemies = GameObject.FindGameObjectsWithTag(enemyTag);
            float shortestDistance = Mathf.Infinity;
            GameObject nearestEnemy = null;
            foreach (GameObject enemy in enemies)
            {
                float distanceToEnemy = Vector3.Distance(transform.position, enemy.transform.position);
                if (distanceToEnemy < shortestDistance)
                {
                    shortestDistance = distanceToEnemy;
                    nearestEnemy = enemy;
                }
            }

            if (nearestEnemy != null && shortestDistance <= range)
            {
                target = nearestEnemy.transform;
                targetEnemy = nearestEnemy.GetComponent<PlayerStats>();


            }
            else
            {
                target = null;
            }
       
    }
    private void Awake()
    {
            m_GameObject = gameObject;
            m_Transform = transform;
            m_AudioSource = GetComponent<AudioSource>();


    }


        /// <summary>
        /// Rotates the turret head and attacks if the character is within range.
        /// </summary>
    private void Update()
    {
     

            if (health > 0)
            {
                if (target == null)
                {
                    if (useLaser)
                    {
                        if (lineRenderer.enabled)
                        {
                            lineRenderer.enabled = false;
                            impactEffect.Stop();
                            impactLight.enabled = false;
                        }
                    }

                    return;
                }

                LockOnTarget();

                if (useLaser)
                {
                    Laser();
                }
                else
                {
                    if (fireCountdown <= 0f)
                    {
                        Shoot();
                        fireCountdown = 1f / fireRate;
                    }

                    fireCountdown -= Time.deltaTime;
                }
            }
            else
            {
            impactEffect.transform.position = firePoint.position;
            impactEffect.Play();
            Death();
            }

    }
    private void Death()
    {

        Destroy(this.gameObject);
    }
    public void ApplyHit(int damage)
    {
       
        health -= damage;
    }
    void LockOnTarget()
    {

        Vector3 dir = target.position - transform.position;
        Quaternion lookRotation = Quaternion.LookRotation(dir);
        Vector3 rotation = Quaternion.Lerp(partToRotate.rotation, lookRotation, Time.deltaTime * turnSpeed).eulerAngles;
        partToRotate.LookAt(target.position);
    }

    void Laser()
    {
     //   targetEnemy.TakeDamage(damageOverTime * Time.deltaTime);
     //   targetEnemy.Slow(slowAmount);

        if (!lineRenderer.enabled)
        {
            lineRenderer.enabled = true;
            impactEffect.Play();
            impactLight.enabled = true;
        }

        lineRenderer.SetPosition(0, firePoint.position);
        lineRenderer.SetPosition(1, target.position);

        Vector3 dir = firePoint.position - target.position;

        impactEffect.transform.position = target.position + dir.normalized;

        impactEffect.transform.rotation = Quaternion.LookRotation(dir);
    }

    void Shoot()
    {
        if (targetEnemy != null)
        {
            GameObject bulletGO = (GameObject)Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
            Bullet bullet = bulletGO.GetComponent<Bullet>();

            if (bullet != null)
                bullet.Seek(target);



            // Play a firing sound.
            if (m_FireAudioClip != null)
            {
                m_AudioSource.clip = m_FireAudioClip;
                m_AudioSource.Play();
            }
        }

    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, range);
    }
    /// <summary>
    /// An object has entered the trigger.
    /// </summary>
    /// <param name="other">The object that entered the trigger.</param>
    private void OnTriggerEnter(Collider other)
        {
            //if (m_Target != null || (other.gameObject.layer != m_ImpactLayers) ){
            //    return;
            //}

            //var characterLocomotion = other.GetComponentInParent<UltimateCharacterLocomotion>();
            //if (characterLocomotion == null) {
            //    return;
            //}

       //     m_Target = characterLocomotion.transform;
       //     m_Health = characterLocomotion.GetComponent<Health>();
        }

        /// <summary>
        /// An object has exited the trigger.
        /// </summary>
        /// <param name="other">The collider that exited the trigger.</param>
        private void OnTriggerExit(Collider other)
        {
            if (other.gameObject != m_Target) {
                return;
            }

            m_Target = null;
        }
    }
