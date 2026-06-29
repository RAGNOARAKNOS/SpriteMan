using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class PlatformerCollectible : MonoBehaviour
{
    private bool collected;

    private void Start()
    {
        PlatformerGameSession.Instance?.RegisterCollectible();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (collected || !other.CompareTag("Player"))
        {
            return;
        }

        collected = true;
        PlatformerGameSession.Instance?.CollectOne();
        Destroy(gameObject);
    }
}
