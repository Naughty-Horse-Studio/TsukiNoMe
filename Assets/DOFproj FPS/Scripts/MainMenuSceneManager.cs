using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DOFprojFPS;
public class MainMenuSceneManager : MonoBehaviour
{
    private UseObjects2 _mUseAudio;
    private InventoryAudioPlayer audioPlayer;

    private void Start()
    {
        audioPlayer = FindObjectOfType<InventoryAudioPlayer>();
        _mUseAudio = GetComponent<UseObjects2>();

        audioPlayer = InventoryAudioPlayer.instance;
    }
    // Use this for initialization
    public void LoadMenu()
    {
        //_mUseAudio.StopAllCoroutines();
        //_mUseAudio.StopAudioRecording();

     //   InventoryAudioPlayer audioPlayer = InventoryAudioPlayer.instance;
        if (InventoryAudioPlayer.instance)
            InventoryAudioPlayer.instance.StopAudio();
        

        if (ApplicationManager.instance) ApplicationManager.instance.LoadMainMenu();
    }


    public void LoadGame()
    {
        if (ApplicationManager.instance)
            ApplicationManager.instance.LoadGame();
    }


    public void QuitGame()
    {
        if (ApplicationManager.instance)
            ApplicationManager.instance.QuitGame();
    }

}
