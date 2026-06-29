using UnityEngine;

public class PlatformerGameSession : MonoBehaviour
{
    public static PlatformerGameSession Instance { get; private set; }

    [Header("HUD")]
    [TextArea(2, 3)]
    [SerializeField] private string instructions = "Move: Arrow Keys / A,D   Jump: Space   Avoid pits and enemies   Collect pickups and reach the goal";

    private int totalCollectibles;
    private int collectedCollectibles;
    private int deaths;
    private bool levelComplete;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    private void OnDestroy()
    {
        if (Instance == this)
        {
            Instance = null;
        }
    }

    public void RegisterCollectible()
    {
        totalCollectibles++;
    }

    public void CollectOne()
    {
        collectedCollectibles++;
    }

    public void RegisterDeath()
    {
        deaths++;
        levelComplete = false;
    }

    public void MarkLevelComplete()
    {
        levelComplete = true;
    }

    private void OnGUI()
    {
        const int left = 12;
        const int top = 12;
        GUI.Label(new Rect(left, top, 500, 24), instructions);
        GUI.Label(new Rect(left, top + 22, 500, 24), $"Collectibles: {collectedCollectibles}/{totalCollectibles}");
        GUI.Label(new Rect(left, top + 44, 500, 24), $"Deaths: {deaths}");
        if (levelComplete)
        {
            GUI.Label(new Rect(left, top + 66, 500, 24), "Goal reached!");
        }
    }
}
