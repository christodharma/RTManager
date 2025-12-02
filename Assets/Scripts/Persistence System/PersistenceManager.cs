using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PersistenceManager : MonoBehaviour
{
    public GameData GameData;
    private StorageType _storageType;
    public StorageType StorageType
    {
        get => _storageType;
        set
        {
            _storageType = value;
            switch (_storageType)
            {
                case StorageType.JSON:
                    if (StorageHandler != null)
                    {
                        Destroy((MonoBehaviour)StorageHandler);
                    }
                    StorageHandler = gameObject.AddComponent<JSONStorageHandler>();
                    break;

                default:
                    break;
            }
        }
    }
    public IStorageHandler StorageHandler;
    List<IPersistable> Subscribers = new();
    public static PersistenceManager Instance { get; private set; }

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    void Start()
    {
        FindAllPersistableScripts();
        StorageHandler = gameObject.AddComponent<JSONStorageHandler>();
    }

    [ContextMenu("Trigger Load Game")]
    public void TriggerLoad()
    {
        GameData = StorageHandler.Read("save");
        foreach (var item in Subscribers)
        {
            item.Load(GameData);
        }
        // Optional: reload scene
    }

    [ContextMenu("Trigger Save Game")]
    public void TriggerSave()
    {
        foreach (var item in Subscribers)
        {
            item.Save(ref GameData);
        }
        StorageHandler.Write(GameData);
    }

    public void AddSubcriber(IPersistable persistable)
    {
        Subscribers.Add(persistable);
    }

    void FindAllPersistableScripts()
    {
        // Finds every MonoBehaviour script that also implements IPersistable
        IEnumerable<IPersistable> persistables = FindObjectsByType<MonoBehaviour>(FindObjectsSortMode.None).OfType<IPersistable>();
        Subscribers = new List<IPersistable>(persistables);
        Debug.Log($"[PersistenceManager] Found {Subscribers.Count} persistables");
    }
}

public enum StorageType
{
    JSON,
    Binary,
    Cloud
    // add more if needed
}