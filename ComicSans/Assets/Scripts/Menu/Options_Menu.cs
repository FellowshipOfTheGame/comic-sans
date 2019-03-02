using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

[AddComponentMenu("Scripts/Menu/Options Menu")]
public class Options_Menu : MonoBehaviour {

    public AudioMixer audiomixer;

    public Dropdown ResolutionDropdown;

    Resolution[] resolutions;

    int currentResolutionIndex = 0;

	void Start () {
        resolutions = Screen.resolutions;

        List<string> options = new List<string>(); 

        for (int i = 0;i < resolutions.Length; i++){
            string option = resolutions[i].width + " x " + resolutions[i].height;
            options.Add(option);

            if(resolutions[i].width == Screen.currentResolution.width &&
                resolutions[i].height == Screen.currentResolution.height)
            {
                currentResolutionIndex = i;
            }
        }

        ResolutionDropdown.ClearOptions();

        ResolutionDropdown.AddOptions(options);
        ResolutionDropdown.value = currentResolutionIndex;
        ResolutionDropdown.RefreshShownValue();
	}

    public void SetResolutiion(int ResolutionIndex)
    {
        Resolution resolution = resolutions[ResolutionIndex];
        Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreen);
    }
	
	public void SetVolume (float volume)
    {
        audiomixer.SetFloat("MainVolume", volume);
    }

    public void SetFullScreen (bool fullscreen)
    {
        Screen.fullScreen = fullscreen;
    }
}
