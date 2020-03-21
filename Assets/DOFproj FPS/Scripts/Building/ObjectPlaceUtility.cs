/// Deacon of Freedom Development (2020) v1
/// If you have any questions feel free to write me at email --- Phil-James_Lapuz@outlook.com ---
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPlaceUtility : MonoBehaviour
{
    public bool canPlaceObject = false;

    public string tagForObjectPlace = "Terrain";
    
    private void Update()
    {
        RaycastHit hit;

        Debug.DrawRay(transform.position, -transform.up);

        if (Physics.Raycast(transform.position, -transform.up, out hit, 0.2f))
        {
            if (hit.collider.tag == tagForObjectPlace)
            {
                canPlaceObject = true;
            }
            else
            {
                canPlaceObject = false;
            }
        }
        else
            canPlaceObject = false;
    }
}
