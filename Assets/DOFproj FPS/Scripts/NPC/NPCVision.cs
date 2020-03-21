/// Deacon of Freedom Development (2020) v1
/// If you have any questions feel free to write me at email --- Phil-James_Lapuz@outlook.com ---

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Here we looking for targets around NPC and calculate closest target

namespace DOFprojFPS
{
    public class NPCVision : MonoBehaviour
    {
        [Tooltip("How often vision will check if player is in sight")]
        public float visibilityCheckInterval = 0.1f;
        [Tooltip("NPC field of fiev range")]
        public float FOV = 100;
        [Tooltip("How far NPC can look")]
        public float detectionRange = 30;

        private Transform player;
        private SphereCollider rangeCollider;
        
        public int targetFraction;

        public List<Collider> objectsInRange;
        public List<Collider> potentialTargets;
        public List<Collider> visibleTargets;

        public bool playerIsFriendly;

        public LayerMask layerMaskForVision;
        public LayerMask layerMaskForOverlap;
        
        private float timer;

        private NPC npc;

        private void Start()
        {
            npc = GetComponentInParent<NPC>();
        }

        private void Update()
        {
            if (Time.time > timer + visibilityCheckInterval)
            {
                objectsInRange = new List<Collider>();
                visibleTargets = new List<Collider>();
                potentialTargets = new List<Collider>();

                var col = Physics.OverlapSphere(transform.position, detectionRange, layerMaskForOverlap);
                objectsInRange.AddRange(col);

                RaycastHit hit;
                
                foreach (var obj in objectsInRange)
                {
                    if (obj == null)
                        objectsInRange.Remove(obj);

                    if (obj.CompareTag("MainCamera") && !playerIsFriendly)
                    {
                        Vector3 direction = obj.transform.position - transform.position;
                        float angle = Vector3.Angle(direction, transform.forward);

                        Debug.DrawRay(transform.position, direction * 100);

                        if (angle < FOV * 0.5f)
                        {
                            if (Physics.Raycast(transform.position, direction, out hit, detectionRange, layerMaskForVision))
                            {
                                if (hit.collider == obj)
                                {
                                    if (!visibleTargets.Contains(obj))
                                    {
                                        visibleTargets.Add(obj);
                                        potentialTargets.Add(obj);
                                    }
                                }
                                else
                                {
                                    if (visibleTargets.Contains(obj))
                                        visibleTargets.Remove(obj);
                                }
                            }
                        }
                    }

                    else if (obj.GetComponent<NPC>() != null && obj.GetComponent<NPC>().myFraction == targetFraction)
                    {
                        Vector3 direction = (obj.transform.position + Vector3.up) - transform.position;
                        float angle = Vector3.Angle(direction, transform.forward);

                        Debug.DrawRay(transform.position, direction * 100);

                        if (angle < FOV * 0.5f)
                        {
                            if (Physics.Raycast(transform.position, direction, out hit, detectionRange, layerMaskForVision))
                            {

                                if (hit.collider == obj)
                                {
                                    if (!visibleTargets.Contains(obj))
                                    {
                                        visibleTargets.Add(obj);
                                        potentialTargets.Add(obj);
                                    }
                                }
                                else
                                {
                                    if (visibleTargets.Contains(obj))
                                        visibleTargets.Remove(obj);
                                }
                            }
                        }
                    }
                    else
                    {
                        if (visibleTargets.Contains(obj))
                            visibleTargets.Remove(obj);
                    }

                    if (obj.gameObject.layer == LayerMask.NameToLayer("Dead"))
                    {
                        if (visibleTargets.Contains(obj))
                            visibleTargets.Remove(obj);
                        if (potentialTargets.Contains(obj))
                            visibleTargets.Remove(obj);
                    }
                }
                timer = Time.time;
            }
        }

        // Get target here!
        public Collider SelectBestTarget(bool visibleOnly)
        {
                // If we have targets in sight then we will select from them closest target
                if (visibleTargets != null)
                {
                    Collider closestTarget = null;
                    float bestDistance = 100f;

                    foreach (var target in visibleTargets)
                    {
                        if (target == null)
                            break;

                        var distance = Vector3.Distance(transform.position, target.transform.position);

                        if (distance < bestDistance)
                        {
                            closestTarget = target;
                            bestDistance = distance;
                        }
                    }

                    return closestTarget;
                }

            if (visibleOnly)
                return null;
            
            // If we have no visible targets, we will take one from visible before
            if(potentialTargets != null)
            {
                Collider closestTarget = null;
                float bestDistance = 100f;

                foreach (var target in potentialTargets)
                {
                    var distance = Vector3.Distance(transform.position, target.transform.position);

                    if (distance < bestDistance)
                    {
                        closestTarget = target;
                        bestDistance = distance;
                    }
                }

                return closestTarget;
            }

            // And finaly if we have not any targets then we just return null;
            return null;
        }
    }
}