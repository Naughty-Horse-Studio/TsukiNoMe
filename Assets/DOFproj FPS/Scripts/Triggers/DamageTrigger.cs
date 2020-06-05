using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DOFprojFPS;
public class DamageTrigger : MonoBehaviour
{
    // Inspector Variables
    public float damageRadius;
    [SerializeField] int _damageAmount = 20;

    Collider[] colliders;

    PlayerStats mPPlayerStats;

    public Animator _animator;
    int _parameterHash = -1;

    private bool _firstContact = false;


    public void Explosivey()
    {
        print("Explosion");

        colliders = Physics.OverlapSphere(transform.position, damageRadius);

        foreach (Collider collider in colliders)
        {
            //if (collider.GetComponent<PlayerStats>())
            //{
            //    collider.GetComponent<PlayerStats>().(Random.Range(1, _damageAmount));
            //}

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





            if (collider.GetComponent<Rigidbody>() != null)
            {
                RaycastHit hit;
                if (Physics.Raycast(transform.position, collider.transform.position - transform.position, out hit, Mathf.Infinity))
                {
                    if (hit.collider == collider)
                    {

                        if (collider.GetComponentInParent<PlayerStats>())
                        {
                            Transform _damageSender = GameObject.Find("Player").transform;
                            _damageSender.GetComponent<PlayerStats>().health -= (int)_damageAmount;
                        }
                    }
                }




            }
        }


    }
    // -------------------------------------------------------------
    // Name	:	OnTriggerStay
    // Desc	:	Called by Unity each fixed update that THIS trigger
    //			is in contact with another.
    // -------------------------------------------------------------
    void OnTriggerStay(Collider col)
    {

        Debug.Log("buddy 1");
        // If we don't have an animator return
        if (!_animator)
            return;
        Debug.Log("buddy 2");
        // If this is the player object and our parameter is set for damage
        //if (col.gameObject.CompareTag("Player") && _animator.GetFloat(_parameterHash) > 0.9f)
        //{
        //    if (GameSceneManager.instance && GameSceneManager.instance.bloodParticles)
        //    {
        //        ParticleSystem system = GameSceneManager.instance.bloodParticles;

        //        // Temporary Code
        //        system.transform.position = transform.position;
        //        system.transform.rotation = Camera.main.transform.rotation;

        //        var settings = system.main;
        //        settings.simulationSpace = ParticleSystemSimulationSpace.World;
        //        system.Emit(_bloodParticlesBurstAmount);
        //    }
        if (col.gameObject.CompareTag("Player") && (_firstContact))
        { 
            Debug.Log("hit Here 1");
        mPPlayerStats = col.GetComponent<PlayerStats>();
            if ((mPPlayerStats != null))
            {
                mPPlayerStats.ApplyDamage(Random.Range(1, _damageAmount));
                Debug.Log("hit Here 2");

            }

            _firstContact = false;
        }
    }
}
