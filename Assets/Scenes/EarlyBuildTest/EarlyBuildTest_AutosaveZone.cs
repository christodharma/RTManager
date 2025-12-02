using UnityEngine;

public class EarlyBuildTest_AutosaveZone : MonoBehaviour
{
    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && PersistenceManager.Instance != null)
        {
            PersistenceManager.Instance.TriggerSave();
        }
    }
}