using System;
using UnityEngine;

public class CloudAnimationEvents : MonoBehaviour
{
    public event EventHandler OnCloudCovered;
    public void CloudCovered()
    {
        OnCloudCovered?.Invoke(this, EventArgs.Empty);
    }
}
