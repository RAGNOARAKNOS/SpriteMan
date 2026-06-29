using UnityEngine;
using UnityEngine.Serialization;

public class PlatformerCameraFollow2D : MonoBehaviour
{
    [Header("Follow Target")]
    [Tooltip("Player to follow. Leave empty to auto-find PlatformerPlayerController2D in scene.")]
    [SerializeField] private Transform target;
    [Tooltip("If enabled and Target is empty, this script will find the player automatically.")]
    [SerializeField] private bool autoFindTarget = true;

    [Header("Follow Tuning")]
    [Min(0f)]
    [Tooltip("Lower values = snappier camera, higher values = softer smoothing.")]
    [FormerlySerializedAs("smoothTime")]
    [SerializeField] private float followSmoothTime = 0.12f;
    [Tooltip("Offset from target in world space.")]
    [SerializeField] private Vector3 offset = new Vector3(0f, 1.5f, -10f);

    [Header("Level Bounds")]
    [Tooltip("Enable to clamp camera movement to level extents.")]
    [SerializeField] private bool clampToLevelBounds = true;
    [Tooltip("Optional collider used to read level bounds. If empty, Manual Bounds are used.")]
    [SerializeField] private Collider2D levelBoundsCollider;
    [Tooltip("World-space bottom-left corner of the camera bounds.")]
    [SerializeField] private Vector2 minBounds = new Vector2(0f, -2f);
    [Tooltip("World-space top-right corner of the camera bounds.")]
    [SerializeField] private Vector2 maxBounds = new Vector2(95f, 10f);
    [Header("Startup")]
    [Tooltip("Snap camera to the target on scene start so gameplay begins centered on player.")]
    [SerializeField] private bool snapToTargetOnStart = true;

    private Vector3 velocity;
    private Camera cachedCamera;

    private void Awake()
    {
        cachedCamera = GetComponent<Camera>();
    }

    private void Start()
    {
        EnsureTarget();
        if (target == null || !snapToTargetOnStart)
        {
            return;
        }

        var position = target.position + offset;
        position.z = offset.z;
        if (clampToLevelBounds)
        {
            ApplyBoundsClamp(ref position);
        }

        transform.position = position;
    }

    private void LateUpdate()
    {
        EnsureTarget();
        if (target == null)
        {
            return;
        }

        var desired = target.position + offset;
        desired.z = offset.z;

        if (clampToLevelBounds)
        {
            ApplyBoundsClamp(ref desired);
        }

        transform.position = Vector3.SmoothDamp(transform.position, desired, ref velocity, followSmoothTime);
    }

    private void EnsureTarget()
    {
        if (!autoFindTarget)
        {
            return;
        }

        if (IsValidPlayerTarget(target))
        {
            return;
        }

        var playerController = FindObjectOfType<PlatformerPlayerController2D>();
        if (playerController != null)
        {
            target = playerController.transform;
            return;
        }

        var taggedPlayer = GameObject.FindGameObjectWithTag("Player");
        if (taggedPlayer != null)
        {
            target = taggedPlayer.transform;
        }
    }

    private static bool IsValidPlayerTarget(Transform candidate)
    {
        return candidate != null
            && candidate.gameObject.activeInHierarchy
            && candidate.GetComponentInParent<PlatformerPlayerController2D>() != null;
    }

    private void ApplyBoundsClamp(ref Vector3 desiredPosition)
    {
        var levelMin = minBounds;
        var levelMax = maxBounds;

        if (levelBoundsCollider != null)
        {
            var colliderBounds = levelBoundsCollider.bounds;
            levelMin = colliderBounds.min;
            levelMax = colliderBounds.max;
        }

        var cameraForClamp = cachedCamera != null ? cachedCamera : Camera.main;
        if (cameraForClamp == null || !cameraForClamp.orthographic)
        {
            desiredPosition.x = Mathf.Clamp(desiredPosition.x, levelMin.x, levelMax.x);
            desiredPosition.y = Mathf.Clamp(desiredPosition.y, levelMin.y, levelMax.y);
            return;
        }

        var halfHeight = cameraForClamp.orthographicSize;
        var halfWidth = halfHeight * cameraForClamp.aspect;
        var minX = levelMin.x + halfWidth;
        var maxX = levelMax.x - halfWidth;
        var minY = levelMin.y + halfHeight;
        var maxY = levelMax.y - halfHeight;

        desiredPosition.x = minX > maxX
            ? (levelMin.x + levelMax.x) * 0.5f
            : Mathf.Clamp(desiredPosition.x, minX, maxX);
        desiredPosition.y = minY > maxY
            ? (levelMin.y + levelMax.y) * 0.5f
            : Mathf.Clamp(desiredPosition.y, minY, maxY);
    }

    private void OnValidate()
    {
        followSmoothTime = Mathf.Max(0f, followSmoothTime);
        minBounds = Vector2.Min(minBounds, maxBounds);
        maxBounds = Vector2.Max(minBounds, maxBounds);
    }

    private void OnDrawGizmosSelected()
    {
        if (!clampToLevelBounds)
        {
            return;
        }

        var levelMin = minBounds;
        var levelMax = maxBounds;
        if (levelBoundsCollider != null)
        {
            levelMin = levelBoundsCollider.bounds.min;
            levelMax = levelBoundsCollider.bounds.max;
        }

        var center = (Vector3)(levelMin + levelMax) * 0.5f;
        var size = new Vector3(levelMax.x - levelMin.x, levelMax.y - levelMin.y, 0f);
        Gizmos.color = new Color(0.2f, 0.8f, 1f, 0.7f);
        Gizmos.DrawWireCube(center, size);
    }
}
