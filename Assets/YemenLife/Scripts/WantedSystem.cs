using UnityEngine;
using UnityEngine.Events;

public class WantedSystem : MonoBehaviour
{
    [Range(0, 5)] public int wantedLevel;
    public Transform player;
    public GameObject policePrefab;
    public float spawnRadius = 45f;
    public float escalationInterval = 18f;
    public UnityEvent<int> onWantedLevelChanged;

    private float escalationTimer;
    private PoliceChaseAI activePolice;

    private void Update()
    {
        if (wantedLevel <= 0 || player == null) return;

        escalationTimer += Time.deltaTime;
        if (escalationTimer >= escalationInterval && wantedLevel < 5)
        {
            escalationTimer = 0;
            SetWantedLevel(wantedLevel + 1);
        }

        if (activePolice == null && policePrefab != null)
            SpawnPolice();
    }

    // Call from gameplay events: 1 minor offence, 2 vehicle theft/assault, 3 major in-game crime.
    public void ReportCrime(int severity)
    {
        int increase = Mathf.Clamp(severity, 1, 3);
        SetWantedLevel(Mathf.Clamp(wantedLevel + increase, 1, 5));
    }

    public void SetWantedLevel(int level)
    {
        int previous = wantedLevel;
        wantedLevel = Mathf.Clamp(level, 0, 5);
        if (previous != wantedLevel) onWantedLevelChanged?.Invoke(wantedLevel);

        if (wantedLevel == 0)
        {
            if (activePolice != null) Destroy(activePolice.gameObject);
            activePolice = null;
        }
    }

    public void EscapeAndClearWanted()
    {
        SetWantedLevel(0);
    }

    private void SpawnPolice()
    {
        Vector2 offset = Random.insideUnitCircle.normalized * spawnRadius;
        Vector3 spawn = player.position + new Vector3(offset.x, 0.5f, offset.y);
        GameObject police = Instantiate(policePrefab, spawn, Quaternion.identity);
        activePolice = police.GetComponent<PoliceChaseAI>();
        if (activePolice != null)
        {
            activePolice.target = player;
            activePolice.wantedSystem = this;
        }
    }
}
