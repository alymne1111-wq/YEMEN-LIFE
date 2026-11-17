using UnityEngine;

public class DayNightCycle : MonoBehaviour
{
    public Light sun;
    [Tooltip("Full in-game day length in real-time minutes.")]
    public float dayLengthMinutes = 24f;
    [Range(0f, 1f)] public float timeOfDay = 0.35f;

    private void Update()
    {
        if (sun == null || dayLengthMinutes <= 0f) return;

        timeOfDay = (timeOfDay + Time.deltaTime / (dayLengthMinutes * 60f)) % 1f;
        float sunAngle = timeOfDay * 360f - 90f;
        sun.transform.rotation = Quaternion.Euler(sunAngle, 170f, 0f);

        float daylight = Mathf.Clamp01(Vector3.Dot(
            sun.transform.forward * -1f, Vector3.up) * 1.4f);

        sun.intensity = Mathf.Lerp(0.04f, 1.15f, daylight);
        RenderSettings.ambientIntensity = Mathf.Lerp(0.25f, 1f, daylight);
    }
}
