using UnityEngine;

[RequireComponent(typeof(Light))]
public class FlickerLight : MonoBehaviour
{
    [Header("Flicker Settings")]
    public float minIntensity = 0.8f;
    public float maxIntensity = 1.5f;
    public float flickerSpeed = 5f;

    private Light fireLight;
    private float baseIntensity;

    void Start()
    {
        fireLight = GetComponent<Light>();
        baseIntensity = fireLight.intensity;
    }

    void Update()
    {
        // Random flicker using Perlin noise
        float noise = Mathf.PerlinNoise(Time.time * flickerSpeed, 0f);
        fireLight.intensity = Mathf.Lerp(minIntensity, maxIntensity, noise);
    }
}