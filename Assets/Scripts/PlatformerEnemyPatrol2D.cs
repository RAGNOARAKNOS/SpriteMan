using UnityEngine;
using UnityEngine.Serialization;

[RequireComponent(typeof(Collider2D))]
public class PlatformerEnemyPatrol2D : MonoBehaviour
{
    [Header("Patrol")]
    [Tooltip("Horizontal patrol speed.")]
    [Min(0f)]
    [FormerlySerializedAs("speed")]
    [SerializeField] private float patrolSpeed = 2f;
    [Tooltip("How far ahead to check for missing ground before turning.")]
    [Min(0.01f)]
    [SerializeField] private float edgeCheckDistance = 0.8f;
    [Tooltip("How far down to check for ground.")]
    [Min(0.01f)]
    [SerializeField] private float groundCheckDistance = 1f;
    [Tooltip("How far ahead to check for blocking walls.")]
    [Min(0.01f)]
    [SerializeField] private float wallCheckDistance = 0.4f;
    [Tooltip("Layers that count as level geometry for edge and wall checks.")]
    [SerializeField] private LayerMask groundMask = ~0;

    [Header("Player Contact")]
    [Tooltip("Small cooldown to avoid repeated respawn calls from the same contact.")]
    [Min(0f)]
    [SerializeField] private float contactCooldown = 0.2f;

    private float direction = 1f;
    private float nextContactTime;

    private void FixedUpdate()
    {
        MoveForward();

        if (ShouldFlip())
        {
            Flip();
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        TryRespawnPlayer(collision.collider);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        TryRespawnPlayer(other);
    }

    private void MoveForward()
    {
        var position = transform.position;
        position.x += direction * patrolSpeed * Time.fixedDeltaTime;
        transform.position = position;
    }

    private bool ShouldFlip()
    {
        var origin = transform.position;
        var edgeOrigin = origin + Vector3.right * direction * edgeCheckDistance;
        var hasGroundAhead = Physics2D.Raycast(edgeOrigin, Vector2.down, groundCheckDistance, groundMask).collider != null;
        var hasWallAhead = Physics2D.Raycast(origin, Vector2.right * direction, wallCheckDistance, groundMask).collider != null;
        return !hasGroundAhead || hasWallAhead;
    }

    private void TryRespawnPlayer(Collider2D other)
    {
        if (Time.time < nextContactTime)
        {
            return;
        }

        var respawn = other.GetComponent<PlatformerRespawnController>();
        if (respawn == null)
        {
            return;
        }

        respawn.Respawn();
        nextContactTime = Time.time + contactCooldown;
    }

    private void Flip()
    {
        direction *= -1f;
        var scale = transform.localScale;
        scale.x = Mathf.Abs(scale.x) * direction;
        transform.localScale = scale;
    }

    private void OnValidate()
    {
        patrolSpeed = Mathf.Max(0f, patrolSpeed);
        edgeCheckDistance = Mathf.Max(0.01f, edgeCheckDistance);
        groundCheckDistance = Mathf.Max(0.01f, groundCheckDistance);
        wallCheckDistance = Mathf.Max(0.01f, wallCheckDistance);
        contactCooldown = Mathf.Max(0f, contactCooldown);
    }
}
