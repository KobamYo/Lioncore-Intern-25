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
    private bool isLanding = false;

    public Rigidbody rigidbody;
    private Vector3 playerMovement;

    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private Animator animator;
    private string currentAnimation;

    // Animation States
    private const string PLAYER_IDLE = "Player_Idle";
    private const string PLAYER_RUN = "Player_Run";
    private const string PLAYER_RUN_END = "Player_RunEnd";
    private const string PLAYER_CROUCH = "Player_Crouch";
    private const string PLAYER_CROUCH_END = "Player_CrouchEnd";
    private const string PLAYER_JUMP = "Player_Jump";
    private const string PLAYER_FALL = "Player_Fall";
    private const string PLAYER_LAND = "Player_Land";
    private const string PLAYER_DASH = "Player_Dash";

    void Awake()
    {
        rigidbody = GetComponent<Rigidbody>();
    }

    void Update()
    {
        isGrounded = CheckIfGrounded();

        if (isDashing)
        {
            PlayAnimation(PLAYER_DASH);
            return;
        }

        HandleJumpFallLand();
        HandleCrouch(InputManager.playerCrouch);
        HandleMovement();

        if (InputManager.playerJump && isGrounded && !isCrouching)
        {
            PerformJump();
        }

        if (InputManager.playerDash && Time.time >= lastDashTime + dashCooldown && !isCrouching)
        {
            StartCoroutine(PerformDash());
        }
    }

    private void HandleMovement()
    {
        playerMovement = InputManager.playerMovement;
        rigidbody.velocity = new Vector2(playerMovement.x * moveSpeed, rigidbody.velocity.y);

        if (playerMovement.x != 0 && isGrounded)
        {
            spriteRenderer.flipX = playerMovement.x < 0;
            PlayAnimation(PLAYER_RUN);

            if (!walkAudio.isPlaying)
            {
                walkAudio.Play();
            }
        }
        else if (isGrounded)
        {
            PlayAnimation(PLAYER_IDLE);

            if (walkAudio.isPlaying)
            {
                walkAudio.Stop();
            }
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
            isCrouching = true;
            PlayAnimation(PLAYER_CROUCH);
            rigidbody.velocity = Vector2.zero;
        }
        else if (!isCrouchingInput && isCrouching)
        {
            isCrouching = false;
            PlayAnimation(PLAYER_CROUCH_END);
        }
    }

    public bool CheckIfGrounded()
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
        PlayAnimation(PLAYER_JUMP);
    }

    private void HandleJumpFallLand()
    {
        float verticalVelocity = rigidbody.velocity.y;

        if (!isGrounded)
        {
            if (verticalVelocity > 0)
            {
                PlayAnimation(PLAYER_JUMP);
            }
            else
            {
                PlayAnimation(PLAYER_FALL);
            }
        }
        else if (isLanding == false && Mathf.Abs(verticalVelocity) < 0.1f)
        {
            StartCoroutine(PlayLandAnimation());
        }
    }

    private IEnumerator PlayLandAnimation()
    {
        isLanding = true;
        PlayAnimation(PLAYER_LAND);
        yield return new WaitForSeconds(0.2f);
        isLanding = false;
    }

    private void PlayAnimation(string newAnimation)
    {
        if (currentAnimation == newAnimation || isLanding && newAnimation != PLAYER_LAND) return;
        animator.Play(newAnimation);
        currentAnimation = newAnimation;
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
