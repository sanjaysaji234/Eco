using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UiInteractions : MonoBehaviour
{
    [SerializeField] private Slider simSpeedSlider;
    public float cameraSpan = 1f;
    [Range(0, 2)] public float timeSpeed = 1f;
    float beforePauseTimeSpeed;
    [SerializeField] GameObject pauseMenu, pauseButton;
    [SerializeField] private Animator cloudeAnimator;
    public void Pause()
    {
        beforePauseTimeSpeed = simSpeedSlider.value;
        simSpeedSlider.value = 0f;
        pauseMenu.SetActive(true);
        pauseButton.SetActive(false);
        simSpeedSlider.gameObject.SetActive(false);
    }

    public void Resume()
    {
        simSpeedSlider.value = beforePauseTimeSpeed;
        pauseMenu.SetActive(false);
        pauseButton.SetActive(true);
        simSpeedSlider.gameObject.SetActive(true);
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
        pauseButton.SetActive(true);
        simSpeedSlider.gameObject.SetActive(true);
    }

    
    public void TimeSpeedChange(float value)
    {
        timeSpeed=value;
    }
    public void CameraSpanChange(float value)
    {
        cameraSpan=value;
    }

    //Socials
    public void Credits()
    {
        Application.OpenURL("https://github.com/sanjaysaji234/Eco/blob/main/Credits.md");
    }

    public void LinkedIn()
    {
        Application.OpenURL("https://www.linkedin.com/in/sanjay-saji-986003301/");
    }

    public void GitHub()
    {
        Application.OpenURL("https://github.com/sanjaysaji234/Eco");
    }
    public void PayPal()
    {
        Application.OpenURL("https://www.paypal.com/paypalme/snj4y");
    }
}
