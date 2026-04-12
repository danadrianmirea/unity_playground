using UnityEngine;
using UnityEngine.UI;

public class SetCrosshairTransparency : MonoBehaviour
{
    public Image dotImage;

    [Range(0f, 100f)]
    public float transparency = 100f;

    void Start()
    {
        ApplyTransparency();
    }

    public void SetTransparency(float value)
    {
        transparency = Mathf.Clamp(value, 0f, 100f);
        ApplyTransparency();
    }

    private void ApplyTransparency()
    {
        Color c = dotImage.color;
        c.a = transparency / 100f;
        dotImage.color = c;
    }
}