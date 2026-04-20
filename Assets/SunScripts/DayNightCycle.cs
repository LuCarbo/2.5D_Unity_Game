using UnityEngine;

public class DayNightCycle : MonoBehaviour
{
    [Header("Time Settings")]
    [Range(0, 24)] public float timeOfDay = 12f; // Current time in hours
    public float dayDurationInSeconds = 120f; // How long a full 24h day takes in real seconds

    [Header("Sun Lighting")]
    public Light sun;
    [Tooltip("Color of the sun over 24 hours. Left is midnight, middle is noon, right is midnight.")]
    public Gradient sunColor;
    [Tooltip("Intensity of the sun over 24 hours.")]
    public AnimationCurve sunIntensity;

    private float timeMultiplier;

    void Start()
    {
        // Calculate how fast time should pass
        timeMultiplier = 24f / dayDurationInSeconds;
    }

    void Update()
    {
        // 1. Advance the time
        timeOfDay += Time.deltaTime * timeMultiplier;
        if (timeOfDay >= 24f)
        {
            timeOfDay = 0f; // Reset to midnight when the day ends
        }

        UpdateSun();
    }

    void UpdateSun()
    {
        // 2. Rotate the Sun
        // 6 AM = 0 degrees (sunrise), 12 PM = 90 degrees (noon), 6 PM = 180 degrees (sunset)
        // We offset by -90 so that 0 time (midnight) points straight up from below
        float sunRotation = (timeOfDay / 24f) * 360f - 90f;
        sun.transform.rotation = Quaternion.Euler(sunRotation, -30f, 0f);

        // 3. Update Color and Intensity using Gradients/Curves
        // Normalize time to a 0-1 scale so it can read the Gradient and Curve correctly
        float normalizedTime = timeOfDay / 24f;

        sun.color = sunColor.Evaluate(normalizedTime);
        sun.intensity = sunIntensity.Evaluate(normalizedTime);
    }
}