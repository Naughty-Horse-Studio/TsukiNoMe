/// Deacon of Freedom Development (2020) v1
/// If you have any questions feel free to write me at email --- Phil-James_Lapuz@outlook.com ---

using DOFprojFPS;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeadShot : MonoBehaviour
{
    public GameObject head;
    public NPC npc;

    public void InstaKill()
    {

        npc.GetHit(1000, this.transform);

    }
}
