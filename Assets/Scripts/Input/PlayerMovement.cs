using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private AudioSource walkAudio;

    [Header("Dash Settings")]
    [SerializeField] private float dashSpeed = 15f;
    [SerializeField] private float dashDuration = 0.2f;
    [SerializeField] private float dashCooldown = 1f;
    private bool isDashing = false;
    private float lastDashTime = -Mathf.Infinity;

    [Header("Crouch Settings")]
    private bool isCrouching = false;

    [Header("Jump Settings")]
    [SerializeField] private float jumpForce = 10f;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private Transform groundCheck;
    [SerializeField] private float groundCheckRadius = 0.2f;
    [SerializeField] private float raycastDistance = 0.3f;
    private bool isGrounded;

    private Vector3 playerMovement;
    private Rigidbody rigidbody;
    [SerializeField] private Animator animator;
    [SerializeField] private SpriteRenderer spriteRenderer;
    private const string RunSpeedParam = "Run Speed";
    private const string IsCrouchingParam = "Is Crouching";
    private const string IsJumpingParam = "Is Jumping";
    private const string IsFallingParam = "Is Falling";
    private const string IsLandingParam = "Is Landing";

    void Awake()
    {
        rigidbody = GetComponent<Rigidbody>();
    }

    void Update()
    {
        isGrounded = CheckIfGrounded();

        if (!isDashing && !isCrouching)
        {
            HandleMovement();
        }

        HandleJumpFallLand();

        if (InputManager.playerJump && isGrounded && !isCrouching)
        {
            PerformJump();
        }

        if (InputManager.playerDash && Time.time >= lastDashTime + dashCooldown && !isCrouching)
        {
            StartCoroutine(PerformDash());
        }

        HandleCrouch(InputManager.playerCrouch);
    }

    private void HandleMovement()
    {
        playerMovement = InputManager.playerMovement;
        rigidbody.velocity = new Vector2(playerMovement.x * moveSpeed, rigidbody.velocity.y);

        animator.SetFloat(RunSpeedParam, Mathf.Abs(playerMovement.x));

        if (playerMovement.x != 0)
        {
            spriteRenderer.flipX = playerMovement.x < 0;

            if (!walkAudio.isPlaying)
            {
                walkAudio.Play();
            }
        }
        else if (walkAudio.isPlaying)
        {
            walkAudio.Stop();
        }
    }

    private IEnumerator PerformDash()
    {
        isDashing = true;
        lastDashTime = Time.time;

        Vector3 dashDirection = new Vector3(playerMovement.x, 0, 0).normalized;
        rigidbody.velocity = dashDirection * dashSpeed;

        yield return new WaitForSeconds(dashDuration);
        isDashing = false;
    }

    private void HandleCrouch(bool isCrouchingInput)
    {
        if (isCrouchingInput && !isCrouching)
        {
            StartCrouch();
        }
        else if (!isCrouchingInput && isCrouching)
        {
            StopCrouch();
        }
    }

    private void StartCrouch()
    {
        isCrouching = true;
        animator.SetBool(IsCrouchingParam, true);
        rigidbody.velocity = Vector2.zero;
    }

    private void StopCrouch()
    {
        isCrouching = false;
        animator.SetBool(IsCrouchingParam, false);
    }

    private bool CheckIfGrounded()
    {
        bool groundCheckHit = Physics.OverlapSphere(groundCheck.position, groundCheckRadius, groundLayer).Length > 0;

        RaycastHit raycastHit;
        bool rayHit = Physics.Raycast(groundCheck.position, Vector3.down, out raycastHit, raycastDistance, groundLayer);
        Debug.DrawRay(groundCheck.position, Vector3.down * raycastDistance, Color.red);

        return groundCheckHit || rayHit;
    }

    private void PerformJump()
    {
        rigidbody.velocity = new Vector2(rigidbody.velocity.x, jumpForce);
        animator.SetBool(IsJumpingParam, true);
    }

    private void HandleJumpFallLand()
    {
        float verticalVelocity = rigidbody.velocity.y;

        if (!isGrounded)
        {
            animator.SetBool(IsJumpingParam, verticalVelocity > 0);
            animator.SetBool(IsFallingParam, verticalVelocity < 0);
            animator.SetBool(IsLandingParam, false);
        }
        else
        {
            animator.SetBool(IsJumpingParam, false);
            animator.SetBool(IsFallingParam, false);
            animator.SetBool(IsLandingParam, Mathf.Abs(verticalVelocity) < 0.1f);
        }
    }

    void OnDrawGizmosSelected()
    {
        if (groundCheck != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
        }
    }
}
