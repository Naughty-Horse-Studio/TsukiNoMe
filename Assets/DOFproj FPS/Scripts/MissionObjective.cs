using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DOFprojFPS;

public class MissionObjective : MonoBehaviour 
{
    private PlayerStats playerStats;

    private void Start()
    {
        playerStats = FindObjectOfType<PlayerStats>();
    }

    void OnTriggerEnter( Collider col )
	{
        if (col.CompareTag("Player"))
        {
            //	PlayerInfo playerInfo = GameSceneManager.instance.GetPlayerInfo( col.GetInstanceID());
            if (playerStats != null)
            playerStats.DoLevelComplete();
        }
    }
}
