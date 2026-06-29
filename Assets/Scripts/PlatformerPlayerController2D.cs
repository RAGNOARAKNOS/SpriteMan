using System;
using System.Reflection;
using UnityEngine;
using UnityEngine.Serialization;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Collider2D))]
public class PlatformerPlayerController2D : MonoBehaviour
{
    [Header("Movement")]
    [Tooltip("Horizontal speed on the ground.")]
    [Min(0f)]
    [SerializeField] private float moveSpeed = 8f;
    [Tooltip("Upward velocity applied when jumping.")]
    [Min(0f)]
    [SerializeField] private float jumpVelocity = 12f;
    [FormerlySerializedAs("airControlMultiplier")]
    [Range(0f, 1f)]
    [Tooltip("How much horizontal control is retained while in the air.")]
    [SerializeField] private float airborneControlPercent = 0.8f;

    [Header("Ground Check")]
    [Tooltip("Optional transform to place the ground check probe. If empty, the probe uses the collider bottom.")]
    [SerializeField] private Transform groundCheck;
    [Min(0.01f)]
    [SerializeField] private float groundCheckRadius = 0.15f;
    [Tooltip("Which layers count as ground for jumping.")]
    [SerializeField] private LayerMask groundMask = ~0;

    private Rigidbody2D rb;
    private Collider2D bodyCollider;
    private float moveInput;
    private bool jumpQueued;
    private bool isGrounded;
    private Vector2 lastGroundProbePosition;

    private static readonly Type InputSystemKeyboardType = Type.GetType("UnityEngine.InputSystem.Keyboard, Unity.InputSystem");
    private static readonly PropertyInfo InputSystemKeyboardCurrentProperty = InputSystemKeyboardType?.GetProperty("current", BindingFlags.Public | BindingFlags.Static);

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        bodyCollider = GetComponent<Collider2D>();
    }

    private void Update()
    {
        moveInput = ReadHorizontalInput();
        if (ReadJumpPressed())
        {
            jumpQueued = true;
        }
    }

    private float ReadHorizontalInput()
    {
        var keyboard = GetInputSystemKeyboard();
        if (keyboard != null)
        {
            var horizontal = 0f;
            if (IsInputSystemKeyPressed(keyboard, "leftArrowKey") || IsInputSystemKeyPressed(keyboard, "aKey"))
            {
                horizontal -= 1f;
            }

            if (IsInputSystemKeyPressed(keyboard, "rightArrowKey") || IsInputSystemKeyPressed(keyboard, "dKey"))
            {
                horizontal += 1f;
            }

            return Mathf.Clamp(horizontal, -1f, 1f);
        }

        try
        {
            return Input.GetAxisRaw("Horizontal");
        }
        catch (InvalidOperationException)
        {
            return 0f;
        }
    }

    private bool ReadJumpPressed()
    {
        var keyboard = GetInputSystemKeyboard();
        if (keyboard != null)
        {
            return WasInputSystemKeyPressedThisFrame(keyboard, "spaceKey")
                   || WasInputSystemKeyPressedThisFrame(keyboard, "upArrowKey")
                   || WasInputSystemKeyPressedThisFrame(keyboard, "wKey");
        }

        try
        {
            return Input.GetButtonDown("Jump");
        }
        catch (InvalidOperationException)
        {
            return false;
        }
    }

    private static object GetInputSystemKeyboard()
    {
        return InputSystemKeyboardCurrentProperty?.GetValue(null);
    }

    private static bool IsInputSystemKeyPressed(object keyboard, string keyPropertyName)
    {
        return ReadInputSystemKeyState(keyboard, keyPropertyName, "isPressed");
    }

    private static bool WasInputSystemKeyPressedThisFrame(object keyboard, string keyPropertyName)
    {
        return ReadInputSystemKeyState(keyboard, keyPropertyName, "wasPressedThisFrame");
    }

    private static bool ReadInputSystemKeyState(object keyboard, string keyPropertyName, string statePropertyName)
    {
        if (keyboard == null)
        {
            return false;
        }

        var keyProperty = keyboard.GetType().GetProperty(keyPropertyName, BindingFlags.Public | BindingFlags.Instance);
        var keyControl = keyProperty?.GetValue(keyboard);
        if (keyControl == null)
        {
            return false;
        }

        var stateProperty = keyControl.GetType().GetProperty(statePropertyName, BindingFlags.Public | BindingFlags.Instance);
        return stateProperty != null && stateProperty.GetValue(keyControl) is bool pressed && pressed;
    }

    private void FixedUpdate()
    {
        isGrounded = CheckGrounded();

        var velocity = rb.linearVelocity;
        var control = isGrounded ? 1f : airborneControlPercent;
        velocity.x = moveInput * moveSpeed * control;

        if (jumpQueued && isGrounded)
        {
            velocity.y = jumpVelocity;
        }
        jumpQueued = false;

        rb.linearVelocity = velocity;
    }

    private bool CheckGrounded()
    {
        if (groundMask.value == 0)
        {
            return false;
        }

        var probePosition = GetGroundProbePosition();
        lastGroundProbePosition = probePosition;

        var hits = Physics2D.OverlapCircleAll(probePosition, groundCheckRadius, groundMask);
        foreach (var hit in hits)
        {
            if (hit != null && hit != bodyCollider && !hit.isTrigger)
            {
                return true;
            }
        }

        return false;
    }

    private Vector2 GetGroundProbePosition()
    {
        if (groundCheck != null)
        {
            return groundCheck.position;
        }

        if (bodyCollider == null)
        {
            return transform.position;
        }

        var bounds = bodyCollider.bounds;
        return new Vector2(bounds.center.x, bounds.min.y + (groundCheckRadius * 0.5f));
    }

    private void OnValidate()
    {
        moveSpeed = Mathf.Max(0f, moveSpeed);
        jumpVelocity = Mathf.Max(0f, jumpVelocity);
        groundCheckRadius = Mathf.Max(0.01f, groundCheckRadius);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        var probePosition = Application.isPlaying ? (Vector3)lastGroundProbePosition : (Vector3)GetGroundProbePosition();
        Gizmos.DrawWireSphere(probePosition, groundCheckRadius);
    }
}
