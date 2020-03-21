/// Deacon of Freedom Development (2020) v1
/// If you have any questions feel free to write me at email --- Phil-James_Lapuz@outlook.com ---
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cover : MonoBehaviour
{
    public bool occupied = false;
    public Collider m_collider;

    private void Start()
    {
        m_collider = GetComponent<Collider>();
    }
}
