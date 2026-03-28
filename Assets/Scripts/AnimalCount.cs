using TMPro;
using UnityEngine;

public class AnimalCount : MonoBehaviour
{
    public float tigerCount = 0f;
    public float deerCount = 0f;

    [SerializeField]private TextMeshProUGUI tigerText,deerText;

    private void Update()
    {
        tigerText.text=tigerCount.ToString();
        deerText.text=deerCount.ToString();
    }
}
