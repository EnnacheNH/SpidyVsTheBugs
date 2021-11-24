using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class OptionPanel : MonoBehaviour
{
    public AudioMixer audioMixer;

    public Slider azertySlider;
    public Slider volumeSlider;
    public Toggle fullscreenToggle;
    public Dropdown resolutionDropdown;

    private Resolution[] resolutions;

    private void Start()
    {
        //Init Azerty
        if (Parameters.azertyParameter)
        {
            azertySlider.value = 0f;
        }
        else
        {
            azertySlider.value = 1f;
        }

        //Init Volume
        audioMixer.SetFloat("volume", Parameters.volumeParameter);
        volumeSlider.value = Parameters.volumeParameter;

        //Init Fullscreen
        Screen.fullScreen = Parameters.fullscreenParameter;
        fullscreenToggle.isOn = Parameters.fullscreenParameter;

        //Init Resolutions
        resolutions = Screen.resolutions;
        resolutionDropdown.ClearOptions();
        List<string> options = new List<string>();
        int currentResolutionIndex = 0;
        for (int i = 0; i < resolutions.Length; i++)
        {
            string option = resolutions[i].width + "x" + resolutions[i].height;
            options.Add(option);

            if (resolutions[i].width == Screen.currentResolution.width && resolutions[i].height == Screen.currentResolution.height)
            {
                currentResolutionIndex = i;
            }
        }
        resolutionDropdown.AddOptions(options);

        if (!Parameters.isInitialized)
        {
            Parameters.resolutionParameter = currentResolutionIndex;
            resolutionDropdown.value = currentResolutionIndex;
            resolutionDropdown.RefreshShownValue();
            Parameters.isInitialized = true;
        }
        else
        {
            resolutionDropdown.value = Parameters.resolutionParameter;
            resolutionDropdown.RefreshShownValue();
        }
    }

    public void AzertySlider(float _azertySlider)
    {
        if (_azertySlider == 0f)
        {
            Parameters.azertyParameter = true;
        }
        else
        {
            Parameters.azertyParameter = false;
        }
    }

    public void VolumeSlider(float _volumeSlider)
    {
        Parameters.volumeParameter = _volumeSlider;
        audioMixer.SetFloat("volume", _volumeSlider);
    }

    public void FullscreenToggle(bool _fullscreenToggle)
    {
        Parameters.fullscreenParameter = _fullscreenToggle;
        Screen.fullScreen = _fullscreenToggle;
    }
    public void ResolutionDropdown(int _resolutionIndex)
    {
        Parameters.resolutionParameter = _resolutionIndex;
        Resolution resolution = resolutions[_resolutionIndex];
        Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreen);
    }

    public void CloseOption()
    {
        this.gameObject.SetActive(false);
    }
}
