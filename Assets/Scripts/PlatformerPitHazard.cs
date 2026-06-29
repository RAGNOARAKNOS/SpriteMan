using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class PlatformerPitHazard : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        var respawn = other.GetComponent<PlatformerRespawnController>();
        if (respawn != null)
        {
            respawn.Respawn();
        }
    }
}
