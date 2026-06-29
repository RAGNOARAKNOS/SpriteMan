using UnityEngine;
using UnityEngine.Serialization;

public class PlatformerRespawnController : MonoBehaviour
{
    [Header("Respawn")]
    [Tooltip("Optional explicit respawn point. If empty, the spawn position at scene start is used.")]
    [FormerlySerializedAs("spawnPointOverride")]
    [SerializeField] private Transform respawnPoint;

    [Header("Fall Safety")]
    [Tooltip("If the player falls below this Y value, they respawn automatically.")]
    [FormerlySerializedAs("killYThreshold")]
    [SerializeField] private float fallRespawnY = -12f;

    private Rigidbody2D rb;
    private Vector3 initialSpawn;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        initialSpawn = respawnPoint != null ? respawnPoint.position : transform.position;
    }

    private void Update()
    {
        if (transform.position.y < fallRespawnY)
        {
            Respawn();
        }
    }

    public void Respawn()
    {
        var spawn = respawnPoint != null ? respawnPoint.position : initialSpawn;
        rb.linearVelocity = Vector2.zero;
        rb.angularVelocity = 0f;
        transform.position = spawn;
        PlatformerGameSession.Instance?.RegisterDeath();
    }
}
