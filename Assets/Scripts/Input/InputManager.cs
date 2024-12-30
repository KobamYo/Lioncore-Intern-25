using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{
    public static Vector2 playerMovement;
    public static bool playerJump;
    public static bool playerDash;
    public static bool playerCrouch;
    public static bool playerCombat;

    private PlayerInput playerInput;
    private InputAction moveAction;
    private InputAction jumpAction;
    private InputAction dashAction;
    private InputAction crouchAction;
    private InputAction attackAction;

    void Awake()
    {
        playerInput = GetComponent<PlayerInput>();
        moveAction = playerInput.actions["Move"];
        jumpAction = playerInput.actions["Jump"];
        dashAction = playerInput.actions["Dash"];
        crouchAction = playerInput.actions["Crouch"];
        attackAction = playerInput.actions["Combat"];
    }

    void Update()
    {
        playerMovement = moveAction.ReadValue<Vector2>();
        playerJump = jumpAction.triggered;
        playerDash = dashAction.triggered;
        playerCrouch = crouchAction.ReadValue<float>() > 0;
        playerCombat = attackAction.triggered;
    }
}
