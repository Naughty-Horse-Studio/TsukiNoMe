/// Deacon of Freedom Development (2020) v1
/// If you have any questions feel free to write me at email --- Phil-James_Lapuz@outlook.com ---

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DOFprojFPS;

public class ZHeadShot : MonoBehaviour
{
    public GameObject head;
    public ZombieNPC npc;

    public void InstaKill()
    {

       npc.ApplyHit(1000);

    }
}
