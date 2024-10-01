using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement")]
    public float walkSpeed = 5f;
    public float runSpeed = 7f;
    public float groundDrag = 5f;
    public float airMultiplier = 0.4f;
    private float moveSpeed;

    [Header("Jumping")]
    public float jumpForce = 4f;
    public float jumpCooldown = 0.25f;
    public float airDrag = 0f;
    private bool readyToJump = true;

    [Header("Ground Check")]
    public float playerHeight = 2f;
    public LayerMask whatIsGround;
    private bool grounded;

    [Header("Slope Handling")]
    public float maxSlopeAngle = 45f;
    private RaycastHit slopeHit;
    private bool exitingSlope;

    [Header("References")]
    public Transform orientation;

    [Header("Audio")]
    public AudioClip walkSound;
    public AudioClip runSound;
    public AudioClip jumpSound;
    public AudioSource walkAudioSource;
    public AudioSource runAudioSource;
    public AudioSource jumpAudioSource;

    // Input fields
    private float horizontalInput;
    private float verticalInput;

    private Vector3 moveDirection;
    private Rigidbody rb;

    // Movement states
    public enum MovementState
    {
        Idle,
        Walking,
        Running,
        Air
    }
    public MovementState state;

    private void Start()
    {
        // Get and configure Rigidbody component
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
    }

    private void Update()
    {
        // Check if player is grounded
        grounded = Physics.Raycast(transform.position, Vector3.down, playerHeight * 0.5f + 0.2f, whatIsGround);

        // Handle user input
        HandleInput();

        // Control player speed
        ControlSpeed();

        // Determine current movement state
        StateHandler();

        // Apply appropriate drag based on ground state
        ApplyDrag();

        // Handle movement audio
        HandleMovementAudio();
    }

    private void FixedUpdate()
    {
        // Move the player
        MovePlayer();
    }

    private void HandleInput()
    {
        // Get horizontal and vertical input
        horizontalInput = Input.GetAxisRaw("Horizontal");
        verticalInput = Input.GetAxisRaw("Vertical");

        // Handle jumping
        if (Input.GetKey(KeyCode.Space) && readyToJump && grounded)
        {
            readyToJump = false;
            Jump();
            Invoke(nameof(ResetJump), jumpCooldown);
        }
    }

    private void StateHandler()
    {
        // Running
        if (grounded && Input.GetKey(KeyCode.LeftShift))
        {
            state = MovementState.Running;
            moveSpeed = runSpeed;
        }
        // Walking
        else if (grounded && (horizontalInput != 0 || verticalInput != 0))
        {
            state = MovementState.Walking;
            moveSpeed = walkSpeed;
        }
        // Idle
        else if (grounded)
        {
            state = MovementState.Idle;
        }
        // In air
        else
        {
            state = MovementState.Air;
        }
    }

    private void MovePlayer()
    {
        // Calculate movement direction
        moveDirection = orientation.forward * verticalInput + orientation.right * horizontalInput;

        // Apply force based on slope or ground state
        if (OnSlope() && !exitingSlope)
        {
            rb.AddForce(GetSlopeMoveDirection() * moveSpeed * 20f, ForceMode.Force);
            if (rb.velocity.y > 0)
                rb.AddForce(Vector3.down * 80f, ForceMode.Force);
        }
        else if (grounded)
            rb.AddForce(moveDirection.normalized * moveSpeed * 10f, ForceMode.Force);
        else if (!grounded)
            rb.AddForce(moveDirection.normalized * moveSpeed * 10f * airMultiplier, ForceMode.Force);

        // Disable gravity while on slope
        rb.useGravity = !OnSlope();
    }

    private void ControlSpeed()
    {
        if (OnSlope() && !exitingSlope)
        {
            // Limit speed on slope
            if (rb.velocity.magnitude > moveSpeed)
                rb.velocity = rb.velocity.normalized * moveSpeed;
        }
        else
        {
            // Limit speed on ground or in air
            Vector3 flatVel = new Vector3(rb.velocity.x, 0f, rb.velocity.z);
            if (flatVel.magnitude > moveSpeed)
            {
                Vector3 limitedVel = flatVel.normalized * moveSpeed;
                rb.velocity = new Vector3(limitedVel.x, rb.velocity.y, limitedVel.z);
            }
        }
    }

    private void Jump()
    {
        exitingSlope = true;

        // Reset y velocity
        rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);

        rb.AddForce(transform.up * jumpForce, ForceMode.Impulse);

        // Play jump sound
        if (jumpSound != null)
        {
            jumpAudioSource.Play();
        }
    }

    private void ResetJump()
    {
        readyToJump = true;
        exitingSlope = false;
    }

    private bool OnSlope()
    {
        if (Physics.Raycast(transform.position, Vector3.down, out slopeHit, playerHeight * 0.5f + 0.3f))
        {
            float angle = Vector3.Angle(Vector3.up, slopeHit.normal);
            return angle < maxSlopeAngle && angle != 0;
        }
        return false;
    }

    private Vector3 GetSlopeMoveDirection()
    {
        return Vector3.ProjectOnPlane(moveDirection, slopeHit.normal).normalized;
    }

    private void ApplyDrag()
    {
        rb.drag = grounded ? groundDrag : airDrag;
    }

    private void HandleMovementAudio()
    {
        switch (state)
        {
            case MovementState.Walking:
                if (!walkAudioSource.isPlaying)
                {
                    walkAudioSource.Play();
                    runAudioSource.Stop();
                }
                break;
            case MovementState.Running:
                if (!runAudioSource.isPlaying)
                {
                    runAudioSource.Play();
                    walkAudioSource.Stop();
                }
                break;
            default:
                walkAudioSource.Stop();
                runAudioSource.Stop();
                break;
        }
    }
}