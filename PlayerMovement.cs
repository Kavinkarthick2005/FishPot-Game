using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Playermovement : MonoBehaviour
{
    // FPS movement variables
    public Transform playerCamera;
    [SerializeField] [Range(0.0f, 0.5f)] float mouseSmoothTime = 0.03f;
    public bool cursorLock = true;
    public float mouseSensitivity = 3.5f;
    public float walkSpeed = 6.0f;
    public float sprintMultiplier = 1.5f; // Sprint multiplier instead of modifying speed
    [SerializeField] [Range(0.0f, 0.5f)] float moveSmoothTime = 0.3f;
    public float gravity = -9.81f;
    public Transform groundCheck;
    public LayerMask ground;

    // Jumping variables
    public float jumpHeight = 2f;
    float velocityY;
    bool isGrounded;

    public Animator anim;

    // Mouse settings
    float cameraCap;
    Vector2 currentMouseDelta;
    Vector2 currentMouseDeltaVelocity;

    CharacterController controller;
    Vector2 currentDir;
    Vector2 currentDirVelocity;
    Vector3 velocity;

    void Start()
    {
        controller = GetComponent<CharacterController>();
        anim = GetComponent<Animator>();
        if (cursorLock)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }

    void Update()
    {
        UpdateMouseLook();
        UpdateMovement();
    }

    void UpdateMouseLook()
    {
        // Get mouse input
        Vector2 targetMouseDelta = new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));

        // Smooth mouse movement
        currentMouseDelta = Vector2.SmoothDamp(currentMouseDelta, targetMouseDelta, ref currentMouseDeltaVelocity, mouseSmoothTime);

        // Vertical rotation
        cameraCap -= currentMouseDelta.y * mouseSensitivity;
        cameraCap = Mathf.Clamp(cameraCap, -90.0f, 90.0f);
        playerCamera.localEulerAngles = Vector3.right * cameraCap;
        // Horizontal rotation
        transform.Rotate(Vector3.up * currentMouseDelta.x * mouseSensitivity);
    }

    void UpdateMovement()
{
    // Check if player is on the ground
    isGrounded = Physics.CheckSphere(groundCheck.position, 0.3f, ground);
    if (isGrounded && velocityY < 0) velocityY = -2f;

    // Get movement input
    float moveX = Input.GetAxisRaw("Horizontal");
    float moveZ = Input.GetAxisRaw("Vertical");
    Vector2 targetDir = new Vector2(moveX, moveZ).normalized;

    // Check if the player is moving
    bool isMoving = targetDir.magnitude > 0;
    bool isRunning = Input.GetKey(KeyCode.LeftShift) && isMoving;

    // Apply movement
    float speed = isRunning ? walkSpeed * sprintMultiplier : walkSpeed;
    Vector3 move = (transform.forward * moveZ + transform.right * moveX) * speed;
    controller.Move(move * Time.deltaTime); // Move using CharacterController

    if(move == Vector3.zero){
        anim.SetFloat("speed",0);
    }
    else if(!Input.GetKey(KeyCode.LeftShift)){
        anim.SetFloat("speed",0.5f);
    }
    else{
        anim.SetFloat("speed",1f);
    }

    // Apply gravity
    velocityY += gravity * Time.deltaTime;
    controller.Move(Vector3.up * velocityY * Time.deltaTime);
}

}
