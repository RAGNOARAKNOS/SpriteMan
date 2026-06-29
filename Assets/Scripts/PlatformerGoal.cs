using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class PlatformerGoal : MonoBehaviour
{
    private bool reached;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (reached || !other.CompareTag("Player"))
        {
            return;
        }

        reached = true;
        PlatformerGameSession.Instance?.MarkLevelComplete();
    }
}
