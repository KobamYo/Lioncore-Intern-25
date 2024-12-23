using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] AudioSource walkAudio;

    [Header("Dash Settings")]
    [SerializeField] private float dashSpeed = 15f;
    [SerializeField] private float dashDuration = 0.2f;
    [SerializeField] private float dashCooldown = 1f;
    private bool isDashing = false;
    private float lastDashTime = -Mathf.Infinity;

    [Header("Jump Settings")]
    [SerializeField] private float jumpForce = 10f;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private Transform groundCheck;
    [SerializeField] private float groundCheckRadius = 0.2f;
    [SerializeField] private float raycastDistance = 0.3f;
    private bool isGrounded;

    private Vector3 playerMovement;
    private Rigidbody rigididbody;
    [SerializeField] private Animator animator;
    private const string horizontal =  "Horizontal";

    void Awake()
    {
        rigididbody = GetComponent<Rigidbody>();
    }

    void Update()
    {
        if (!isDashing)
        {
            MovePlayer();
        }

        isGrounded = IsGrounded();

        if (InputManager.playerJump && isGrounded)
        {
            JumpPlayer();
        }

        if (InputManager.playerDash && Time.time >= lastDashTime + dashCooldown)
        {
            StartCoroutine(Dash());
        }
    }

    private void MovePlayer()
    {
        playerMovement.Set(InputManager.playerMovement.x, 0, 0);
        rigididbody.velocity = new Vector2(playerMovement.x * moveSpeed, GetComponent<Rigidbody>().velocity.y);

        animator.SetFloat(horizontal, playerMovement.x);

        if (playerMovement.x != 0)
        {
            if (!walkAudio.isPlaying)
            {
                walkAudio.Play();
            }
        }
        else
        {
            if (walkAudio.isPlaying)
            {
                walkAudio.Stop();
            }
        }
    }

    private IEnumerator Dash()
    {
        isDashing = true;
        lastDashTime = Time.time;

        Vector3 dashDirection = new Vector3(InputManager.playerMovement.x, 0, 0).normalized;
        rigididbody.velocity = dashDirection * dashSpeed;

        yield return new WaitForSeconds(dashDuration);
        isDashing = false;
    }

    private bool IsGrounded()
    {
        bool groundCheckHit = Physics.OverlapSphere(groundCheck.position, groundCheckRadius, groundLayer).Length > 0;
    
        RaycastHit raycastHit;
        bool rayHit = Physics.Raycast(groundCheck.position, Vector3.down, out raycastHit, raycastDistance, groundLayer);
        Debug.DrawRay(groundCheck.position, Vector3.down * raycastDistance, Color.red);

        return groundCheckHit || rayHit;
    }

    private void JumpPlayer()
    {
        GetComponent<Rigidbody>().velocity = new Vector3(GetComponent<Rigidbody>().velocity.x, jumpForce, 0);
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
