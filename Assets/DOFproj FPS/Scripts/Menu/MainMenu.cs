/// Deacon of Freedom Development (2020) v1
/// If you have any questions feel free to write me at email --- Phil-James_Lapuz@outlook.com ---

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    int demoLevelId = 1;

    public void LoadDemoLevel()
    {
        SceneManager.LoadScene(demoLevelId);
    }
    
    public void Quit()
    {
        Application.Quit();
    }
}
