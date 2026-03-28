using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UiInteractions : MonoBehaviour
{
    [SerializeField]private Slider simSpeedSlider;
    public float cameraSpan = 1f;
    [Range(0,2)] public float timeSpeed=1f;
    float beforePauseTimeSpeed;
    [SerializeField] GameObject pauseMenu;
    [SerializeField] private Animator cloudeAnimator;
    public void Pause()
    {
        beforePauseTimeSpeed=timeSpeed;
        simSpeedSlider.value = 0f;
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
       
        simSpeedSlider.value = 2f;

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
