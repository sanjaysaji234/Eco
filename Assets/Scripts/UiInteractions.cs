using System;
using TMPro;
using UnityEngine;

public class UiInteractions : MonoBehaviour
{
    public float cameraSpan = 1f;
    [Range(0,2)] public float timeSpeed=1f;
    float beforePauseTimeSpeed;
    [SerializeField] GameObject pauseMenu;
    [SerializeField] private Animator cloudeAnimator;
    public void Pause()
    {
        beforePauseTimeSpeed=timeSpeed;
        timeSpeed = 0f;
        pauseMenu.SetActive(true);
        gameObject.SetActive(false);
    }

    public void Resume()
    {
        timeSpeed = beforePauseTimeSpeed;
        pauseMenu.SetActive(false);
        gameObject.SetActive(false);
    }

    public void Quit()
    {
        Application.Quit();
    }

    public void NewMap()
    {
        timeSpeed = 1f;
        cloudeAnimator.Play("CloudCover");
        pauseMenu.SetActive(false);
        gameObject.SetActive (false);
    }

    public void TimeSpeedChange(float value)
    {
        timeSpeed=value;
    }
    public void CameraSpanChange(float value)
    {
        cameraSpan=value;
    }
}
