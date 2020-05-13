using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DistanceDisable : MonoBehaviour
{
    Transform player;
    public GameObject objToDisable;




    public float distanceToDisable = 50;

    private void Start()
    {
        player = GameObject.Find("Player").transform;
    }

    void Update()
    {
        if(objToDisable.activeInHierarchy && Vector3.Distance(transform.position, player.transform.position) > distanceToDisable)
        {
            objToDisable.SetActive(false);
        }
        else
        {
            objToDisable.SetActive(true);
        }

        //++++++++++++++++++++++++++++++++++++++++++++++++
        //objNPC_1


    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, distanceToDisable);
    }
}
