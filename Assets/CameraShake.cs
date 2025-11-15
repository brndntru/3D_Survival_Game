using UnityEngine;

public class CameraShake : MonoBehaviour
{
    public static CameraShake I;

    [Header("Strength")]
    public float traumaDecay = 1.6f;        // how fast shake fades (per second)
    public float noiseFrequency = 26f;      // shake speed
    public Vector3 posAmount = new Vector3(0.08f, 0.08f, 0f); // max local-pos offset
    public Vector3 rotAmount = new Vector3(0.6f, 0.6f, 3.5f); // max local-rot (deg)

    float trauma; // 0..1
    Vector3 basePos;
    Quaternion baseRot;

    void Awake()
    {
        I = this;
        basePos = transform.localPosition;
        baseRot = transform.localRotation;
    }

    /// Call this to shake. 0.15 = light hit, 0.35 = strong hit.
    public void AddTrauma(float amount)
    {
        trauma = Mathf.Clamp01(trauma + amount);
    }

    void LateUpdate()
    {
        float dt = Time.unscaledDeltaTime;

        if (trauma > 0f)
        {
            float t = Time.unscaledTime * noiseFrequency;
            float tt = trauma * trauma; // nicer falloff

            float nx = (Mathf.PerlinNoise(t, 0f) * 2f - 1f);
            float ny = (Mathf.PerlinNoise(0f, t) * 2f - 1f);
            float nz = (Mathf.PerlinNoise(t, t) * 2f - 1f);

            Vector3 pos = basePos + new Vector3(nx * posAmount.x, ny * posAmount.y, nz * posAmount.z) * tt;
            Vector3 eul = new Vector3(nx * rotAmount.x, ny * rotAmount.y, nz * rotAmount.z) * tt;

            transform.localPosition = pos;
            transform.localRotation = baseRot * Quaternion.Euler(eul);

            trauma = Mathf.Max(0f, trauma - traumaDecay * dt);
        }
        else
        {
            // return to rest
            transform.localPosition = Vector3.Lerp(transform.localPosition, basePos, 10f * dt);
            transform.localRotation = Quaternion.Slerp(transform.localRotation, baseRot, 10f * dt);
        }
    }
}
