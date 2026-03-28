using UnityEngine;

public class GameVariables : MonoBehaviour
{
    UiInteractions uiInteractions;
    private void Start()
    {
        uiInteractions = FindAnyObjectByType<UiInteractions>();
    }
    private void Update()
    {
        Time.timeScale = uiInteractions.timeSpeed;
    }
}
