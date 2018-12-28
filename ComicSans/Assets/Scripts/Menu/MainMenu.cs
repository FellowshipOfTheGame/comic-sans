using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

[AddComponentMenu("Scripts/Menu/Main Menu")]
public class MainMenu : MonoBehaviour {

    public void Play()
    {
        Debug.Log("(MainMenu) Loading scene with index " + (SceneManager.GetActiveScene().buildIndex + 1) + "...");
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1); 
    }

    public void Quit()
    {
        Debug.Log("Application.Quit()");
        Application.Quit();
    }


}
