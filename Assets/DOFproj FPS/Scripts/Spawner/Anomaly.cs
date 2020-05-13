using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DOFprojFPS;

public class Anomaly : MonoBehaviour
{
    private Transform player;
    private AudioSource audioSource;
    private Animator headBobAnimator;

    public AudioClip anomalyEnterReaction;
    public GameObject triggerToEnabled_1;
    public GameObject triggerToEnabled_2;
    public GameObject triggerToEnabled_3;

    [Header("NPC")]
    [Tooltip("The AI(enemy)to use for the door opening and closing initiate.")]
    [SerializeField]
    private GameObject rebel_Prefab;
    [SerializeField]
    private int rebel_Enemy_Count;

    [SerializeField]
    private GameObject zombie_Prefab;
    [SerializeField]
    private int zombie_Enemy_Count;

    public Transform[] rebel_SpawnPoints, zombie_SpawnPoints;



    private int initial_rebel_Count, initial_zombie_Count;

    public float wait_Before_Spawn_Enemies_Time = 3f;


    private void Start()
    {
        player = FindObjectOfType<DOFprojFPS.FPSController >().transform;
        audioSource = GetComponent<AudioSource>();
        headBobAnimator = Camera.main.GetComponent<Animator>();

        initial_rebel_Count = rebel_Enemy_Count;
        initial_zombie_Count = zombie_Enemy_Count;

    }


    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            headBobAnimator.Play("CameraKick");

            //player.GetComponent<PlayerStats>().ApplyDamage(20);
            //audioSource.PlayOneShot(anomalyEnterReaction);

            StartCoroutine("CheckToSpawnEnemies");

            if(triggerToEnabled_1!=null)
                triggerToEnabled_1.SetActive(true);

            if (triggerToEnabled_2 != null)
                triggerToEnabled_2.SetActive(true);

            if (triggerToEnabled_3 != null)
                triggerToEnabled_3.SetActive(true);

            Destroy(gameObject, 4f);
        }

    }

    IEnumerator CheckToSpawnEnemies()
    {
        yield return new WaitForSeconds(wait_Before_Spawn_Enemies_Time);

        SpawnRebels();

        SpawnZombies();

    }

    void SpawnRebels()
    {

        int index = 0;

        for (int i = 0; i < rebel_Enemy_Count; i++)
        {

            if (index >= rebel_SpawnPoints.Length)
            {
                index = 0;
            }

            Instantiate(rebel_Prefab, rebel_SpawnPoints[index].position, Quaternion.identity);

            index++;

        }

        rebel_Enemy_Count = 0;

    }
    void SpawnZombies()
    {

        int index = 0;

        for (int i = 0; i < zombie_Enemy_Count; i++)
        {

            if (index >= zombie_SpawnPoints.Length)
            {
                index = 0;
            }

            Instantiate(zombie_Prefab, zombie_SpawnPoints[index].position, Quaternion.identity);

            index++;

        }

        zombie_Enemy_Count = 0;

    }

    private void StopSpawning()
    {
        StopCoroutine("CheckToSpawnEnemies");
    }

}
