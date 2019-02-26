using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

[AddComponentMenu("Scripts/Menu/Menu")]
public class Menu : MonoBehaviour {

    [System.Serializable]
    private class Options
    {
        public Slider gameVolume = null;
        public Slider musicVolume = null;
    }
    [SerializeField] private Options options;
    

    void Start()
    {
        GetVolume();
    }

    public void Play()
    {
        Debug.Log("MainMenu.Play: Loading scene with index " + (SceneManager.GetActiveScene().buildIndex + 1) + "...");
        SceneManager.LoadSceneAsync(SceneManager.GetActiveScene().buildIndex + 1); 
    }

    public void GetVolume()
    {

        if(!PlayerPrefs.HasKey("game_volume"))
            PlayerPrefs.SetFloat("game_volume", 1.0f);

        if(!PlayerPrefs.HasKey("music_volume"))
            PlayerPrefs.SetFloat("music_volume", 1.0f);

        options.gameVolume.value = PlayerPrefs.GetFloat("game_volume");
        options.musicVolume.value = PlayerPrefs.GetFloat("music_volume");

    }

    public void SetVolume()
    {

        PlayerPrefs.SetFloat("game_volume", options.gameVolume.value);
        PlayerPrefs.SetFloat("music_volume", options.musicVolume.value);

        AudioListener.volume = options.gameVolume.value;

    }

    public void ResetOptions()
    {

        PlayerPrefs.SetFloat("game_volume", 1.0f);
        PlayerPrefs.SetFloat("music_volume", 1.0f);

        options.gameVolume.value = PlayerPrefs.GetFloat("game_volume");
        options.musicVolume.value = PlayerPrefs.GetFloat("music_volume");
    }

    public void Quit()
    {
        Debug.Log("MainMenu.Quit: Application.Quit()");
        Application.Quit();
    }


}
