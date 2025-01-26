using UnityEngine;

public class BackgroundColorChanger : MonoBehaviour
{
    public Camera mainCamera; // Reference to the main camera
    public float colorChangeSpeed = 1f; // Speed of hue rotation
    private float hue = 0f; // Start at the beginning of the hue circle
    private float saturation = 0.5f; // Keep saturation fixed (medium saturation)
    private float brightness = 0.4f; // Keep brightness fixed (darker region)

    void Update()
    {
        // Increment hue value over time to rotate through all colors
        hue += colorChangeSpeed * Time.deltaTime;
        if (hue > 1f) hue -= 1f; // Wrap hue back to 0 when it exceeds 1

        // Convert HSV to RGB
        Color newColor = Color.HSVToRGB(hue, saturation, brightness);

        // Apply the new color to the camera background
        mainCamera.backgroundColor = newColor;
    }
}
