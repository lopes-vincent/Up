using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class Timer : MonoBehaviour
{
    [SerializeField] public TextMeshProUGUI gameTimer;
    [SerializeField] public TextMeshProUGUI pauseTimer;
    
    [SerializeField] private int _msElapsed;
    [SerializeField] private bool stopTimer = true;
    
    private bool displayGameTimer;
    
    void Awake()
    {
        GameObject[] objects = GameObject.FindGameObjectsWithTag("Timer");

        if (objects.Length > 1)
        {
            Destroy(this.gameObject);
        }

        DontDestroyOnLoad(this.gameObject);
        StartCoroutine(Clock());
    }

    public void HideGameTimer()
    {
        displayGameTimer = false;
        gameTimer.gameObject.SetActive(false);
    }

    public void ToggleGameTimer()
    {
        displayGameTimer = !displayGameTimer;
        gameTimer.gameObject.SetActive(displayGameTimer && false == pauseTimer.gameObject.activeSelf);
    }

    public void TogglePause()
    {
        GameObject pauseTimerGameObject = pauseTimer.gameObject;
        pauseTimerGameObject.SetActive(!pauseTimerGameObject.activeSelf);
        gameTimer.gameObject.SetActive(displayGameTimer && false == pauseTimerGameObject.activeSelf);
    }

    public void StopTimer()
    {
        stopTimer = true;
    }

    public void LaunchTimer()
    {
        stopTimer = false;
    }

    public void Restart()
    {
        stopTimer = true;
        _msElapsed = 0;
        gameTimer.text = "00:00:00";
        pauseTimer.text = "00:00:00";
    }

    public int getMsElapsed()
    {
        return _msElapsed;
    }

    public string getMsElapsedAsText()
    {
        return msToText(_msElapsed);
    }

    IEnumerator Clock()
    {
        while (true)
        {                
            yield return new WaitForSeconds(0.001f);
            if (!stopTimer)
            {
                _msElapsed++;
                gameTimer.text = msToText(_msElapsed);
                pauseTimer.text = msToText(_msElapsed);
            }
        }
    }

    public static string msToText(int ms)
    {
        int msDisplay = ms % 100;
        int seconds = ms / 100;
        int secondsDisplay = seconds % 60;
        int minutesDisplay = seconds / 60;
        return String.Format("{0:00}:{1:00}:{2:00}", minutesDisplay, secondsDisplay, msDisplay);
    }
}
