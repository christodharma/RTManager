using UnityEngine;

public class DailySpawnManager : MonoBehaviour
{
    [Header("Referensi Player")]
    public GameObject playerObject;

    [Header("Lokasi Spawn Harian")]
    public Transform dailySpawnPoint;

    private void Start()
    {
        if (playerObject == null || dailySpawnPoint == null)
        {
            Debug.LogError("Player Object atau Daily Spawn Point belum di-set di Inspector!");
            return;
        }

        if (GameTimeManager.Instance != null)
        {
            GameTimeManager.Instance.OnDayChanged += TeleportPlayerToSpawn;
        }
    }

    private void OnDestroy()
    {
        if (GameTimeManager.Instance != null)
        {
            GameTimeManager.Instance.OnDayChanged -= TeleportPlayerToSpawn;
        }
    }

    public void TeleportPlayerToSpawn(int dayNumber)
    {
        if (playerObject != null && dailySpawnPoint != null)
        {
            Rigidbody2D rb = playerObject.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                rb.linearVelocity = Vector2.zero;
                rb.angularVelocity = 0f;
            }

            playerObject.transform.position = dailySpawnPoint.position;

            Debug.Log($"Hari baru dimulai (Day {dayNumber}). Player dipindahkan ke: {dailySpawnPoint.name}");
        }
    }
}