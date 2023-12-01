using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    [SerializeField] private GameObject pauseMenu;
    [SerializeField] private Timer _timer;
    [SerializeField] private AudioMixer audioMixer;
    [SerializeField] private Slider musicSlider;
    [SerializeField] private Slider effectSlider;

    public void Start()
    {
        if (GameObject.FindGameObjectWithTag("Timer"))
        {
            _timer = GameObject.FindGameObjectWithTag("Timer").GetComponent<Timer>();
        }

        if (musicSlider)
        {
            float musicVolume;
            audioMixer.GetFloat("MusicVolume", out musicVolume);   
            musicSlider.value = Mathf.Pow(2,musicVolume / 20f) ;
        }

        if (effectSlider)
        {
            float effectVolume;
            audioMixer.GetFloat("EffectVolume", out effectVolume);
            effectSlider.value = Mathf.Pow(2,effectVolume / 20f) ;
        }
    }

    public void Pause()
    {
        _timer.TogglePause();
        pauseMenu.SetActive(!pauseMenu.activeSelf);
        Time.timeScale = Time.timeScale == 1f ? 0f : 1f;
    }
    
    public void Restart()
    {
        _timer.TogglePause();
        Time.timeScale = 1f;
        _timer.Restart();
        SceneManager.LoadScene(1);
    }

    public void ToggleGameTimerDisplay()
    {
        _timer.ToggleGameTimer();
    }

    public void LaunchTimer()
    {
        _timer.LaunchTimer();    
    }
    
    public void LoadScene(int index)
    {
        SceneManager.LoadScene(index);
    }

    public void MusicVolume(float volume)
    {
        audioMixer.SetFloat("MusicVolume",  Mathf.Log(volume, 2) * 20);
    }

    public void EffectVolume(float volume)
    {
        audioMixer.SetFloat("EffectVolume",  Mathf.Log(volume, 2) * 20);
    }
}
