using UnityEngine;

public class FirstPersonLook : MonoBehaviour
{
    [Header("Refs")]
    public Transform playerBody;     // assign Player here
    public Transform cameraPivot;    // assign Main Camera here

    [Header("Settings")]
    public float sensitivity = 200f; // mouse sensitivity
    public bool clampPitch = true;   // turn off if you truly want 360 vertical
    public float minPitch = -89f;
    public float maxPitch = 89f;

    float pitch; // up/down accumulator

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        float mouseX = Input.GetAxis("Mouse X") * sensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * sensitivity * Time.deltaTime;

        // horizontal (yaw) on the body — unlimited 360°
        playerBody.Rotate(Vector3.up * mouseX);

        // vertical (pitch) on the camera
        pitch -= mouseY;
        if (clampPitch) pitch = Mathf.Clamp(pitch, minPitch, maxPitch);
        cameraPivot.localRotation = Quaternion.Euler(pitch, 0f, 0f);

        // toggle cursor with Esc (useful in editor)
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Cursor.visible = !Cursor.visible;
            Cursor.lockState = Cursor.visible ? CursorLockMode.None : CursorLockMode.Locked;
        }
    }
}
