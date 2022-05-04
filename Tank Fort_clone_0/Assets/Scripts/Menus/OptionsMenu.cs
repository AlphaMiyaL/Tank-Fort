using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class OptionsMenu : MonoBehaviour{
    public AudioMixer audioMixer;
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

}
