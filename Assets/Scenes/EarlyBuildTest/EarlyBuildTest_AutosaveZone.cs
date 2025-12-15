using UnityEngine;

public class EarlyBuildTest_AutosaveZone : MonoBehaviour
{
    public float AutosaveCooldown = 30; // so the player doesn't save everytime
    [SerializeField] float _autosaveCooldown = 0;
    [SerializeField] bool IsSaved = false;
    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && PersistenceManager.Instance != null && IsSaved == false)
        {
            PersistenceManager.Instance.TriggerSave();
            _autosaveCooldown = AutosaveCooldown;
            IsSaved = true;
        }
    }

    void Update()
    {
        if (IsSaved && _autosaveCooldown > 0)
        {
            _autosaveCooldown -= Time.deltaTime;
        }
        else if (_autosaveCooldown <= 0)
        {
            IsSaved = false;
        }
    }
}