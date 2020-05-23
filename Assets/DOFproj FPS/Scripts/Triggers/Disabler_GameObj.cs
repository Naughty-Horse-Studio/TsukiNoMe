using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Disabler_GameObj : MonoBehaviour
{
    Transform player;
    public GameObject objToDisable_1;
    public GameObject objToDisable_2;
    public GameObject objToDisable_3;
    public GameObject objToDisable_4;
    public GameObject objToDisable_5;
    public GameObject objToDisable_6;
    public GameObject objToDisable_7;
    public GameObject objToDisable_8;
    public GameObject objToDisable_9;
    public GameObject objToDisable_10;
    public GameObject objToDisable_11;
    public GameObject objToDisable_12;
    public GameObject objToDisable_13;
    public GameObject objToDisable_14;
    public GameObject objToDisable_15;
    public GameObject objToDisable_16;
    public GameObject objToDisable_17;
    public GameObject objToDisable_18;
    public GameObject objToDisable_19;
    public GameObject objToDisable_20;
    public GameObject objToDisable_21;
    public GameObject objToDisable_22;
    public GameObject objToDisable_23;
    public GameObject objToDisable_24;
    public GameObject objToDisable_25;
    public GameObject objToDisable_26;

    public GameObject objToDisable_27;
    public GameObject objToDisable_28;
    public GameObject objToDisable_29;
    public GameObject objToDisable_30;
    public GameObject objToDisable_31;

    public GameObject objToDisable_32;
    public GameObject objToDisable_33;
    public float distanceToDisable = 50;

    private void Start()
    {
        player = GameObject.Find("Player").transform;
    }

    void Update()
    {
        if (Vector3.Distance(transform.position, player.transform.position) > distanceToDisable)
        {
            if (objToDisable_1 != null && objToDisable_1.activeInHierarchy)
            {
                objToDisable_1.SetActive(false);
            }
            
            if (objToDisable_2 != null && objToDisable_2.activeInHierarchy)
            {
                objToDisable_2.SetActive(false);
            }

            if (objToDisable_3 != null && objToDisable_3.activeInHierarchy)
            {
                objToDisable_3.SetActive(false);
            }

            if (objToDisable_4 != null && objToDisable_4.activeInHierarchy)
            {
                objToDisable_4.SetActive(false);
            }

            if (objToDisable_5 != null && objToDisable_5.activeInHierarchy)
            {
                objToDisable_5.SetActive(false);
            }

            if (objToDisable_6 != null && objToDisable_6.activeInHierarchy)
            {
                objToDisable_6.SetActive(false);
            }

            if (objToDisable_7 != null && objToDisable_7.activeInHierarchy)
            {
                objToDisable_7.SetActive(false);
            }
            if (objToDisable_8 != null && objToDisable_8.activeInHierarchy)
            {
                objToDisable_8.SetActive(false);
            }
            if (objToDisable_9 != null && objToDisable_9.activeInHierarchy)
            {
                objToDisable_9.SetActive(false);
            }
            if (objToDisable_10 != null && objToDisable_10.activeInHierarchy)
            {
                objToDisable_10.SetActive(false);
            }
            if (objToDisable_11 != null && objToDisable_11.activeInHierarchy)
            {
                objToDisable_11.SetActive(false);
            }
            if (objToDisable_12 != null && objToDisable_12.activeInHierarchy)
            {
                objToDisable_12.SetActive(false);
            }
            if (objToDisable_13 != null && objToDisable_13.activeInHierarchy)
            {
                objToDisable_13.SetActive(false);
            }
            if (objToDisable_14 != null && objToDisable_14.activeInHierarchy)
            {
                objToDisable_14.SetActive(false);
            }
            if (objToDisable_15 != null && objToDisable_15.activeInHierarchy)
            {
                objToDisable_15.SetActive(false);
            }
            if (objToDisable_16 != null && objToDisable_16.activeInHierarchy)
            {
                objToDisable_16.SetActive(false);
            }
            if (objToDisable_17 != null && objToDisable_17.activeInHierarchy)
            {
                objToDisable_17.SetActive(false);
            }
            if (objToDisable_18 != null && objToDisable_18.activeInHierarchy)
            {
                objToDisable_18.SetActive(false);
            }
            if (objToDisable_19 != null && objToDisable_19.activeInHierarchy)
            {
                objToDisable_19.SetActive(false);
            }
            if (objToDisable_20 != null && objToDisable_20.activeInHierarchy)
            {
                objToDisable_20.SetActive(false);
            }
            if (objToDisable_21 != null && objToDisable_21.activeInHierarchy)
            {
                objToDisable_21.SetActive(false);
            }
            if (objToDisable_22 != null && objToDisable_22.activeInHierarchy)
            {
                objToDisable_22.SetActive(false);
            }
            if (objToDisable_23 != null && objToDisable_23.activeInHierarchy)
            {
                objToDisable_23.SetActive(false);
            }
            if (objToDisable_24 != null && objToDisable_24.activeInHierarchy)
            {
                objToDisable_24.SetActive(false);
            }
            if (objToDisable_26 != null && objToDisable_26.activeInHierarchy)
            {
                objToDisable_26.SetActive(false);
            }
            if (objToDisable_25 != null && objToDisable_25.activeInHierarchy)
            {
                objToDisable_25.SetActive(false);
            }
            if (objToDisable_27 != null && objToDisable_27.activeInHierarchy)
            {
                objToDisable_27.SetActive(false);
            }
            if (objToDisable_28 != null && objToDisable_28.activeInHierarchy)
            {
                objToDisable_28.SetActive(false);
            }
            if (objToDisable_29 != null && objToDisable_29.activeInHierarchy)
            {
                objToDisable_29.SetActive(false);
            }
            if (objToDisable_30 != null && objToDisable_30.activeInHierarchy)
            {
                objToDisable_30.SetActive(false);
            }
            if (objToDisable_31 != null && objToDisable_31.activeInHierarchy)
            {
                objToDisable_31.SetActive(false);
            }
            if (objToDisable_32 != null && objToDisable_32.activeInHierarchy)
            {
                objToDisable_32.SetActive(false);

            }
            if (objToDisable_33 != null && objToDisable_33.activeInHierarchy)
            {
                objToDisable_33.SetActive(false);
            }
        }
        else
        {
            if (objToDisable_1 != null && !objToDisable_1.activeInHierarchy)
            {
                objToDisable_1.SetActive(true);
            }

            if (objToDisable_2 != null && !objToDisable_2.activeInHierarchy)
            {
                objToDisable_2.SetActive(true);
            }

            if (objToDisable_3 != null && !objToDisable_3.activeInHierarchy)
            {
                objToDisable_3.SetActive(true);
            }

            if (objToDisable_4 != null && !objToDisable_4.activeInHierarchy)
            {
                objToDisable_4.SetActive(true);
            }

            if (objToDisable_5 != null && !objToDisable_5.activeInHierarchy)
            {
                objToDisable_5.SetActive(true);
            }

            if (objToDisable_6 != null && !objToDisable_6.activeInHierarchy)
            {
                objToDisable_6.SetActive(true);
            }

            if (objToDisable_7 != null && !objToDisable_7.activeInHierarchy)
            {
                objToDisable_7.SetActive(true);
            }
            if (objToDisable_8 != null && !objToDisable_8.activeInHierarchy)
            {
                objToDisable_8.SetActive(true);
            }
            if (objToDisable_9 != null && !objToDisable_9.activeInHierarchy)
            {
                objToDisable_9.SetActive(true);
            }
            if (objToDisable_10 != null && !objToDisable_10.activeInHierarchy)
            {
                objToDisable_10.SetActive(true);
            }
            if (objToDisable_11 != null && !objToDisable_11.activeInHierarchy)
            {
                objToDisable_11.SetActive(true);
            }
            if (objToDisable_12 != null && !objToDisable_12.activeInHierarchy)
            {
                objToDisable_12.SetActive(true);
            }
            if (objToDisable_13 != null && !objToDisable_13.activeInHierarchy)
            {
                objToDisable_13.SetActive(true);
            }
            if (objToDisable_14 != null && !objToDisable_14.activeInHierarchy)
            {
                objToDisable_14.SetActive(true);
            }
            if (objToDisable_15 != null && !objToDisable_15.activeInHierarchy)
            {
                objToDisable_15.SetActive(true);
            }
            if (objToDisable_16 != null && !objToDisable_16.activeInHierarchy)
            {
                objToDisable_16.SetActive(true);
            }
            if (objToDisable_17 != null && !objToDisable_17.activeInHierarchy)
            {
                objToDisable_17.SetActive(true);
            }
            if (objToDisable_18 != null && !objToDisable_18.activeInHierarchy)
            {
                objToDisable_18.SetActive(true);
            }
            if (objToDisable_19 != null && !objToDisable_19.activeInHierarchy)
            {
                objToDisable_19.SetActive(true);
            }
            if (objToDisable_20 != null && !objToDisable_20.activeInHierarchy)
            {
                objToDisable_20.SetActive(true);
            }
            if (objToDisable_21 != null && !objToDisable_21.activeInHierarchy)
            {
                objToDisable_21.SetActive(true);
            }
            if (objToDisable_22 != null && !objToDisable_22.activeInHierarchy)
            {
                objToDisable_22.SetActive(true);
            }
            if (objToDisable_23 != null && !objToDisable_23.activeInHierarchy)
            {
                objToDisable_23.SetActive(true);
            }
            if (objToDisable_24 != null && !objToDisable_24.activeInHierarchy)
            {
                objToDisable_24.SetActive(true);
            }
            if (objToDisable_26 != null && !objToDisable_26.activeInHierarchy)
            {
                objToDisable_26.SetActive(true);
            }
            if (objToDisable_25 != null && !objToDisable_25.activeInHierarchy)
            {
                objToDisable_25.SetActive(true);
            }
            if (objToDisable_27 != null && !objToDisable_27.activeInHierarchy)
            {
                objToDisable_27.SetActive(true);
            }
            if (objToDisable_28 != null && !objToDisable_28.activeInHierarchy)
            {
                objToDisable_28.SetActive(true);
            }
            if (objToDisable_29 != null && !objToDisable_29.activeInHierarchy)
            {
                objToDisable_29.SetActive(true);
            }
            if (objToDisable_30 != null && !objToDisable_30.activeInHierarchy)
            {
                objToDisable_30.SetActive(true);
            }
            if (objToDisable_31 != null && !objToDisable_31.activeInHierarchy)
            {
                objToDisable_31.SetActive(true);
            }
            if (objToDisable_32 != null && !objToDisable_32.activeInHierarchy)
            {
                objToDisable_32.SetActive(true);
            }
            if (objToDisable_33 != null && !objToDisable_33.activeInHierarchy)
            {
                objToDisable_33.SetActive(true);
            }
        }


    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, distanceToDisable);
    }
}
