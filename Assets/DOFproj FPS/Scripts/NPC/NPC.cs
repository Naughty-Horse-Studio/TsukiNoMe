/// Deacon of Freedom Development (2020) v1
/// If you have any questions feel free to write me at email --- Phil-James_Lapuz@outlook.com ---

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace DOFprojFPS
{
    /// <summary>
    /// 1. - NPC class contains methods from the Unity Standard Assets - ThirdPersonController
    /// ---  They are inside Character Controller region.
    /// 2. - NPC system WIP and any parts of code may be modified in future.
    /// </summary>

    /// NPC are have very basic logic now. But i'm looking for a way to make it better with every update
    /// Main logic like shooting, wandering, patrol, tactical behavior an cover finding are ready now you can find them in NPCStates.cs
    /// so you can play with them any way you want
    public class NPC : MonoBehaviour
    {
        [Header("Debug settings")]
        [Tooltip("Enable debug helpers")]
        public bool debug = true;

        public int myFraction = 0;

        [Header("Public objects")]
        [Tooltip("Muzzle flash particle system")]
        public ParticleSystem fireFX;
        [Tooltip("Shot sound effet")]
        public AudioClip fireSFX;
        [Tooltip("Hurt sound effect")]
        public AudioClip[] painSFX;

        [Header("NPC stats")]
        public string NPCNameInDatabase = "Assaulter";
        [Tooltip("NPC health")]
        public int health = 100;
        [Header("Time to change worried state to calm. If player out of vision range")]
        public float timeToCalmDown = 10f;

        [Header("Fire stats and settings")]
        public float ammo = 30;
        [Tooltip("Average damage applied to target on hit")]
        public int damage = 10;
        [Tooltip("Vector3 which affects look direction of humanoid's spine in aim state")]
        public Vector3 aimOffset;
        [Tooltip("Random value from -n to +n which will be used for raycast direction calculation. Than less number than more accuracy NPC have")]
        public float shootingAccuracySpread = 0.1f;
        [Range(0, 3)]
        [Tooltip("1 - One bullet per fire method call. 2 - Two bullets... 3 - Three bullets... | To make automatical fire mode change fire mode to 1, and fire rate to 0.1f")]
        public int fireMode = 3;
        [Tooltip("Time between shots if player visible")]
        public float fireRate = 0.5f;
        [Tooltip("Use random rate and mode to simulate realistic shooting behavior")]
        public bool useRandomRateAndMode = true;

        [Header("Shooting variables and objects")]
        [Tooltip("Point where shot raycast starts")]
        public Transform shootPoint;
        public GameObject projectile;
        public int projectilePoolSize = 10;

        #region Utility variables
        //Objects references
        private Animator animator;
        [HideInInspector]
        public NavMeshAgent navMeshAgent;
        private AIControl aiControl;
        private PlayerStats playerHealth;
        private HitFXManager hitFXManager;
        [HideInInspector]
        public NPCVision vision;
        private AudioSource source;

        //Private objects and calculations
        private Transform chest;
        private bool isReloading = false;
        private float nextFire;
        private GameObject visibilityCheckObject;
        [HideInInspector]
        public Vector3 desiredPosition;
        public List<NPC> friendsList; //If enemy under attack, he'll be transmit his states to friends arround

        private int decalIndex_wood = 0;
        private int decalIndex_concrete = 0;
        private int decalIndex_dirt = 0;
        private int decalIndex_metal = 0;

        public bool destinationReached = true;

        private float calmDownTimer;

        public Transform curretTarget;

        private CoverList coverList;

        private float timer;
        private float timerToLeaveCover;

        #endregion

        #region Character controller variables

        [Header("Character controller variables")]
        [Header("Same as in the ThirdPersonController")]

        public float m_MovingTurnSpeed = 360;
        public float m_StationaryTurnSpeed = 180;
        public float m_JumpPower = 12f;
        [Range(1f, 4f)]
        public float m_GravityMultiplier = 2f;
        public float m_RunCycleLegOffset = 0.2f;
        public float m_MoveSpeedMultiplier = 1f;
        public float m_AnimSpeedMultiplier = 1f;
        public float m_GroundCheckDistance = 0.1f;

        private Rigidbody m_Rigidbody;
        private bool m_IsGrounded;
        private float m_OrigGroundCheckDistance;
        private const float k_Half = 0.5f;
        private float m_TurnAmount;
        private float m_ForwardAmount;
        private Vector3 m_GroundNormal;
        private float m_CapsuleHeight;
        private Vector3 m_CapsuleCenter;
        private CapsuleCollider m_Capsule;

        private bool m_Crouching;
        [HideInInspector]
        public bool lookAtTarget = false;
        #endregion

        private InputManager inputManager;

        private void Start()
        {
            animator = GetComponent<Animator>();
            chest = animator.GetBoneTransform(HumanBodyBones.Chest);
            source = GetComponent<AudioSource>();
            navMeshAgent = GetComponent<NavMeshAgent>();
            aiControl = GetComponent<AIControl>();
            vision = GetComponentInChildren<NPCVision>();
            hitFXManager = GetComponent<HitFXManager>();

            visibilityCheckObject = new GameObject();
            visibilityCheckObject.name = this.name + " visibilityCheckObject";
            visibilityCheckObject.AddComponent<SphereCollider>().radius = 0.3f;
            visibilityCheckObject.GetComponent<SphereCollider>().enabled = false;

            playerHealth = FindObjectOfType<PlayerStats>();

            curretTarget = GameObject.FindGameObjectWithTag("Player").transform;

            foreach (Collider collider in GetComponentsInChildren<Collider>())
            {
                collider.enabled = false;
                if (collider.transform.tag == "HeadShot")
                {
                    collider.enabled = true;
                }
            }

            foreach (Rigidbody rigidbody in GetComponentsInChildren<Rigidbody>())
            {
                rigidbody.isKinematic = true;
            }

            GetComponent<Rigidbody>().isKinematic = false;
            GetComponent<CapsuleCollider>().enabled = true;
            GetComponent<BoxCollider>().enabled = true;

            StartCoroutine(CheckFriends());

            //Set own position as NavMesh target on start 
            desiredPosition = transform.position;

            //Controller variables
            m_Rigidbody = GetComponent<Rigidbody>();
            // m_Capsule = GetComponent<CapsuleCollider>();
            //m_CapsuleHeight = m_Capsule.height;
            //m_CapsuleCenter = m_Capsule.center;

            m_Rigidbody.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationY | RigidbodyConstraints.FreezeRotationZ;
            m_OrigGroundCheckDistance = m_GroundCheckDistance;

            coverList = FindObjectOfType<CoverList>();

            timerToLeaveCover = Random.Range(4, 10);

            lookPosition = transform.forward;

            inputManager = FindObjectOfType<InputManager>();
        }

        void Update()
        {
            aiControl.SetTarget(desiredPosition);

            if (health <= 0)
            {
                Death();
                Destroy(this.gameObject);
            }

            if (debug)
                Debug.DrawRay(shootPoint.transform.position, shootPoint.forward * 100);

            if (!debug)
                TargetMachine();

            if (m_Crouching)
            {
                animator.SetBool("Crouch", true);
                //ScaleCapsuleForCrouching(true);
            }
            else
            {
                animator.SetBool("Crouch", false);
                //ScaleCapsuleForCrouching(false);
            }

            if (curretTarget != null)
            {
                var _lookPosition = lookPosition - transform.position;
                _lookPosition.y = 0;
                var rotation = Quaternion.LookRotation(_lookPosition);
                transform.rotation = Quaternion.Slerp(transform.rotation, rotation, Time.deltaTime * 2);
            }

            Attack();
        }

        public void Attack()
        {
            if (vision.SelectBestTarget(true) != null)
            {
                Fire();
                Shout();
            }
        }

        public float turnSpeed = 1;

        private void LateUpdate()
        {
            if (vision.SelectBestTarget(false) != null && vision.SelectBestTarget(false).CompareTag("MainCamera") == false)
            {
                animator.SetLayerWeight(1, Mathf.Lerp(animator.GetLayerWeight(1), 1, Time.deltaTime * 1.5f));
                chest.LookAt(lookPosition + Vector3.up);
                var newRot = chest.rotation * Quaternion.Euler(aimOffset);//Quaternion.Lerp(chest.rotation, chest.rotation * Quaternion.Euler(aimOffset), Time.deltaTime * 3);
                chest.rotation = newRot;
            }
            else if (vision.SelectBestTarget(false) != null && vision.SelectBestTarget(false).CompareTag("MainCamera") == true)
            {
                animator.SetLayerWeight(1, Mathf.Lerp(animator.GetLayerWeight(1), 1, Time.deltaTime * 1.5f));
                chest.LookAt(lookPosition);
                chest.rotation = chest.rotation * Quaternion.Euler(aimOffset);
            }
            else
            {
                chest.rotation = chest.rotation;
            }
        }

        public void Fire()
        {
            if (ammo > 0 && !isReloading)
            {
                if (Time.time > nextFire)
                {
                    switch (fireMode)
                    {
                        case 1:
                            Invoke("Shot", 0f);
                            nextFire = Time.time + fireRate;
                            ammo -= 1;
                            break;
                        case 2:
                            Invoke("Shot", 0.1f);
                            Invoke("Shot", 0.2f);

                            nextFire = Time.time + fireRate;
                            ammo -= 2;
                            break;
                        case 3:

                            Invoke("Shot", 0.1f);
                            Invoke("Shot", 0.2f);
                            Invoke("Shot", 0.3f);

                            nextFire = Time.time + fireRate;
                            ammo -= 3;
                            break;
                    }
                }
            }
            else
            {
                Reload();
                isReloading = true;
            }

            if (useRandomRateAndMode)
            {
                fireMode = Random.Range(1, 3);
                fireRate = Random.Range(0.5f, 1f);
            }
        }

        public LayerMask shotLayerMask;

        private void Shot()
        {
            animator.Play("Shot", 1);
            fireFX.Stop();
            fireFX.time = 0;
            fireFX.Play(true);
            source.Stop();
            source.PlayOneShot(fireSFX);

            RaycastHit hit;

            Vector3 directionWithSpread = new Vector3(shootPoint.transform.forward.x + Random.Range(-shootingAccuracySpread, shootingAccuracySpread),
                                                      shootPoint.transform.forward.y + Random.Range(-shootingAccuracySpread, shootingAccuracySpread),
                                                      shootPoint.transform.forward.z + Random.Range(-shootingAccuracySpread, shootingAccuracySpread));

            if (Physics.Raycast(shootPoint.transform.position, directionWithSpread, out hit, Mathf.Infinity, shotLayerMask))
            {
                ApplyHit(hit);
            }
        }

        public void Reload()
        {
            //ik.IKActive = false;
            animator.Play("Reload");
            Invoke("AddAmmo", 3f);
        }

        public void AddAmmo()
        {
            ammo = 30;
            //ik.IKActive = true;
            isReloading = false;
        }

        //Public method to apply damage to an enemy from other scripts
        public void GetHit(int damage, Transform damageSender)
        {
            if (damageSender.GetComponent<NPC>() != null && damageSender.GetComponent<NPC>().myFraction != myFraction)
            {
                lookAtTarget = true;
                if (curretTarget == null)
                {
                    curretTarget = damageSender;
                    lookPosition = damageSender.position;
                }
                if (cover != null)
                {
                    m_Crouching = false;
                    cover.occupied = false;
                    cover = null;
                }

                health -= damage;
                PlayPainClip();
                Shout();
            }
            else
            {
                lookAtTarget = true;
                curretTarget = damageSender;
                lookPosition = damageSender.position;
                if (cover != null)
                {
                    m_Crouching = false;
                    cover.occupied = false;
                    cover = null;
                }

                health -= damage;
                PlayPainClip();
                Shout();
            }
        }

        public void PlayPainClip()
        {
            if (painSFX.Length > 0)
                source.PlayOneShot(painSFX[Random.Range(0, painSFX.Length)]);
        }

        public void ApplyHit(RaycastHit hit)
        {
            if (hit.collider.tag == "MainCamera")
            {
                if (playerHealth.health != 0)
                     playerHealth.ApplyDamage(Random.Range(damage / 2, damage));
            }
            else
            {
                //Play ricochet sfx
                RicochetSFX();
                //Set tag and transform of hit to HitParticlesFXManager
                HitParticlesFXManager(hit);

                //if (curretTarget != null && Vector3.Distance(transform.position, curretTarget.position) < 8)
                //{
                //    curretTarget.GetComponent<NPC>().GetHit(damage, transform);
                //}
                //else
                //{
                //    if (hit.collider.GetComponent<NPC>() != null)
                //        hit.collider.GetComponent<NPC>().GetHit(damage, transform);
                //}
                if (hit.collider.GetComponent<ObjectHealth>() && Vector3.Distance(transform.position, curretTarget.position) < 8)
                {
                    hit.collider.GetComponent<NPC>().GetHit(damage, transform);
                }
                else
                {
                    if (hit.collider.GetComponent<NPC>() != null)
                        hit.collider.GetComponent<NPC>().GetHit(damage, transform);
                }


                //if (hit.collider.GetComponent<HeadShot>())
                //{
                //    Debug.Log("HEADSHOT!!!!");
                //    hit.collider.GetComponent<HeadShot>().InstaKill();
                //    hit.collider.GetComponent<HeadShot>().enabled = false;

                //}

                //Decrease health of object by calculatedDamage
                //if (hit.collider.GetComponent<ObjectHealth>())
                //    hit.collider.GetComponent<ObjectHealth>().health -= damage;

                //if (!hit.rigidbody)
                //{
                //    //  Set hit position to decal manager
                //    DecalManager(hit, false);
                //}

            }
        }

        public GameObject DeadBody;

        public void Death()
        {
            CancelInvoke();
            animator.enabled = false;
            GetComponent<IK>().enabled = false;
            GetComponent<AIControl>().enabled = false;
            navMeshAgent.enabled = false;

            foreach (Collider collider in GetComponentsInChildren<Collider>())
            {
                collider.enabled = true;
            }
            foreach (Rigidbody rigidbody in GetComponentsInChildren<Rigidbody>())
            {
                rigidbody.isKinematic = false;
            }

            if(DeadBody!= null)
            Instantiate(DeadBody, transform.position, transform.rotation);

            GetComponent<BoxCollider>().enabled = false;
            GetComponent<CapsuleCollider>().enabled = false;
            GetComponent<Rigidbody>().isKinematic = true;

            gameObject.layer = LayerMask.NameToLayer("Dead");


            this.enabled = false;
        }

        private Cover cover;
        [HideInInspector]
        public Vector3 lookPosition;
        private bool randomTimeGenerated = false;

        bool idle = true;

        public void GoToIdle()
        {
            if (cover != null)
            {
                cover.occupied = false;
                cover = null;
            }

            idle = true;
        }

        public void TargetMachine()
        {
            if (curretTarget == null && idle == false)
            {
                timer = Time.time;
                Invoke("GoToIdle", calmDownTimer);
                lookingForTarget = false;
            }

            if (vision.SelectBestTarget(true) == null && cover != null && curretTarget != null && !lookingForTarget)
            {
                idle = false;

                if (Time.time > timer + timerToLeaveCover)
                {
                    ExitCoverToCheckTarget();
                }
            }

            if (vision.SelectBestTarget(true) != null)
            {
                timer = Time.time;
                CancelInvoke("GoToIdle");
                m_Crouching = false;
                curretTarget = vision.SelectBestTarget(true).transform;
                lookPosition = vision.SelectBestTarget(true).transform.position;
                lookAtTarget = true;
                lookingForTarget = false;
                idle = false;
            }

            if (!lookingForTarget)
            {
                if (curretTarget == null && idle)
                {
                    m_Crouching = false;
                    lookAtTarget = false;

                    animator.SetLayerWeight(1, Mathf.Lerp(animator.GetLayerWeight(1), 0, Time.deltaTime * 1.5f));

                    switch (defaultBehavior)
                    {
                        case DefaultBehavior.FollowToTarget:
                            FollowToTargetBehavior();
                            break;
                        case DefaultBehavior.Idle:
                            //Do nothing
                            break;
                        case DefaultBehavior.Patrol:
                            PatrolBehavior();
                            break;
                        case DefaultBehavior.Wander:
                            WanderingBehavior();
                            break;
                    }
                }
                else
                {

                    animator.SetLayerWeight(1, Mathf.Lerp(animator.GetLayerWeight(1), 1, Time.deltaTime * 1.5f));

                    if (curretTarget != null && cover == null)
                    {
                        cover = coverList.FindClosestCover(transform.position, curretTarget.transform.position);

                        if (cover != null)
                        {
                            cover.occupied = true;
                            desiredPosition = cover.transform.position;
                            navMeshAgent.stoppingDistance = 0.2f;
                            navMeshAgent.speed = 2;
                            destinationReached = false;
                            lookAtTarget = true;
                        }
                        else
                        {
                            TacticalBehavior();
                        }
                    }

                    if (curretTarget != null && Vector3.Distance(transform.position, curretTarget.position) < 10)
                    {
                        m_Crouching = false;
                        if (cover != null)
                        {
                            cover.occupied = false;
                            cover = null;
                        }

                        TacticalBehavior();
                    }

                    if (cover != null && Vector3.Distance(transform.position, cover.transform.position) < navMeshAgent.stoppingDistance && destinationReached == false)
                    {
                        m_Crouching = true;
                        destinationReached = true;
                        timer = Time.time;
                    }
                }
            }

        }
        

        float GetRandomTimeForAction(int max)
        {
            randomTimeGenerated = true;
            return Random.Range(Time.time, Time.time + max);
        }

        void TacticalBehavior()
        {
            idle = false;
            navMeshAgent.speed = 1;

            m_Crouching = false;

            if (curretTarget && destinationReached || navMeshAgent.pathStatus == NavMeshPathStatus.PathPartial)
            {
                lookPosition = curretTarget.position;
                lookAtTarget = true;
                GetRandomPositonOutsidePlayerFOV(transform.position, 20);
                destinationReached = false;
            }

            if (Vector3.Distance(transform.position, navMeshAgent.destination) < navMeshAgent.stoppingDistance)
            {
                destinationReached = true;
            }
        }

        /// <summary>
        /// 1. Tactical runner will find cover out of player visibility range and try to keep distance. In some cases he can run behind player's back to attack
        /// 2. Seeker know player position and will be chase player to kill
        /// 3. Idle will stay on one place and attack player only if he in visibility range
        /// </summary>
        public enum EnemyTactics { Tactical, Seeker, Idle }
        /// <summary>
        /// 1. Idle is default behavior when enemy just stay on he's own position on the \
        /// 2. Patrol. Enemy will be walking through patrol points
        /// 3. FollowToTarget. Enemy will follow a target assigned to the targetToFollow field
        /// 4. Wander. Enemy will randomly wandering within navmesh
        /// </summary>
        public enum DefaultBehavior { Idle, Patrol, FollowToTarget, Wander }

        [Header("Enemy behavior types")]
        [Tooltip("Tactic to follow when NPC alarmed")]
        public EnemyTactics enemyTactics;
        [Tooltip("Default enemy state when idle")]
        public DefaultBehavior defaultBehavior;

        [Header("Patrol - behavior")]
        public Transform[] patrolPoints;
        [Range(0, 1)]
        public float patrolSpeed = 0.5f;

        [Header("Target to follow - behavior")]
        public Transform targetToFollow;
        [Range(0, 1)]
        public float followSpeed = 0.5f;

        [Header("Wandering - behavior")]
        public float radiusToTakeRandomPosition = 15;
        [Range(0, 1)]
        public float wanderingSpeed = 0.5f;

        [Header("Seeker - behavior")]
        public float seekerStopingDistance = 1f;
        #region Default Behaviors

        private int wayPointIndex;

        private void PatrolBehavior()
        {
            var _lookPosition = navMeshAgent.pathEndPosition;
            var direction = _lookPosition - transform.position;
            var _lookRot = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.RotateTowards(transform.rotation, _lookRot, 360), Time.deltaTime * 2);

            navMeshAgent.speed = patrolSpeed;
            navMeshAgent.stoppingDistance = 1;

            desiredPosition = patrolPoints[wayPointIndex].position;

            if (navMeshAgent.remainingDistance <= navMeshAgent.stoppingDistance)
            {
                wayPointIndex++;

                if (wayPointIndex == patrolPoints.Length)
                {
                    wayPointIndex = 0;
                }
            }
        }

        private float wanderingTimer;

        private void WanderingBehavior()
        {
            var _lookPosition = navMeshAgent.pathEndPosition;
            var direction = _lookPosition - transform.position;
            var _lookRot = Quaternion.LookRotation(_lookPosition);
            transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.RotateTowards(transform.rotation, _lookRot, 360), Time.deltaTime * 2);

            navMeshAgent.stoppingDistance = 1f;
            navMeshAgent.speed = wanderingSpeed;

            if (navMeshAgent.remainingDistance <= navMeshAgent.stoppingDistance && Time.time > wanderingTimer)
            {
                desiredPosition = GetRandomPosition(transform.position, radiusToTakeRandomPosition, 1);

                wanderingTimer = Time.time + Random.Range(5, 10);
            }
        }

        private void FollowToTargetBehavior()
        {
            if (targetToFollow.name == "Player")
            {
                if(inputManager.IsRunning())
                {
                    navMeshAgent.speed = Mathf.Lerp(navMeshAgent.speed, followSpeed * 2f, Time.deltaTime * 3);
                }
                else
                {
                    navMeshAgent.speed = Mathf.Lerp(navMeshAgent.speed, followSpeed, Time.deltaTime * 3);
                }
            }
            else
            {
                navMeshAgent.speed = followSpeed;
            }

            navMeshAgent.stoppingDistance = 1.5f;

            var _lookPosition = navMeshAgent.pathEndPosition;
            var direction = _lookPosition - transform.position;
            var _lookRot = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.RotateTowards(transform.rotation, _lookRot, 360), Time.deltaTime * 2);

            desiredPosition = targetToFollow.position;
        }

        #endregion

        #region Utility methods

        //Method to tell other NPC that player has been spoted
        private void Shout()
        {
            if (friendsList != null)
            {
                foreach (NPC friend in friendsList)
                {
                    if (friend != null)
                    {
                        if (friend.curretTarget == null && curretTarget != null)
                        {
                            friend.lookAtTarget = true;
                            friend.curretTarget = curretTarget;
                            friend.lookPosition = curretTarget.position;
                            //friend.desiredPosition = friend.transform.position;
                        }
                    }
                }
            }
        }

        public bool CheckTargetTransformVisibility()
        {
            RaycastHit hit;

            if (curretTarget == null)
                return false;


            var direction = visibilityCheckObject.transform.position - curretTarget.transform.position;

            if (Physics.Raycast(curretTarget.transform.position, direction, out hit, Mathf.Infinity))
            {
                if (hit.collider.name == visibilityCheckObject.name)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            return true;
        }

        bool lookingForTarget;

        public void ExitCoverToCheckTarget()
        {
            print("ExitCoverToCheckTarget " + name);

            m_Crouching = false;

            if (cover != null)
            {
                cover.occupied = false;
                cover = null;
            }

            destinationReached = false;
            desiredPosition = curretTarget.position;
            lookPosition = curretTarget.position;
            lookingForTarget = true;
        }

        public void GetRandomPositonOutsidePlayerFOV(Vector3 origin, float dist)
        {
            Vector3 randomDirection = Random.insideUnitSphere * dist;
            randomDirection += transform.position;
            NavMeshHit hit;
            for (int i = 0; i < 10; i++)
            {
                if (NavMesh.SamplePosition(randomDirection, out hit, dist, 1))
                {
                    visibilityCheckObject.GetComponent<SphereCollider>().enabled = true;
                    visibilityCheckObject.transform.position = hit.position + Vector3.up;

                    if (CheckTargetTransformVisibility() == false)
                    {
                        desiredPosition = hit.position;
                        if (debug)
                            print("Founded position is safe. Updating desiredPosition to it : Attempt " + i);
                        break;
                    }
                    else
                    {
                        if (debug)
                            print("No safe position found : Attempt " + i);
                    }
                }
            }

            visibilityCheckObject.GetComponent<SphereCollider>().enabled = false;
        }

        IEnumerator CheckFriends()
        {
            for (; ; )
            {
                GetFriendsAround();
                yield return new WaitForSeconds(.5f);
            }
        }

        private void GetFriendsAround()
        {
            if (friendsList != null)
            {
                friendsList.Clear();
                friendsList.Capacity = 0;
            }

            var colliders = Physics.OverlapSphere(transform.position, 100f);

            foreach (Collider _collider in colliders)
            {
                //if (_collider != null && _collider.gameObject.layer == gameObject.layer && _collider.gameObject.GetComponent<NPC>().myFraction == myFraction)
                //{
                if (friendsList != null && !friendsList.Contains(_collider.GetComponent<NPC>()))
                {
                    if (_collider.gameObject != this.gameObject)
                    {
                        friendsList.Add(_collider.GetComponent<NPC>());
                    }
                }
                //}
            }
        }

        public void RicochetSFX()
        {
            source.PlayOneShot(hitFXManager.ricochetSounds[Random.Range(0, hitFXManager.ricochetSounds.Length)]);
        }

        public void HitParticlesFXManager(RaycastHit hit)
        {
            if (hit.collider.tag == "Wood")
            {
                hitFXManager.objWoodHitFX.Stop();
                hitFXManager.objWoodHitFX.transform.position = new Vector3(hit.point.x, hit.point.y, hit.point.z);
                hitFXManager.objWoodHitFX.transform.LookAt(transform.position);
                hitFXManager.objWoodHitFX.Play(true);
            }
            else if (hit.collider.tag == "Concrete")
            {
                hitFXManager.objConcreteHitFX.Stop();
                hitFXManager.objConcreteHitFX.transform.position = new Vector3(hit.point.x, hit.point.y, hit.point.z);
                hitFXManager.objConcreteHitFX.transform.LookAt(transform.position);
                hitFXManager.objConcreteHitFX.Play(true);
            }
            else if (hit.collider.tag == "Dirt")
            {
                hitFXManager.objDirtHitFX.Stop();
                hitFXManager.objDirtHitFX.transform.position = new Vector3(hit.point.x, hit.point.y, hit.point.z);
                hitFXManager.objDirtHitFX.transform.LookAt(transform.position);
                hitFXManager.objDirtHitFX.Play(true);
            }
            else if (hit.collider.tag == "Metal")
            {
                hitFXManager.objMetalHitFX.Stop();
                hitFXManager.objMetalHitFX.transform.position = new Vector3(hit.point.x, hit.point.y, hit.point.z);
                hitFXManager.objMetalHitFX.transform.LookAt(transform.position);
                hitFXManager.objMetalHitFX.Play(true);
            }
            else if (hit.collider.tag == "Flesh" || hit.collider.tag == "Player")
            {
                hitFXManager.objBloodHitFX.Stop();
                hitFXManager.objBloodHitFX.transform.position = new Vector3(hit.point.x, hit.point.y, hit.point.z);
                hitFXManager.objBloodHitFX.transform.LookAt(transform.position);
                hitFXManager.objBloodHitFX.Play(true);
            }
            else
            {
                hitFXManager.objConcreteHitFX.Stop();
                hitFXManager.objConcreteHitFX.transform.position = new Vector3(hit.point.x, hit.point.y, hit.point.z);
                hitFXManager.objConcreteHitFX.transform.LookAt(transform.position);
                hitFXManager.objConcreteHitFX.Play(true);
            }

        }

        public void DecalManager(RaycastHit hit, bool applyParent)
        {
            if (hit.collider.CompareTag("Concrete"))
            {
                hitFXManager.concreteDecal_pool[decalIndex_concrete].SetActive(true);
                var decalPostion = hit.point + hit.normal * 0.025f;
                hitFXManager.concreteDecal_pool[decalIndex_concrete].transform.position = decalPostion;
                hitFXManager.concreteDecal_pool[decalIndex_concrete].transform.rotation = Quaternion.FromToRotation(-Vector3.forward, hit.normal);
                if (applyParent)
                    hitFXManager.concreteDecal_pool[decalIndex_concrete].transform.parent = hit.transform;

                decalIndex_concrete++;

                if (decalIndex_concrete == hitFXManager.decalsPoolSizeForEachType)
                {
                    decalIndex_concrete = 0;
                }
            }
            else if (hit.collider.CompareTag("Wood"))
            {
                hitFXManager.woodDecal_pool[decalIndex_wood].SetActive(true);
                var decalPostion = hit.point + hit.normal * 0.025f;
                hitFXManager.woodDecal_pool[decalIndex_wood].transform.position = decalPostion;
                hitFXManager.woodDecal_pool[decalIndex_wood].transform.rotation = Quaternion.FromToRotation(-Vector3.forward, hit.normal);
                if (applyParent)
                    hitFXManager.woodDecal_pool[decalIndex_wood].transform.parent = hit.transform;

                decalIndex_wood++;

                if (decalIndex_wood == hitFXManager.decalsPoolSizeForEachType)
                {
                    decalIndex_wood = 0;
                }
            }
            else if (hit.collider.CompareTag("Dirt"))
            {
                hitFXManager.dirtDecal_pool[decalIndex_dirt].SetActive(true); var decalPostion = hit.point + hit.normal * 0.025f;
                hitFXManager.dirtDecal_pool[decalIndex_dirt].transform.position = decalPostion;
                hitFXManager.dirtDecal_pool[decalIndex_dirt].transform.rotation = Quaternion.FromToRotation(-Vector3.forward, hit.normal);
                if (applyParent)
                    hitFXManager.dirtDecal_pool[decalIndex_dirt].transform.parent = hit.transform;

                decalIndex_dirt++;

                if (decalIndex_dirt == hitFXManager.decalsPoolSizeForEachType)
                {
                    decalIndex_dirt = 0;
                }
            }
            else if (hit.collider.CompareTag("Metal"))
            {
                hitFXManager.metalDecal_pool[decalIndex_metal].SetActive(true);
                var decalPostion = hit.point + hit.normal * 0.025f;
                hitFXManager.metalDecal_pool[decalIndex_metal].transform.position = decalPostion;
                hitFXManager.metalDecal_pool[decalIndex_metal].transform.rotation = Quaternion.FromToRotation(-Vector3.forward, hit.normal);
                if (applyParent)
                    hitFXManager.metalDecal_pool[decalIndex_metal].transform.parent = hit.transform;

                decalIndex_metal++;

                if (decalIndex_metal == hitFXManager.decalsPoolSizeForEachType)
                {
                    decalIndex_metal = 0;
                }
            }
            else
            {
                hitFXManager.concreteDecal_pool[decalIndex_concrete].SetActive(true);
                var decalPostion = hit.point + hit.normal * 0.025f;
                hitFXManager.concreteDecal_pool[decalIndex_concrete].transform.position = decalPostion;
                hitFXManager.concreteDecal_pool[decalIndex_concrete].transform.rotation = Quaternion.FromToRotation(-Vector3.forward, hit.normal);
                if (applyParent)
                    hitFXManager.concreteDecal_pool[decalIndex_concrete].transform.parent = hit.transform;

                decalIndex_concrete++;

                if (decalIndex_concrete == hitFXManager.decalsPoolSizeForEachType)
                {
                    decalIndex_concrete = 0;
                }
            }
        }

        public Vector3 GetRandomPosition(Vector3 origin, float dist, int layermask)
        {
            Vector3 randomDirection = Random.insideUnitSphere * dist;
            randomDirection += transform.position;
            NavMeshHit hit;
            Vector3 finalPosition = transform.forward;
            if (NavMesh.SamplePosition(randomDirection, out hit, dist, layermask))
            {
                finalPosition = hit.position;
            }
            return finalPosition;
        }

        #endregion

        #region Character controller

        /// <summary>
        /// Calculate object postion relative to npc's forward (left, right, ahead, behind)
        /// </summary>
        /// <param name="myTransform"></param>
        /// <param name="targetTransform"></param>
        Vector2 CalculateRelativeDirection(Transform myTransform, Transform targetTransform)
        {
            var direction = new Vector2();

            /// If direction.y > 0 -> object is ahead of npc
            /// If direction.y < 0 -> object is behind of npc
            /// If direction.x > 0 -> object is right of npc
            /// If direction.x < 0 -> object is left of npc

            direction.y = Vector3.Dot(myTransform.TransformDirection(Vector3.forward), targetTransform.position - myTransform.position);
            direction.x = Vector3.Dot(myTransform.TransformDirection(Vector3.right), targetTransform.position - myTransform.position);

            return direction;
        }

        public void Move(Vector3 move, bool crouch, bool jump)
        {
            animator.SetBool("LookAtTarget", lookAtTarget);
            // convert the world relative moveInput vector into a local-relative
            // turn amount and forward amount required to head in the desired
            // direction.
            if (move.magnitude > 1f) move.Normalize();
            move = transform.InverseTransformDirection(move);
            CheckGroundStatus();
            move = Vector3.ProjectOnPlane(move, m_GroundNormal);

            /*
            if (lookAtTarget == false)
            {
                m_TurnAmount = Mathf.Atan2(move.x, move.y);
            }*/

            m_ForwardAmount = move.z;

            ApplyExtraTurnRotation();

            // control and velocity handling is different when grounded and airborne:
            if (m_IsGrounded)
            {
                HandleGroundedMovement(crouch, jump);
            }
            else
            {
                HandleAirborneMovement();
            }

            //ScaleCapsuleForCrouching(crouch);

            // send input and other state parameters to the animator
            UpdateAnimator(move);
        }


        void ScaleCapsuleForCrouching(bool crouch)
        {
            if (m_IsGrounded && crouch)
            {
                if (m_Crouching) return;
                m_Capsule.height = m_Capsule.height / 2f;
                m_Capsule.center = m_Capsule.center / 2f;
                m_Crouching = true;
            }
            else
            {
                Ray crouchRay = new Ray(m_Rigidbody.position + Vector3.up * m_Capsule.radius * k_Half, Vector3.up);
                float crouchRayLength = m_CapsuleHeight - m_Capsule.radius * k_Half;
                if (Physics.SphereCast(crouchRay, m_Capsule.radius * k_Half, crouchRayLength, LayerMask.NameToLayer("Default"), QueryTriggerInteraction.Ignore))
                {
                    m_Crouching = true;
                    return;
                }
                m_Capsule.height = m_CapsuleHeight;
                m_Capsule.center = m_CapsuleCenter;
                m_Crouching = false;
            }
        }

        void UpdateAnimator(Vector3 move)
        {
            // update the animator parameters
            animator.SetFloat("Forward", m_ForwardAmount, 0.1f, Time.deltaTime);
            animator.SetFloat("Turn", m_TurnAmount, 0.1f, Time.deltaTime);
            animator.SetFloat("Horizontal", move.x, 0.1f, Time.deltaTime);
            animator.SetBool("OnGround", m_IsGrounded);
            if (!m_IsGrounded)
            {
                animator.SetFloat("Jump", m_Rigidbody.velocity.y);
            }

            // calculate which leg is behind, so as to leave that leg trailing in the jump animation
            // (This code is reliant on the specific run cycle offset in our animations,
            // and assumes one leg passes the other at the normalized clip times of 0.0 and 0.5)
            float runCycle =
                Mathf.Repeat(
                    animator.GetCurrentAnimatorStateInfo(0).normalizedTime + m_RunCycleLegOffset, 1);
            float jumpLeg = (runCycle < k_Half ? 1 : -1) * m_ForwardAmount;
            if (m_IsGrounded)
            {
                animator.SetFloat("JumpLeg", jumpLeg);
            }

            // the anim speed multiplier allows the overall speed of walking/running to be tweaked in the inspector,
            // which affects the movement speed because of the root motion.
            if (m_IsGrounded && move.magnitude > 0)
            {
                animator.speed = m_AnimSpeedMultiplier;
            }
            else
            {
                // don't use that while airborne
                animator.speed = 1;
            }
        }


        void HandleAirborneMovement()
        {
            // apply extra gravity from multiplier:
            Vector3 extraGravityForce = (Physics.gravity * m_GravityMultiplier) - Physics.gravity;
            m_Rigidbody.AddForce(extraGravityForce);

            m_GroundCheckDistance = m_Rigidbody.velocity.y < 0 ? m_OrigGroundCheckDistance : 0.01f;
        }


        void HandleGroundedMovement(bool crouch, bool jump)
        {
            // check whether conditions are right to allow a jump:
            if (jump && !crouch && animator.GetCurrentAnimatorStateInfo(0).IsName("Grounded"))
            {
                // jump!
                m_Rigidbody.velocity = new Vector3(m_Rigidbody.velocity.x, m_JumpPower, m_Rigidbody.velocity.z);
                m_IsGrounded = false;
                animator.applyRootMotion = false;
                m_GroundCheckDistance = 0.1f;
            }
        }

        void ApplyExtraTurnRotation()
        {
            // help the character turn faster (this is in addition to root rotation in the animation)
            float turnSpeed = Mathf.Lerp(m_StationaryTurnSpeed, m_MovingTurnSpeed, m_ForwardAmount);
            transform.Rotate(0, m_TurnAmount * turnSpeed * Time.deltaTime, 0);
        }


        public void OnAnimatorMove()
        {
            // we implement this function to override the default root motion.
            // this allows us to modify the positional speed before it's applied.
            if (m_IsGrounded && Time.deltaTime > 0)
            {
                Vector3 v = (animator.deltaPosition * m_MoveSpeedMultiplier) / Time.deltaTime;

                // we preserve the existing y part of the current velocity.
                v.y = m_Rigidbody.velocity.y;
                m_Rigidbody.velocity = v;
            }
        }
        void CheckGroundStatus()
        {
            RaycastHit hitInfo;
#if UNITY_EDITOR
            // helper to visualise the ground check ray in the scene view
            Debug.DrawLine(transform.position + (Vector3.up * 0.1f), transform.position + (Vector3.up * 0.1f) + (Vector3.down * m_GroundCheckDistance));
#endif
            // 0.1f is a small offset to start the ray from inside the character
            // it is also good to note that the transform position in the sample assets is at the base of the character
            if (Physics.Raycast(transform.position + (Vector3.up * 0.1f), Vector3.down, out hitInfo, m_GroundCheckDistance))
            {
                m_GroundNormal = hitInfo.normal;
                m_IsGrounded = true;
                animator.applyRootMotion = true;
            }
            else
            {
                m_IsGrounded = false;
                m_GroundNormal = Vector3.up;
                animator.applyRootMotion = false;
            }
        }
        #endregion
    }

}