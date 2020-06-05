/// Deacon of Freedom Development (2020) v1
/// If you have any questions feel free to write me at email --- Phil-James_Lapuz@outlook.com ---

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace DOFprojFPS
{
    public class ZombieNPC : MonoBehaviour
    {
        public float health = 100f;
        public int maxDamage = 10;

        public bool isAgressive;
        
        public AudioClip roar;
        public AudioClip[] attackSounds;

        public AudioClip[] ragdollSounds;

        [Header("Wandering parameters")]
        public float wanderingSpeed = 0.6f;
        public float wanderingTimer = 13;
        public float wanderingRadius = 10;

        public float attackDistance = 3f;
        public GameObject zombieRagdoll;
        public float runSpeed = 0.9f;

        float timerW;

        bool isVisible;
        NavMeshAgent agent;
        Animator animator;
        AudioSource audioSource;
        NPCVision vision;

        public bool notRoared = true;

        CapsuleCollider capsule;
        List<ZombieNPC> friendsList;

        private GameObject target;
        private Transform player;
        public PlayerStats mPPlayerStats;
        float timer;

        [HideInInspector]
        public bool isWorried = false;

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
        private bool lookAtTarget = false;
        #endregion

        private bool _firstContact = false;
        Collider[] colliders;

        public float damageRadius;
        private void OnEnable()
        {
            audioSource = GetComponent<AudioSource>();
            agent = GetComponentInChildren<NavMeshAgent>();
            animator = GetComponent<Animator>();
            player = GameObject.FindGameObjectWithTag("Player").transform;

            mPPlayerStats = player.gameObject.GetComponent<PlayerStats>();
            capsule = GetComponent<CapsuleCollider>();
            vision = GetComponentInChildren<NPCVision>();
        }

        private void Start()
        {
            audioSource = GetComponent<AudioSource>();
            agent = GetComponentInChildren<NavMeshAgent>();
            animator = GetComponent<Animator>();
            player = GameObject.FindGameObjectWithTag("Player").transform;
            mPPlayerStats = player.gameObject.GetComponent<PlayerStats>();

            capsule = GetComponent<CapsuleCollider>();

            StartCoroutine(CheckFriends());

            foreach (Collider collider in GetComponentsInChildren<Collider>())
            {
                collider.enabled = false;
            }
            foreach (Rigidbody rigidbody in GetComponentsInChildren<Rigidbody>())
            {
                rigidbody.isKinematic = true;
            }

            GetComponent<Collider>().enabled = true;

            //Controller variables
            m_Rigidbody = GetComponent<Rigidbody>();
            m_Capsule = GetComponent<CapsuleCollider>();
            m_CapsuleHeight = m_Capsule.height;
            m_CapsuleCenter = m_Capsule.center;

            m_Rigidbody.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationY | RigidbodyConstraints.FreezeRotationZ;
            m_OrigGroundCheckDistance = m_GroundCheckDistance;
        }

        private void Update()
        {
            if (health > 0)
            {
                if (Vector3.Distance(transform.position, player.position) < 25 || isWorried)
                {
                    if (isAgressive)
                    {
                        Shout();
                        Roar();
                        agent.speed = runSpeed;
                        agent.SetDestination(player.position);
                        if (Vector3.Distance(transform.position, player.position) < attackDistance)
                        {


                            animator.Play("Attack");

                            GetComponent<AudioSource>().PlayOneShot(attackSounds[Random.Range(0, attackSounds.Length)]);

                            //    StartCoroutine(Attack());

                        }
                    }
                }
                else
                {
                    notRoared = true;
                    Wandering();
                }
            }
            else
            {
                GetComponent<AudioSource>().PlayOneShot(ragdollSounds[Random.Range(0, ragdollSounds.Length)]);
                Death();
            }

        }
        IEnumerator Attack()
        {
            if (_firstContact)
                player.gameObject.GetComponent<PlayerStats>().ApplyDamage(maxDamage);
            //GetComponent<AudioSource>().PlayOneShot(attackSounds[Random.Range(0, attackSounds.Length)]);




            yield return null;

            _firstContact = false;

           // FightHit();

        }
        public void Roar()
        {
            if (notRoared)
            {
                GetComponent<AudioSource>().PlayOneShot(roar);
                notRoared = false;
            }
        }

        private void Death()
        {

         
             
          

            foreach (Collider collider in GetComponentsInChildren<Collider>())
            {
                collider.enabled = true;
            }
            foreach (Rigidbody rigidbody in GetComponentsInChildren<Rigidbody>())
            {
                rigidbody.isKinematic = false;
            }

            GetComponent<Collider>().enabled = false;
            GetComponent<AIControl>().enabled = false;

            agent.enabled = false;
            animator.enabled = false;
            Destroy(this);
        }

        public Vector3 RandomNavSphere(Vector3 origin, float dist, int layermask)
        {
            Vector3 randomDirection = Random.insideUnitSphere * dist;
            randomDirection += transform.position;
            NavMeshHit hit;
            Vector3 finalPosition = Vector3.zero;
            if (NavMesh.SamplePosition(randomDirection, out hit, dist, layermask))
            {
                finalPosition = hit.position;
            }
            return finalPosition;
        }

        public void Wandering()
        {
            agent.speed = wanderingSpeed;

            timerW += Time.deltaTime;
            if (timerW >= wanderingTimer)
            {
                agent.SetDestination(RandomNavSphere(transform.position, wanderingRadius, 1));
                timerW = 0;
            }
            if (!agent.hasPath)
            {
                agent.SetDestination(RandomNavSphere(transform.position, wanderingRadius, 1));
            }
        }

        public void IsVisible(bool visible)
        {
            isVisible = visible;
        }

        public void FightHit()
        {


            _firstContact = true;

            //if (Vector3.Distance(transform.position, player.transform.position) < 4)
            //{



            //mPPlayerStats = player.gameObject.GetComponent<PlayerStats>();
            //if (mPPlayerStats != null)
            //{
            // //   mPPlayerStats.ApplyDamage(maxDamage);
            //  //  Debug.Log("hit Here");

            //    // audioSource.PlayOneShot(attackSounds[Random.Range(0, attackSounds.Length)]);
            //    GetComponent<AudioSource>().PlayOneShot(attackSounds[Random.Range(0, attackSounds.Length)]);




            //}

            //}
        }

        private void Shout()
        {
            if (friendsList != null)
            {
                foreach (ZombieNPC friend in friendsList)
                {
                    if (friend != null && !friend.isWorried)
                    {
                        friend.isWorried = true;
                    }
                }
            }
        }

        IEnumerator CheckFriends()
        {
            for (; ; )
            {
                GetFriendsAround();
                yield return new WaitForSeconds(.5f);
            }
        }

        public void ApplyHit(int damage)
        {
            isWorried = true;
            Shout();
            health -= damage;
        }

        public void Explosivex()
        {
            print("Explosion");

            colliders = Physics.OverlapSphere(transform.position, damageRadius);

            foreach (Collider collider in colliders)
            {
                if (collider.GetComponentInParent<PlayerStats>())
                {
                    collider.GetComponentInParent<PlayerStats>().ApplyDamage(maxDamage);
                }

                //if (collider.GetComponent<NPC>())
                //{
                //    collider.GetComponent<NPC>().GetHit((int)damage, GameObject.Find("Player").transform);
                //}

                //if (collider.GetComponent<ZombieNPC>())
                //{
                //    collider.GetComponent<ZombieNPC>().ApplyHit((int)damage);
                //}

                //if (collider.GetComponent<ObjectHealth>())
                //{
                //    collider.GetComponent<ObjectHealth>().health -= damage;
                //}





                //if (collider.GetComponent<Rigidbody>() != null)
                //{
                //    RaycastHit hit;
                //    if (Physics.Raycast(transform.position, collider.transform.position - transform.position, out hit, Mathf.Infinity))
                //    {
                //        if (hit.collider == collider)
                //        {

                //            if (collider.GetComponentInParent<PlayerStats>())
                //            {
                //                Transform _damageSender = GameObject.Find("Player").transform;
                //                _damageSender.GetComponent<PlayerStats>().health -= (int)maxDamage;
                //            }
                //        }
                //    }




                //}
            }


        }

        private void GetFriendsAround()
        {
            if (friendsList != null)
            {
                friendsList.Clear();
                friendsList.Capacity = 0;
            }

            var colliders = Physics.OverlapSphere(transform.position, 50f);
            foreach (Collider _collider in colliders)
            {
                if (_collider.gameObject.layer == this.gameObject.layer)
                {
                    if (friendsList != null && !friendsList.Contains(_collider.GetComponent<ZombieNPC>()))
                    {
                        if (_collider.gameObject != this.gameObject)
                        {
                            friendsList.Add(_collider.GetComponent<ZombieNPC>());
                        }
                    }
                }
            }
        }

        #region Character Controller

        public void Move(Vector3 move, bool crouch, bool jump)
        {
            //animator.SetBool("LookAtTarget", lookAtTarget);
            // convert the world relative moveInput vector into a local-relative
            // turn amount and forward amount required to head in the desired
            // direction.
            if (move.magnitude > 1f) move.Normalize();
            move = transform.InverseTransformDirection(move);
            CheckGroundStatus();
            move = Vector3.ProjectOnPlane(move, m_GroundNormal);

            if (!lookAtTarget)
                m_TurnAmount = Mathf.Atan2(move.x, move.z);
            else
            {
                var lookPosition = player.transform.position - transform.position;
                lookPosition.y = 0;
                var rotation = Quaternion.LookRotation(lookPosition);
                transform.rotation = Quaternion.Slerp(transform.rotation, rotation, Time.deltaTime * 4);

            }


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

            ScaleCapsuleForCrouching(crouch);

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
            //animator.SetFloat("Horizontal", move.x, 0.1f, Time.deltaTime);
            animator.SetBool("Crouch", m_Crouching);
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
