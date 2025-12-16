using UnityEngine;

public class HandSway : MonoBehaviour
{
    [Header("References")]
    public Transform target;               
    public CharacterController cc;       

    [Header("Mouse sway")]
    public float swayPos = 0.01f;
    public float swayRot = 1.2f;
    public float swayClampRot = 7f;
    public float mouseDeadzone = 0.03f;

    [Header("Walk/Run bob")]
    public float bobAmpPos = 0.03f;          
    public float bobAmpRot = 1.4f;          
    public float bobFreqWalk = 8.5f;         
    public float runMultiplier = 1.7f;       
    public float speedForRun = 7f;           
    public float moveThreshold = 0.05f;     

    [Header("Smoothing & clamps")]
    public float swaySmooth = 10f;          
    public float speedSmooth = 12f;         
    public float maxPosMovePerSec = 3.0f;    
    public float maxRotDegPerSec = 300f;    

    Vector3 basePos;
    Quaternion baseRot;
    float bobT, smoothedSpeed;

    void Awake()
    {
        if (!target) target = transform;
        basePos = target.localPosition;
        baseRot = target.localRotation;

        if (!cc)
        {
            var root = GetComponentInParent<Camera>()?.transform?.root;
            if (root) cc = root.GetComponent<CharacterController>();
        }
    }

    void LateUpdate()
    {
        float dt = Time.deltaTime;
        float mx = Input.GetAxis("Mouse X");
        float my = Input.GetAxis("Mouse Y");
        if (Mathf.Abs(mx) < mouseDeadzone) mx = 0f;
        if (Mathf.Abs(my) < mouseDeadzone) my = 0f;

        Vector3 swayPosOffset = new Vector3(-mx, -my, 0f) * swayPos; 
        Vector3 swayRotEuler = new Vector3(+my, +mx, -mx) * swayRot;
        swayRotEuler = Vector3.ClampMagnitude(swayRotEuler, swayClampRot);

        float speed = cc ? cc.velocity.magnitude : 0f;
        smoothedSpeed = Mathf.Lerp(smoothedSpeed, speed, 1f - Mathf.Exp(-speedSmooth * dt));
        bool moving = smoothedSpeed > moveThreshold;
        float move01 = Mathf.InverseLerp(moveThreshold, speedForRun, smoothedSpeed);

        if (moving)
        {
            float f = Mathf.Lerp(bobFreqWalk, bobFreqWalk * runMultiplier, move01);
            bobT += dt * f;
            if (bobT > 999f) bobT -= 999f; 
        }

        float s = Mathf.Sin(bobT * Mathf.PI * 2f);
        float c = Mathf.Cos(bobT * Mathf.PI * 2f);

        float posAmp = moving ? bobAmpPos * (0.3f + 0.7f * move01) : 0f;
        float rotAmp = moving ? bobAmpRot * (0.3f + 0.7f * move01) : 0f;

        Vector3 bobPosOffset = new Vector3(
            s * posAmp * 0.35f,
            s * posAmp * 1.00f,            
            -Mathf.Abs(s) * posAmp * 0.12f  
        );

        Vector3 bobRotEuler = new Vector3(
            -Mathf.Abs(s) * rotAmp * 0.5f,  
            c * rotAmp * 0.9f,              
            s * rotAmp * 0.35f              
        );

        Vector3 targetPos = basePos + swayPosOffset + bobPosOffset;
        Quaternion targetRot = baseRot * Quaternion.Euler(swayRotEuler + bobRotEuler);

        float lerp = 1f - Mathf.Exp(-swaySmooth * dt);
        Vector3 softPos = Vector3.Lerp(target.localPosition, targetPos, lerp);
        Quaternion softRot = Quaternion.Slerp(target.localRotation, targetRot, lerp);

        float maxPosDelta = maxPosMovePerSec * dt;
        float maxRotDelta = maxRotDegPerSec * dt;

        target.localPosition = Vector3.MoveTowards(target.localPosition, softPos, maxPosDelta);
        target.localRotation = Quaternion.RotateTowards(target.localRotation, softRot, maxRotDelta);
    }
}
