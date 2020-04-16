/// Deacon of Freedom Development (2020) v1
/// If you have any questions feel free to write me at email --- Phil-James_Lapuz@outlook.com ---

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DOFprojFPS {

    public class Grenade : MonoBehaviour
    {
        public float explosionTimer;
        public float explosionForce;
        public float damageRadius;
        public float damage;

        public GameObject explosionEffects;

        Collider[] colliders;
        GameObject effects_temp;

        void OnEnable()
        {
            if (explosionEffects != null)
            {
                effects_temp = Instantiate(explosionEffects);
                effects_temp.SetActive(false);
            }

            StartCoroutine(Timer(explosionTimer));
        }
        
        IEnumerator Timer(float explosionTimer)
        {
                yield return new WaitForSeconds(explosionTimer);
                print("Coroutine ended");
                Explosion();
        }

        void Explosion()
        {
            print("Explosion");

            colliders = Physics.OverlapSphere(transform.position, damageRadius);

            foreach (Collider collider in colliders)
            {
                if (collider.GetComponent<Turret>())
                {
                    collider.GetComponent<Turret>().ApplyHit((int)damage);
                }

                if (collider.GetComponent<NPC>())
                {
                    collider.GetComponent<NPC>().GetHit((int)damage, GameObject.Find("Player").transform);
                }

                if(collider.GetComponent<ZombieNPC>())
                {
                    collider.GetComponent<ZombieNPC>().ApplyHit((int)damage);
                }

                if(collider.GetComponent<ObjectHealth>())
                {
                    collider.GetComponent<ObjectHealth>().health -= damage;
                }





                if (collider.GetComponent<Rigidbody>()!= null)
                {
                    RaycastHit hit;
                    if(Physics.Raycast(transform.position,collider.transform.position-transform.position,out hit, Mathf.Infinity))
                    {
                        if(hit.collider == collider)
                        {
                            collider.GetComponent<Rigidbody>().AddExplosionForce(explosionForce, transform.position, damageRadius);

                            //if (collider.GetComponentInParent<PlayerStats>())
                            //{
                                Transform _damageSender = GameObject.Find("Player").transform;
                                _damageSender.GetComponent<PlayerStats>().health -= (int)damage;
                            //}
                        }
                    }
                

               

                }
            }

            if (explosionEffects != null)
            {
                effects_temp.transform.position = transform.position;
                effects_temp.transform.rotation = transform.rotation;

                effects_temp.SetActive(true);
            }

            Destroy(gameObject);
        }
    }
}
