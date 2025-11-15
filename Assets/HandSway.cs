using UnityEngine;

public class HandSway : MonoBehaviour
{
    [Header("References")]
    public Transform target;                 // HandSocket (leave empty = this)
    public CharacterController cc;           // drag your Player's CharacterController

    [Header("Mouse sway")]
    public float swayPos = 0.01f;
    public float swayRot = 1.2f;
    public float swayClampRot = 7f;
    public float mouseDeadzone = 0.03f;

    [Header("Walk/Run bob")]
    public float bobAmpPos = 0.03f;          // vertical bob strength (visible)
    public float bobAmpRot = 1.4f;           // rotational bob strength
    public float bobFreqWalk = 8.5f;         // base walk frequency
    public float runMultiplier = 1.7f;       // run = walkFreq * this
    public float speedForRun = 7f;           // your run speed (m/s)
    public float moveThreshold = 0.05f;      // below this = idle (no bob)

    [Header("Smoothing & clamps")]
    public float swaySmooth = 10f;           // lower = snappier
    public float speedSmooth = 12f;          // smoothing on speed input
    public float maxPosMovePerSec = 3.0f;    // per-frame pos clamp
    public float maxRotDegPerSec = 300f;    // per-frame rot clamp

    Vector3 basePos;
    Quaternion baseRot;
    float bobT, smoothedSpeed;

    void Awake()
    {
        if (!target) target = transform;
        basePos = target.localPosition;
        baseRot = target.localRotation;

        // Auto-find CC if not assigned
        if (!cc)
        {
            var root = GetComponentInParent<Camera>()?.transform?.root;
            if (root) cc = root.GetComponent<CharacterController>();
        }
    }

    void LateUpdate()
    {
        float dt = Time.deltaTime;

        // --- Mouse sway (with deadzone so it doesn't wiggle at rest) ---
        float mx = Input.GetAxis("Mouse X");
        float my = Input.GetAxis("Mouse Y");
        if (Mathf.Abs(mx) < mouseDeadzone) mx = 0f;
        if (Mathf.Abs(my) < mouseDeadzone) my = 0f;

        Vector3 swayPosOffset = new Vector3(-mx, -my, 0f) * swayPos;   // slight counter-move
        Vector3 swayRotEuler = new Vector3(+my, +mx, -mx) * swayRot;
        swayRotEuler = Vector3.ClampMagnitude(swayRotEuler, swayClampRot);

        // --- Speed ? bob timer ---
        float speed = cc ? cc.velocity.magnitude : 0f;
        smoothedSpeed = Mathf.Lerp(smoothedSpeed, speed, 1f - Mathf.Exp(-speedSmooth * dt));
        bool moving = smoothedSpeed > moveThreshold;
        float move01 = Mathf.InverseLerp(moveThreshold, speedForRun, smoothedSpeed);

        if (moving)
        {
            float f = Mathf.Lerp(bobFreqWalk, bobFreqWalk * runMultiplier, move01);
            bobT += dt * f;
            if (bobT > 999f) bobT -= 999f; // wrap to avoid float blow-up
        }

        // --- Offsets (classic FPS feel) ---
        float s = Mathf.Sin(bobT * Mathf.PI * 2f);
        float c = Mathf.Cos(bobT * Mathf.PI * 2f);

        float posAmp = moving ? bobAmpPos * (0.3f + 0.7f * move01) : 0f;
        float rotAmp = moving ? bobAmpRot * (0.3f + 0.7f * move01) : 0f;

        // Vertical dominates; subtle side/in-out
        Vector3 bobPosOffset = new Vector3(
            s * posAmp * 0.35f,             // small side sway
            s * posAmp * 1.00f,             // strong vertical bob
            -Mathf.Abs(s) * posAmp * 0.12f  // tiny forward/back pulse
        );

        Vector3 bobRotEuler = new Vector3(
            -Mathf.Abs(s) * rotAmp * 0.5f,  // pitch dip on each step
            c * rotAmp * 0.9f,              // yaw swing
            s * rotAmp * 0.35f              // roll
        );

        // --- Target pose ---
        Vector3 targetPos = basePos + swayPosOffset + bobPosOffset;
        Quaternion targetRot = baseRot * Quaternion.Euler(swayRotEuler + bobRotEuler);

        // --- Smooth, then clamp per-frame change (anti-shake) ---
        float lerp = 1f - Mathf.Exp(-swaySmooth * dt);
        Vector3 softPos = Vector3.Lerp(target.localPosition, targetPos, lerp);
        Quaternion softRot = Quaternion.Slerp(target.localRotation, targetRot, lerp);

        float maxPosDelta = maxPosMovePerSec * dt;
        float maxRotDelta = maxRotDegPerSec * dt;

        target.localPosition = Vector3.MoveTowards(target.localPosition, softPos, maxPosDelta);
        target.localRotation = Quaternion.RotateTowards(target.localRotation, softRot, maxRotDelta);
    }
}
