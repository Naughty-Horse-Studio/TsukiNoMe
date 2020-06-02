using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerPDA : MonoBehaviour
{
    public GameObject gameObjec1;
    public GameObject gameObjec2;
    public GameObject gameObjec3;
    public GameObject gameObjec4;
    public GameObject gameObjec5;
    public void Step1_Pickup()
    {
        gameObjec1.SetActive(false);
        gameObjec2.SetActive(true);
    }

    public void Step2_Pickup()
    {
        gameObjec2.SetActive(false);
        gameObjec3.SetActive(true);
    }

    public void Step3_Pickup()
    {
        gameObjec3.SetActive(false);
        gameObjec4.SetActive(true);
    }

    public void Step4_Pickup()
    {
        gameObjec4.SetActive(false);
        gameObjec5.SetActive(true);
    }
}
