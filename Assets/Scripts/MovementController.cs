using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class MovementController : MonoBehaviour
{
    [Header("Movement")]
    public float walkSpeed = 4.5f;
    public float sprintSpeed = 7f;

    [Header("Jump & Gravity")]
    public float gravity = -20f;      
    public float jumpHeight = 1.2f;    // meters
    public float jumpCooldown = 0.5f;  // seconds

    CharacterController cc;
    float verticalVelocity;
    float lastJumpTime = -999f;

    void Awake()
    {
        cc = GetComponent<CharacterController>();
    }

    void Update()
    {
        bool grounded = cc.isGrounded;

        if (grounded && verticalVelocity < 0f)
            verticalVelocity = -2f;

        float x = Input.GetAxisRaw("Horizontal"); 
        float z = Input.GetAxisRaw("Vertical");  

        // move relative to where the Player is facing 
        Vector3 move = (transform.right * x + transform.forward * z).normalized;
        float speed = Input.GetKey(KeyCode.LeftShift) ? sprintSpeed : walkSpeed;
        cc.Move(move * speed * Time.deltaTime);

        // jump with cooldown
        if (grounded && Input.GetKeyDown(KeyCode.Space) && Time.time >= lastJumpTime + jumpCooldown)
        {
            verticalVelocity = Mathf.Sqrt(jumpHeight * -2f * gravity); 
            lastJumpTime = Time.time;
        }

        // gravity
        verticalVelocity += gravity * Time.deltaTime;
        cc.Move(new Vector3(0f, verticalVelocity, 0f) * Time.deltaTime);
    }
}
