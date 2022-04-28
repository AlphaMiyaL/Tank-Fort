using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class GamePreferencesManager : MonoBehaviour
{
    public AudioMixer audioMixer;
    const string Vol = "Volume";
    const string Map = "Map";

    void Start()
    {
        LoadPrefs();
    }

    void OnApplicationQuit()
    {
        SavePrefs();
    }

    public void SavePrefs() {
        audioMixer.GetFloat("MasterVolume", out float value);
        PlayerPrefs.SetFloat(Vol+0, value);
        audioMixer.GetFloat("MusicVolume", out float value1);
        PlayerPrefs.SetFloat(Vol+1, value1);
        audioMixer.GetFloat("SFXVolume", out float value2);
        PlayerPrefs.SetFloat(Vol+2, value2);
        audioMixer.GetFloat("DrivingVolume", out float value3);
        PlayerPrefs.SetFloat(Vol+3, value3);
        PlayerPrefs.Save();

    }

    public void LoadPrefs() {
        float volume = PlayerPrefs.GetFloat(Vol + 0, 0);
        audioMixer.SetFloat("MasterVolume", volume);
        volume = PlayerPrefs.GetFloat(Vol + 1, 0);
        audioMixer.SetFloat("MusicVolume", volume);
        volume = PlayerPrefs.GetFloat(Vol + 2, 0);
        audioMixer.SetFloat("SFXVolume", volume);
        volume = PlayerPrefs.GetFloat(Vol + 3, 0);
        audioMixer.SetFloat("DrivingVolume", volume);
    }

    public void SaveMap(int map) {
        PlayerPrefs.SetInt(Map, map);
        PlayerPrefs.Save();
    }

    public int LoadMap() {
        return PlayerPrefs.GetInt(Map, 0);
    }


}
