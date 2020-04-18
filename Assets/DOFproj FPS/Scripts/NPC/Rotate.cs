using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rotate : MonoBehaviour
{
    // Inspector Assigned
    public GameObject target;

    void Update()
    {
        // Spin the object around the world origin at 20 degrees/second.
        transform.RotateAround(target.transform.position, Vector3.up, 30 * Time.deltaTime);
    }
}
