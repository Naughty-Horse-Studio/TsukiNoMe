using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuSceneManager : MonoBehaviour 
{

    // Use this for initialization
    public void LoadMenu()
    {
        if (ApplicationManager.instance) ApplicationManager.instance.LoadMainMenu();
    }


    public void LoadGame ()
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
