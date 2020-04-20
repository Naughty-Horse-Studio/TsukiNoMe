using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioTrigger : MonoBehaviour
{
    public AudioSource audioComnponent;

  //  public MainMenuSceneManager _sceneManager;




    private void Start()
    {
    //    _sceneManager = GetComponent<MainMenuSceneManager>();
        StartCoroutine(waitAudio ());
    }



    private IEnumerator waitAudio()
    {
        
        yield return new WaitForSeconds(20f);

        audioComnponent.Play();

        yield return new WaitForSeconds(audioComnponent.clip.length);

        if (ApplicationManager.instance) ApplicationManager.instance.LoadMainMenu();
        print("end of sound");
    }

}
