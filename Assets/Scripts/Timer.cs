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
    
    [SerializeField] private float _sElapsed;
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
    
    void Update()
    {
        if (!stopTimer)
        {
            _sElapsed += Time.deltaTime;
        }
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
        _sElapsed = 0;
        gameTimer.text = "00:00:00";
        pauseTimer.text = "00:00:00";
    }

    public float getMsElapsed()
    {
        return _sElapsed;
    }

    public string getMsElapsedAsText()
    {
        return sToText(_sElapsed);
    }

    IEnumerator Clock()
    {
        while (true)
        {                
            yield return new WaitForSeconds(0.001f);
            if (!stopTimer)
            {
                gameTimer.text = sToText(_sElapsed);
                pauseTimer.text = sToText(_sElapsed);
            }
        }
    }

    public static string sToText(float seconds)
    {
        float ms = seconds * 100;
        float msDisplay = ms % 100;
        float secondsDisplay = seconds % 60;
        float minutesDisplay = Mathf.FloorToInt(seconds / 60);
        return String.Format("{0:00}:{1:00}:{2:00}", minutesDisplay, secondsDisplay, Mathf.Clamp(msDisplay, 0, 99));
    }
}
