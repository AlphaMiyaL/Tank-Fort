using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class OptionsMenu : MonoBehaviour{
    public AudioMixer audioMixer;
    public Slider[] Sliders;
    public int lowest;
    public void SetMasterVolume(float volume) {
        if (volume == lowest) {
            audioMixer.SetFloat("MasterVolume", -80);
        }
        else {
            audioMixer.SetFloat("MasterVolume", volume);
        }
    }

    public void SetMusicVolume(float volume) {
        if (volume == lowest) {
            audioMixer.SetFloat("MusicVolume", -80);
        }
        else {
            audioMixer.SetFloat("MusicVolume", volume);
        }
    }

    public void SetSFXVolume(float volume) {
        if (volume == lowest) {
            audioMixer.SetFloat("SFXVolume", -80);
        }
        else {
            audioMixer.SetFloat("SFXVolume", volume);
        }
    }

    public void SetDrivingVolume(float volume) {
        if (volume == lowest) {
            audioMixer.SetFloat("DrivingVolume", -80);
        }
        else {
            audioMixer.SetFloat("DrivingVolume", volume);
        }
    }

    public void SetVolumeValues() {
        audioMixer.GetFloat("MasterVolume", out float value);
        Sliders[0].value = value;
        audioMixer.GetFloat("MusicVolume", out float value2);
        Sliders[1].value = value2;
        audioMixer.GetFloat("SFXVolume", out float value3);
        Sliders[2].value = value3;
        audioMixer.GetFloat("DrivingVolume", out float value4);
        Sliders[3].value = value4;
    }

}
