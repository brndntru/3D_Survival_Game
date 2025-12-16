using UnityEngine;

public class FirstPersonLook : MonoBehaviour
{
    [Header("Refs")]
    public Transform playerBody;     
    public Transform cameraPivot;    

    [Header("Settings")]
    public float sensitivity = 200f; // mouse sensitivity
    public bool clampPitch = true;  
    public float minPitch = -89f;
    public float maxPitch = 89f;

    float pitch; 

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        float mouseX = Input.GetAxis("Mouse X") * sensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * sensitivity * Time.deltaTime;

        playerBody.Rotate(Vector3.up * mouseX);

        pitch -= mouseY;
        if (clampPitch) pitch = Mathf.Clamp(pitch, minPitch, maxPitch);
        cameraPivot.localRotation = Quaternion.Euler(pitch, 0f, 0f);

        // toggles cursor with esc
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Cursor.visible = !Cursor.visible;
            Cursor.lockState = Cursor.visible ? CursorLockMode.None : CursorLockMode.Locked;
        }
    }
}
