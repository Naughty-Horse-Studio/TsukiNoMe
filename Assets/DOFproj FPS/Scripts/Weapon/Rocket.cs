/// Deacon of Freedom Development (2020) v1
/// If you have any questions feel free to write me at email --- Phil-James_Lapuz@outlook.com ---

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DOFprojFPS
{
    public class Rocket : MonoBehaviour
    {
        public float startReactiveForce = 1500f;
        public float explosionForce;
        public float damageRadius;
        public float damage;

        public GameObject explosionEffects;

        private float time;

        private float livingTime = 5f;

        Vector3 lastPosition;

        public Weapon weapon;
        Collider[] colliders;
        GameObject effects_temp;
        
        public void BeginMovement()
        {
            GetComponent<Rigidbody>().AddForce(transform.forward * startReactiveForce, ForceMode.Impulse);
            lastPosition = transform.position;
            effects_temp = Instantiate(explosionEffects);
            effects_temp.SetActive(false);
        }

        private void Update()
        {
            time += Time.deltaTime;

            RaycastHit hit;
            if (Physics.Linecast(lastPosition, transform.position, out hit))
            {
                Explosion(hit, true);
            }

            lastPosition = transform.position;

            if (time > livingTime)
            {
                Explosion(new RaycastHit(), false);
            }
        }
        
        void Explosion(RaycastHit hit, bool useHit)
        {
            print("Rocket Explosion");

            colliders = Physics.OverlapSphere(transform.position, damageRadius);

            foreach (Collider collider in colliders)
            {

                if (collider.GetComponent<NPC>())
                {
                    collider.GetComponent<NPC>().GetHit((int)damage, GameObject.Find("Player").transform);
                }

                if (collider.GetComponent<ZombieNPC>())
                {
                    collider.GetComponent<ZombieNPC>().ApplyHit((int)damage);
                }

                if (collider.GetComponent<ObjectHealth>())
                {
                    collider.GetComponent<ObjectHealth>().health -= damage;
                }

                if (collider.GetComponent<PlayerStats>())
                {
                    collider.GetComponent<PlayerStats>().health -= (int)damage;
                }
                
                if (collider.GetComponent<Rigidbody>())
                {
                    if(useHit)
                        collider.GetComponent<Rigidbody>().AddExplosionForce(explosionForce, hit.point, damageRadius);
                    else
                        collider.GetComponent<Rigidbody>().AddExplosionForce(explosionForce, transform.position, damageRadius);

                }
            }

            if (useHit)
            {
                effects_temp.transform.position = hit.point;
            }
            else
            {
                effects_temp.transform.position = transform.position;
            }

            effects_temp.SetActive(true);

            Destroy(gameObject);
        }
    }
}
