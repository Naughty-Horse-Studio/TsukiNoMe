using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeliRotor : MonoBehaviour
{
    public float speed = 360;

    void Update()
    {
        transform.Rotate(0, Time.deltaTime * speed, 0, Space.Self);
    }
}
