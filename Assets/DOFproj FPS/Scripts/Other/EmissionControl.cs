using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EmissionControl : MonoBehaviour
{
    public Material material;

    void Awake()
    {
        material.DisableKeyword("_EMISSION");
    }


    private float time = 0f;
    private bool emit = false;

    void Update()
    {
        if (time >= 3.0f)
        {
            emit = !emit;
            if (emit)
                material.EnableKeyword("_EMISSION");
            else
                material.DisableKeyword("_EMISSION");
            time = 0f;
        }

        time += Time.deltaTime;
    }
}
